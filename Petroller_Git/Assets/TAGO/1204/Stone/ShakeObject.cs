using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    // 控制開關
    public bool isShaking = false;
    public bool isBreathing = false; // 新增：呼吸模式開關

    [Header("Shake Settings")]
    [Range(0f, 2f)]
    public float shakeAmount = 0.5f;
    [Range(0f, 50f)]
    public float shakeSpeed = 20f;

    [Header("Breath Settings")]
    [Range(0f, 2f)]
    public float breathAmount = 0.2f; // 呼吸的幅度
    [Range(0f, 10f)]
    public float breathSpeed = 2f;   // 呼吸的速度

    private Vector3 originalPosition;

    void Start()
    {
        // 紀錄初始位置
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            // 原有的震動邏輯 (Perlin Noise)
            float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeAmount;
            float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeAmount;

            transform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0f);
        }
        else if (isBreathing)
        {
            // 新增：呼吸邏輯 (使用 Sin 波實現平滑上下晃動)
            // Mathf.Sin 會在 -1 到 1 之間循環
            float breathY = Mathf.Sin(Time.time * breathSpeed) * breathAmount;
            
            transform.localPosition = originalPosition + new Vector3(0f, breathY, 0f);
        }
        else
        {
            // 停止時回到原位
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
        }
    }
}