using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Condition_HandInteraction", menuName = "JC/Conditions/HandInteraction")]
public class HandInteractionCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public HandInteraction targetType;
    public bool triggerOnFrameOnly = true;

    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
            cachedInfo = owner.GetComponentInParent<PetrollerObjectInfo>();

        if (cachedInfo == null) return false;

        // 1. 檢查類別是否符合
        if (cachedInfo.CurrentInteraction != targetType) return false;

        // 2. 如果勾選了 "只在觸發幀"，檢查幀數
        if (triggerOnFrameOnly)
        {
            return Time.frameCount == cachedInfo.LastInteractionFrame;
        }

        return true;
    }
}


