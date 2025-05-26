using UnityEngine;
using DG.Tweening;

public class MirrorItem : Item
{
    protected override void OnMouseUpExtended()
    {
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
                if (IsFoodItem(otherItem))
                {
                    if (GameManager.instance.itemManager.TryCloneItem(otherItem))
                    {
                        PlayExplodeEffectAndDestroy();
                    }
                    else
                    {
                        SwapWithItem(otherItem, nearestSlot);
                    }
                }
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }


    public override string GetItemDescription()
    {
        return $"{itemData.itemName}/Lv.{itemData.itemLevel} : 아이템에 합치면 2개로 복사됩니다. 합쳐서 다음 레벨에 도달하세요.";
    }
}