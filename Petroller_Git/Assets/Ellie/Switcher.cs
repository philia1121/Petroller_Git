using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class Switcher : MonoBehaviour
{
    public MyInputMap myInputMap;
    public GameObject[] BG_Sets;
    int currentSet = 0;
    int currentMove = 0;
    public UnityEvent OnMoveAEvent, OnMoveBEvent, OnMoveCEvent;

    void Awake()
    {
        myInputMap = new MyInputMap();
    }
    void OnEnable()
    {
        myInputMap.Ellie.Enable();
        myInputMap.Ellie.SwitchBG.started += ctx => SwitchScene();
        myInputMap.Ellie.SwitchMove.started += ctx => SwitchAndTriggerMove();
    }
    void OnDisable()
    {
        myInputMap.Ellie.Disable();
        myInputMap.Ellie.SwitchBG.started -= ctx => SwitchScene();
        myInputMap.Ellie.SwitchMove.started -= ctx => SwitchAndTriggerMove();
    }
    void Start()
    {
        foreach (var item in BG_Sets)
        {
            item.SetActive(false);
        }
        BG_Sets[currentSet].SetActive(true);
    }
    public void SwitchScene()
    {
        BG_Sets[currentSet].SetActive(false);
        currentSet += 1;
        if (currentSet > BG_Sets.Length - 1) currentSet = 0;

        BG_Sets[currentSet].SetActive(true);
    }

    public void SwitchAndTriggerMove()
    {
        currentMove += 1;
        if (currentMove > 3) currentMove = 1;
        switch (currentMove)
        {
            case 1:
                OnMoveAEvent.Invoke();
                break;
            case 2:
                OnMoveBEvent.Invoke();
                break;
            case 3:
                OnMoveCEvent.Invoke();
                break;
        }
    }
}
