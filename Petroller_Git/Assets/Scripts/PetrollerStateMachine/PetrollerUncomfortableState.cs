using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PetrollerUncomfortableState : PetrollerBaseState
{
    public PetrollerUncomfortableState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }

    bool haveSpited = false;
    float LT_Timer = 0;
    float PLT_Timer = 0;
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, true);
        _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[5] }, 1, 3);
        _ctx.MyHaptic.StartIntervalRumble(_ctx.UncomfortableHaticDuration, _ctx.UncomfortableHapticInterval, _ctx.HapticAmplitude[3]);
        _ctx.ClipEnd = false;
        LT_Timer = 0;
        PLT_Timer = 0;
    }
    public override void UpdateState()
    {
        if (_ctx.PetrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked)
        {
            PLT_Timer += Time.deltaTime;
        }
        else if (_ctx.PetrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.LostTracked)
        {
            LT_Timer += Time.deltaTime;
        }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, false);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
        _ctx.MyHaptic.StopRumble();
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PetrollerInfo.CurrentTrackingState == PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Idle());
        }
        else if (LT_Timer > _ctx.PassOutThreshold) // being lost tracked for too long
        {
            SwitchState(_factory.Inactive());
        }
        else if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }
    }
    public override void InitializeSubState()
    {

    }
}
