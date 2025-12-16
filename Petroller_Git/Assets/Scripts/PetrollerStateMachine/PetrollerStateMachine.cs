using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0414
public class PetrollerStateMachine : MonoBehaviour
{
    // State Machine //
    PetrollerBaseState _currentState;
    public PetrollerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public string currentState_string;
    PetrollerStateFactory _states;

    // Petroller Info //
    public PetrollerObjectInfo PetrollerInfo { get; private set; }

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
    public int IsHappyHash { get; private set; }
    public int IsUncomfortableHash { get; private set; }
    public int GetRebootHash { get; private set; }
    #endregion

    [Header("Audio")]
    public SimpleAudioPlayer IdleAudioPlayer;
    public SimpleAudioPlayer HappyAudioPlayer;
    public SimpleAudioPlayer SleepAudioPlayer;
    public AudioSource AllForOneAudioSourse;

    [Header("Automation")]
    private bool simpleTimeUp;

    [Header("Interaction")]
    [SerializeField] private float speedingThreshold = 2f;
    [SerializeField] private float spitThreshold = 5f;
    public float SpitThreshold { get { return spitThreshold; } }
    [SerializeField] private float passOutThreshold = 5f;
    public float PassOutThreshold { get { return passOutThreshold; } }
    [SerializeField] private float happyThreshold = 15f;
    public float HappyThreshold { get { return happyThreshold; } }
    [SerializeField] private float sleepThreshold = 15f;
    public float SleepThreshold { get { return sleepThreshold; } }
    public bool Speeding { get; private set; }
    public bool PulledEar { get; private set; }
    public bool Pressed { get; private set; }
    public bool IsCozy { get; private set; }
    public float CozyTimer { get; private set; } = 0;
    public float PreLTTimer { get; private set; } = 0;
    public float LTTimer { get; private set; } = 0;
    public float OverallLTTimer { get; private set; } = 0;
    [HideInInspector] public bool Reboot = false;

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
        if (!MyAnimator) MyAnimator = GetComponent<Animator>();
        if (!myAnimationEvent) myAnimationEvent = MyAnimator.GetComponent<AnimationEvent>();
        PetrollerInfo = FindFirstObjectByType<PetrollerObjectInfo>();

        // setup animator parameter hash
        SetupAnimatorHash();
    }

    void Start()
    {
        _states = new PetrollerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    void Update()
    {
        CheckInteraction();
        CheckSpeeding();
        CountLostTrackedTime();
        CountCozyTime();

        currentState_string = _currentState.ToString();
        _currentState.UpdateState();
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


    public void CheckInteraction()
    {
        PulledEar = PetrollerInfo.CurrentJoystickDir == PetrollerObjectInfo.JoystickDir.Ear;
        Pressed = PetrollerInfo.HorizontalPress | PetrollerInfo.VerticalPress;
    }
    public void CheckSpeeding()
    {
        Speeding = PetrollerInfo.Speed > speedingThreshold;
    }
    void AnimationEventReceiver() { ClipEnd = true; }
    public void GetReboot() { Reboot = true; }
    void CountCozyTime()
    {
        if (!Speeding & PetrollerInfo.IsInZone("Cozy"))
        {
            if (!IsCozy) IsCozy = true;
            CozyTimer += Time.deltaTime;
        }
        else
        {
            IsCozy = false;
            CozyTimer = 0;
        }
    }
    void CountLostTrackedTime()
    {
        var currentTrackingState = PetrollerInfo.CurrentTrackingState;
        // if (currentTrackingState == PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked)
        // {
        //     PreLTTimer += Time.deltaTime;
        // }
        // else
        // {
        //     PreLTTimer = 0;
        // }

        // if (currentTrackingState == PetrollerObjectInfo.TrackingStatus.LostTracked)
        // {
        //     LTTimer += Time.deltaTime;
        // }
        // else
        // {
        //     LTTimer = 0;
        // }

        if (currentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            OverallLTTimer += Time.deltaTime;
        }
        else
        {
            OverallLTTimer = 0;
            PreLTTimer = 0;
            LTTimer = 0;
        }
    }
    public void ResetCatStateMachine()
    {
        // reset all parameters


        // reset state
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    void SetupAnimatorHash()
    {
        IdleBlendHash = Animator.StringToHash("IdleBlend");
        GetAngryHash = Animator.StringToHash("GetAngry");
        GetSurprisedHash = Animator.StringToHash("GetSurprised");
        IsSleepingHash = Animator.StringToHash("IsSleeping");
        SleepBlendHash = Animator.StringToHash("SleepBlend");
        GetPassOutHash = Animator.StringToHash("GetPassOut");
        GetSpitHash = Animator.StringToHash("GetSpit");
        GetHappyHash = Animator.StringToHash("GetHappy");
        IsHappyHash = Animator.StringToHash("IsHappy");
        IsUncomfortableHash = Animator.StringToHash("IsUncomfortable");
        GetRebootHash = Animator.StringToHash("GetReboot");
    }
}
