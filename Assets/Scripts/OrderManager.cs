using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

// OrderManager.cs
// 주문을 관리하는 클래스
public class OrderManager : MonoBehaviour
{
    public Queue<OrderData> orderQueue = new Queue<OrderData>();
    public int initialOrderCount = 5; // 초기 주문 수량

    [SerializeField] private GameObject orderPanelPrefab;
    [SerializeField] private Transform orderPanelParent;

    private List<OrderPanelUI> orderPanels = new List<OrderPanelUI>();
    private const int maxVisibleOrders = 3; // 화면에 보일 최대 주문 수


    // Start is called before the first frame update
    void Start()
    {
        GenerateInitialOrders();
        DisplayInitialOrders();
    }

    // 초기 주문 생성
    private void GenerateInitialOrders()
    {
        List<ItemData> allItems = GameManager.instance.itemManager.GetAllItemData()
            .Where(item => item.itemLevel > 1)
            .ToList();

        // 중복 방지를 위한 HashSet
        HashSet<ItemData> usedItems = new HashSet<ItemData>();

        int attempts = 0;
        int maxAttempts = 100;

        while (orderQueue.Count < initialOrderCount && attempts < maxAttempts)
        {
            attempts++;

            ItemData randomItem = allItems[Random.Range(0, allItems.Count)];

            if (usedItems.Contains(randomItem))
            {
                continue;
            }

            usedItems.Add(randomItem);

            int reward = GetRewardByItemLevel(randomItem.itemLevel);

            OrderData newOrder = new OrderData
            {
                itemData = randomItem,
                reward = reward
            };

            orderQueue.Enqueue(newOrder);
        }
    }

    // 초기 주문을 화면에 표시
    private void DisplayInitialOrders()
    {
        for (int i = 0; i < maxVisibleOrders; i++)
        {
            if (orderQueue.Count == 0) break;

            OrderData order = orderQueue.Dequeue();

            GameObject orderPanelObj = Instantiate(orderPanelPrefab, orderPanelParent);
            OrderPanelUI orderPanel = orderPanelObj.GetComponent<OrderPanelUI>();
            orderPanel.SetOrder(order);

            orderPanels.Add(orderPanel);
        }
    }

    private int GetRewardByItemLevel(int level)
    {
        switch (level)
        {
            case 1: return 100;
            case 2: return 300;
            case 3: return 500;
            case 4: return 700;
            case 5: return 1000;
            case 6: return 1500;
            case 7: return 2000;
            default: return 0;
        }
    }

    // 주문 완료 시 호출되는 메서드
    public void CompleteCurrentOrder(OrderPanelUI orderPanel)
    {
        if (orderQueue.Count > 0)
        {
            orderPanel.transform.DOScale(Vector3.zero, 0.7f).SetEase(Ease.InBack).OnComplete(() =>
            {
                OrderData nextOrder = orderQueue.Dequeue();
                orderPanel.SetOrder(nextOrder);

                orderPanel.transform.localScale = Vector3.zero;
                orderPanel.transform.DOScale(Vector3.one, 1f).SetEase(Ease.OutBack);
                CheckAllOrders();
            });
        }
        else
        {
            orderPanel.gameObject.SetActive(false);
            orderPanels.Remove(orderPanel);
        }
    }

    // 모든 주문 패널을 체크하여 아이템이 보드에 있는지 확인
    public void CheckAllOrders()
    {
        foreach (OrderPanelUI panel in orderPanels)
        {
            panel.CheckItemInBoard();
        }
    }
}
