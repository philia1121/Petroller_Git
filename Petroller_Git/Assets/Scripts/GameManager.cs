using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Samples;
using UnityEngine;
using static OVRVirtualKeyboard;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public CatAnimationControl playpressSE;
    [Header("階段物件")]
    public GameObject[] stages; // 存放每個階段的物件

    private int currentStage = 0; // 當前階段索引 (從0開始)
    private int pressCount = 0;    // 按壓次數計數器
    public int requiredPresses; // 每階段需要的按壓次數
    private bool isCooldown = false; // 是否處於冷卻中
    public float cooldownTime; // 按鈕按壓後的冷卻時間（秒）
    private float remainingCooldownTime; // 剩餘冷卻時間

    // 每個輸入來源的計數器
    private int joystickUpCount = 0;
    private int joystickDownCount = 0;
    private int joystickLeftCount = 0;
    private int joystickRightCount = 0;
    private int triggerPressCount = 0;//UaDPress count
    private int gripPressCount = 0;//LaRPress count

    private float timer = 0f; // 用於計時的變數
    public float countdownTime = 5f; // 設置倒數計時秒數
    public int final_stage_countdown;

    [SerializeField] private AudioClip transSceneSE;
    
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // 增加按壓計數
    public void AddPress(string inputSource) {
        if (isCooldown) return; // 如果處於冷卻時間，忽略輸入

        // 根據輸入來源增加計數
        switch (inputSource)
        {
            case "JoystickUp":
                joystickUpCount++;
                break;
            case "JoystickDown":
                joystickDownCount++;
                break;
            case "JoystickLeft":
                joystickLeftCount++;
                break;
            case "JoystickRight":
                joystickRightCount++;
                break;
            case "Trigger":
                triggerPressCount++;
                break;
            case "Grip":
                gripPressCount++;
                break;
            default:
                break;
        }

        pressCount++; // 增加總計數次數

        // 檢查是否達到要求的按壓次數
        if ((currentStage == 0) && (gripPressCount >= requiredPresses))
        {
            SwitchToNextStage();
            ResetCounter();
           // Debug.Log("currentStage: "+ currentStage+ " , gripPressCount: "+ gripPressCount + ", requiredPresses, " + requiredPresses);
        }
        if ((currentStage == 1) && (triggerPressCount >= requiredPresses))
        {
            SwitchToNextStage();
            ResetCounter();
            //Debug.Log("currentStage: " + currentStage + " , gripPressCount: " + gripPressCount + ", requiredPresses, " + requiredPresses);
        }
        if (currentStage == 2 && gripPressCount >= requiredPresses)
        {
            SwitchToNextStage();
            ResetCounter();
            //Debug.Log("currentStage: " + currentStage + " , gripPressCount: " + gripPressCount + ", requiredPresses, " + requiredPresses);
        }
        if (currentStage == 3 && joystickUpCount >= requiredPresses)
        {
            SwitchToNextStage();
            ResetCounter();
            //Debug.Log("Time Sec: " + timer);
        }
        if (currentStage == 4 && joystickRightCount >= requiredPresses)
        {
            SwitchToNextStage();
            ResetCounter();
            //Debug.Log("Time Sec: " + timer);
        }
        if (currentStage == 5 && joystickLeftCount >= requiredPresses)
        {
            SwitchToNextStage();
            ResetCounter();
            timer = 0f;// 重置計時
            //Debug.Log("Time Sec: "+ timer);
        }
        if (currentStage == 6 && timer >= final_stage_countdown)
        {
            SwitchToNextStage();
            ResetCounter();
        }
  

        // 啟動冷卻時間
        StartCoroutine(CooldownCoroutine());
    }

    private void ResetCounter() {
        joystickUpCount = 0;
        joystickDownCount = 0;
        joystickLeftCount = 0;
        joystickRightCount = 0;
        triggerPressCount = 0;
        gripPressCount = 0;
        pressCount = 0;
    }

    //// 用來增加按壓次數並且更新邏輯
    //public void AddPress() {
    //    // 檢查是否在冷卻中
    //    if (isCooldown) return;

    //    pressCount++; // 增加按壓次數
    //    Debug.Log($"當前按壓次數：{pressCount}/{requiredPresses}");

    //    if (pressCount >= requiredPresses) // 如果達到要求的按壓次數，切換階段
    //    {
    //        SwitchToNextStage();
    //    }

    //    // 開啟冷卻時間
    //    StartCoroutine(CooldownCoroutine());
    //}

    // 切換到下一階段
    public void SwitchToNextStage() {
        stages[currentStage].SetActive(false);
        currentStage++;
        stages[currentStage].SetActive(true);
        ////pressCount = 0; // 歸零計數器
        if (currentStage >= stages.Length)
        {
            Debug.Log("已達到最大階段，無法再切換！");
            return;
        }
        //// 隱藏當前階段的物件
        //if (currentStage < stages.Length && stages[currentStage] != null)
        //{
        //    stages[currentStage].SetActive(false);  // 隱藏當前階段物件
        //}
        //currentStage++;
        //// 顯示下一階段的物件
        //if (currentStage < stages.Length && stages[currentStage] != null)
        //{
        //    stages[currentStage].SetActive(true);
        //}

        // 進入下一階段，計數器歸零

        Debug.Log($"切換到階段 {currentStage}");
    }

    // 冷卻協程，控制冷卻時間
    private IEnumerator CooldownCoroutine() {
        isCooldown = true; // 設定為冷卻中
        remainingCooldownTime = cooldownTime; // 初始化剩餘冷卻時間

        while (remainingCooldownTime > 0) // 當冷卻時間大於 0 時
        {
            remainingCooldownTime -= Time.deltaTime; // 減少剩餘冷卻時間
            yield return null; // 等待下一幀
        }

        isCooldown = false; // 冷卻時間結束
        remainingCooldownTime = 0; // 冷卻時間歸零
    }
       
        // 更新每幀的剩餘時間回報
        private void Update() {
        if (isCooldown)
        {
            //Debug.Log($"剩餘冷卻時間：{remainingCooldownTime:F0}秒"); // 輸出剩餘冷卻時間
        }
        //Debug.Log("目前階段" + currentStage + "," + "目前按壓" + pressCount);


        timer += Time.deltaTime; // 每幀累加時間
        //Debug.Log(timer);
        //if (timer == 30)
        //{
        //    Debug.Log(timer);
        //}
        //if (timer >= countdownTime) // 判斷計時是否結束
        //{
        //    Debug.Log("時間到！");
        //    timer = 0f; // 重置計時
        //}

    }

    //// 獲取每個方向的按壓次數
    //public int GetJoystickUpCount() {
    //    return joystickUpCount;
    //}

    //public int GetJoystickDownCount() {
    //    return joystickDownCount;
    //}

    //public int GetJoystickLeftCount() {
    //    return joystickLeftCount;
    //}

    //public int GetJoystickRightCount() {
    //    return joystickRightCount;
    //}
    //public int GettriggerPressCount() {
    //    return triggerPressCount;
    //}
    //public int GetGripPressCount() {
    //    return gripPressCount;
    //}
}
