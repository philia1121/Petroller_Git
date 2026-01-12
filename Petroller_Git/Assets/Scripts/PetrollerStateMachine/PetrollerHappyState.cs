using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerHappyState : PetrollerBaseState
{
    public PetrollerHappyState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsHappyHash, true);
        _ctx.MyHaptic.SetConstantRumble(_ctx.SleepThreshold, _ctx.HapticAmplitude[1]);
        if (_ctx.AllAudioClips[2]) _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[2] }, 1, 3);
    }
    public override void UpdateState()
    {
        // if (_ctx.ClipEnd)
        // {
        //     _ctx.MyHaptic.StopRumble();
        //     _ctx.MyHaptic.SetConstantRumble(5, _ctx.HapticAmplitude[1]);
        //     _ctx.ClipEnd = false;
        // }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsHappyHash, false);
        _ctx.MyHaptic.StopRumble();
        _ctx.ResetPat();
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar | _ctx.Slaped)
        {
            SwitchState(_factory.Angry());
        }
        else if (_ctx.LT_Compensation && _ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Umcomfortable());
        }
        else if (_ctx.Pressed | _ctx.Speeding)
        {
            SwitchState(_factory.Surprised());
        }
        else if (_ctx.CozyTimer > _ctx.SleepThreshold)
        {
            SwitchState(_factory.Sleep());
        }
        else if (_ctx.CozyTimer == 0)
        {
            SwitchState(_factory.Idle());
        }
    }
    public override void InitializeSubState()
    {

    }
}
