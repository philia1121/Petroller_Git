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
        if (_ctx.Slaped) _ctx.ResetSlaped();
    }
    public override void UpdateState()
    {
        _ctx.MyAnimator.SetTrigger(_ctx.GetAngryHash);
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.ResetTrigger(_ctx.GetAngryHash);
        _ctx.MyHaptic.StopRumble();
        _ctx.ResetCozyTime();
        if (_ctx.Slaped) _ctx.ResetSlaped();
    }
    public override void CheckSwitchStates()
    {
        if (!_ctx.PulledEar)
        {
            if (_ctx.LT_Compensation && _ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
            {
                SwitchState(_factory.Umcomfortable());
            }
            else if (_ctx.Pressed | _ctx.Speeding)
            {
                SwitchState(_factory.Surprised());
            }
            else
            {
                SwitchState(_factory.Idle());
            }
        }

    }
    public override void InitializeSubState()
    {

    }
}
