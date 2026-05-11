using UnityEngine;
using UnityEngine.VFX;

public class VFXRainbowController : MonoBehaviour
{
    [Header("VFX Graph 設定")]
    public VisualEffect vfxGraph; 

    [Header("彩虹漸變設定")]
    [Tooltip("控制顏色變化的速度")]
    public float speed = 0.5f; 

    // **新增飽和度控制變數**
    [Tooltip("顏色的飽和度 (0.0 = 灰色, 1.0 = 純色)")]
    [Range(0.0f, 1.0f)] // 確保值在 0 到 1 之間
    public float saturation = 1.0f; // 預設為純色

    // **新增亮度控制變數 (建議保留)**
    [Tooltip("顏色的亮度 (0.0 = 黑色, 1.0 = 最亮)")]
    [Range(0.0f, 1.0f)]
    public float brightness = 1.0f; // 預設為最亮

    [Header("屬性名稱")]
    public string colorPropertyName = "RainbowColor"; 

    private int colorPropertyID;

    private void Start()
    {
        if (vfxGraph == null)
        {
            Debug.LogError("VisualEffect 元件未設定。請檢查 Inspector。");
            enabled = false;
            return;
        }
        colorPropertyID = Shader.PropertyToID(colorPropertyName);
    }

    private void Update()
    {
        // 1. 計算時間因子
        float timeFactor = Time.time * speed;

        // 2. 計算 H (色相/Hue) 值
        // 色相值 (hue) 隨時間在 [0, 1) 之間平滑循環，形成彩虹效果
        float hue = timeFactor % 1.0f;

        // 3. 使用 H (hue)、S (saturation) 和 V (brightness) 轉換為 RGB 顏色
        // Color.HSVToRGB(H, S, V)
        Color rainbowColor = Color.HSVToRGB(hue, saturation, brightness);

        // 4. 將計算出的顏色設定給 VFX Graph
        // 注意：VFX Graph 的 SetVector4 接收的是 Vector4，但 Color 類型會自動轉換為 Vector4 (R, G, B, A)
        vfxGraph.SetVector4(colorPropertyID, rainbowColor);
    }
}