using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutoLogManager : MonoBehaviour, IConfigInitializable
{
    public bool auto = true;
    public float LogInterval = 1;
    public bool Participant_Tracked = true;
    public bool Observer_Tracked = true;
    public bool Participant_Changed = false;
    public bool Observer_Changed = false;
    public bool RTouch_Tracked, LTouch_Tracked;
    Coroutine cor;
    bool LT_C;
    bool Life;
    void Start()
    {
        if (auto)
            CSVWriter.CSV_WriteTableTitle("Real World Time,Game World Time,Participant,Observer,RTouch");
        if (cor != null) StopCoroutine(cor);
        cor = StartCoroutine(AutoLog());
    }
    void OnEnable()
    {
        GameSignals.OnRequestStartGame += StartAutoLog;
        GameSignals.OnRequestEndGame += StopAutoLog;
    }
    void OnDisable()
    {
        GameSignals.OnRequestStartGame -= StartAutoLog;
        GameSignals.OnRequestEndGame -= StopAutoLog;
    }
    public void Initialize_LTCompensation(bool value)
    {
        LT_C = value;
    }
    public void Initialize_LifeBeing(bool value)
    {
        Life = value;
    }

    public void StartAutoLog()
    {
        auto = true;
        CSVWriter.CSV_WriteTableTitle((LT_C ? "LT_Compensation" : "None") + "," + (Life ? "Cat" : "Stone"));
        CSVWriter.CSV_WriteTableTitle("");
        CSVWriter.CSV_WriteTableTitle("Real World Time,Game World Time,Participant,Observer,RTouch");
        if (cor != null) StopCoroutine(cor);
        cor = StartCoroutine(AutoLog());
    }
    public void StopAutoLog()
    {
        auto = false;
        if (cor != null) StopCoroutine(cor);
        cor = null;
    }

    IEnumerator AutoLog()
    {
        while (auto)
        {
            string participant = Participant_Tracked ? "Tracked" : "Lost";
            string observer = Observer_Tracked ? "Tracked" : "Lost";
            participant += Participant_Changed ? " O," : ",";
            observer += Observer_Changed ? " O," : ",";
            Participant_Changed = false;
            Observer_Changed = false;
            string R = OVRInput.GetControllerPositionTracked(OVRInput.Controller.RTouch) ? "Tracked," : "Lost,";
            string data = participant + observer + R;
            CSVWriter.CSV_WriteByTime(true, true, data);

            yield return new WaitForSeconds(LogInterval);
        }
        yield return null;
    }
    public void ParticipantTrackedReport(bool tracked)
    {
        Participant_Tracked = tracked;
        Participant_Changed = true;
    }
    public void ObserverTrackedReport(bool tracked)
    {
        Observer_Tracked = tracked;
        Observer_Changed = true;
    }
}
