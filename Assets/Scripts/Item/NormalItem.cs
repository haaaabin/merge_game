using UnityEngine;

public class NormalItem : Item
{
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