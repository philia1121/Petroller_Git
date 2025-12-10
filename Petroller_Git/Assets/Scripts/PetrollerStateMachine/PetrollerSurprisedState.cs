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
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSurprisedHash);
    }
    public override void CheckSwitchStates()
    {
        if (!_ctx.ClipEnd) return;

        if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }
        else if(_ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Umcomfortable());
        }
        else
        {
            SwitchState(_factory.Idle());
        }
    }
    public override void InitializeSubState()
    {

    }
}
