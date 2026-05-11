using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothBetweenObjects : MonoBehaviour
{
    [Header("目標物體")]
    public Transform objectA;
    public Transform objectB;

    [Header("偏移設定")]
    public Vector3 offset;

    [Header("平滑移動設定")]
    [Range(0.01f, 1.0f)]
    public float smoothTime = 0.15f; // 數值越小，跟隨越緊湊；數值越大，越平滑延遲

    private Vector3 _currentVelocity = Vector3.zero;

    void LateUpdate()
    {
        // 確保兩個目標都存在，避免報錯
        if (objectA == null || objectB == null) return;

        // 1. 計算兩物體的中間點
        Vector3 midPoint = (objectA.position + objectB.position) / 2f;

        // 2. 加入偏移量得到目標位置
        Vector3 targetPosition = midPoint + offset;

        // 3. 使用 SmoothDamp 進行平滑位移
        // transform.position: 當前位置
        // targetPosition: 目標位置
        // _currentVelocity: 內部參考速度（不需手動修改）
        // smoothTime: 到達目標的大約時間
        transform.position = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref _currentVelocity, 
            smoothTime
        );
    }
}