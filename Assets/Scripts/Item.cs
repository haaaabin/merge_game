using UnityEngine;

public class Item : MonoBehaviour
{
    private Vector3 offset; // 아이템과 마우스 포인터 간의 거리
    private bool isDragging = false; // 드래그 상태를 나타내는 변수
    private SpriteRenderer spriteRenderer;
    public Slot currentSlot; // 현재 아이템이 위치한 슬롯
    private Board board;
    private RectTransform dragLimitArea;
    public ItemData itemData;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        board = FindObjectOfType<Board>();
        dragLimitArea = GameObject.Find("DragLimitArea").GetComponent<RectTransform>();
    }

    private void OnMouseDown()
    {
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
            // 슬롯이 비어있는지 확인
            if (nearestSlot.currentItem == null)
            {
                // 드랍한 슬롯에 아이템을 배치
                transform.position = nearestSlot.transform.position;
                transform.parent = nearestSlot.transform;

                // 현재 슬롯 비워주기
                if (currentSlot != null)
                {
                    currentSlot.currentItem = null;
                }

                nearestSlot.currentItem = this;
                currentSlot = nearestSlot;

                Debug.Log("아이템이 슬롯에 배치되었습니다: " + nearestSlot.name);
            }
            // 슬롯이 비어있지 않은 경우 아이템 스왑
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
                    // 종류나 레벨이 다르면 위치 교환
                    Slot originalSlot = currentSlot;

                    // 위치 교환
                    otherItem.transform.position = originalSlot.transform.position;
                    otherItem.transform.parent = originalSlot.transform;

                    transform.position = nearestSlot.transform.position;
                    transform.parent = nearestSlot.transform;

                    // 슬롯 정보 업데이트
                    nearestSlot.currentItem = this;
                    currentSlot = nearestSlot;

                    originalSlot.currentItem = otherItem;
                    otherItem.currentSlot = originalSlot;
                }
            }
        }
        else
        {
            // 원래 위치로 되돌리기
            if (currentSlot != null)
            {
                transform.position = currentSlot.transform.position;
                transform.parent = currentSlot.transform;
            }
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