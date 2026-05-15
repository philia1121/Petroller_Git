using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class SimpleAudioPlayer : MonoBehaviour
{
    public string _Name;
    [Header("Basic Setings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    [Header("Random Setings")]
    [SerializeField] private float minInterval;
    [SerializeField] private float maxInterval;
    [SerializeField] private bool keepRandomPlay = false;
    bool randomPlay = false;
    IEnumerator cor;
    void Awake()
    {
        if (!audioSource) audioSource = this.AddComponent<AudioSource>();
    }
    public void PlayAudio_RandomPick()
    {
        audioSource.clip = audioClips[audioClips.Length == 1 ? 0 : UnityEngine.Random.Range(0, audioClips.Length)];
        if (audioSource.clip != null) audioSource.Play();
    }
    public void PlayAudio_Assigned(AudioClip clip)
    {
        audioSource.clip = clip;
        if (audioSource.clip != null) audioSource.Play();
    }
    public void AssignNewClipsArray(AudioClip[] newClips)
    {
        audioClips = newClips;
    }

    public void SetRandomPlay(bool play)
    {
        randomPlay = play;
        if (play)
        {
            if (cor != null) StopCoroutine(cor);
            cor = PlayRandomSound(audioClips, minInterval, maxInterval, keepRandomPlay);
            StartCoroutine(cor);
        }
        else
        {
            if (cor != null) StopCoroutine(cor);
            cor = null;
        }
    }
    public void StartRandomPLay(AudioClip[] clips, float min_Interval, float max_Interval, bool delayFirstPlay = false, bool keepRandom = true, Action onPlayCallback = null)
    {
        if (cor != null) StopCoroutine(cor);
        cor = null;

        randomPlay = true;
        cor = PlayRandomSound(clips, min_Interval, max_Interval, delayFirstPlay, keepRandom, onPlayCallback);
        StartCoroutine(cor);
    }
    public void StopRandomPlay(bool forceStop = false)
    {
        randomPlay = false;
        if (cor != null) StopCoroutine(cor);
        cor = null;
        if (forceStop) audioSource.Stop();
    }
    IEnumerator PlayRandomSound(AudioClip[] clips, float minIn, float maxIn, bool delayFirst = false, bool keepRandom = true, Action callBack = null)
    {
        if (delayFirst) yield return new WaitForSeconds(UnityEngine.Random.Range(minIn, maxIn));

        while (randomPlay)
        {
            float currentClipLength = PlaySoundAndGetLength(clips, callBack);
            if (currentClipLength > 0) yield return new WaitForSeconds(currentClipLength);

            if (!keepRandom)
            {
                randomPlay = false;
                yield break;
            }

            float interval = UnityEngine.Random.Range(minIn, maxIn);
            yield return new WaitForSeconds(interval);
        }
    }

    float PlaySoundAndGetLength(AudioClip[] clips, Action onPlay = null)
    {
        AudioClip[] targetClips = (clips != null && clips.Length > 0) ? clips : audioClips;

        if (targetClips == null || targetClips.Length == 0) return 0f;

        AudioClip selectedClip = targetClips[targetClips.Length == 1 ? 0 : UnityEngine.Random.Range(0, targetClips.Length)];
        audioSource.clip = selectedClip;

        if (audioSource.clip != null)
        {
            audioSource.Play();
            onPlay?.Invoke();
            return audioSource.clip.length;
        }

        return 0f;
    }
}