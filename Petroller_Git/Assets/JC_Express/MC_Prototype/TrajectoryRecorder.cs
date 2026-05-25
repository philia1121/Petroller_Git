using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using System.Xml.XPath;
public class TrajectoryRecorder : MonoBehaviour
{
    public float recordInterval = 0.015f;
    public bool isRecording = false;
    public Material mat;
    public AudioSource audioSource;
    public float waitTime = 60;
    Coroutine soundCor;
    private TrajectorySession currentSession;
    private float startTime;
    bool visualTracked = true;
    bool startM = false;
    ControlMap controlMap;
    Transform VRMainCam;
    public HandSkeletonFinder[] skeletonFinder;
    public TrackingInfo trackingInfo;
    Coroutine cor;

    //added for making this into singleton
    public static TrajectoryRecorder instance;
    string filePrefix = "MultiTraj";
    string motionType = "";

    void Awake()
    {
        if (instance == null)
            instance = this;

        controlMap = new ControlMap();
        VRMainCam = Camera.main.transform;
        trackingInfo = FindFirstObjectByType<TrackingInfo>();
        if (!trackingInfo) trackingInfo = gameObject.AddComponent<TrackingInfo>();
    }
    void OnEnable()
    {
        controlMap.Prototype.Enable();
        controlMap.Prototype.RecordButton.started += ctx => ToggleRecording();
        controlMap.Prototype.SayLT.started += ctx => SetVisualTracked(false);
        controlMap.Prototype.SayBK.started += ctx => SetVisualTracked(true);
        controlMap.Prototype.SayStart.started += ctx => SetMotionStart(true);
    }

    public void ToggleRecording()
    {
        isRecording = !isRecording;
        if (isRecording)
        {
            if (mat) mat.color = Color.red;
            StartNewSession();
            if (cor != null) StopCoroutine(cor);
            cor = StartCoroutine(RecordRoutine());
            soundCor = StartCoroutine(SoundTimer(waitTime));
            Debug.Log("start recording");
        }
        else
        {
            if (mat) mat.color = Color.white;
            if (cor != null) StopCoroutine(cor);
            if (soundCor != null) StopCoroutine(soundCor);
            SaveToFile();
            Debug.Log("stop recording");

            ResetAll();
        }
    }
    void ResetAll()
    {
        visualTracked = true;
        startM = false;
    }

    public void StartRecording()
    {
        if (!isRecording)
        {
            isRecording = !isRecording;
            if (mat) mat.color = Color.red;
            StartNewSession();
            if (cor != null) StopCoroutine(cor);
            cor = StartCoroutine(RecordRoutine());
            Debug.Log("start recording");
        }
    }
    public void StopRecording()
    {
        if (isRecording)
        {
            isRecording = !isRecording;
            if (mat) mat.color = Color.white;
            if (cor != null) StopCoroutine(cor);
            SaveToFile();
            Debug.Log("stop recording");
        }
    }

    IEnumerator RecordRoutine()
    {
        while (isRecording)
        {
            OnRecord();
            yield return new WaitForSeconds(recordInterval);
        }
    }
    void StartNewSession()
    {
        currentSession = new TrajectorySession();
        currentSession.motionType = motionType;
        startTime = Time.time;
    }
    void OnRecord()
    {
        // Time Stamp
        float timeSinceStart = Time.time - startTime;
        MultiTrackWaypoint wp = new MultiTrackWaypoint();
        wp.timestamp = timeSinceStart;

        // Left Controller
        wp.pos_LCont = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        wp.rot_LCont = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
        wp.pos_LPalm = skeletonFinder[0].MidFingerTransfrom ? skeletonFinder[0].MidFingerTransfrom.position : Vector3.zero;
        wp.rot_LPalm = skeletonFinder[0].MidFingerTransfrom ? skeletonFinder[0].MidFingerTransfrom.rotation : Quaternion.identity;

        // Right Controller
        wp.pos_RCont = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        wp.rot_RCont = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
        wp.pos_RPalm = skeletonFinder[1].MidFingerTransfrom ? skeletonFinder[1].MidFingerTransfrom.position : Vector3.zero;
        wp.rot_RPalm = skeletonFinder[1].MidFingerTransfrom ? skeletonFinder[1].MidFingerTransfrom.rotation : Quaternion.identity;

        // Left Hand
        wp.pos_LHand = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
        wp.rot_LHand = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);

        // Right Hand
        wp.pos_RHand = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
        wp.rot_RHand = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand);

        // HMD
        wp.pos_HMD = VRMainCam.position;
        wp.rot_HMD = VRMainCam.rotation;

        // Tracking State
        wp.RHand_PosTracked = trackingInfo.Get_RHand_PosTracked();
        wp.RHand_RotTracked = trackingInfo.Get_RHand_RotTracked();
        wp.RCont_PosTracked = trackingInfo.Get_RController_PosTracked();
        wp.RCont_RotTracked = trackingInfo.Get_RController_RotTracked();
        wp.LHand_PosTracked = trackingInfo.Get_LHand_PosTracked();
        wp.LHand_RotTracked = trackingInfo.Get_LHand_RotTracked();
        wp.LCont_PosTracked = trackingInfo.Get_LController_PosTracked();
        wp.LCont_RotTracked = trackingInfo.Get_RController_RotTracked();
        wp.VisualTracked = visualTracked;
        wp.startMotion = startM;

        currentSession.waypoints.Add(wp);
    }
    public void SetVisualTracked(bool value)
    {
        visualTracked = value;
    }
    public bool GetVisualTracked() { return visualTracked; }
    public void SetMotionStart(bool value)
    {
        startM = true;
    }
    void SaveToFile()
    {
        string json = JsonUtility.ToJson(currentSession, true);
        string path = Path.Combine(Application.persistentDataPath, $"{filePrefix}_{System.DateTime.Now:yyyyMMdd_HHmmss}.json");
        File.WriteAllText(path, json);
        Debug.Log($"File saved at : {path}");
    }
    public void SetFilePrefix(string prefix)
    {
        if (prefix == null) return;
        filePrefix = prefix;
    }
    public string GetFilePrefix() { return filePrefix; }
    public void SetMotionType(string motion)
    {
        if (motion == null) return;
        motionType = motion;
    }

    IEnumerator SoundTimer(float time)
    {
        yield return new WaitForSeconds(time);
        audioSource.Play();
    }


}
