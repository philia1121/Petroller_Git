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
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.GetRebootHash, true);
    }
    public override void CheckSwitchStates()
    {
        if(_ctx.Reboot){ SwitchState(_factory.Idle()); }
    }
    public override void InitializeSubState()
    {

    }
}
