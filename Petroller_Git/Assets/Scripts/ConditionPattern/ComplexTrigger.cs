using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ComplexTrigger : MonoBehaviour
{

    [Tooltip("要參考的玩家 (或其他目標)")] public Transform playerReference;

    public List<TriggerRule> rules;

    void Update()
    {
        foreach (var rule in rules)
        {
            rule.Tick(this.gameObject, playerReference, Time.deltaTime);
        }
    }
}
