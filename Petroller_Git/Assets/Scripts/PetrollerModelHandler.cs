using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerModelHandler : MonoBehaviour
{
    [SerializeField] private PetrollerObjectInfo petrollerInfo;
    [SerializeField] private bool autoInitializeModel;
    [SerializeField] private Transform modelTransform, fakeTransform;
    public HandModelHandler LHandHandler, RHandHandler;
    bool LHandTracked, RHandTracked;
    public bool debugSolution;
    bool oldDebug;
    public enum LostTrackedSolution { StayLastKnown, Fixed, SnapToCenter, SnapToRightHand, SnapToLeftHand, AsDefault }
    [SerializeField] private LostTrackedSolution solution = LostTrackedSolution.StayLastKnown;
    enum SnapHandException { StayLastKnown, Available }
    [SerializeField] private SnapHandException snapHandException = SnapHandException.Available;
    [SerializeField] private SolutionSetting[] settings;
    private Dictionary<LostTrackedSolution, SolutionSetting> SolutionSettingPairs = new Dictionary<LostTrackedSolution, SolutionSetting>();
    PetrollerObjectInfo.ControllerPairing oldConnection = PetrollerObjectInfo.ControllerPairing.Connected;
    PetrollerObjectInfo.TrackingStatus oldTrackingState;
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
        oldDebug = debugSolution;
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
        LHandTracked = LHandHandler.isTrackingGood;
        RHandTracked = RHandHandler.isTrackingGood;

        // should be remove once the debugging option is no longer needed
        if (oldDebug != debugSolution)
        {
            Debug.Log("Reset Pos and Rot offset from debug solution");
            if (debugSolution)
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


        // On Tracked State Changed
        if (oldTrackingState != petrollerInfo.CurrentTrackingState) // On Tracked State Changed
        {
            Debug.Log("Reset Pos and Rot offset from normal option");
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
                        Vector3 LHandPos = LHandHandler.PalmTransform.position;
                        Vector3 RHandPos = RHandHandler.PalmTransform.position;

                        finalPos = Vector3.Lerp(LHandPos, RHandPos, 0.5f);

                        Vector3 leftForward = LHandHandler.PalmTransform.forward;
                        Vector3 leftRight = LHandHandler.PalmTransform.right;

                        Vector3 rightForward = RHandHandler.PalmTransform.forward;
                        Vector3 rightRight = -RHandHandler.PalmTransform.right;

                        Vector3 leftUp = Vector3.Cross(leftForward, leftRight);
                        Quaternion leftStandardRot = Quaternion.LookRotation(leftForward, leftUp);

                        Vector3 rightUp = Vector3.Cross(rightForward, rightRight);
                        Quaternion rightStandardRot = Quaternion.LookRotation(rightForward, rightUp);

                        finalRot = Quaternion.Slerp(leftStandardRot, rightStandardRot, 0.5f);
                    }
                    else
                    {
                        if (snapHandException == SnapHandException.Available & (RHandTracked | LHandTracked))
                        {
                            if (RHandTracked)
                            {
                                finalPos = RHandHandler.PalmTransform.position;
                                finalRot = RHandHandler.PalmTransform.rotation;
                            }
                            else if (LHandTracked)
                            {
                                finalPos = LHandHandler.PalmTransform.position;
                                finalRot = LHandHandler.PalmTransform.rotation;
                            }
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
                        finalPos = RHandHandler.PalmTransform.position;
                        finalRot = RHandHandler.PalmTransform.rotation;
                    }
                    else
                    {
                        if (snapHandException == SnapHandException.Available & LHandTracked)
                        {
                            finalPos = LHandHandler.PalmTransform.position;
                            finalRot = LHandHandler.PalmTransform.rotation;
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
                        finalPos = LHandHandler.PalmTransform.position;
                        finalRot = LHandHandler.PalmTransform.rotation;
                    }
                    else
                    {
                        if (snapHandException == SnapHandException.Available & RHandTracked)
                        {
                            finalPos = RHandHandler.PalmTransform.position;
                            finalRot = RHandHandler.PalmTransform.rotation;
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
        oldDebug = debugSolution;
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
