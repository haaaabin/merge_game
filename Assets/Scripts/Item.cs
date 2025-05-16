using DG.Tweening;
using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector3 offset; // 아이템과 마우스 포인터 간의 거리
    private bool isDragging = false; // 드래그 상태를 나타내는 변수
    private SpriteRenderer spriteRenderer;
    public Slot currentSlot; // 현재 아이템이 위치한 슬롯
    private Board board;
    public ItemData itemData;

    public bool isItemSpawner = false;
    private float lastClickTime;
    private const float doubleClickThreshold = 0.3f; // 더블 클릭 간격

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<Board>();
    }

    private void OnMouseDown()
    {
        if (isItemSpawner)
        {
            if (Time.time - lastClickTime < doubleClickThreshold)
            {
                ItemManager.instance.SpawnRandomLevel1Item();
            }

            lastClickTime = Time.time;
        }

        if (currentSlot != null)
        {
            currentSlot.currentItem = null;
        }

        offset = transform.position - GetMouseWorldPos();
        isDragging = true;

        spriteRenderer.sortingOrder = 10;
    }

    private void OnMouseDrag()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private void OnMouseUp()
    {
        isDragging = false;

        // 드랍한 위치에서 가장 가까운 슬롯 찾기
        Slot nearestSlot = FindClosetSlot();

        if (nearestSlot != null)
        {
            if (nearestSlot.currentItem == null)
            {
                MoveToSlot(nearestSlot);
            }
            else
            {
                Debug.Log("해당 슬롯에 이미 아이템이 있습니다: " + nearestSlot.name);

                Item otherItem = nearestSlot.currentItem;
                if (ItemManager.instance.CanMerge(this, otherItem))
                {
                    ItemManager.instance.MergeItems(this, otherItem);
                }
                else
                {
                    SwapWithItem(otherItem, nearestSlot);
                }
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    private void MoveToSlot(Slot slot)
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

        Debug.Log("아이템이 슬롯에 배치되었습니다: " + slot.name);
    }

    private void SwapWithItem(Item otherItem, Slot nearestSlot)
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

    private void ReturnToOriginalPosition()
    {
        if (currentSlot != null)
        {
            transform.position = currentSlot.transform.position;
            transform.parent = currentSlot.transform;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        // 마우스 포인터의 월드 좌표를 가져옵니다.
        // Camera.main.ScreenToWorldPoint() 메서드를 사용하여 스크린 좌표를 월드 좌표로 변환합니다.
        // Input.mousePosition은 스크린 좌표를 나타내므로, z축을 0으로 설정하여 2D 평면에서의 위치를 가져옵니다.
        // Camera.main은 현재 활성화된 카메라를 나타내며, ScreenToWorldPoint() 메서드는 스크린 좌표를 월드 좌표로 변환합니다.

        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = 0;
        return mousePoint;
    }

    // 현재 아이템의 위치와 가장 가까운 슬롯을 찾습니다.
    private Slot FindClosetSlot()
    {
        Slot closestSlot = null;
        float closestDistance = Mathf.Infinity;  // 가장 가까운 슬롯과의 거리 초기화

        foreach (Slot slot in board.GetAllSlots())
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
}