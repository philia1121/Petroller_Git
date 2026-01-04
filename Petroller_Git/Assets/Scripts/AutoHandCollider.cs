using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoHandCollider : MonoBehaviour
{
    public HandModelHandler hand;
    [SerializeField] private GameObject colliderPrefab;
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
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
        yield return new WaitUntil(() => hand.PalmTransform != null);
        colliderSelf = Instantiate(colliderPrefab, hand.PalmTransform).GetComponent<Collider>();
    }


}
