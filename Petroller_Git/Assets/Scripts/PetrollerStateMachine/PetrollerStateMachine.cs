using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class PetrollerStateMachine : MonoBehaviour
{
    PetrollerBaseState _currentState;
    public PetrollerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public string currentState_string;
    PetrollerStateFactory _states;

    [Header("Animation")]
    public Animator MyAnimator;
    AnimationEvent myAnimationEvent;
    [HideInInspector] public bool ClipEnd = false;

    #region AnimationHash
    public int IdleBlendHash { get; private set; }
    public int GetAngryHash { get; private set; }
    public int GetSurprisedHash { get; private set; }
    public int IsSleepingHash { get; private set; }
    public int SleepBlendHash { get; private set; }
    public int GetPassOutHash { get; private set; }
    public int GetSpitHash { get; private set; }
    public int GetHappyHash { get; private set; }
    public int IsUncomfortableHash { get; private set; }
    public int GetRebootHash { get; private set; }
    #endregion

    [Header("Audio")]
    public AudioSource MyAudioSource;
    public AudioClip[] MyAudioClips { get; private set; }

    [Header("Automation")]
    private bool simpleTimeUp;

    [Header("Interaction")]
    [HideInInspector] public bool Speeding;
    [HideInInspector] public bool PulledEar;
    [HideInInspector] public bool Pressed;
    [HideInInspector] public bool CozyForHappy;
    [HideInInspector] public bool CozyForSleep;
    void OnEnable()
    {
        myAnimationEvent.AnimationTriggerEvent.AddListener(AnimationEventReceiver);
    }
    void OnDisable()
    {
        myAnimationEvent.AnimationTriggerEvent.RemoveListener(AnimationEventReceiver);
    }
    void Awake()
    {
        // Auto Get Refs
        if (!myAnimationEvent) myAnimationEvent = GetComponent<AnimationEvent>();
        if (!MyAnimator) MyAnimator = GetComponent<Animator>();
        if (!MyAudioSource) MyAudioSource = GetComponent<AudioSource>();

        // Animation Hash Setup
        IdleBlendHash = Animator.StringToHash("IdleBlend");
        GetAngryHash = Animator.StringToHash("GetAngry");
        GetSurprisedHash = Animator.StringToHash("GetSurprised");
        IsSleepingHash = Animator.StringToHash("IsSleeping");
        SleepBlendHash = Animator.StringToHash("SleepBlend");
        GetPassOutHash = Animator.StringToHash("GetPassOut");
        GetSpitHash = Animator.StringToHash("GetSpit");
        GetHappyHash = Animator.StringToHash("GetHappy");
        IsUncomfortableHash = Animator.StringToHash("IsUncomfortable");
        GetRebootHash = Animator.StringToHash("GetReboot");
    }

    void Start()
    {
        _states = new PetrollerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    void Update()
    {
        currentState_string = _currentState.ToString();
        _currentState.UpdateState();
    }
    void AnimationEventReceiver()
    {
        ClipEnd = true;
    }

    public IEnumerator AnimatorFloatTransition(int parameter, float targetValue, float duration)
    {
        float time = 0;
        float startValue = MyAnimator.GetFloat(parameter);

        while (time < duration)
        {
            MyAnimator.SetFloat(parameter, Mathf.Lerp(startValue, targetValue, time / duration));
            time += Time.deltaTime;
            yield return null;
        }

        MyAnimator.SetFloat(parameter, targetValue);
    }
    public IEnumerator RandomSimpleTimer(float min, float max)
    {
        float randomTime = Random.Range(min, max);
        yield return new WaitForSeconds(randomTime);
        simpleTimeUp = true;
    }

    public void RuleFired_IsSpeeding(bool value)
    {
        Speeding = value;
    }
    public void RuleFired_GetPressed(bool value)
    {
        Pressed = value;
    }
    public void RuleFired_GetPulledEar(bool value)
    {
        PulledEar = value;
    }
    public void RuleFired_GetCozyForHappy(bool value) // {stay within [assigned zone]} with {speed no faster than [value]} (for [value] sec.)
    {
        CozyForHappy = value;
    }
    public void RuleFired_GetCozyForSleep(bool value)
    {
        CozyForSleep = value;
    }

    public void ResetCatStateMachine()
    {
        // reset all parameters


        // reset state
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
}
