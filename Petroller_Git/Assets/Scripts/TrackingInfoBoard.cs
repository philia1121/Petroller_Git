using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TrackingInfoBoard : MonoBehaviour
{
    public TrackingInfo trackingInfo;
    public Image[] Pos_Signal, Rot_Signal;    // Start is called before the first frame update
    public Color[] SignalColors;
    public Image user;
    void Start()
    {
        if (!trackingInfo) trackingInfo = FindFirstObjectByType<TrackingInfo>();
        foreach (var signal in Pos_Signal) signal.color = SignalColors[0];
        foreach (var signal in Rot_Signal) signal.color = SignalColors[0];
        user.color = SignalColors[0];
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosSignal();
        UpdateRotSignal();
        UpdateUserSignal();
    }

    public void UpdatePosSignal()
    {
        Pos_Signal[0].color = trackingInfo.Get_LHand_PosTracked() ? SignalColors[1] : SignalColors[0];
        Pos_Signal[1].color = trackingInfo.Get_LController_PosTracked() ? SignalColors[1] : SignalColors[0];
        Pos_Signal[2].color = trackingInfo.Get_RController_PosTracked() ? SignalColors[1] : SignalColors[0];
        Pos_Signal[3].color = trackingInfo.Get_RHand_PosTracked() ? SignalColors[1] : SignalColors[0];
    }
    public void UpdatePosSignal(bool LH, bool LC, bool RC, bool RH)
    {
        Pos_Signal[0].color = LH ? SignalColors[1] : SignalColors[0];
        Pos_Signal[1].color = LC ? SignalColors[1] : SignalColors[0];
        Pos_Signal[2].color = RC ? SignalColors[1] : SignalColors[0];
        Pos_Signal[3].color = RH ? SignalColors[1] : SignalColors[0];
    }
    public void UpdateRotSignal()
    {
        Rot_Signal[0].color = trackingInfo.Get_LHand_RotTracked() ? SignalColors[1] : SignalColors[0];
        Rot_Signal[1].color = trackingInfo.Get_LController_RotTracked() ? SignalColors[1] : SignalColors[0];
        Rot_Signal[2].color = trackingInfo.Get_RController_RotTracked() ? SignalColors[1] : SignalColors[0];
        Rot_Signal[3].color = trackingInfo.Get_RHand_RotTracked() ? SignalColors[1] : SignalColors[0];
    }
    public void UpdateRotSignal(bool LH, bool LC, bool RC, bool RH)
    {
        Rot_Signal[0].color = LH ? SignalColors[1] : SignalColors[0];
        Rot_Signal[1].color = LC ? SignalColors[1] : SignalColors[0];
        Rot_Signal[2].color = RC ? SignalColors[1] : SignalColors[0];
        Rot_Signal[3].color = RH ? SignalColors[1] : SignalColors[0];
    }
    public void UpdateUserSignal()
    {
        user.color = trackingInfo.GetComponent<TrajectoryRecorder>().GetVisualTracked() ? SignalColors[1] : SignalColors[0];
    }
}
