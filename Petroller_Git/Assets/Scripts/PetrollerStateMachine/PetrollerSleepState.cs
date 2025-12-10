using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerSleepState : PetrollerBaseState
{
    public PetrollerSleepState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {

    }
    public override void UpdateState()
    {

    }
    public override void ExitState()
    {

    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }
    }
    public override void InitializeSubState()
    {

    }
}
