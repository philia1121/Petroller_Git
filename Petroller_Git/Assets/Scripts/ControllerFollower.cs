using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerFollower : MonoBehaviour
{
    public OVRInput.Controller targetController = OVRInput.Controller.RTouch; 
    public Transform modelTransform; 
    private Vector3 lastKnownPosition;
    private Quaternion lastKnownRotation;
    public Vector3 offset = new Vector3(1,1,1);

    void Start()
    {
        if (modelTransform == null)
            modelTransform = this.transform;
    }
    void Update()
    {
        if (OVRInput.GetControllerPositionTracked(targetController))
        {
            var pos = OVRInput.GetLocalControllerPosition(targetController);
            modelTransform.position = new Vector3(pos.x*offset.x, pos.y*offset.y, pos.z*offset.z);
            var rot = OVRInput.GetLocalControllerRotation(targetController);
            var rot_flip = rot.eulerAngles;
            modelTransform.rotation = Quaternion.Euler(new Vector3(rot_flip.x*offset.x, rot_flip.y*offset.y, rot_flip.z*offset.z));

            lastKnownPosition = modelTransform.position;
            lastKnownRotation = modelTransform.rotation;
        }
        else
        {
            modelTransform.position = lastKnownPosition;
            modelTransform.rotation = lastKnownRotation;
        }
    }
}
