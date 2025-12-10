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
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsHappyHash, false);
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }
        else if (_ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            SwitchState(_factory.Umcomfortable());
        }
        else if (_ctx.Pressed | _ctx.Speeding)
        {
            SwitchState(_factory.Surprised());
        }
        else if(_ctx.CozyTimer > _ctx.SleepThreshold)
        {
            SwitchState(_factory.Sleep());
        }
    }
    public override void InitializeSubState()
    {

    }
}
