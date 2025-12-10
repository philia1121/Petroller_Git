using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerIdleState : PetrollerBaseState
{
    public PetrollerIdleState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetFloat(_ctx.IdleBlendHash, 0);
    }
    public override void UpdateState()
    {
        if (_ctx.ClipEnd)
        {
            RandomAnimationBlend();
            _ctx.ClipEnd = false;
        }
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetFloat(_ctx.IdleBlendHash, 0);
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar)
        {
            SwitchState(_factory.Angry());
        }

        if (_ctx.Pressed | _ctx.Speeding)
        {
            SwitchState(_factory.Surprised());
        }
    }
    void RandomAnimationBlend()
    {
        float rd = Random.Range(0, 1f);
        float blend;
        switch (rd)
        {
            // auto states
            case <= 0.1f: // 10%
                blend = 1;
                break;
            case <= 0.2f: // 10%
                blend = 0.5f;
                break;
            default: // 80%
                blend = 0;
                break;
        }
        _ctx.StartCoroutine(_ctx.AnimatorFloatTransition(_ctx.IdleBlendHash, blend, 0.2f));
    }
    public override void InitializeSubState()
    {

    }
}
