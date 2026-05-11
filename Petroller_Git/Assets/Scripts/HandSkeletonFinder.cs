using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSkeletonFinder : MonoBehaviour
{
    public OVRSkeleton skeleton;
    public Transform PalmTransform { get; private set; }
    public Transform MidFingerTransfrom { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        if (skeleton == null) skeleton = GetComponent<OVRSkeleton>();
        if (skeleton == null || skeleton.Bones == null || skeleton.Bones.Count == 0)
        {
            Debug.LogError("HandConfidenceMonitor: can't find OVRSkeleton");
            this.enabled = false;
            return;
        }
        PalmTransform = skeleton.Bones[(int)OVRSkeleton.BoneId.XRHand_Palm].Transform;
        MidFingerTransfrom = skeleton.Bones[(int)OVRPlugin.BoneId.XRHand_MiddleProximal - 3].Transform;
    }
}
