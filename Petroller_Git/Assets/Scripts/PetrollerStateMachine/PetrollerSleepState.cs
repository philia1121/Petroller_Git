using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerSleepState : PetrollerBaseState
{
    public PetrollerSleepState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsSleepingHash, true);
        _ctx.MyAnimator.SetFloat(_ctx.SleepBlendHash, 0);
        _ctx.MyHaptic.SetFadeOutRumble(10, _ctx.HapticAmplitude[1]);
        _ctx.AFO_AudioPlayer.StopRandomPlay(true);
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
        _ctx.MyAnimator.SetBool(_ctx.IsSleepingHash, false);
        _ctx.MyAnimator.SetFloat(_ctx.SleepBlendHash, 0);
        _ctx.MyHaptic.StopRumble();
    }
    public override void CheckSwitchStates()
    {
        if (_ctx.PulledEar | _ctx.Slaped)
        {
            SwitchState(_factory.Angry());
        }
        else if (_ctx.LT_Compensation && _ctx.PetrollerInfo.CurrentTrackingState != PetrollerObjectInfo.TrackingStatus.Tracked)
        {
            // SwitchState(_factory.Umcomfortable());
            SwitchState(_factory.Angry());
        }
        else if (_ctx.Pressed | _ctx.Speeding)
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
            case <= 0.3f: // 30%
                blend = 1;
                break;
            default: // 70%
                blend = 0;
                break;
        }
        Debug.Log("Sleep Blend as " + blend);
        _ctx.StartCoroutine(_ctx.AnimatorFloatTransition(_ctx.SleepBlendHash, blend, 0.2f));
    }
    public override void InitializeSubState()
    {

    }
}
