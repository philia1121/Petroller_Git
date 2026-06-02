using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchCatController : MonoBehaviour
{
    [Header("控制物件")]
    [Tooltip("控制貓咪底部 (屁股) 的物件 A")]
    [SerializeField] private OVRInput.Controller pointA_Controller = OVRInput.Controller.LTouch;

    [Tooltip("控制拉伸與臉部方向的物件 B")]
    [SerializeField] private OVRInput.Controller pointB_Controller = OVRInput.Controller.RTouch;

    public Transform pointA, pointB;

    [Header("手風琴長度校正參數")]
    [Tooltip("模型在 Scale Y = 1 時的『實際原始長度』。如果模型預設長度是 2 單位，請填 2")]
    public float originalLength = 1f;

    [Header("貓咪其他軸向粗細")]
    public float catWidth = 1f;  // X 軸 (寬度)
    public float catDepth = 1f;  // Z 軸 (厚度)

    [Header("軸向與面朝向修正 (重要)")]
    [Tooltip("用來修正模型匯入時的特異旋轉。如果臉上下顛倒或看錯方向，直接在 Inspector 調整這裡！\n預設 (-90, 0, 0) 是將 Local Y 軸對齊 LookRotation 的方向。")]
    public Vector3 rotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("模型軸心 (Pivot) 設定")]
    [Tooltip("如果模型的 Pivot 在屁股（底部），請勾選；若在正中間，請取消勾選")]
    public bool isPivotAtBottom = true;

    void Update()
    {
        pointA.position = OVRInput.GetLocalControllerPosition(pointA_Controller);
        pointA.rotation = OVRInput.GetLocalControllerRotation(pointA_Controller);
        pointB.position = OVRInput.GetLocalControllerPosition(pointB_Controller);
        pointB.rotation = OVRInput.GetLocalControllerRotation(pointB_Controller);

        // 1. 計算 A 到 B 的向量與距離
        Vector3 directionToB = pointB.position - pointA.position;
        float distance = directionToB.magnitude;

        if (distance < 0.001f) return;

        // 2. 處理旋轉：結合 directionToB 與 pointB 的旋轉狀態
        // LookRotation(前方向量, 上方向量)
        // 這裡讓 directionToB 決定延伸方向，並將 pointB 的正上方 (pointB.up) 作為貓咪的頭頂參考
        Quaternion lookWithB = Quaternion.LookRotation(directionToB, pointB.up);

        // 因為模型的臉部朝向是 Local Y 軸，我們必須乘上一個 local 偏移量修正它
        // 四元數乘法由左至右：先套用 lookWithB 的世界旋轉，再套用 local 偏移
        transform.rotation = lookWithB * Quaternion.Euler(rotationOffset);

        // 3. 處理縮放（手風琴拉伸）
        float targetYScale = distance / originalLength;
        transform.localScale = new Vector3(catWidth, targetYScale, catDepth);

        // 4. 處理位移
        if (isPivotAtBottom)
        {
            transform.position = pointA.position;
        }
        else
        {
            transform.position = pointA.position + (directionToB * 0.5f);
        }
    }
}
