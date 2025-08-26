using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
public class WorkoutManager : MonoBehaviour
{
    public Animator animator;               // 角色 Animator
    public TextMeshProUGUI textMeshPro;     // 顯示倒數文字
    public RawImage img;
    public Color restColor, workoutColor, defaultColor;

    private float currentTime;
    private bool isCounting = false;        // 是否正在運動倒數
    private bool isResting = false;         // 是否正在休息
    private string currentAnimation = "";   // 紀錄現在是哪個動作

    void Start()
    {
        img.color = defaultColor;
    }

    void Update()
    {
        // 按下 S 開始 Squat
        // if (Input.GetKeyDown(KeyCode.S) && !isCounting && !isResting)
        // {
        //     StartMyWorkout("isSquating");
        // }

        // // 按下 P 開始 Plank
        // if (Input.GetKeyDown(KeyCode.P) && !isCounting && !isResting)
        // {
        //     StartMyWorkout("isPlanking");
        // }

        // // 按下 U 開始 Uping
        // if (Input.GetKeyDown(KeyCode.U) && !isCounting && !isResting)
        // {
        //     StartMyWorkout("isUping");
        // }

        // 運動倒數中
        // if (isCounting)
        // {
        //     currentTime -= Time.deltaTime;
        //     textMeshPro.text = Mathf.Ceil(currentTime).ToString();

        //     if (currentTime <= 0)
        //     {
        //         // 運動結束 → 關閉動畫，進入休息
        //         animator.SetBool(currentAnimation, false);
        //         isCounting = false;

        //         currentTime = 10f; // 休息時間
        //         isResting = true;
        //     }
        // }

        // // 休息倒數中
        // if (isResting)
        // {
        //     currentTime -= Time.deltaTime;
        //     textMeshPro.text = "Rest: " + Mathf.Ceil(currentTime).ToString();

        //     if (currentTime <= 0)
        //     {
        //         isResting = false;
        //         textMeshPro.text = "Ready!";
        //     }
        // }
    }

    public void StartWorkout(string move)
    {
        if (!isCounting && !isResting)
        {
            img.color = workoutColor;
            isCounting = true;

            currentAnimation = move;
            animator.SetBool(move, true);
        }
    }
    public void EndWorkout()
    {
        img.color = defaultColor;
        isCounting = false;

        animator.SetBool(currentAnimation, false);
    }
    public void StartRest()
    {
        img.color = restColor;
        isResting = true;
    }
    public void EndRest()
    {
        img.color = defaultColor;
        isResting = false;
    }
    void StartMyWorkout(string animationBool)
    {
        currentTime = 20f;
        isCounting = true;
        currentAnimation = animationBool;
        animator.SetBool(animationBool, true);
    }
    public void ChangeUIText(string txt)
    {
        textMeshPro.text = txt;
    }
    public void CountDownTimer(float duration)
    {
        StartCoroutine(RandomTimer(duration));
    }

    IEnumerator RandomTimer(float duration)
    {
        var timer = duration;
        while (timer >= 0)
        {
            textMeshPro.text = timer.ToString("0");
            yield return new WaitForSeconds(1);
            timer -= 1;
        }
    }
}
