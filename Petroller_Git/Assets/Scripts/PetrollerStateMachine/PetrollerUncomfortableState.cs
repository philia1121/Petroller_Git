using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PetrollerUncomfortableState : PetrollerBaseState
{
    public PetrollerUncomfortableState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }

    float spitTime = 0;
    bool setSpitTime = false;
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, true);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
        _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[5] }, 1, 3);
        _ctx.MyHaptic.StartIntervalRumble(_ctx.UncomfortableHaticDuration, _ctx.UncomfortableHapticInterval, _ctx.HapticAmplitude[3]);
        setSpitTime = false;
    }
    public override void UpdateState()
    {
        switch (_ctx.PetrollerInfo.CurrentTrackingState)
        {
            case PetrollerObjectInfo.TrackingStatus.PresumptiveLostTracked:
                if (!setSpitTime)
                {
                    setSpitTime = true;
                    spitTime = _ctx.PLT_Timer + Random.Range(_ctx.SpitThreshold, _ctx.SpitThreshold + _ctx.SpitRandomizeRange);
                }
                if (_ctx.PLT_Timer > spitTime)
                {
                    setSpitTime = false;
                    _ctx.MyAnimator.SetTrigger(_ctx.GetSpitHash);
                }
                break;
            case PetrollerObjectInfo.TrackingStatus.LostTracked:
                _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, true);
                _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
                break;
            default:
                break;
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
        else if (_ctx.LT_Timer > _ctx.PassOutThreshold) // being lost tracked for too long
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
