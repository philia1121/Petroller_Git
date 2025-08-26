using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class NewInputEventCaller : MonoBehaviour
{
    public MyInputMap myInputMap;
    public UnityEvent OnMoveAKeyEvent, OnMoveBKeyEvent, OnMoveCKeyEvent, OnResetEvent;
    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.Ellie.Enable();
        myInputMap.Ellie.MoveA.started += ctx => OnMoveAKeyEvent.Invoke();
        myInputMap.Ellie.MoveB.started += ctx => OnMoveBKeyEvent.Invoke();
        myInputMap.Ellie.MoveC.started += ctx => OnMoveCKeyEvent.Invoke();
        myInputMap.Ellie.Reset.started += ctx => OnResetEvent.Invoke();
    }
    void OnDisable()
    {
        myInputMap.Ellie.Disable();
        myInputMap.Ellie.MoveA.started -= ctx => OnMoveAKeyEvent.Invoke();
        myInputMap.Ellie.MoveB.started -= ctx => OnMoveBKeyEvent.Invoke();
        myInputMap.Ellie.MoveC.started -= ctx => OnMoveCKeyEvent.Invoke();
        myInputMap.Ellie.Reset.started -= ctx => OnResetEvent.Invoke();
    }
}
