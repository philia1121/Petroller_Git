using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerModelHandler : MonoBehaviour
{
    [SerializeField] private PetrollerObjectInfo petrollerInfo;
    [SerializeField] private bool autoInitializeModel;
    [SerializeField] private Transform modelTransform;
    public enum LostTrackedSolution { StayLastKnown, Fixed, SnapToCenter, SnapToRightHand, SnapToLeftHand, AsDefault }
    [SerializeField] private LostTrackedSolution solution = LostTrackedSolution.StayLastKnown;
    [SerializeField] private SolutionSetting[] settings;
    private Dictionary<LostTrackedSolution, SolutionSetting> SolutionSettingPairs = new Dictionary<LostTrackedSolution, SolutionSetting>();
    PetrollerObjectInfo.ControllerPairing oldConnection = PetrollerObjectInfo.ControllerPairing.Connected;
    PetrollerObjectInfo.TrackingStatus oldTrackingState;
    bool LHandTracked, RHandTracked;
    public bool debugSolution;
    Vector3 selfLastKnownPos;
    Quaternion selfLastKnownRot;
    void Awake()
    {
        foreach (var setting in settings)
        {
            SolutionSettingPairs.Add(setting.lostTrackedSolution, setting);
        }
    }
    void Start()
    {
        if (modelTransform == null) modelTransform = this.transform.GetChild(0).transform;

        if (autoInitializeModel)
        {
            modelTransform.position = SolutionSettingPairs[LostTrackedSolution.AsDefault].offsetPos;
            modelTransform.rotation = Quaternion.Euler(SolutionSettingPairs[LostTrackedSolution.AsDefault].offesetRot);
        }
        oldTrackingState = petrollerInfo.CurrentTrackingState;
    }

    void Update()
    {
        // For the scenario in which the R controller is not connected to the HMD
        if (petrollerInfo.CurrentControllerConnection != oldConnection)
        {
            modelTransform.gameObject.SetActive(petrollerInfo.CurrentControllerConnection == PetrollerObjectInfo.ControllerPairing.Connected ? true : false);
        }
        oldConnection = petrollerInfo.CurrentControllerConnection;

        // Check for Hand Tracking
        LHandTracked = OVRInput.GetControllerPositionTracked(OVRInput.Controller.LHand);
        RHandTracked = OVRInput.GetControllerPositionTracked(OVRInput.Controller.RHand);

        // On Tracked State Changed
        if (oldTrackingState != petrollerInfo.CurrentTrackingState) // On Tracked State Changed
        {
            if (petrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.Tracked)
            {
                modelTransform.localPosition = SolutionSettingPairs[LostTrackedSolution.AsDefault].offsetPos;
                modelTransform.localEulerAngles = SolutionSettingPairs[LostTrackedSolution.AsDefault].offesetRot;
            }
            else
            {
                modelTransform.localPosition = SolutionSettingPairs[solution].offsetPos;
                modelTransform.localEulerAngles = SolutionSettingPairs[solution].offesetRot;
            }
        }

        // updating position and rotation
        if (petrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.Tracked & !debugSolution)
        {
            transform.position = petrollerInfo.transform.position;
            transform.rotation = petrollerInfo.transform.rotation;

            SolutionSettingPairs[LostTrackedSolution.StayLastKnown].snapPos = transform.position;
            SolutionSettingPairs[LostTrackedSolution.StayLastKnown].snapRot = transform.rotation.eulerAngles;
        }
        else
        {
            Vector3 finalPos = Vector3.zero;
            Quaternion finalRot = Quaternion.identity;

            switch (solution)
            {
                case LostTrackedSolution.StayLastKnown:
                    finalPos = SolutionSettingPairs[solution].snapPos;
                    finalRot = Quaternion.Euler(SolutionSettingPairs[solution].snapRot);
                    break;
                case LostTrackedSolution.Fixed:
                    finalPos = SolutionSettingPairs[solution].snapPos;
                    finalRot = Quaternion.Euler(SolutionSettingPairs[solution].snapRot);
                    break;
                case LostTrackedSolution.SnapToCenter:
                    if (LHandTracked & RHandTracked)
                    {
                        Vector3 LHandPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                        Vector3 RHandPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
                        Quaternion LHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
                        Quaternion RHandRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand);

                        finalPos = Vector3.Lerp(LHandPos, RHandPos, 0.5f);
                        finalRot = Quaternion.Lerp(LHandRot, RHandRot, 0.5f);
                    }
                    else
                    {
                        if (RHandTracked)
                        {
                            finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
                            finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand);
                        }
                        else if (LHandTracked)
                        {
                            finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                            finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
                        }
                        else
                        {
                            finalPos = selfLastKnownPos;
                            finalRot = selfLastKnownRot;
                        }
                    }
                    break;
                case LostTrackedSolution.SnapToRightHand:
                    if (RHandTracked)
                    {
                        finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RHand);
                        finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RHand);
                    }
                    else
                    {
                        if (LHandTracked)
                        {
                            finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                            finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
                        }
                        else
                        {
                            finalPos = selfLastKnownPos;
                            finalRot = selfLastKnownRot;
                        }
                    }
                    break;
                case LostTrackedSolution.SnapToLeftHand:
                    if (LHandTracked)
                    {
                        finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                        finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
                    }
                    else
                    {
                        if (LHandTracked)
                        {
                            finalPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LHand);
                            finalRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LHand);
                        }
                        else
                        {
                            finalPos = selfLastKnownPos;
                            finalRot = selfLastKnownRot;
                        }
                    }
                    break;
            }

            transform.position = finalPos;
            transform.rotation = finalRot;
            modelTransform.localPosition = SolutionSettingPairs[solution].offsetPos;
            modelTransform.localEulerAngles = SolutionSettingPairs[solution].offesetRot;
        }

        oldTrackingState = petrollerInfo.CurrentTrackingState;
        selfLastKnownPos = transform.position;
        selfLastKnownRot = transform.rotation;
    }

    public void RecordCoords()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Current Offset |  Pos: " + modelTransform.localPosition + " , Rot: " + modelTransform.localEulerAngles);
        }
    }
}


[System.Serializable]
public class SolutionSetting // TEMP
{
    public PetrollerModelHandler.LostTrackedSolution lostTrackedSolution;
    public Vector3 snapPos;
    public Vector3 snapRot;
    public Vector3 offsetPos;
    public Vector3 offesetRot;
}
