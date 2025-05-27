using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [HideInInspector] public ItemManager itemManager;
    [HideInInspector] public OrderManager orderManager;
    [HideInInspector] public ResourceManager resourceManager;

    public Item selectedItem;

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
        resourceManager = GetComponent<ResourceManager>();
    }
}
