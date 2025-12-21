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
        _ctx.MyHaptic.StopRumble();
        _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[0], _ctx.AllAudioClips[1] }, 1, 5, true);
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
        _ctx.MyHaptic.StopRumble();
        _ctx.AFO_AudioPlayer.StopRandomPlay(true);
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
        else if (_ctx.CozyTimer > _ctx.HappyThreshold)
        {
            SwitchState(_factory.Happy());
        }
    }
    void RandomAnimationBlend()
    {
        float rd = Random.Range(0, 1f);
        float blend;
        switch (rd)
        {
            // auto states
            case <= 0.2f: // 20%
                blend = 1;
                break;
            case <= 0.4f: // 20%
                blend = 0.5f;
                break;
            default: // 60%
                blend = 0;
                break;
        }
        _ctx.StartCoroutine(_ctx.AnimatorFloatTransition(_ctx.IdleBlendHash, blend, 0.2f));
    }
    public override void InitializeSubState()
    {

    }
}
