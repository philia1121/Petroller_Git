using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObject : MonoBehaviour
{
    // 控制開關
    public bool isShaking = false;
    [Range(0f, 2f)]
    public float shakeAmount = 0.5f;
    [Range(0f, 50f)]
    public float shakeSpeed = 20f;

    private Vector3 originalPosition;

    void Start()
    {
        // 紀錄初始位置，避免震動後回不去
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isShaking)
        {
            // 使用 Mathf.PerlinNoise 讓震動看起來更自然，而非單純隨機
            float shakeX = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f * shakeAmount;
            float shakeY = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f * shakeAmount;

            transform.localPosition = originalPosition + new Vector3(shakeX, shakeY, 0f);
        }
        else
        {
            // 停止震動時回到原位
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * 5f);
        }
    }
}