using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerTriggerExtenstion : MonoBehaviour
{
    [SerializeField]private PetrollerObjectInfo petrollerInfo;
    void Awake()
    {
        if (!petrollerInfo) petrollerInfo = FindFirstObjectByType<PetrollerObjectInfo>();
        if (!petrollerInfo) Debug.LogWarning("PetrollerObjectInfo not exit");
    }
    void OnTriggerEnter(Collider other)
    {
        ZoneMarker marker = other.GetComponent<ZoneMarker>();
        if (marker != null & petrollerInfo)
        {
            petrollerInfo.AddZoneID(marker.zoneID);
        }
    }
    void OnTriggerExit(Collider other)
    {
        ZoneMarker marker = other.GetComponent<ZoneMarker>();
        if (marker != null & petrollerInfo)
        {
            petrollerInfo.RemoveZoneID(marker.zoneID);
        }
    }
}
