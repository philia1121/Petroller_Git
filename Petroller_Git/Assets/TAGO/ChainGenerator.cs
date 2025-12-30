using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGeneratorScaled : MonoBehaviour
{
    [Header("端點設定")]
    public Transform pointA;
    public Transform pointB;

    [Header("鍊條設定")]
    public GameObject linkPrefab; 
    public int linkCount = 5;
    
    [Tooltip("縮放倍率，1表示完全填滿距離")]
    public float scaleOffset = 1.0f; 

    private List<Transform> spawnedLinks = new List<Transform>();
    private Vector3 originalScale; // 儲存 Prefab 一開始的原始縮放

    void Start()
    {
        if (linkPrefab != null)
        {
            // 在一開始記錄 Prefab 的原始縮放值
            originalScale = linkPrefab.transform.localScale;
            GenerateLinks();
        }
    }

    void Update()
    {
        UpdateLinksTransform();
    }

    void GenerateLinks()
    {
        if (linkPrefab == null || pointA == null || pointB == null) return;

        for (int i = 0; i < linkCount; i++)
        {
            GameObject link = Instantiate(linkPrefab, transform);
            spawnedLinks.Add(link.transform);
        }
    }

    void UpdateLinksTransform()
    {
        if (pointA == null || pointB == null || spawnedLinks.Count == 0) return;

        // 1. 計算 A B 點目前的距離
        float currentDistance = Vector3.Distance(pointA.position, pointB.position);
        
        // 2. 計算基準間隔 (0~1 之間的比例)
        float segment = 1f / (linkCount + 1);

        // 3. 計算縮放比例
        // 我們以當前距離除以數量，得到每個方塊分配到的「長度空間」
        // 再以此長度作為 multiplier 乘上原始縮放
        float distMultiplier = currentDistance / (linkCount + 1);

        for (int i = 0; i < spawnedLinks.Count; i++)
        {
            // 更新位置
            float t = (i + 1) * segment;
            spawnedLinks[i].position = Vector3.Lerp(pointA.position, pointB.position, t);

            // 更新旋轉 (始終指向 B)
            spawnedLinks[i].LookAt(pointB.position);

            // 更新縮放：原始大小 * 距離倍率
            // 我們主要縮放 Z 軸（長度方向）
            Vector3 newScale = originalScale;
            newScale= originalScale * distMultiplier * scaleOffset;
            
            // 如果你希望 X 和 Y 也跟著等比例變大，可以取消下面兩行的註解
            // newScale.x = originalScale.x * distMultiplier * scaleOffset;
            // newScale.y = originalScale.y * distMultiplier * scaleOffset;

            spawnedLinks[i].localScale = newScale;
        }
    }
}