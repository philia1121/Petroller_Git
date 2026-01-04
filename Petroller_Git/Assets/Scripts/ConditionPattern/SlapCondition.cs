using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Condition_Slap", menuName = "JC/Conditions/Slap")]
public class SlapCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public string targetZoneID = "";
    public float minSlapSpeed = 1.5f;

    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponentInParent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        int currentFrame = Time.frameCount;
        int frameDiff = currentFrame - cachedInfo.LastImpactFrame;

        if (frameDiff < 0 || frameDiff > 2)
        {
            return false;
        }

        if (cachedInfo.LastImpactZoneID != targetZoneID)
        {
            return false;
        }

        if (cachedInfo.LastImpactSpeed < minSlapSpeed)
        {
            return false;
        }

        return true;
    }
}
