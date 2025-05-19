using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OrderPanelUI : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private Button completeButton;

    private OrderData currentOrder;

    void Start()
    {
        completeButton.gameObject.SetActive(false);
        completeButton.onClick.AddListener(OnCompleteOrder);
    }

    // 주문 패널 UI를 설정하는 메서드
    public void SetOrder(OrderData order)
    {
        currentOrder = order;
        itemImage.sprite = order.itemData.itemImage;
        rewardText.text = order.reward.ToString();
        completeButton.gameObject.SetActive(false);
    }

    public void CheckItemInBoard()
    {
        bool exists = Board.instance.HasItem(currentOrder.itemData);
        completeButton.gameObject.SetActive(exists);
    }

    // 주문 완료 버튼 클릭 시 호출되는 메서드
    private void OnCompleteOrder()
    {
        if (Board.instance.HasItem(currentOrder.itemData))
        {
            Item foundItem = Board.instance.GetItem(currentOrder.itemData);
            if (foundItem != null)
            {
                foundItem.currentSlot.currentItem = null;
                foundItem.transform.SetParent(null);

                // 1. UI 위치를 스크린 좌표로 변환
                Vector3 screenPos = RectTransformUtility.WorldToScreenPoint(itemImage.canvas.worldCamera, itemImage.rectTransform.position);
                // 2. 현재 아이템 위치 기준으로 Z 설정
                screenPos.z = Camera.main.WorldToScreenPoint(foundItem.transform.position).z;
                // 3. 스크린 -> 월드
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);

                Vector3 originalScale = foundItem.transform.localScale;
                Vector3 peakScale = originalScale * 1.3f;

                Sequence seq = DOTween.Sequence();

                // 위치 이동 먼저 시작 (1초 동안 부드럽게)
                seq.Append(foundItem.transform.DOMove(worldPos, 0.9f).SetEase(Ease.InOutSine));

                // 크기 키우기와 줄이기를 중간에 겹쳐서 실행
                seq.Insert(0.1f, foundItem.transform.DOScale(peakScale, 0.4f).SetEase(Ease.OutCubic));
                seq.Insert(0.4f, foundItem.transform.DOScale(originalScale, 0.4f).SetEase(Ease.InCubic));

                seq.OnComplete(() =>
                {
                    Destroy(foundItem.gameObject);
                    OrderManager.instance.CompleteCurrentOrder(this);
                });
            }
        }
        else
        {
            Debug.LogWarning("주문을 완료할 수 없습니다. 아이템이 보드에 없습니다.");
        }
    }
}
