using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
public class NarrativeBoardManager : MonoBehaviour, IConfigInitializable
{
    ControlMap controlMap;
    public GameObject[] AllStarterPages_Lifeless, AllStarterPagers_Life;
    GameObject[] allStarterPages;
    public GameObject GameOver_LifeLess, GameOver_Life;
    GameObject gameOver;
    public GameObject EndingPage;
    int currentPage = 0;
    public bool gameStarted = false;
    public UnityEvent OnGameStart;

    public void Initialize_LTCompensation(bool LT_Compensation)
    {
        gameObject.SetActive(LT_Compensation);
    }

    public void Initialize_LifeBeing(bool LifeBeing)
    {
        allStarterPages = LifeBeing ? AllStarterPagers_Life : AllStarterPages_Lifeless;
        gameOver = LifeBeing ? GameOver_Life : GameOver_LifeLess;
    }
    void Awake()
    {
        controlMap = new ControlMap();
    }
    void Start()
    {
        foreach (var page in AllStarterPages_Lifeless)
        {
            page.SetActive(false);
        }
        foreach (var page in AllStarterPagers_Life)
        {
            page.SetActive(false);
        }
        EndingPage.SetActive(false);
        GameOver_LifeLess.SetActive(false);
        GameOver_Life.SetActive(false);
        allStarterPages[0].SetActive(true);
        currentPage = 0;
    }
    void OnEnable()
    {
        controlMap.PlayerInput.Enable();

        controlMap.PlayerInput.NextPage.started += ctx => ShowNextPage();
        controlMap.PlayerInput.PreviousPage.started += ctx => ShowPreviousPage();
    }
    void OnDisable()
    {
        controlMap.PlayerInput.NextPage.started -= ctx => ShowNextPage();
        controlMap.PlayerInput.PreviousPage.started -= ctx => ShowPreviousPage();
    }

    void ShowNextPage()
    {
        if (gameStarted) return;

        allStarterPages[currentPage].SetActive(false);
        currentPage++;
        if (currentPage < allStarterPages.Length)
        {
            allStarterPages[currentPage].SetActive(true);
        }
        else
        {
            foreach (var page in allStarterPages)
            {
                page.gameObject.SetActive(false);
            }
            gameStarted = true;
            OnGameStart?.Invoke();
        }
    }
    void ShowPreviousPage()
    {
        allStarterPages[currentPage].SetActive(false);
        currentPage--;
        if (currentPage >= 0)
        {
            allStarterPages[currentPage].SetActive(true);
        }
    }
    public void ShowEndPage()
    {
        EndingPage.SetActive(true);
    }
    public void ShowGameOver()
    {
        gameOver.SetActive(true);
    }
}

