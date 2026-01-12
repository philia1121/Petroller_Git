using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum HandInteraction
{
    Slap, Pat, Stroke, None
}
public class HandInteractionDetector : MonoBehaviour
{
    public PetrollerObjectInfo petroller;
    public ManualVelocityTracker sphereTracker;
    [Header("門檻設定")]
    public float slapThreshold = 1.8f;      // 拍打的速度下限
    public float patMaxThreshold = 0.8f;    // 輕拍的速度上限
    public float strokeMoveThreshold = 0.2f; // 撫摸時手部移動的最短距離

    private float contactStartTime;
    private Vector3 handPosOnEnter;
    private bool isTouching = false;
    private ManualVelocityTracker activeHandTracker;
    void Start()
    {
        petroller = FindFirstObjectByType<PetrollerObjectInfo>();
    }

    private void OnTriggerEnter(Collider other)
    {

        var handTracker = other.GetComponent<ManualVelocityTracker>();
        if (handTracker != null)
        {
            activeHandTracker = handTracker;
            // 計算碰撞瞬間的「相對速度」
            Vector3 relativeVelocity = handTracker.CurrentVelocity - sphereTracker.CurrentVelocity;
            float speed = relativeVelocity.magnitude;
            Debug.Log(speed);

            if (speed > slapThreshold)
            {
                OnSlap(speed);
                petroller.ReceiveInteraction(HandInteraction.Slap);
            }
            else
            {
                // 進入「潛在」的輕拍或撫摸狀態
                contactStartTime = Time.time;
                handPosOnEnter = other.transform.position;
                isTouching = true;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {

        if (isTouching && activeHandTracker != null)
        {
            float duration = Time.time - contactStartTime;
            float moveDistance = Vector3.Distance(other.transform.position, handPosOnEnter);

            // 判斷邏輯
            if (duration > 0.4f && moveDistance > strokeMoveThreshold)
            {
                OnStroke(); // 接觸久且有位移：撫摸
                petroller.ReceiveInteraction(HandInteraction.Stroke);
            }
            else if (duration < 0.3f)
            {
                OnPat();    // 接觸時間短：輕拍
                petroller.ReceiveInteraction(HandInteraction.Pat);
            }

            isTouching = false;
            activeHandTracker = null;
        }
    }

    void OnSlap(float force) => Debug.Log($"<color=red>【拍打】</color> 強度：{force:F2}");
    void OnPat() => Debug.Log("<color=yellow>【輕拍】</color> 哄睡力道");
    void OnStroke() => Debug.Log("<color=green>【撫摸】</color> 溫柔互動");
}
