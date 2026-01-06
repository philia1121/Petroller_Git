using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LostTrackedTimerBar : MonoBehaviour, IConfigInitializable
{
    public bool ignore = false;
    public bool lifeBeing = true;
    public GameObject barUI;
    public Image bar;
    public TextMeshProUGUI description;
    PetrollerStateMachine petroller;
    float countdown;
    Coroutine cor;
    void Awake()
    {
        petroller = FindFirstObjectByType<PetrollerStateMachine>();
        countdown = petroller.PassOutThreshold;
    }
    public void Start()
    {
        ShowBar(false);
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
            description.text = lifeBeing ? "Floatite 的靈魂離開倒數 " : "Fossilite 融合倒數 " + timer.ToString("0.00");

            yield return new WaitForSeconds(0.01f);
        }
        bar.fillAmount = 0;
        yield return new WaitForSeconds(1);
        barUI.SetActive(false);
    }
    public void Initialize_LTCompensation(bool value) { }
    public void Initialize_LifeBeing(bool value)
    {
        if (lifeBeing != value) gameObject.SetActive(false);
    }
}
