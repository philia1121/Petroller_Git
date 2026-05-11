using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class InfoBoard : MonoBehaviour
{
    public AutoLogManager logManager;
    public PetrollerObjectInfo petrollerInfo;
    public GameFlowManager gameFlowManager;
    public TextMeshProUGUI Timer;
    public TextMeshProUGUI CountDown;
    public Image RH_stateIcon;
    public Image ObserverState, ParticipantState;

    void Awake()
    {
        logManager = FindFirstObjectByType<AutoLogManager>();
        petrollerInfo = FindFirstObjectByType<PetrollerObjectInfo>();
        gameFlowManager = FindObjectOfType<GameFlowManager>();
    }
    void Update()
    {
        Timer.text = Time.time.ToString("0.00");
        CountDown.text = gameFlowManager.CountDown.ToString("0");
        ObserverState.color = logManager.Observer_Tracked ? Color.green : Color.red;
        ParticipantState.color = logManager.Participant_Tracked ? Color.green : Color.red;
        RH_stateIcon.color = ShowSystemState(petrollerInfo.CurrentTrackingState);
    }
    Color ShowSystemState(PetrollerObjectInfo.TrackingStatus trackingStatus)
    {
        switch (trackingStatus)
        {
            case PetrollerObjectInfo.TrackingStatus.Tracked:
                return Color.green;
            case PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked:
                return Color.yellow;
            case PetrollerObjectInfo.TrackingStatus.LostTracked:
                return Color.red;
            default:
                return Color.white;
        }
    }
}
