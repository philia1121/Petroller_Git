using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLanguageData", menuName = "JC/LanguageData/LT_Compensation")]//
public class LTC_LanguageData : ScriptableObject
{
    [TextArea(5, 10)] public string WelcomeText;
    [TextArea(5, 10)] public string DescriptionText;
    [TextArea(5, 10)] public string EndingText;
}

[CreateAssetMenu(fileName = "NewLanguageData", menuName = "JC/LanguageData/None")]
public class NLTC_LanguageData : ScriptableObject
{
    [TextArea(5, 10)] public string WelcomeText;
    [TextArea(5, 10)] public string EndingText;
}

[CreateAssetMenu(fileName = "NewLanguageData", menuName = "JC/LanguageData/SharedContent")]
public class SharedContent_LanguageData : ScriptableObject
{
    [TextArea(3, 10)] public string InstructionText;
    [TextArea(5, 10)] public string TutorialText;
    public string[] VideoTexts = new string[3];

    [Header("Life")]
    public string Countdown_Life;
    public string GameOver_Life;

    [Header("Lifeless")]
    public string Countdown_Lifeless;
    public string GameOver_Lifeless;
}



