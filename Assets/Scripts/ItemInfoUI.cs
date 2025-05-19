using TMPro;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    public void ShowItemInfo(Item item)
    {
        if (item != null)
        {
            if (item.isItemSpawner)
            {
                infoText.text = $"{item.itemData.itemName}" + "/" +
                                $"Lv.{item.itemData.itemLevel}" + " : " +
                                "탭하여 아이템을 생성하세요.";
            }
            else
            {
                infoText.text = $"{item.itemData.itemName}" + "/" +
                                $"Lv.{item.itemData.itemLevel}" + " : " +
                                "합쳐서 다음 레벨에 도달하세요.";
            }
        }
        else
        {
            infoText.text = "No item selected.";
        }
    }
}