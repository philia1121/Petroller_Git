using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerModelHandler : MonoBehaviour
{
    [SerializeField] private PetrollerObjectInfo petrollerInfo;
    [SerializeField] private bool autoInitializeModel;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Vector3 model_initialPos, model_initialRot;
    private enum Solution { StayLastKnown, Fixed }
    [SerializeField] private Solution solution = Solution.StayLastKnown;
    [SerializeField] private Transform lostTrackedSpot;
    private Vector3 lastKnownPosition;
    private Quaternion lastKnownRotation;
    void Start()
    {
        if (modelTransform == null) modelTransform = this.transform.GetChild(0).transform;

        if (autoInitializeModel)
        {
            modelTransform.position = model_initialPos;
            modelTransform.rotation = Quaternion.Euler(model_initialRot);
        }
    }

    void Update()
    {
        if (petrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            transform.position = petrollerInfo.transform.position;
            transform.rotation = petrollerInfo.transform.rotation;

            lastKnownPosition = transform.position;
            lastKnownRotation = transform.rotation;
        }
        else
        {
            switch (solution)
            {
                case Solution.StayLastKnown:
                    transform.position = lastKnownPosition;
                    transform.rotation = lastKnownRotation;
                    break;
                case Solution.Fixed:
                    transform.position = lostTrackedSpot.position;
                    transform.rotation = lostTrackedSpot.rotation;
                    break;
            }
        }
    }
}
