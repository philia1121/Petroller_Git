using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_PlayerRange", menuName = "JC/Conditions/PlayerRange")]
public class PlayerRangeCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public bool specialRange = true;
    [HideInInspector] public Transform rangeCenter;
    public float triggerRadius = 0.4f;
    bool inRange;
    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        if (!specialRange && rangeCenter == null && Camera.main != null)
        {
            rangeCenter = Camera.main.transform;
        }
        if (specialRange && rangeCenter == null) return false;

        Vector3 headPos = specialRange ? rangeCenter.position : player.position;
        Vector3 objPos = cachedInfo.transform.position;

        Vector3 headPosFlat = new Vector3(headPos.x, 0, headPos.z);
        Vector3 objPosFlat = new Vector3(objPos.x, 0, objPos.z);

        float horizontalDistance = Vector3.Distance(headPosFlat, objPosFlat);
        // Debug.Log(horizontalDistance);

        if (horizontalDistance < triggerRadius)
        {
            inRange = true;
        }
        else
        {
            // 加上 0.05f 的緩衝區，避免在邊界閃爍
            if (inRange && horizontalDistance > (triggerRadius + 0.05f))
            {
                inRange = false;
            }
        }
        return inRange;
    }
}
