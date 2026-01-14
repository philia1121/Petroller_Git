using System.Collections.Generic;
using UnityEngine;

public class ChainGeneratorScaled : MonoBehaviour
{
    [Header("端點設定")]
    public Transform pointA;
    public Transform pointB;

    [Header("曲線設定")]
    public Transform midPoint;
    public bool useCurve = true;

    [Header("鍊條單元（由外部指派，不生成）")]
    public List<Transform> chainLinks = new List<Transform>();

    [Header("縮放設定")]
    public float scaleOffset = 1.0f;
    public float scaleMultiplier = 1.0f;

    [Header("旋轉修正（所有鍊條共用）")]
    public Vector3 rotationOffset;   // Euler Offset

    private Vector3 originalScale;

    void Start()
    {
        if (chainLinks.Count > 0 && chainLinks[0] != null)
        {
            originalScale = chainLinks[0].localScale;
        }
    }

    void Update()
    {
        UpdateLinksTransform();
    }

    void UpdateLinksTransform()
    {
        if (pointA == null || pointB == null) return;
        if (chainLinks == null || chainLinks.Count == 0) return;

        int linkCount = chainLinks.Count;

        float currentDistance = Vector3.Distance(pointA.position, pointB.position);
        float segment = 1f / (linkCount + 1);
        float distMultiplier = currentDistance / (linkCount + 1);

        Quaternion offsetRotation = Quaternion.Euler(rotationOffset);

        for (int i = 0; i < linkCount; i++)
        {
            Transform link = chainLinks[i];
            if (link == null) continue;

            float t = (i + 1) * segment;

            // 📍 位置（Bezier / 直線）
            Vector3 pos = GetChainPointPosition(t);
            link.position = pos;

            // 🔄 旋轉（沿曲線切線 + Offset）
            float tNext = Mathf.Clamp01(t + segment * 0.5f);
            Vector3 nextPos = GetChainPointPosition(tNext);

            link.LookAt(nextPos);
            link.rotation = link.rotation * offsetRotation;

            // 📏 縮放
            Vector3 newScale = originalScale;
            newScale = originalScale * (1f + distMultiplier * scaleMultiplier) * scaleOffset;
            link.localScale = newScale;
        }
    }

    // =========================
    // 🌈 曲線位置計算
    // =========================
    Vector3 GetChainPointPosition(float t)
    {
        if (useCurve && midPoint != null)
        {
            return QuadraticBezier(
                pointA.position,
                midPoint.position,
                pointB.position,
                t
            );
        }
        else
        {
            return Vector3.Lerp(pointA.position, pointB.position, t);
        }
    }

    // =========================
    // ✨ 二次 Bezier 曲線
    // =========================
    Vector3 QuadraticBezier(Vector3 a, Vector3 c, Vector3 b, float t)
    {
        float u = 1f - t;
        return u * u * a
             + 2f * u * t * c
             + t * t * b;
    }
}
