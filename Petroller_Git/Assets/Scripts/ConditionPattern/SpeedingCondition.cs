using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_Speeding", menuName = "JC/Conditions/Speeding")]

public class SpeedingCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public float speedingThreshold = 3f;

    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        if (cachedInfo.IsMoving)
        {
            return cachedInfo.Speed > speedingThreshold;
        }
        else
        {
            return false;
        }
    }
}
