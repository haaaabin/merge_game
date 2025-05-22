using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public static Board instance;
    public int width;
    public int height;
    public GameObject tilePrefab;
    public GameObject itemSpawnerPrefab;
    private Slot[,] slots;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetUp();
    }

    private void SetUp()
    {
        slots = new Slot[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject slotObj = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
                slotObj.transform.parent = this.transform;
                slotObj.name = "( " + i + ", " + j + " )";

                Slot slot = slotObj.GetComponent<Slot>();
                slot.gridPos = new Vector2Int(i, j);
                slots[i, j] = slot;
            }
        }

        GameObject itemSpawnerObj = Instantiate(itemSpawnerPrefab, new Vector3(0, 5), Quaternion.identity);
        itemSpawnerObj.transform.parent = this.transform;
        itemSpawnerObj.name = "ItemSpawner";

        Item itemSpawner = itemSpawnerObj.GetComponent<Item>();
        itemSpawner.currentSlot = slots[0, 5];
        itemSpawner.transform.parent = slots[0, 5].transform;
        slots[0, 5].currentItem = itemSpawner;
    }

    public List<Slot> GetAllSlots()
    {
        List<Slot> allSlots = new List<Slot>();
        foreach (Slot slot in slots)
        {
            allSlots.Add(slot);
        }
        return allSlots;
    }

    public List<Slot> GetEmptySlots()
    {
        List<Slot> emptySlots = new List<Slot>();
        foreach (Slot slot in slots)
        {
            if (slot.currentItem == null)
            {
                emptySlots.Add(slot);
            }
        }
        return emptySlots;
    }

    public bool HasItem(ItemData itemData)
    {
        foreach (var slot in slots)
        {
            if (slot.currentItem != null && slot.currentItem.itemData == itemData)
                return true;
        }
        return false;
    }

    public Item GetItem(ItemData itemData)
    {
        foreach (var slot in slots)
        {
            if (slot.currentItem != null && slot.currentItem.itemData == itemData)
                return slot.currentItem;
        }
        return null;
    }
}
