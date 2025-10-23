using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_IsMoving", menuName = "JC/Conditions/Is Moving")]
public class IsMovingCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;

    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        // 直接讀取狀態，非常乾淨！
        return cachedInfo.CurrentMovementState == PetrollerObjectInfo.MovementState.Moving;
    }
}
