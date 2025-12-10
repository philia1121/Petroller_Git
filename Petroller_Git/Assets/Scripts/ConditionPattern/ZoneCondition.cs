using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_Zone", menuName = "JC/Conditions/Zone")]
public class ZoneCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public string targetZoneID = "Goal";
    public bool checkForIn = true;
    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        bool isInZone = cachedInfo.IsInZone(targetZoneID);
        return checkForIn? isInZone : !isInZone;
    }
}
