using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScaleLocal : MonoBehaviour
{
    [Header("目標設定")]
    public Transform target;
    public Vector3 rotationOffset; // 修正模型正面的偏移角度

    [Header("縮放設定")]
    [Tooltip("距離每增加 1 單位，localScale 增加多少倍")]
    public float scaleMultiplier = 1.0f;
    public float distance;
    
    [Tooltip("基礎縮放值 (當距離為 0 時的大小)")]
    public float baseScale = 0.0f;

    [Tooltip("模型拉伸的軸向")]
    public bool stretchX = false;
    public bool stretchY = false;
    public bool stretchZ = true;

    [Header("限制與平滑")]
    public float minScale = 0.01f;
    public float maxScale = 50.0f;
    public float lerpSpeed = 20f;


    [Header("Shader 指派")]
    [Tooltip("手動指派要控制的材質 (若不填則自動抓取自身)")]
    public Material targetMaterial;
    public string parameterName = "_Strengh_Adjust";

    void Update()
    {
        //targetMaterial = targetMaterial.GetComponent<Renderer>();

        if (target == null) return;

        // 1. 轉向目標
        Vector3 direction = target.position - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(direction) * Quaternion.Euler(rotationOffset);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);
        }
        
        // 2. 計算距離並轉換為 localScale
        distance = Vector3.Distance(transform.position, target.position);
        targetMaterial.SetFloat(parameterName, distance);
        // 公式：(距離 * 倍率) + 基礎大小
        float calculatedScale = (distance * scaleMultiplier) + baseScale;
        
        // 限制範圍
        calculatedScale = Mathf.Clamp(calculatedScale, minScale, maxScale);

        // 3. 套用至指定的 localScale 軸向
        Vector3 currentScale = transform.localScale;
        Vector3 targetScale = new Vector3(
            stretchX ? calculatedScale : currentScale.x,
            stretchY ? calculatedScale : currentScale.y,
            stretchZ ? calculatedScale : currentScale.z
        );

        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.deltaTime);
    }
}