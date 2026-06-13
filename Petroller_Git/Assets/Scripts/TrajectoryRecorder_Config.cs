using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryRecorder_Config : MonoBehaviour
{
    [SerializeField] private bool setOnEnable = false;
    [SerializeField] private NamingType namingType = NamingType.Custom;
    [SerializeField] private string filePrefix = "MultiTraj";
    [SerializeField] private string userName = "userN";
    [SerializeField] private string motionType = "Undefined";

    public static TrajectoryRecorder_Config instance;
    void OnEnable()
    {
        if (!setOnEnable) return;

        if (instance == null)
            instance = this;

        string fPrefix = "";
        switch (namingType)
        {
            case NamingType.Custom:
                fPrefix = (filePrefix == null) ? "" : filePrefix;
                break;
            case NamingType.TimeLog:
                fPrefix = System.DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
                break;
            case NamingType.Guid:
                fPrefix = System.Guid.NewGuid().ToString();
                break;
            case NamingType.UserName:
                fPrefix = (userName == null) ? "" : userName;
                break;
        }
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetFilePrefix(fPrefix);
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetUserName(userName);
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetMotionType(motionType);
    }
    public void ChangeConfig(string prefix)
    {
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetFilePrefix(prefix);
    }
    public void ChangeConfigRandom()
    {
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetFilePrefix(System.Guid.NewGuid().ToString());
    }
    public void ChangeMotionType(string motion)
    {
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetMotionType(motion);
    }
    public void ChangeUserName(string name)
    {
        if (TrajectoryRecorder.instance) TrajectoryRecorder.instance.SetUserName(name);
    }
}
