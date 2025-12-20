using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public PetrollerObjectInfo petroller;
    public Material mat;
    public Color[] colors = new Color[3];
    void Update()
    {
        if (petroller.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            mat.color = colors[0];
        }
        else if (petroller.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked)
        {
            mat.color = colors[1];
        }
        else if (petroller.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.LostTracked)
        {
            mat.color = colors[2];
        }

    }
}
