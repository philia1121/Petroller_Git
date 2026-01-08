using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    public float gameDuration = 120;
    public float CountDown { get; private set; }
    Coroutine cor;
    void OnEnable()
    {
        GameSignals.OnRequestStartGame += StartGame;
        GameSignals.OnRequestEndGame += EndGame;
    }
    void OnDisable()
    {
        GameSignals.OnRequestStartGame -= StartGame;
        GameSignals.OnRequestEndGame -= EndGame;
    }
    public void StartGame()
    {
        cor = StartCoroutine(GameCountDown());
    }
    public void EndGame()
    {
        if (cor != null) StopCoroutine(cor);
    }
    IEnumerator GameCountDown()
    {
        CountDown = gameDuration;
        while (CountDown > 0)
        {
            yield return new WaitForSeconds(1);
            CountDown--;
        }
        GameSignals.OnRequestEndGame?.Invoke();
    }
    public void ForceStartGame()
    {
        GameSignals.OnRequestStartGame?.Invoke();
    }
    public void ForceEndGame()
    {
        GameSignals.OnRequestEndGame?.Invoke();
    }
}
public static class GameSignals
{
    public static System.Action OnRequestStartGame;
    public static System.Action OnRequestEndGame;
}
