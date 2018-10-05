using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoSingleton<SoundManager> {
    [SerializeField]
    bool _muteSe;
    [SerializeField]
    bool _muteBgm;

    AudioSource[] _audioSourcies;
	void Start () {
        _audioSourcies = GetComponents<AudioSource>();
        if (_muteBgm)
        {
            foreach (var audioSource in _audioSourcies)
            {
                audioSource.Stop();
            }
        }
    }
    void Update()
    {

    }

    public void PlaySE(AudioClip clip)
    {
        if (_muteSe) { return; }
        _audioSourcies[0].PlayOneShot(clip);
    }

    public void PlayBGM(AudioClip clip)
    {
        if (_muteBgm) { return; }
        int count = 1;
        while (true)
        {
            if (_audioSourcies.Length <= count)
            {
                Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
                return;
            }
            if (!_audioSourcies[count].isPlaying)
            {
                break;
            }else
            {
                count++;
            }
        }

        _audioSourcies[count].clip = clip;
        _audioSourcies[count].Play();
    }

    public bool IsPlayingBGM(AudioClip clip)
    {
        foreach(var audioSource in _audioSourcies)
        {
            if (audioSource.clip == clip)
            {
                return true;
            }
        }
        return false;
    }

    public List<AudioClip> GetPlayingList()
    {
        var clips = new List<AudioClip>();
        foreach (var audioSource in _audioSourcies)
        {
            if (audioSource.clip)
            {
                clips.Add(audioSource.clip);
            }
        }

        return clips;
    }

    public void StopBGM(AudioClip clip)
    {
        foreach(var audioSource in _audioSourcies)
        {
            if (audioSource.clip == clip)
            {
                audioSource.Stop();
                return;
            }
        }
        Debug.LogWarning("止めるBGMがありません");
    }

    public void StopAllBGM()
    {
        foreach (var audioSource in _audioSourcies)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    const float DefaultFadeTime = 1;
    
    public void FadeInBGM(AudioClip clip,Action action=null,float fadeTime= DefaultFadeTime)
    {
        if (_muteBgm) { return; }
        foreach (var audioSource in _audioSourcies)
        {
            if (!audioSource.isPlaying)
            {
                StartCoroutine(FadeInCoroutine(audioSource ,action , fadeTime));
                return;
            }
        }
        Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
    }
    public void FadeOutBGM(AudioClip clip, Action action=null, float fadeTime = DefaultFadeTime)
    {
        foreach (var audioSource in _audioSourcies)
        {
            if (audioSource.clip == clip)
            {
                audioSource.clip = clip;
                audioSource.Play();
                StartCoroutine(FadeOutCoroutine(audioSource, action, fadeTime));
                return;
            }
        }
        Debug.LogWarning("FadeOutするBGMがありません");
    }

    IEnumerator FadeInCoroutine(AudioSource audioSource, Action action, float fadeTime)
    {
        float timer = 0;
        while (audioSource.volume < 1)
        {
            yield return null;
            timer += Time.deltaTime;
            audioSource.volume = timer / fadeTime;
        }
        audioSource.volume = 1;
        if (action != null) { action.Invoke(); }
    }
    IEnumerator FadeOutCoroutine(AudioSource audioSource ,Action action , float fadeTime)
    {
        float timer=0;
        while (0 < audioSource.volume)
        {
            yield return null;
            timer += Time.deltaTime;
            audioSource.volume = 1 - timer / fadeTime;
        }
        audioSource.volume = 0;
        audioSource.Stop();
        audioSource.clip = null;
        if (action != null) { action.Invoke(); }
    }
}
