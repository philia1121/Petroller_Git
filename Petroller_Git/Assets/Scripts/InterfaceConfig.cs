using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class InterfaceConfig : MonoBehaviour
{
    public bool LT_Compensation = true;
    public bool LifeBeing = true;
    public enum LanguageConfig { EN, CH }
    public LanguageConfig Lan = LanguageConfig.EN;
    void Awake()
    {
        var con = FindObjectsOfType<MonoBehaviour>().OfType<IConfigInitializable>();
        foreach (var item in con)
        {
            item.Initialize_LTCompensation(LT_Compensation);
            item.Initialize_LifeBeing(LifeBeing);
        }
        var lan = FindObjectsOfType<MonoBehaviour>().OfType<ILanguageInitializable>();
        foreach (var item in lan)
        {
            item.Initialize_Language(Lan);
        }
    }
}

public interface IConfigInitializable
{
    void Initialize_LTCompensation(bool LT_Compensation);
    void Initialize_LifeBeing(bool LifeBeing);
}
public interface ILanguageInitializable
{
    void Initialize_Language(InterfaceConfig.LanguageConfig Language);
}
