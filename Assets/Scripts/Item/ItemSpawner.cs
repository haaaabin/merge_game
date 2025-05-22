using UnityEngine;

public class ItemSpawner : Item
{
    protected override void OnMouseDown()
    {
        base.OnMouseDown();

        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            GameManager.instance.itemManager.SpawnRandomLevel1Item();
        }
        
        lastClickTime = Time.time;
    }

    public override string GetItemDescription()
    {
        return $"{itemData.itemName}/Lv.{itemData.itemLevel} : 탭하여 아이템을 생성하세요.";
    }
}