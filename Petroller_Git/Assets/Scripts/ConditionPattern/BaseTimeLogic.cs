using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class BaseTimeLogic
{
    public abstract bool Tick(bool isConditionMet, float deltaTime);
    public abstract void Reset();
}

// 範例 1：瞬間達成 (只要滿足就觸發)
[System.Serializable]
public class InstantLogic : BaseTimeLogic
{
    private bool alreadyFired = false; // 用於 "僅觸發一次" 的邏輯

    public override bool Tick(bool isConditionMet, float deltaTime)
    {
        if (isConditionMet && !alreadyFired)
        {
            alreadyFired = true;
            return true;
        }
        if (!isConditionMet)
        {
            alreadyFired = false; // 條件不滿足時重置
        }
        return false;
    }

    public override void Reset() { alreadyFired = false; }
}

// 範例 2：維持 N 秒
[System.Serializable]
public class MaintainForNSecondsLogic : BaseTimeLogic
{
    public float secondsToMaintain = 3.0f;
    private float currentTimer = 0f;

    public override bool Tick(bool isConditionMet, float deltaTime)
    {
        if (isConditionMet)
        {
            currentTimer += deltaTime;
            if (currentTimer >= secondsToMaintain)
            {
                return true; // 達成！
            }
        }
        else
        {
            currentTimer = 0f; // 條件中斷，計時器重置
        }
        return false;
    }

    public override void Reset() { currentTimer = 0f; }
}

// 範例 3：N 秒內達成 M 次
[System.Serializable]
public class MTimesInNSecondsLogic : BaseTimeLogic
{
    public int timesToHappen = 5;
    public float timeWindow = 10.0f;

    // 使用佇列來儲存事件發生的時間戳
    private Queue<float> eventTimestamps = new Queue<float>();
    private bool wasMetLastFrame = false; // 用於偵測 "上升緣" (剛達成的那一幀)

    public override bool Tick(bool isConditionMet, float deltaTime)
    {
        float currentTime = Time.time;

        // 1. 移除佇列中超過 N 秒的舊時間戳
        while (eventTimestamps.Count > 0 && currentTime - eventTimestamps.Peek() > timeWindow)
        {
            eventTimestamps.Dequeue();
        }

        // 2. 偵測條件是否 "剛被滿足" (從 false 變 true)
        if (isConditionMet && !wasMetLastFrame)
        {
            eventTimestamps.Enqueue(currentTime);
        }
        wasMetLastFrame = isConditionMet;

        // 3. 檢查次數是否達標
        if (eventTimestamps.Count >= timesToHappen)
        {
            return true; // 達成！
        }
        
        return false;
    }

    public override void Reset()
    {
        eventTimestamps.Clear();
        wasMetLastFrame = false;
    }
}