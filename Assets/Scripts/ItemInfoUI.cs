using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoUI : MonoBehaviour
{
    public TextMeshProUGUI infoText;
    public Button sellButton;
    public TextMeshProUGUI sellButtonText;
    public TextMeshProUGUI sellPriceText;
    public GameObject sellPricePanel;

    private bool isItemSold = false;

    void Start()
    {
        infoText.text = "아이템을 탭하세요.";
        sellPricePanel.SetActive(false);
        if (sellButton != null)
        {
            sellButton.gameObject.SetActive(false);
            sellButton.onClick.AddListener(OnSellButtonClicked);
        }
    }

    public void ShowItemInfo(Item item)
    {
        if (item != null)
        {
            infoText.text = item.GetItemDescription();
            
            sellPricePanel.SetActive(true);
            sellButton.gameObject.SetActive(true);
            sellPriceText.gameObject.SetActive(true);
            sellButtonText.text = "판매";
            sellPriceText.text = GameManager.instance.itemManager.GetPriceByItemLevel(item.itemData.itemLevel).ToString();
        }
        else
        {
            infoText.text = "No item selected.";
            sellPriceText.gameObject.SetActive(false);
            sellButton.gameObject.SetActive(false);
        }
    }

    public void OnSellButtonClicked()
    {
        if (!isItemSold)
        {
            if (GameManager.instance.selectedItem != null)
            {
                GameManager.instance.itemManager.SellingItem(GameManager.instance.selectedItem);
                sellPricePanel.SetActive(false);
                GameManager.instance.selectedItem = null;

                sellButtonText.text = $"<size=25> 실행 취소 </size>";
                infoText.text = "실수로 팔았나요? 아직 취소할 수 있습니다.";
                isItemSold = true;
            }
            else
            {
                Debug.LogWarning("No item selected to sell.");
            }
        }
        else
        {
            GameManager.instance.itemManager.CancelLastSell();
            sellButtonText.text = "판매";
            ShowItemInfo(GameManager.instance.selectedItem);
            isItemSold = false;
        }
    }
}