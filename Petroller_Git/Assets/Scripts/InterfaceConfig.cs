using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
public class InterfaceConfig : MonoBehaviour
{
    public bool LT_Compensation = true;
    public bool LifeBeing = true;
    void Awake()
    {
        var initializables = FindObjectsOfType<MonoBehaviour>().OfType<IConfigInitializable>();
        foreach (var item in initializables)
        {
            item.Initialize_LTCompensation(LT_Compensation);
            item.Initialize_LifeBeing(LifeBeing);
        }
    }
}

public interface IConfigInitializable
{
    void Initialize_LTCompensation(bool LT_Compensation);
    void Initialize_LifeBeing(bool LifeBeing);
}
