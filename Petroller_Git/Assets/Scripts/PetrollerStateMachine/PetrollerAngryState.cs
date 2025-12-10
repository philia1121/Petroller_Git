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
    }
    public override void CheckSwitchStates()
    {
        if (!_ctx.PulledEar)
        {
            _ctx.MyAnimator.ResetTrigger(_ctx.GetAngryHash);
            SwitchState(_factory.Idle());
        }
    }
    public override void InitializeSubState()
    {

    }
}
