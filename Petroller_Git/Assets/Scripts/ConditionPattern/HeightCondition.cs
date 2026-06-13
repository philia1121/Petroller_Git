using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Condition_Height", menuName = "JC/Conditions/Height")]
public class HeightCondition : BaseCondition
{
    private PetrollerObjectInfo cachedInfo;
    public float heightThreshold = 0;

    enum Comparison
    {
        Higher, Lower
    }
    [SerializeField] private Comparison comparison = Comparison.Higher;
    public override bool IsMet(GameObject owner, Transform player)
    {
        if (cachedInfo == null)
        {
            cachedInfo = owner.GetComponent<PetrollerObjectInfo>();
        }
        if (cachedInfo == null) return false;

        switch (comparison)
        {
            case Comparison.Higher:
                return cachedInfo.PetrollerTransform.position.y > heightThreshold;
            case Comparison.Lower:
                return cachedInfo.PetrollerTransform.position.y < heightThreshold;
            default:
                return false;
        }
    }
}
