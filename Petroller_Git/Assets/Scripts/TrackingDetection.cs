using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TrackingDetection : MonoBehaviour
{
    MyInputMap inputMap;
    public string usedMethod = "A";
    void Awake()
    {
        inputMap = new MyInputMap();
    }
    void Onable()
    {
        inputMap.VRBasic.Enable();
        inputMap.VRBasic.LH_TrackState.started += GetTrackState_LH;
        inputMap.VRBasic.RH_TrackState.started += GetTrackState_RH;
    }
    void ODisable()
    {
        inputMap.VRBasic.Disable();
        inputMap.VRBasic.LH_TrackState.started -= GetTrackState_LH;
        inputMap.VRBasic.RH_TrackState.started -= GetTrackState_RH;
    }
    void GetTrackState_LH(InputAction.CallbackContext ctx)
    {
        bool isTracked = (ctx.ReadValue<float>() > 0.5f)? true : false; 
        if(isTracked)
        {
            Debug.Log("LH XR Controller isTracked: TRUE");
        }
        else
        {
            Debug.Log("LH XR Controller isTracked: FALSE");
        }
    }
    void GetTrackState_RH(InputAction.CallbackContext ctx)
    {
        bool isTracked = (ctx.ReadValue<float>() > 0.5f)? true : false; 
        if(isTracked)
        {
            Debug.Log("RH XR Controller isTracked: TRUE");
        }
        else
        {
            Debug.Log("RH XR Controller isTracked: FALSE");
        }
    }


    void Start()
    {
        OVRManager.TrackingAcquired += OnTrackingAcquired;
        OVRManager.TrackingLost += OnTrackingLost;

    }

    // Update is called once per frame
    void Update()
    {
        switch(usedMethod)
        {
            case "A":
                MethodA();
                break;
            case "B":
                MethodB();
                break;
            case "C":
                MethodC();
                break;
        }
    }

    void MethodA()
    {
        OVRInput.Controller activeController = OVRInput.GetActiveController();
        
        if(activeController == OVRInput.Controller.None)
        {
            Debug.Log("Contorller Lost Tracknig");
        }
        else
        {
            Debug.Log("COntorller Tracked: " + activeController);
        }
    } 
    void MethodB()
    {
        bool leftTracked = OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
        bool rightTracked = OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch);

        Debug.Log($"Left Controller Tracked: {leftTracked}, Right Controller Tracked: {rightTracked}");
    }
    void MethodC()
    {
        bool leftValid = OVRInput.GetControllerPositionValid(OVRInput.Controller.LTouch);
        bool rightValid = OVRInput.GetControllerPositionValid(OVRInput.Controller.RTouch);

        Debug.Log($"Left Controller Position Valid: {leftValid}, Right Controller Position Valid: {rightValid}");
    }

    void OnTrackingAcquired()
    {
        Debug.Log("Method D: Tracking Restored!");
    }
    void OnTrackingLost()
    {
        Debug.Log("Method D: Tracking Lost!");
    }
    void OnDestroy()
    {
        OVRManager.TrackingAcquired -= OnTrackingAcquired;
        OVRManager.TrackingLost -= OnTrackingLost;
    }
}
