using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Lumin;

public class ItemManager : MonoBehaviour
{
    public List<ItemData> allItemDatas;
    private Transform itemSpawner;

    private ItemData lastSoldItemData = null;
    private Slot lastSoldSlot = null;

    void Start()
    {
        itemSpawner = GameObject.FindWithTag("ItemSpawner").transform;
    }

    public void SpawnRandomLevel1Item()
    {
        if (GameManager.instance.resourceManager.energyCount <= 0)
        {
            Debug.LogWarning("전기가 부족합니다.");
            return;
        }

        var level1Items = allItemDatas.Where(i => i.itemLevel == 1).ToArray();
        if (level1Items.Length == 0)
        {
            Debug.LogWarning("레벨 1 아이템이 없습니다.");
            return;
        }

        List<Slot> emptySlots = Board.instance.GetEmptySlots();
        if (emptySlots.Count == 0)
        {
            Debug.LogWarning("비어있는 슬롯이 없습니다.");
            return;
        }

        GameManager.instance.resourceManager.UseEnergy();

        Slot randomSlot = emptySlots[Random.Range(0, emptySlots.Count)];
        ItemData randomItemData = level1Items[Random.Range(0, level1Items.Length)];

        GameObject itemObj = Instantiate(randomItemData.itemPrefab, itemSpawner.position, Quaternion.identity);

        Item item = itemObj.GetComponent<Item>();
        item.currentSlot = randomSlot;
        item.transform.parent = randomSlot.transform;

        randomSlot.currentItem = item;

        // effect
        itemObj.transform.localScale = Vector3.zero;
        itemObj.transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutBack);

        itemObj.transform
               .DOMove(randomSlot.transform.position, 0.7f)
               .SetEase(Ease.OutCubic)
               .OnComplete(() =>
               {
                   // 정확한 슬롯 위치로 정렬 (부드럽게 스냅)
                   itemObj.transform.position = randomSlot.transform.position;
               });

        GameManager.instance.orderManager.CheckAllOrders();
    }

    public bool CanMerge(Item itemA, Item itemB)
    {
        if (itemA.itemData == null || itemB.itemData == null)
        {
            return false;
        }

        return itemA.itemData.itemType == itemB.itemData.itemType &&
               itemA.itemData.itemLevel == itemB.itemData.itemLevel;
    }

    public void MergeItems(Item itemA, Item itemB)
    {
        itemA.currentSlot.selectionOutline.SetActive(false);
        ItemData nextLevelData = GetNextLevelData(itemA.itemData);
        if (nextLevelData != null)
        {
            Vector3 spawnPos = itemB.transform.position;
            Slot spawnSlot = itemB.currentSlot;

            Destroy(itemA.gameObject);
            Destroy(itemB.gameObject);

            GameObject newItemObj = Instantiate(nextLevelData.itemPrefab, spawnPos, Quaternion.identity);
            Item newItem = newItemObj.GetComponent<Item>();
            newItem.currentSlot = spawnSlot;
            newItem.transform.parent = spawnSlot.transform;
            spawnSlot.currentItem = newItem;
            newItem.itemData = nextLevelData;

            spawnSlot.selectionOutline.SetActive(true);

            newItemObj.transform.localScale = Vector3.zero;
            newItemObj.transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutBack);

            GameManager.instance.selectedItem = newItem;
            GameManager.instance.orderManager.CheckAllOrders();
        }
        else
        {
            Debug.LogWarning("머지할 수 있는 다음 레벨 아이템이 없습니다.");
        }
    }

    private ItemData GetNextLevelData(ItemData currentItemData)
    {
        foreach (var data in allItemDatas)
        {
            if (data.itemType == currentItemData.itemType && data.itemLevel == currentItemData.itemLevel + 1)
            {
                return data;
            }
        }
        return null;
    }

    private ItemData GetPreviousLevelData(ItemData currentItemData)
    {
        foreach (var data in allItemDatas)
        {
            if (data.itemType == currentItemData.itemType && data.itemLevel == currentItemData.itemLevel - 1)
            {
                return data;
            }
        }
        return null;
    }

    public List<ItemData> GetAllItemData()
    {
        return allItemDatas;
    }

    public bool TryCloneItem(Item original)
    {
        // 현재 슬롯 위치 파악
        Slot originalSlot = original.currentSlot;
        List<Slot> nearbyEmptySlots = Board.instance.GetNearbyEmptySlots(originalSlot, 1);

        if (nearbyEmptySlots.Count < 2)
        {
            Debug.LogWarning("주변에 비어있는 슬롯이 충분하지 않습니다.");
            return false;
        }

        Slot randomSlot = nearbyEmptySlots[Random.Range(0, nearbyEmptySlots.Count)];

        SetupNewItem(Instantiate(original.itemData.itemPrefab, randomSlot.transform.position, Quaternion.identity), randomSlot);
        // GameManager.instance.selectedItem = original;
        // originalSlot.selectionOutline.SetActive(true);

        GameManager.instance.orderManager.CheckAllOrders();
        return true;
    }

    public bool TryDivideItem(Item original)
    {
        // 현재 슬롯 위치 파악
        Slot originalSlot = original.currentSlot;
        List<Slot> nearbyEmptySlots = Board.instance.GetNearbyEmptySlots(originalSlot, 1);

        if (nearbyEmptySlots.Count < 2)
        {
            Debug.LogWarning("주변에 비어있는 슬롯이 충분하지 않습니다.");
            return false;
        }

        // 랜덤으로 2개의 슬롯 선택
        Slot slot1 = nearbyEmptySlots[Random.Range(0, nearbyEmptySlots.Count)];
        nearbyEmptySlots.Remove(slot1);
        Slot slot2 = nearbyEmptySlots[Random.Range(0, nearbyEmptySlots.Count)];

        // 이전 레벨 데이터 가져오기
        ItemData previousLevelData = GetPreviousLevelData(original.itemData);
        if (previousLevelData == null)
        {
            Debug.LogWarning("이전 레벨 아이템 데이터가 없습니다.");
            return false;
        }

        // 기존 아이템 제거
        Destroy(original.gameObject);

        // original 위치에 생성
        SetupNewItem(Instantiate(previousLevelData.itemPrefab, originalSlot.transform.position, Quaternion.identity), originalSlot);

        // slot1 위치에 생성
        SetupNewItem(Instantiate(previousLevelData.itemPrefab, slot1.transform.position, Quaternion.identity), slot1);

        // slot2 위치에 생성
        SetupNewItem(Instantiate(previousLevelData.itemPrefab, slot2.transform.position, Quaternion.identity), slot2);

        GameManager.instance.orderManager.CheckAllOrders();
        return true;
    }

    private void SetupNewItem(GameObject itemObj, Slot slot)
    {
        Item newItem = itemObj.GetComponent<Item>();
        newItem.currentSlot = slot;
        slot.currentItem = newItem;
        itemObj.transform.SetParent(slot.transform);

        // Effect
        itemObj.transform.localScale = Vector3.zero;
        itemObj.transform.DOScale(0.3f, 0.5f).SetEase(Ease.OutBack);
        itemObj.transform.DOMove(slot.transform.position, 0.7f).SetEase(Ease.OutCubic).OnComplete(() =>
        {
            itemObj.transform.position = slot.transform.position;
        });
    }

    public void SellingItem(Item item)
    {
        if (item == null || item.currentSlot == null)
        {
            Debug.LogWarning("아이템이나 슬롯이 유효하지 않습니다.");
            return;
        }

        lastSoldItemData = item.itemData;
        lastSoldSlot = item.currentSlot;

        item.PlayExplodeEffectAndDestroy();
        item.currentSlot.currentItem = null;
        item.currentSlot.selectionOutline.SetActive(false);
        item.currentSlot = null;

        GameManager.instance.resourceManager.AddCoins(GetPriceByItemLevel(item.itemData.itemLevel));
    }

    public void CancelLastSell()
    {
        if (lastSoldItemData == null || lastSoldSlot == null)
        {
            Debug.LogWarning("마지막 판매를 취소할 아이템이나 슬롯이 없습니다.");
            return;
        }

        if (!lastSoldSlot.isEmpty)
        {
            Debug.LogWarning("마지막 판매를 취소할 슬롯이 비어있지 않습니다.");
            return;
        }

        GameObject itemObj = Instantiate(lastSoldItemData.itemPrefab, lastSoldSlot.transform.position, Quaternion.identity);
        SetupNewItem(itemObj, lastSoldSlot);
        lastSoldSlot.selectionOutline.SetActive(true);

        GameManager.instance.resourceManager.AddCoins(-GetPriceByItemLevel(lastSoldItemData.itemLevel));

        lastSoldItemData = null;
        lastSoldSlot = null;

        GameManager.instance.selectedItem = itemObj.GetComponent<Item>();
    }

    public int GetPriceByItemLevel(int level)
    {
        switch (level)
        {
            case 1: return 2;
            case 2: return 4;
            case 3: return 8;
            case 4: return 16;
            case 5: return 30;
            case 6: return 52;
            case 7: return 66;
            default: return 0;
        }
    }
}