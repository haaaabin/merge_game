using TMPro;
using UnityEngine;

public class ItemInfoUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;

    public void ShowItemInfo(Item item)
    {
        if (item != null)
        {
            infoText.text = item.GetItemDescription();
        }
        else
        {
            infoText.text = "No item selected.";
        }
    }
}