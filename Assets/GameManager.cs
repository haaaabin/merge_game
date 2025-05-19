using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public ItemManager itemManager;
    public OrderManager orderManager;
    public CoinManager coinManager;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        itemManager = GetComponent<ItemManager>();
        orderManager = GetComponent<OrderManager>();
        coinManager = GetComponent<CoinManager>();
    }
}
