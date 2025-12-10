using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerUncomfortableState : PetrollerBaseState
{
    public PetrollerUncomfortableState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }

    bool haveSpited = false;
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, true);
        haveSpited = _ctx.OverallLTTimer > _ctx.SpitThreshold; // in case this states is interrupted by Angry state
    }
    public override void UpdateState()
    {
        if (_ctx.OverallLTTimer > _ctx.SpitThreshold & !haveSpited)
        {
            haveSpited = true;
            _ctx.MyAnimator.SetTrigger(_ctx.GetSpitHash);
        }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, false);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
        haveSpited = false;
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.OverallLTTimer == 0)
        {
            SwitchState(_factory.Idle());
        }
        else if (_ctx.OverallLTTimer > _ctx.PassOutThreshold) // being lost tracked for too long
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
