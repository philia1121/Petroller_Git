using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class YellCount : MonoBehaviour
{
    public int countNum = 0;
    public int countMax = 3;
    public UnityEvent ReachCount, ResetAll;
    public void Count()
    {
        countNum += 1;
        CheckCount();
    }
    public void CheckCount()
    {
        if (countNum == countMax)
        {
            ReachCount.Invoke();
        }
    }
    public void Reset()
    {
        countNum = 0;
        ResetAll.Invoke();
    }

}
