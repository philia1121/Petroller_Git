using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFollower : MonoBehaviour
{
    [SerializeField] private OVRInput.Controller targetController = OVRInput.Controller.RTouch;
    [SerializeField] private bool autoInitializeModel;
    [SerializeField] private Transform modelTransform;
    [SerializeField] private Vector3 model_initialPos, model_initialRot;
    [SerializeField] private bool keepFollow = true;
    private Vector3 lastKnownPosition;
    private Quaternion lastKnownRotation;

    void Start()
    {
        if (modelTransform == null)
            modelTransform = this.transform.GetChild(0).transform;
        if (autoInitializeModel)
        {
            modelTransform.position = model_initialPos;
            modelTransform.rotation = Quaternion.Euler(model_initialRot);
        }
    }
    void Update()
    {
        if (keepFollow)
        {
            transform.position = OVRInput.GetLocalControllerPosition(targetController);
            transform.rotation = OVRInput.GetLocalControllerRotation(targetController);

            lastKnownPosition = transform.position;
            lastKnownRotation = transform.rotation;
        }
        else
        {
            if (OVRInput.GetControllerPositionTracked(targetController))
            {
                transform.position = OVRInput.GetLocalControllerPosition(targetController);
                lastKnownPosition = transform.position;
            }
            else
            {
                transform.position = lastKnownPosition;
            }

            if (OVRInput.GetControllerOrientationTracked(targetController))
            {
                transform.rotation = OVRInput.GetLocalControllerRotation(targetController);
                lastKnownRotation = transform.rotation;
            }
            else
            {
                transform.rotation = lastKnownRotation;
            }
        }
    }
}
