using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public int itemLevel;
    public GameObject itemPrefab;
    public Sprite itemImage;
    public ItemType itemType;
}

public enum ItemType
{
    Bread,
    Drink,
    Rice,
    ItemSpawner,
    Item

}