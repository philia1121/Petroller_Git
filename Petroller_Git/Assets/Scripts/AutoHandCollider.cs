using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHandCollider : MonoBehaviour
{
    public HandModelHandler hand;
    [SerializeField] private GameObject colliderPrefab;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    public Vector3 offsetPos;
    public Vector3 offsetRot;
    Collider colliderSelf;

    void Start()
    {
        if (!skinnedMeshRenderer) skinnedMeshRenderer = hand.skinnedMeshRenderer;
        StartCoroutine(AddHandCollider());
    }
    void LateUpdate()
    {
        colliderSelf.enabled = skinnedMeshRenderer.enabled;
    }
    IEnumerator AddHandCollider()
    {
        yield return new WaitUntil(() => hand.MidFingerTransfrom != null);
        colliderSelf = Instantiate(colliderPrefab, hand.MidFingerTransfrom).GetComponent<Collider>();
        colliderSelf.transform.localPosition = offsetPos;
        colliderSelf.transform.localRotation = Quaternion.Euler(offsetRot);
    }
}
