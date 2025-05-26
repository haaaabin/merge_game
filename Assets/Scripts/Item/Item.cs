using DG.Tweening;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    protected Vector3 offset; // 아이템과 마우스 포인터 간의 거리
    protected bool isDragging = false;
    protected SpriteRenderer spriteRenderer;
    protected float lastClickTime;
    protected const float doubleClickThreshold = 0.3f; // 더블 클릭 간격
    protected Vector3 originalScale;
    protected ItemInfoUI itemInfoUI;

    public Slot currentSlot;
    public ItemData itemData;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        itemInfoUI = FindObjectOfType<ItemInfoUI>();
    }

    protected virtual void OnMouseDown()
    {
        if (currentSlot != null)
        {
            currentSlot.currentItem = null;
        }

        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

    }

    protected virtual void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    protected virtual void OnMouseUp()
    {
        isDragging = false;

        if (originalScale == Vector3.zero)
        {
            originalScale = transform.localScale;
        }

        transform.DOScale(originalScale * 1.2f, 0.3f).OnComplete(() =>
        {
            transform.DOScale(originalScale, 0.2f);
        });

        // 아이템 정보 표시
        DisplayItemInfo();

        OnMouseUpExtended();
    }

    protected virtual void OnMouseUpExtended() { }

    protected void MoveToSlot(Slot slot)
    {
        // 드랍한 슬롯에 아이템을 배치
        transform.position = slot.transform.position;
        transform.parent = slot.transform;

        // 현재 슬롯 비워주기
        if (currentSlot != null)
        {
            currentSlot.currentItem = null;
        }

        slot.currentItem = this;
        currentSlot = slot;
    }

    protected void SwapWithItem(Item otherItem, Slot nearestSlot)
    {
        Slot originalSlot = currentSlot;

        // 위치 교환
        otherItem.transform
            .DOMove(originalSlot.transform.position, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 정확한 슬롯 위치로 정렬 (부드럽게 스냅)
                otherItem.transform.position = originalSlot.transform.position;
            });

        transform.DOMove(nearestSlot.transform.position, 0.3f)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // 정확한 슬롯 위치로 정렬 (부드럽게 스냅)
                transform.position = nearestSlot.transform.position;
            });

        // 슬롯 정보 업데이트
        otherItem.transform.parent = originalSlot.transform;
        transform.parent = nearestSlot.transform;

        nearestSlot.currentItem = this;
        currentSlot = nearestSlot;

        originalSlot.currentItem = otherItem;
        otherItem.currentSlot = originalSlot;
    }

    protected void ReturnToOriginalPosition()
    {
        if (currentSlot != null)
        {
            transform.position = currentSlot.transform.position;
            transform.parent = currentSlot.transform;
        }
    }

    protected void DisplayItemInfo()
    {
        if (itemInfoUI != null)
        {
            itemInfoUI.ShowItemInfo(this);
        }
        else
        {
            Debug.LogWarning("ItemInfoUI를 찾을 수 없읍");
        }
    }

    protected Vector3 GetMouseWorldPos()
    {
        Vector3 mousePos = Input.mousePosition;

        // 현재 오브젝트가 카메라에서 얼마나 떨어져 있는지
        mousePos.z = Camera.main.WorldToScreenPoint(transform.position).z;

        // 마우스 위치를 MainCamera 기준 월드 좌표로 변환
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        return worldPos;
    }

    // 현재 아이템의 위치와 가장 가까운 슬롯을 찾습니다.
    protected Slot FindClosestSlot()
    {
        Slot closestSlot = null;
        float closestDistance = Mathf.Infinity;  // 가장 가까운 슬롯과의 거리 초기화

        foreach (Slot slot in Board.instance.GetAllSlots())
        {
            float distance = Vector3.Distance(transform.position, slot.transform.position);
            if (distance < closestDistance && distance < 1)
            {
                closestDistance = distance;
                closestSlot = slot;
            }
        }

        return closestSlot;
    }

    public virtual string GetItemDescription()
    {
        return $"{itemData.itemName}/Lv.{itemData.itemLevel} : 합쳐서 다음 레벨에 도달하세요.";
    }

    public bool IsFoodItem(Item item)
    {
        return item.itemData.itemType == ItemType.Bread ||
               item.itemData.itemType == ItemType.Drink ||
               item.itemData.itemType == ItemType.Rice;
    }

    public void PlayExplodeEffectAndDestroy()
    {
        // 스케일을 0으로 줄이고 투명도도 0으로 만들어서 사라지게 함
        spriteRenderer.DOFade(0f, 0.5f);
        transform.DOScale(Vector3.zero, 0.3f).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}