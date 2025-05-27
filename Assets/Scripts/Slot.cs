using UnityEngine;

public class Slot : MonoBehaviour
{
    public Vector2Int gridPos; // (x,y) 위치
    public Item currentItem;

    public bool isEmpty => currentItem == null;
}
