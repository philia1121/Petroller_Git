using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VRTrajectoryPlayer : MonoBehaviour
{
    [Header("File Settings")]
    public string folderPath;
    public string subFolder;
    public string fileName = "MultiTraj_20251212_161936.json";

    [Header("VR 綁定物件")]
    public Transform hmdTarget;
    public Transform lHandTarget;
    public Transform lPalmTarget;
    public Transform rHandTarget;
    public Transform rPalmTarget;
    public Transform lContTarget;
    public Transform rContTarget;

    [Header("UI 控制與註記設定")]
    public Slider timelineSlider;
    public RectTransform markerContainer; // 請放置在 Slider Background 下的空物件
    public Button playPauseButton;

    [Header("LostTracked 顏色標記 (8個參數)")]
    public Color[] lostTrackingColors = new Color[8] {
        Color.red,                  // 0: RHand_Pos
        new Color(1f, 0.5f, 0f),    // 1: RHand_Rot (橘)
        Color.yellow,               // 2: RCont_Pos
        Color.green,                // 3: RCont_Rot
        Color.cyan,                 // 4: LHand_Pos
        Color.blue,                 // 5: LHand_Rot
        Color.magenta,              // 6: LCont_Pos
        new Color(0.5f, 0f, 0.5f)   // 7: LCont_Rot (紫)

};

    private TrajectorySession data;
    private float maxTime;
    private float currentTime;
    private bool isPlaying = false;
    private bool isUpdatingSliderFromCode = false;

    void Start()
    {
        timelineSlider.onValueChanged.AddListener(OnSliderScrubbed);
        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(TogglePlay);

        LoadTrajectory();
    }

    void Update()
    {
        if (isPlaying && data != null && data.waypoints.Count > 0)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= maxTime)
            {
                currentTime = maxTime;
                isPlaying = false;
            }

            // 更新 UI Slider (阻斷觸發 OnSliderScrubbed)
            isUpdatingSliderFromCode = true;
            timelineSlider.value = currentTime / maxTime;
            isUpdatingSliderFromCode = false;

            ApplyTransformAtTime(currentTime);
        }
    }

    // ================= 檔案載入與解析 =================
    public void LoadTrajectory()
    {
        if (string.IsNullOrEmpty(folderPath))
        {
            folderPath = Path.Combine(Application.persistentDataPath, subFolder);
        }

        fileName += fileName.EndsWith(".json") ? "" : ".json";
        string fullPath = Path.Combine(folderPath, fileName);

        if (File.Exists(fullPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(fullPath);
                data = JsonUtility.FromJson<TrajectorySession>(jsonContent);
                if (data != null && data.waypoints.Count > 0)
                {
                    maxTime = data.waypoints[data.waypoints.Count - 1].timestamp;
                    GenerateLostTrackingMarkers();
                    currentTime = 0;
                    ApplyTransformAtTime(0);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"JSON unpack failed: {e.Message}");
            }
        }
        else
        {
            Debug.Log("Can't find files");
        }
    }

    // ================= 回放邏輯 (二元搜尋與插值) =================
    private void ApplyTransformAtTime(float time)
    {
        if (data == null || data.waypoints.Count == 0) return;

        // Binary Search 尋找當前時間的 Waypoint
        int left = 0, right = data.waypoints.Count - 1;
        int index = 0;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;
            if (data.waypoints[mid].timestamp <= time)
            {
                index = mid;
                left = mid + 1;
            }
            else right = mid - 1;
        }

        if (index >= data.waypoints.Count - 1)
        {
            ApplyWaypointExact(data.waypoints[data.waypoints.Count - 1]);
        }
        else
        {
            MultiTrackWaypoint a = data.waypoints[index];
            MultiTrackWaypoint b = data.waypoints[index + 1];
            float t = (time - a.timestamp) / (b.timestamp - a.timestamp);
            LerpWaypoint(a, b, t);
        }
    }

    private void LerpWaypoint(MultiTrackWaypoint a, MultiTrackWaypoint b, float t)
    {
        if (hmdTarget)
        {
            hmdTarget.localPosition = Vector3.Lerp(a.pos_HMD, b.pos_HMD, t);
            hmdTarget.localRotation = Quaternion.Slerp(a.rot_HMD, b.rot_HMD, t);
        }
        if (lContTarget)
        {
            lContTarget.localPosition = Vector3.Lerp(a.pos_LCont, b.pos_LCont, t);
            lContTarget.localRotation = Quaternion.Slerp(a.rot_LCont, b.rot_LCont, t);
        }
        if (rContTarget)
        {
            rContTarget.localPosition = Vector3.Lerp(a.pos_RCont, b.pos_RCont, t);
            rContTarget.localRotation = Quaternion.Slerp(a.rot_RCont, b.rot_RCont, t);
        }
        if (lHandTarget)
        {
            lHandTarget.localPosition = Vector3.Lerp(a.pos_LHand, b.pos_LHand, t);
            lHandTarget.localRotation = Quaternion.Slerp(a.rot_LHand, b.rot_LHand, t);
        }
        if (rHandTarget)
        {
            rHandTarget.localPosition = Vector3.Lerp(a.pos_RHand, b.pos_RHand, t);
            rHandTarget.localRotation = Quaternion.Slerp(a.rot_RHand, b.rot_RHand, t);
        }
        if (lPalmTarget)
        {
            lPalmTarget.localPosition = Vector3.Lerp(a.pos_LPalm, b.pos_LPalm, t);
            lPalmTarget.localRotation = Quaternion.Slerp(a.rot_LPalm, b.rot_LPalm, t);
        }
        if (rPalmTarget)
        {
            rPalmTarget.localPosition = Vector3.Lerp(a.pos_RPalm, b.pos_RPalm, t);
            rPalmTarget.localRotation = Quaternion.Slerp(a.rot_RPalm, b.rot_RPalm, t);
        }
    }

    private void ApplyWaypointExact(MultiTrackWaypoint wp) { LerpWaypoint(wp, wp, 0f); }

    // ================= UI Slider 與標記邏輯 =================
    public void TogglePlay() { isPlaying = !isPlaying; }

    private void OnSliderScrubbed(float val)
    {
        if (isUpdatingSliderFromCode) return;
        currentTime = val * maxTime;
        ApplyTransformAtTime(currentTime);
    }

    private void GenerateLostTrackingMarkers()
    {
        foreach (Transform child in markerContainer) Destroy(child.gameObject);

        // 參數順序對應陣列 0~7
        DrawLostTrackingBlocks(wp => !wp.RHand_PosTracked, 0);
        DrawLostTrackingBlocks(wp => !wp.RHand_RotTracked, 1);
        DrawLostTrackingBlocks(wp => !wp.RCont_PosTracked, 2);
        DrawLostTrackingBlocks(wp => !wp.RCont_RotTracked, 3);
        DrawLostTrackingBlocks(wp => !wp.LHand_PosTracked, 4);
        DrawLostTrackingBlocks(wp => !wp.LHand_RotTracked, 5);
        DrawLostTrackingBlocks(wp => !wp.LCont_PosTracked, 6);
        DrawLostTrackingBlocks(wp => !wp.VisualTracked, 7);
    }

    // 合併連續的 Lost Tracked 訊號為一個區塊，減少 UI 繪製壓力
    private void DrawLostTrackingBlocks(Func<MultiTrackWaypoint, bool> isLost, int rowIndex)
    {
        bool isTrackingLost = false;
        float lostStartTime = 0f;

        for (int i = 0; i < data.waypoints.Count; i++)
        {
            bool currentLost = isLost(data.waypoints[i]);
            if (currentLost && !isTrackingLost)
            {
                isTrackingLost = true;
                lostStartTime = data.waypoints[i].timestamp;
            }
            else if (!currentLost && isTrackingLost)
            {
                isTrackingLost = false;
                CreateMarkerBlock(lostStartTime, data.waypoints[i - 1].timestamp, rowIndex);
            }
        }

        if (isTrackingLost) // 處理資料尾端依然是 Lost 的情況
            CreateMarkerBlock(lostStartTime, data.waypoints[^1].timestamp, rowIndex);
    }

    private void CreateMarkerBlock(float startTime, float endTime, int rowIndex)
    {
        float startNorm = startTime / maxTime;
        float endNorm = endTime / maxTime;

        // 若遺失時間極短（如單一 Frame），強制給予最小寬度確保視覺上可見
        if (endNorm - startNorm < 0.003f) endNorm = startNorm + 0.003f;

        GameObject block = new GameObject($"LostBlock_Track{rowIndex}");
        block.transform.SetParent(markerContainer, false);

        Image img = block.AddComponent<Image>();
        img.color = lostTrackingColors[rowIndex];

        RectTransform rt = block.GetComponent<RectTransform>();

        // 垂直切割：將整個容器高度分為 8 等份
        float trackHeight = 1f / 8f;
        rt.anchorMin = new Vector2(startNorm, rowIndex * trackHeight);
        rt.anchorMax = new Vector2(endNorm, (rowIndex + 1) * trackHeight);

        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }
}