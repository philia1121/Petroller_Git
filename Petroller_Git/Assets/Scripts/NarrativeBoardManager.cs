using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using TMPro;
public class NarrativeBoardManager : MonoBehaviour, IConfigInitializable
{
    ControlMap controlMap;
    public GameObject[] WelcomePages;
    public GameObject[] DescriptionPages;
    public GameObject TutorialPage;
    public GameObject[] VideoSets;
    List<GameObject> allPages = new List<GameObject>();
    public GameObject[] EndingPages;
    int currentPage = 0;
    public bool gameStarted = false;
    bool LT_Compensation;
    bool lifebeing;

    public void Initialize_LTCompensation(bool value)
    {
        LT_Compensation = value;
    }
    public void Initialize_LifeBeing(bool value)
    {
        lifebeing = value;
        VideoSets[0].SetActive(lifebeing);
        VideoSets[1].SetActive(!lifebeing);
    }
    void Awake()
    {
        controlMap = new ControlMap();
    }
    void Start()
    {
        foreach (var page in WelcomePages) { page.SetActive(false); }
        foreach (var page in DescriptionPages) { page.SetActive(false); }
        foreach (var page in EndingPages) { page.SetActive(false); }
        TutorialPage.SetActive(false);

        PickPages();
        currentPage = 0;
        allPages[currentPage].SetActive(true);
    }
    void PickPages()
    {
        allPages.Add(WelcomePages[LT_Compensation ? 0 : 1]);
        if (LT_Compensation) allPages.Add(DescriptionPages[lifebeing ? 0 : 1]);
        allPages.Add(TutorialPage);
    }
    void OnEnable()
    {
        controlMap.PlayerInput.Enable();

        controlMap.PlayerInput.NextPage.started += ctx => ShowNextPage();
        controlMap.PlayerInput.PreviousPage.started += ctx => ShowPreviousPage();

        GameSignals.OnRequestEndGame += ShowEndPage;
    }
    void OnDisable()
    {
        controlMap.PlayerInput.NextPage.started -= ctx => ShowNextPage();
        controlMap.PlayerInput.PreviousPage.started -= ctx => ShowPreviousPage();

        GameSignals.OnRequestEndGame -= ShowEndPage;
    }

    void ShowNextPage()
    {
        if (gameStarted) return;

        allPages[currentPage].SetActive(false);
        currentPage++;
        if (currentPage < allPages.Count)
        {
            allPages[currentPage].SetActive(true);
        }
        else
        {
            foreach (var page in allPages)
            {
                page.gameObject.SetActive(false);
            }
            gameStarted = true;
            GameSignals.OnRequestStartGame?.Invoke();
        }
    }
    void ShowPreviousPage()
    {
        if (gameStarted) return;

        allPages[currentPage].SetActive(false);
        currentPage--;
        currentPage = currentPage <= 0 ? 0 : currentPage;
        if (currentPage >= 0)
        {
            allPages[currentPage].SetActive(true);
        }
    }
    public void ShowEndPage()
    {
        EndingPages[LT_Compensation ? 0 : 1].SetActive(true);
    }
}

