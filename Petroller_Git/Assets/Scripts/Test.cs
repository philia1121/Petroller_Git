using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("目標相機，若不填則預設抓 MainCamera")]
    public Transform targetCamera;

    [Tooltip("正面判定容許角度 (度)，例如 15 代表左右各 15 度")]
    [Range(0f, 90f)]
    public float frontAngleThreshold = 15f;

    [Tooltip("背面判定容許角度 (度)，通常與正面相同或是更寬")]
    [Range(0f, 90f)]
    public float backAngleThreshold = 15f;

    // 定義狀態 Enum，方便外部讀取
    public enum FacingState
    {
        FrontFacing, // 正面面對相機
        BackFacing,  // 背對相機
        SideFacing   // 側面
    }

    public FacingState CurrentState { get; private set; }

    void Start()
    {
        // 如果沒有手動拉相機，自動抓主攝影機
        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main.transform;
        }
    }

    void Update()
    {
        if (targetCamera == null) return;

        CurrentState = CalculateFacingDirection();

        // 測試用：印出結果
        // Debug.Log($"Current State: {CurrentState}");
    }

    private FacingState CalculateFacingDirection()
    {
        // 1. 計算從「物件」指向「相機」的向量
        Vector3 directionToCamera = targetCamera.position - transform.position;

        // *重要*：如果你的遊戲是平面的 (如 RPG)，不希望高度影響判定
        // 請取消下面這行的註解，將 Y 軸抹平
        // directionToCamera.y = 0; 

        // 2. 計算物件正前方 (forward) 與 指向相機向量 的夾角
        // Vector3.Angle 會回傳 0 到 180 度的無正負值
        float angle = Vector3.Angle(transform.forward, directionToCamera);
        Debug.Log(angle);

        // 3. 判斷角度區間
        // 夾角越接近 0，代表物件正看著相機
        if (angle <= frontAngleThreshold)
        {
            return FacingState.FrontFacing;
        }
        // 夾角越接近 180，代表物件看著反方向 (也就是背對相機)
        else if (angle >= 180f - backAngleThreshold)
        {
            return FacingState.BackFacing;
        }
        // 其他中間的角度都算側面
        else
        {
            return FacingState.SideFacing;
        }
    }

    // 編輯器視覺化輔助 (Gizmos)
    void OnDrawGizmos()
    {
        if (targetCamera == null) return;

        // 畫出物件正前方 (藍色)
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

        // 畫出指向相機的方向 (綠色)
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, targetCamera.position);

        // 根據狀態改變 Gizmo 球體顏色
        switch (CurrentState)
        {
            case FacingState.FrontFacing: Gizmos.color = Color.red; break; // 正面示警
            case FacingState.BackFacing: Gizmos.color = Color.gray; break; // 背面
            default: Gizmos.color = Color.yellow; break; // 側面
        }
        Gizmos.DrawWireSphere(transform.position + Vector3.up, 0.5f);
    }
}
