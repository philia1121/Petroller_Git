using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class AnimationEvent : MonoBehaviour
{
    [SerializeField] private int targetNumber;
    public UnityEvent AnimationTriggerEvent;
    public UnityEvent<int> AnimationTriggerNumEvent; // event with trigger number
    [SerializeField] private bool showLog = false;
    void AnimationNumberEvent(int i) //Animaiton > Animation Clip > Events > Function
    {
        if (targetNumber != 0 && i == targetNumber)
        {
            AnimationTriggerEvent.Invoke();
            AnimationTriggerNumEvent.Invoke(targetNumber);
            if (showLog) Debug.Log("Trigger On Animation Event: with number " + targetNumber);
        }
    }
    public void ChangeTargetNumber(int value)
    {
        targetNumber = value;
    }
}
