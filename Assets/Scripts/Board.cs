using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefab;
    private Slot[,] slots;

    public GameObject[] itemPrefabs;

    void Start()
    {
        SetUp();

        Slot slot = GetSlot(new Vector2Int(2,2));
        GameObject itemObj = Instantiate(itemPrefabs[0], slot.transform.position, Quaternion.identity);

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

    public Slot GetSlot(Vector2Int gridPos)
    {
        if (gridPos.x < 0 || gridPos.x >= width || gridPos.y < 0 || gridPos.y >= height)
        {
            return null;
        }
        return slots[gridPos.x, gridPos.y];
    }

}
