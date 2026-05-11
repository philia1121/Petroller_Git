using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class LanguageSetup : MonoBehaviour, ILanguageInitializable, IConfigInitializable
{
    public LTC_LanguageData[] LTC_Life_LanConfig, LTC_Lifeless_LanConfig;
    public NLTC_LanguageData[] NLTC_LanConfig;
    public SharedContent_LanguageData[] Shared_LanConfig;
    public GameObject WelcomeText, DescriptionText, EndText;
    public GameObject[] InstructionTexts;
    public GameObject TutorialText;
    public GameObject[] VideoTexts;

    bool LT_Compensation;
    bool LifeBeing;
    InterfaceConfig.LanguageConfig Language;
    int lan;

    public void Initialize_LTCompensation(bool value)
    {
        LT_Compensation = value;
    }
    public void Initialize_LifeBeing(bool value)
    {
        LifeBeing = value;
    }

    public void Initialize_Language(InterfaceConfig.LanguageConfig value)
    {
        Language = value;
    }

    void Start()
    {
        SetLanData();
        SetText();
    }
    public void SetText()
    {
        WelcomeText.GetComponentInChildren<TextMeshProUGUI>().text = LT_Compensation ? (LifeBeing ? LTC_Life_LanConfig[lan].WelcomeText : LTC_Lifeless_LanConfig[lan].WelcomeText) : NLTC_LanConfig[lan].WelcomeText;
        DescriptionText.GetComponentInChildren<TextMeshProUGUI>().text = LT_Compensation ? (LifeBeing ? LTC_Life_LanConfig[lan].DescriptionText : LTC_Lifeless_LanConfig[lan].DescriptionText) : "";
        EndText.GetComponentInChildren<TextMeshProUGUI>().text = LT_Compensation ? (LifeBeing ? LTC_Life_LanConfig[lan].EndingText : LTC_Lifeless_LanConfig[lan].EndingText) : NLTC_LanConfig[lan].EndingText;

        TutorialText.GetComponentInChildren<TextMeshProUGUI>().text = Shared_LanConfig[lan].TutorialText;
        foreach (var item in InstructionTexts) { item.GetComponentInChildren<TextMeshProUGUI>().text = Shared_LanConfig[lan].InstructionText; }
        for (int i = 0; i < 3; i++)
        {
            VideoTexts[i].GetComponentInChildren<TextMeshProUGUI>().text = Shared_LanConfig[lan].VideoTexts[i];
        }
    }
    public void SetLanData()
    {
        lan = Language == InterfaceConfig.LanguageConfig.CH ? 0 : 1;

    }
}
