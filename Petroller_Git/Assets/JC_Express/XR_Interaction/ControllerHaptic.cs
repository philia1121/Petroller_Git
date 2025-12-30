using System.Collections;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.UserInterface;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ControllerHaptic : MonoBehaviour
{
    public enum RumbleMode
    {
        Constant,   // as given value
        FadeIn,     // 0 -> given value
        FadeOut,    // given value -> 0
        Pulse       // 0 -> given value -> 0
    }

    [Header("Settings")]
    [SerializeField] private bool active = true;
    public float defaultDuration = 0.5f;
    [Range(0, 1)] public float defaultAmplitude = 1f;
    public OVRInput.Controller targetController = OVRInput.Controller.RTouch;

    Coroutine cor;
    float timer;
    public void SetRumbleCallActive(bool value) { active = value; }
    public void SetConstantRumble(float dur, float amp, float delay = 0)
    {
        Rumble(RumbleMode.Constant, dur, amp, delay);
    }
    public void SetFadeInRumble(float dur, float amp, float delay = 0)
    {
        Rumble(RumbleMode.FadeIn, dur, amp, delay);
    }
    public void SetFadeOutRumble(float dur, float amp, float delay = 0)
    {
        Rumble(RumbleMode.FadeOut, dur, amp, delay);
    }
    public void SetPulseRumble(float dur, float amp, float delay = 0)
    {
        Rumble(RumbleMode.Pulse, dur, amp, delay);
    }
    public void StopRumble()
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;

        OVRInput.SetControllerVibration(0, 0, targetController);
    }
    public void StartIntervalRumble(float vibrateDuration, float pauseDuration, float amplitude, int loops = -1)
    {
        if (!active) return;
        if (cor != null) StopCoroutine(cor);

        cor = StartCoroutine(SetIntervalHaptic(vibrateDuration, pauseDuration, amplitude, loops));
    }
    IEnumerator SetIntervalHaptic(float vibrateDuration, float pauseDuration, float amplitude, int loops)
    {
        int count = 0;

        while (loops == -1 || count < loops)
        {
            float timer = 0f;
            while (timer < vibrateDuration)
            {
                timer += Time.deltaTime;
                OVRInput.SetControllerVibration(1, amplitude, targetController);
                yield return null;
            }

            OVRInput.SetControllerVibration(0, 0, targetController);
            yield return new WaitForSeconds(pauseDuration);

            if (loops != -1)
            {
                count++;
            }
        }

        OVRInput.SetControllerVibration(0, 0, targetController);
        cor = null;
    }
    public void Rumble(RumbleMode mode = RumbleMode.Constant, float duration = -1, float amplitude = -1, float delay = 0)
    {
        if (!active) return;

        float finalDur = (duration < 0) ? defaultDuration : duration;
        float finalAmp = (amplitude < 0) ? defaultAmplitude : amplitude;

        if (cor != null) StopCoroutine(cor);
        cor = StartCoroutine(SetHaptic(mode, finalDur, finalAmp, delay));
    }
    IEnumerator SetHaptic(RumbleMode mode, float duration, float maxAmplitude, float delay = 0)
    {
        float timer = 0f;

        if (delay > 0) yield return new WaitForSeconds(delay);

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float currentAmp = maxAmplitude;
            float progress = timer / duration;

            switch (mode)
            {
                case RumbleMode.FadeIn:
                    currentAmp = Mathf.Lerp(0f, maxAmplitude, progress);
                    break;

                case RumbleMode.FadeOut:
                    currentAmp = Mathf.Lerp(maxAmplitude, 0f, progress);
                    break;

                case RumbleMode.Pulse:
                    if (progress < 0.5f)
                        currentAmp = Mathf.Lerp(0f, maxAmplitude, progress * 2f);
                    else
                        currentAmp = Mathf.Lerp(maxAmplitude, 0f, (progress - 0.5f) * 2f);
                    break;

                case RumbleMode.Constant:
                default:
                    currentAmp = maxAmplitude;
                    break;
            }
            OVRInput.SetControllerVibration(1, currentAmp, targetController);
            yield return null;
        }

        OVRInput.SetControllerVibration(0, 0, targetController);
        cor = null;
    }
}
