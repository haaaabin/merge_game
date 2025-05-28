using UnityEngine;

public class Slot : MonoBehaviour
{
    public Vector2Int gridPos; // (x,y) 위치
    public Item currentItem;
    public GameObject selectionOutline;
    public GameObject effectPrefab;

    public bool isEmpty => currentItem == null;
}
