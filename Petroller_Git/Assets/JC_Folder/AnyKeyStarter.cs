using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Valve.VR;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class AnyKeyStarter : MonoBehaviour
{
    // public SteamVR_Action_Boolean interactAction;
    // [SerializeField]private bool keyboard_debug;
    public UnityEvent OnAnyKeyEvent;
    public MyInputMap myInputMap;
    [SerializeField] private bool showLog = false;

    // void Update()
    // {
    //     // steam vr method    
    //     if (interactAction.GetStateDown(SteamVR_Input_Sources.Any) | (keyboard_debug && Input.anyKeyDown))
    //     {
    //         OnAnyKeyEvent?.Invoke();
    //         if (showLog) Debug.Log("Action 'interact' triggered!");
    //     }
    // }
    // void Update()
    // {
    //     if (OVRInput.GetDown(OVRInput.Button.Any))
    //     {
    //         Debug.Log("get any");
    //     }
    // }
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.Gameplay.Enable();
        myInputMap.Gameplay.AnyInput.started += ctx => OnAnyKey();
    }
    void OnDisable()
    {
        myInputMap.Gameplay.Disable();
    }

    void OnAnyKey()
    {
        OnAnyKeyEvent?.Invoke();
        if (showLog) Debug.Log("Action 'interact' triggered!");
    }
}
