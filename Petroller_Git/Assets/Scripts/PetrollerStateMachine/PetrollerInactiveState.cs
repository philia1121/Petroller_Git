using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PetrollerInactiveState : PetrollerBaseState
{
    public PetrollerInactiveState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetPassOutHash);
        _ctx.MyHaptic.StopRumble();
        _ctx.AFO_AudioPlayer.StopRandomPlay(true);
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetRebootHash);

        // reset all animator parameters
        _ctx.MyAnimator.ResetTrigger(_ctx.GetAngryHash);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSurprisedHash);
        _ctx.MyAnimator.SetBool(_ctx.IsSleepingHash, false);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetPassOutHash);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
        _ctx.MyAnimator.SetBool(_ctx.IsHappyHash, false);
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, false);

        _ctx.ResetCozyTime();

        _ctx.GetReboot();
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.Reboot) { SwitchState(_factory.Idle()); }
    }
    public override void InitializeSubState()
    {

    }
}
