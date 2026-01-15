using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

#pragma warning disable 0414
public class PetrollerStateMachine : MonoBehaviour, IConfigInitializable
{
    // State Machine //
    PetrollerBaseState _currentState;
    public PetrollerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public string currentState_string;
    PetrollerStateFactory _states;
    public bool FinishedRead { get; private set; } = false;
    public bool Started { get; private set; } = false;

    // Petroller Info //
    public PetrollerObjectInfo PetrollerInfo { get; private set; }

    [Header("Experiment Interface")]
    public bool LT_Compensation = true;
    public bool LifeBeing = true;

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
    public int IsHappyHash { get; private set; }
    public int IsUncomfortableHash { get; private set; }
    public int GetRebootHash { get; private set; }
    #endregion

    [Header("Audio")]
    public SimpleAudioPlayer AFO_AudioPlayer;
    public AudioSource AFO_AudioSourse;
    public AudioClip[] AllAudioClips; // 0:idle, 1:idle, 2:happy, 3:angry, 4:surprise, 5:uncomfortable, 6: spit

    [Header("Haptic")]
    public ControllerHaptic MyHaptic;
    public float[] HapticAmplitude = new float[] { 0, 0.3f, 0.6f, 1 };

    [Header("Automation")]
    private bool simpleTimeUp;

    [Header("Interaction")]
    [SerializeField] private float speedingThreshold = 2f;
    [SerializeField] private float uncomfortableHapticDuration = 1;
    [SerializeField] private float uncomfortableHapticInterval = 0.2f;
    [SerializeField] private float spitThreshold = 5f;
    [SerializeField] private float spitRandomizeRange = 5f;
    [SerializeField] private float passOutThreshold = 5f;
    [SerializeField] private float happyThreshold = 15f;
    [SerializeField] private float sleepThreshold = 15f;
    public bool Speeding { get; private set; }
    public bool PulledEar { get; private set; }
    public bool Slaped { get; private set; }
    public bool Patted { get; private set; }
    public bool Pressed { get; private set; }
    public bool IsCozy { get; private set; }
    public float CozyTimer { get; private set; } = 0;
    public bool Reboot { get; private set; } = false;
    public float LT_Timer { get; private set; }
    public float PLT_Timer { get; private set; }
    public float UncomfortableHaticDuration { get { return uncomfortableHapticDuration; } }
    public float UncomfortableHapticInterval { get { return uncomfortableHapticInterval; } }
    public float SpitThreshold { get { return spitThreshold; } }
    public float SpitRandomizeRange { get { return spitRandomizeRange; } }
    public float PassOutThreshold { get { return passOutThreshold; } }
    public float HappyThreshold { get { return happyThreshold; } }
    public float SleepThreshold { get { return sleepThreshold; } }

    void OnEnable()
    {
        GameSignals.OnRequestStartGame += StartGame;
        myAnimationEvent.AnimationTriggerEvent.AddListener(AnimationEventReceiver);
    }
    void OnDisable()
    {
        myAnimationEvent.AnimationTriggerEvent.RemoveListener(AnimationEventReceiver);
    }
    public void Initialize_LTCompensation(bool value)
    {
        LT_Compensation = value;
    }
    public void Initialize_LifeBeing(bool value)
    {
        if (LifeBeing != value) this.gameObject.SetActive(false);
    }
    void Awake()
    {
        // Auto Get Refs
        if (!MyAnimator) MyAnimator = GetComponent<Animator>();
        if (!myAnimationEvent) myAnimationEvent = MyAnimator.GetComponent<AnimationEvent>();
        if (!MyHaptic) MyHaptic = this.AddComponent<ControllerHaptic>();
        PetrollerInfo = FindFirstObjectByType<PetrollerObjectInfo>();

        // setup animator parameter hash
        SetupAnimatorHash();
    }
    void Start()
    {
        _states = new PetrollerStateFactory(this);
        _currentState = _states.Sleep();
        _currentState.EnterState();
    }
    public void StartGame()
    {
        ResetLostTrackedTime();
        ResetCozyTime();
        Started = true;
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
    public void GetSlaped() { Slaped = true; }
    public void ResetSlaped() { Slaped = false; }
    public void GetPat() { Patted = true; }
    public void ResetPat() { Patted = false; }
    public void CheckSpeeding()
    {
        Speeding = PetrollerInfo.Speed > speedingThreshold;
    }
    public void SetFinishedRead(bool value) { FinishedRead = value; }
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
    public void ResetCozyTime() { CozyTimer = 0; }
    void CountLostTrackedTime()
    {
        switch (PetrollerInfo.CurrentTrackingState)
        {
            case PetrollerObjectInfo.TrackingStatus.Tracked:
                PLT_Timer = 0;
                LT_Timer = 0;
                break;
            case PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked:
                PLT_Timer += Time.deltaTime;
                LT_Timer = 0;
                break;
            case PetrollerObjectInfo.TrackingStatus.LostTracked:
                PLT_Timer = 0;
                LT_Timer += Time.deltaTime;
                break;
            default:
                break;
        }
    }
    void ResetLostTrackedTime()
    {
        PLT_Timer = 0;
        LT_Timer = 0;
    }
    // for event calling on angry animation event triggered
    public void TriggerAngryFeedback() // trigger via animation event
    {
        if (!Started) return;
        MyHaptic.SetConstantRumble(1, HapticAmplitude[2]);
        if (AllAudioClips[3]) AFO_AudioPlayer.PlayAudio_Assigned(AllAudioClips[3]);
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
        IsHappyHash = Animator.StringToHash("IsHappy");
        IsUncomfortableHash = Animator.StringToHash("IsUncomfortable");
        GetRebootHash = Animator.StringToHash("GetReboot");
    }
}
