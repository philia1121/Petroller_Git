using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerAngryState : PetrollerBaseState
{
    public PetrollerAngryState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetAngryHash);
    }
    public override void UpdateState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetAngryHash);
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.ResetTrigger(_ctx.GetAngryHash);
    }
    public override void CheckSwitchStates()
    {
        if (!_ctx.PulledEar)
        {
            SwitchState(_factory.Idle());
        }
        else if (_ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Umcomfortable());
        }
        else if (_ctx.Pressed | _ctx.Speeding)
        {
            SwitchState(_factory.Surprised());
        }
    }
    public override void InitializeSubState()
    {

    }
}
