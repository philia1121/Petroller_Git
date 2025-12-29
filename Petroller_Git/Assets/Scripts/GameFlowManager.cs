using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowManager : MonoBehaviour
{
    NarrativeBoardManager narrativeManager;
    AutoLogManager autoLogManager;
    public float gameDuration = 120;
    public float CountDown { get; private set; }
    void Awake()
    {
        narrativeManager = FindFirstObjectByType<NarrativeBoardManager>();
        autoLogManager = FindFirstObjectByType<AutoLogManager>();
    }
    void Start()
    {
        narrativeManager.OnGameStart.AddListener(StartGame);
    }
    public void StartGame()
    {
        autoLogManager.StartAutoLog();
        StartCoroutine(GameCountDown());
    }
    public void EndGame()
    {
        autoLogManager.StopAutoLog();
        narrativeManager.ShowEndPage();
    }
    IEnumerator GameCountDown()
    {
        CountDown = gameDuration;
        while (CountDown > 0)
        {
            yield return new WaitForSeconds(1);
            CountDown--;
        }
        EndGame();
    }
}
