using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.PlayerLoop;

public class LostTrackedTimerBar : MonoBehaviour, IConfigInitializable, ILanguageInitializable
{
    public bool ignore = false;
    public bool lifeBeing = true;

    [Header("UI")]
    public GameObject barUI;
    public Image bar;
    public TextMeshProUGUI description;
    public GameObject[] GameOver;

    [Header("Language")]
    public SharedContent_LanguageData[] lanConfigs;
    string countdownText;

    [Header("System")]
    public MonoBehaviour behaviour;
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
            description.text = countdownText + (timer > 0 ? timer.ToString("0.00") : "0.00");

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
    public void Initialize_LTCompensation(bool value)
    {
        gameObject.SetActive(value);
    }
    public void Initialize_LifeBeing(bool value)
    {
        lifeBeing = value;
    }
    public void Initialize_Language(InterfaceConfig.LanguageConfig lan)
    {
        int i = lan == InterfaceConfig.LanguageConfig.CH ? 0 : 1;
        countdownText = lifeBeing ? lanConfigs[i].Countdown_Life : lanConfigs[i].Countdown_Lifeless;
    }
}
