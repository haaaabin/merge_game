using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Slot[,] slots; 

    void Start()
    {
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
}
