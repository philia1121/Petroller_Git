using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerSurprisedState : PetrollerBaseState
{
    public PetrollerSurprisedState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetSurprisedHash);
        _ctx.ClipEnd = false;
        _ctx.MyHaptic.SetConstantRumble(0.2f, _ctx.HapticAmplitude[3]);
        _ctx.AFO_AudioPlayer.StopRandomPlay();
        _ctx.AFO_AudioPlayer.PlayAudio_Assigned(_ctx.AllAudioClips[4]);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.ClipEnd = false;
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSurprisedHash);
        _ctx.MyHaptic.StopRumble();
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }
        else if (_ctx.LT_Compensation && _ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Umcomfortable());
        }
        else if(_ctx.ClipEnd)
        {
            SwitchState(_factory.Idle());
        }
    }
    public override void InitializeSubState()
    {

    }
}
