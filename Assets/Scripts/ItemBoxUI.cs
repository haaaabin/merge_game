using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBoxUI : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public Image iconImg;
    public GameObject countTextPanel;
    
    public int count;
    public List<ItemData> itemBox = new List<ItemData>();
    public Transform itemBoxTransform;

    void Start()
    {
        if (itemBox.Count > 0)
        {
            iconImg.sprite = itemBox[0].itemImage;
            count = itemBox.Count;
            countText.text = count.ToString();
        }
    }

    public void OnClickItemBox()
    {
        if (itemBox.Count == 0)
        {
            return;
        }

        List<Slot> emptySlots = Board.instance.GetEmptySlots();
        if (emptySlots.Count == 0)
        {
            Debug.LogWarning("비어있는 슬롯이 없습니다.");
            return;
        }

        Slot randomSlot = emptySlots[Random.Range(0, emptySlots.Count)];
        ItemData spawnData = itemBox[0];

        Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(iconImg.canvas.worldCamera, iconImg.rectTransform.position);
        screenPos.z = Camera.main.WorldToScreenPoint(randomSlot.transform.position).z;

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

        GameObject itemObj = Instantiate(spawnData.itemPrefab, worldPos, Quaternion.identity);

        Item item = itemObj.GetComponent<Item>();
        item.currentSlot = randomSlot;
        item.transform.parent = randomSlot.transform;

        randomSlot.currentItem = item;

        RemoveItemBox(itemBox[0]);

        itemObj.transform.localScale = Vector3.zero;
        itemObj.transform.DOScale(0.1f, 0.5f).SetEase(Ease.OutBack);

        itemObj.transform
                .DOMove(randomSlot.transform.position, 0.7f)
                .SetEase(Ease.OutCubic)
                .OnComplete(() =>
                {
                    itemObj.transform.position = randomSlot.transform.position;
                });
    }

    public void RemoveItemBox(ItemData item)
    {
        count--;
        itemBox.Remove(item);

        if (itemBox.Count > 0)
        {
            iconImg.sprite = itemBox[0].itemImage;
            iconImg.gameObject.SetActive(true);
            countTextPanel.SetActive(true);
            countText.text = count.ToString();
        }
        else
        {
            iconImg.gameObject.SetActive(false);
            countTextPanel.SetActive(false);
        }
    }
}