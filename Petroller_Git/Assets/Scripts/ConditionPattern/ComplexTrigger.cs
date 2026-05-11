using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ComplexTrigger : MonoBehaviour
{
    public GameObject petrollerObject;
    public Transform playerReference;

    public List<TriggerRule> rules;
    void Start()
    {
        if (!petrollerObject) petrollerObject = FindFirstObjectByType<PetrollerObjectInfo>().gameObject;
        if (playerReference == null && Camera.main != null)
        {
            playerReference = Camera.main.transform;
        }
    }
    void Update()
    {
        foreach (var rule in rules)
        {
            if (!rule.Ignore) rule.Tick(petrollerObject, playerReference, Time.deltaTime);
        }
    }
}
