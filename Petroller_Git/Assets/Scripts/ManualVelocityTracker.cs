using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManualVelocityTracker : MonoBehaviour
{
    private Vector3 lastPosition;
    public Vector3 CurrentVelocity { get; private set; }

    void Start()
    {
        lastPosition = transform.position;
    }

    void LateUpdate()
    {
        CurrentVelocity = (transform.position - lastPosition) / Time.deltaTime;
        lastPosition = transform.position;
    }
}
