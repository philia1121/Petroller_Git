using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PetrollerUncomfortableState : PetrollerBaseState
{
    public PetrollerUncomfortableState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    : base(currentContext, stateFactory) { }

    bool haveSpited = false;
    public override void EnterState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, true);
        haveSpited = _ctx.OverallLTTimer > _ctx.SpitThreshold; // in case this states is interrupted by Angry state
        _ctx.MyHaptic.SetConstantRumble(5, _ctx.HapticAmplitude[3]);
        _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[5] }, 1, 3);
        _ctx.ClipEnd = false;
    }
    public override void UpdateState()
    {
        if (_ctx.OverallLTTimer < _ctx.PassOutThreshold)
        {
            if (_ctx.OverallLTTimer > _ctx.SpitThreshold & !haveSpited)
            {
                // spit
                haveSpited = true;
                _ctx.MyAnimator.SetTrigger(_ctx.GetSpitHash);
                _ctx.ClipEnd = false;
                _ctx.AFO_AudioPlayer.StopRandomPlay();
                _ctx.AFO_AudioPlayer.PlayAudio_Assigned(_ctx.AllAudioClips[6]);
                _ctx.MyHaptic.StopRumble();
                _ctx.MyHaptic.SetConstantRumble(0.3f, _ctx.HapticAmplitude[3]);
            }
            if (haveSpited & _ctx.ClipEnd)
            {
                // after spit but still uncomfortable
                _ctx.ClipEnd = false;
                _ctx.MyHaptic.SetConstantRumble(8, _ctx.HapticAmplitude[3]);
                _ctx.AFO_AudioPlayer.StartRandomPLay(new AudioClip[] { _ctx.AllAudioClips[5] }, 1, 3);
            }
        }

        CheckSwitchStates();
    }
    public override void ExitState()
    {
        _ctx.MyAnimator.SetBool(_ctx.IsUncomfortableHash, false);
        _ctx.MyAnimator.ResetTrigger(_ctx.GetSpitHash);
        //_ctx.AFO_AudioPlayer.PlayAudio_Assigned(_ctx.AllAudioClips[7]);
        _ctx.MyHaptic.StopRumble();
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
