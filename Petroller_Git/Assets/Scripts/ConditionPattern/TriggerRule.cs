using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class TriggerRule
{
    public string ruleName; // 方便在 Inspector 中辨識

    [Tooltip("要檢測的條件 (ScriptableObject)")]
    public BaseCondition condition;

    // 關鍵：這會讓 Unity Inspector 顯示一個下拉選單，
    // 讓你選擇 BaseTimeLogic 的所有子類 (Instant, Maintain, MTimes)
    [SerializeReference]
    [SubclassSelector]
    public BaseTimeLogic timeLogic = new MaintainForNSecondsLogic(); // 預設值

    [Tooltip("當規則被觸發時要執行的動作")]
    public UnityEvent OnRuleMet;

    public bool fireOnce = true; // 是否只觸發一次
    private bool hasFired = false;

    // 由 ComplexTrigger 在 Update() 中呼叫
    public void Tick(GameObject owner, Transform player, float deltaTime)
    {
        if (fireOnce && hasFired) return;

        bool isConditionMet = (condition == null) ? true : condition.IsMet(owner, player);

        bool didTimeLogicTrigger = timeLogic.Tick(isConditionMet, deltaTime);

        if (didTimeLogicTrigger)
        {
            Debug.Log($"Trigger Rule '{ruleName}' MET!");
            OnRuleMet.Invoke();
            hasFired = true;
            timeLogic.Reset();
        }
    }
}