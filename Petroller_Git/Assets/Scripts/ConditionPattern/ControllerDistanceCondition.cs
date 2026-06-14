using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Oculus.Interaction;
using UnityEngine;

[CreateAssetMenu(fileName = "Condition_ControllerDistance", menuName = "JC/Conditions/Controller Distance")]
public class ControllerDistanceCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    enum Comparison
    {
        Bigger, Smaller
    }
    [SerializeField]
    private Comparison comparison = Comparison.Bigger;
    public float distanceThreshold = 0.1f;


    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        var posR = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        var posL = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        var dist = Vector3.Distance(posR, posL);
        return comparison == Comparison.Bigger ? dist > distanceThreshold : dist < distanceThreshold;
    }
}
