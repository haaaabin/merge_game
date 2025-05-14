using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    public List<ItemData> itemDatas;
    public Board board;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnRandomLevel1Item()
    {
        var level1Items = itemDatas.Where(i => i.itemLevel == 1).ToArray();
        if (level1Items.Length == 0)
        {
            Debug.LogWarning("레벨 1 아이템이 없습니다.");
            return;
        }

        List<Slot> emptySlots = board.GetEmptySlots();
        if (emptySlots.Count == 0)
        {
            Debug.LogWarning("비어있는 슬롯이 없습니다.");
            return;
        }

        Slot randomSlot = emptySlots[Random.Range(0, emptySlots.Count)];
        ItemData randomItemData = level1Items[Random.Range(0, level1Items.Length)];

        GameObject itemObj = Instantiate(randomItemData.itemPrefab, randomSlot.transform.position, Quaternion.identity);
        Item item = itemObj.GetComponent<Item>();
        item.currentSlot = randomSlot;
        item.transform.parent = randomSlot.transform;

        randomSlot.currentItem = item;
    }

    public bool CanMerge(Item itemA, Item itemB)
    {
        return itemA.itemData.itemType == itemB.itemData.itemType &&
               itemA.itemData.itemLevel == itemB.itemData.itemLevel;
    }

    public void MergeItems(Item itemA, Item itemB)
    {
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
        }
        else
        {
            Debug.LogWarning("머지할 수 있는 다음 레벨 아이템이 없습니다.");
        }
    }

    private ItemData GetNextLevelData(ItemData currentItemData)
    {
       foreach (var data in itemDatas)
        {
            if (data.itemType == currentItemData.itemType && data.itemLevel == currentItemData.itemLevel + 1)
            {
                return data;
            }
        }
        return null;
    }
}