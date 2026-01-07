using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LostTrackedTimerBar : MonoBehaviour, IConfigInitializable
{
    public bool ignore = false;
    public bool lifeBeing = true;

    [Header("UI")]
    public GameObject barUI;
    public Image bar;
    public TextMeshProUGUI description;
    public GameObject[] GameOver;

    [Header("Camera Following")]
    public Transform vrCamera;
    public float distance = 2.0f;
    public float smoothSpeed = 2.0f;
    public float updateInterval = 10f;

    [Header("System")]
    public MonoBehaviour behaviour;
    Vector3 targetPosition;
    Quaternion targetRotation;
    float countdown;
    Coroutine cor;
    void Awake()
    {
        countdown = FindFirstObjectByType<PetrollerStateMachine>().PassOutThreshold;
        behaviour.enabled = false;
    }
    void OnEnable()
    {
        GameSignals.OnRequestStartGame += StartDetecting;
        GameSignals.OnRequestEndGame += StopAll;
    }
    void OnDisable()
    {
        GameSignals.OnRequestStartGame -= StartDetecting;
        GameSignals.OnRequestEndGame -= StopAll;
    }
    void StartDetecting()
    {
        behaviour.enabled = true;
    }
    void StopAll()
    {
        behaviour.enabled = false;
        if (cor != null) StopCoroutine(cor);
        ShowBar(false);
    }
    public void Start()
    {
        ShowBar(false);
        foreach (var page in GameOver) { page.SetActive(false); }

        if (vrCamera == null)
            vrCamera = Camera.main.transform;

        UpdateTargetTransform();
        transform.position = targetPosition;
        transform.rotation = targetRotation;
        StartCoroutine(UpdatePositionTimer());
    }
    public void ShowBar(bool value)
    {
        if (ignore) return;

        barUI.SetActive(value);
        if (value) bar.fillAmount = 1;
    }
    public void StopBar()
    {
        if (ignore) return;

        if (cor != null) StopCoroutine(cor);
    }
    public void UpdateBar()
    {
        if (ignore) return;

        if (cor != null) StopCoroutine(cor);
        cor = StartCoroutine(BarCountDown());
    }
    IEnumerator BarCountDown()
    {
        float timer = countdown;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            bar.fillAmount = timer / countdown;
            description.text = (lifeBeing ? "Floatite 的靈魂離開倒數 " : "Fossilite 融合倒數 ") + (timer > 0 ? timer.ToString("0.00") : "0.00");

            yield return new WaitForSeconds(0.01f);
        }
        bar.fillAmount = 0;
        barUI.SetActive(false);
        GameSignals.OnRequestEndGame?.Invoke();
        ShowGameOver();
    }

    void ShowGameOver()
    {
        GameOver[lifeBeing ? 0 : 1].SetActive(true);
    }

    void Update()
    {
        // 每一幀都平滑地移向目標位置與旋轉
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - vrCamera.position), Time.deltaTime * smoothSpeed);
    }
    IEnumerator UpdatePositionTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);
            UpdateTargetTransform();
        }
    }
    void UpdateTargetTransform()
    {
        // 計算新位置：相機位置 + 相機正前方 * 距離
        targetPosition = vrCamera.position + (vrCamera.forward * distance);

        // 讓 UI 面向使用者 (LookAt 反向)
        // 如果你希望 UI 始終保持水平，不隨頭部仰角轉動，可以鎖定 Y 軸
        targetRotation = Quaternion.LookRotation(transform.position - vrCamera.position);
    }
    public void Initialize_LTCompensation(bool value)
    {
        gameObject.SetActive(value);
    }
    public void Initialize_LifeBeing(bool value)
    {
        if (lifeBeing != value) gameObject.SetActive(false);
    }
}
