using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LostTrackedTimerBar : MonoBehaviour
{
    public bool ignore = false;
    public GameObject barUI;
    public Image bar;
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
            yield return null;
        }
        bar.fillAmount = 0;
        yield return new WaitForSeconds(1);
        barUI.SetActive(false);
    }
}
