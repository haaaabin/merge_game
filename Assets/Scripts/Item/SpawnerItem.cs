using UnityEngine;

public class SpawnerItem : Item
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

    protected override void OnMouseUpExtended()
    {
        // 드랍한 위치에서 가장 가까운 슬롯 찾기
        Slot nearestSlot = FindClosestSlot();
        if (nearestSlot != null)
        {
            if (nearestSlot.currentItem == null)
            {
                MoveToSlot(nearestSlot);
            }
            else
            {
                Item otherItem = nearestSlot.currentItem;
                if (GameManager.instance.itemManager.CanMerge(this, otherItem))
                {
                    GameManager.instance.itemManager.MergeItems(this, otherItem);
                }
                else
                {
                    SwapWithItem(otherItem, nearestSlot);
                }
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }
}