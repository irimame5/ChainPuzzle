using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundManager : MonoSingleton<SoundManager> {
    [SerializeField]
    bool muteSe;
    [SerializeField]
    bool muteBgm;
    [Disable]
    public bool QuaternoteEventPlaying=false;
    [SerializeField, Tooltip("4分音符のタイミングで呼ばれる,シーンごとに初期化する")]
    public UnityEvent QuaterNoteEvent;

    const int BgmAudioSourcesNum = 3;
    AudioSource[] BgmAudioSourcies = new AudioSource[BgmAudioSourcesNum];
    AudioSource SeAudioSourcies;
    int tempo = -1;
    float timer;
    float quarterNoteTime = -1;

    protected override void Awake()
    {
        if (Instance != null)
        {
            print(typeof(SoundManager) + "was instantiated");
            Instance.QuaterNoteEvent = null;
            Instance.QuaterNoteEvent = QuaterNoteEvent;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        SubAwake();
    }

    protected override void SubAwake () {
        DontDestroyOnLoad(gameObject);
        for(int i=0;i<BgmAudioSourcesNum;i++)
        {
            BgmAudioSourcies[i] = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
        }
        SeAudioSourcies = (AudioSource)gameObject.AddComponent(typeof(AudioSource));
    }

    public void StartQuarterNoteEvent(int _tempo)
    {
        QuaternoteEventPlaying = true;
        tempo = _tempo;
        timer = 0;
        quarterNoteTime = 60f / tempo;
    }
    public void StopQuarterNoteEvent()
    {
        QuaternoteEventPlaying = false;
        tempo = -1;
        timer = 0;
    }

    private void Update()
    {
        if (QuaternoteEventPlaying)
        {
            Debug.Assert(tempo != -1 && quarterNoteTime != -1);

            timer += Time.deltaTime;
            if (quarterNoteTime <= timer)
            {
                timer = (timer - quarterNoteTime) % quarterNoteTime;
                if (QuaterNoteEvent != null) { QuaterNoteEvent.Invoke(); }
                print("quaternoteevent");
            }
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (muteSe) { return; }
        SeAudioSourcies.PlayOneShot(clip);
    }

    /// <summary>
    /// 今流れているのに重ねてbgm再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgmAdd(AudioClip clip)
    {
        if (muteBgm) { return; }
        if (IsPlayingBGM(clip)) { return; }

        int count = 1;
        while (true)
        {
            if (BgmAudioSourcies.Length <= count)
            {
                Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
                return;
            }
            if (!BgmAudioSourcies[count].isPlaying)
            {
                break;
            }else
            {
                count++;
            }
        }
        BgmAudioSourcies[count].clip = clip;
        BgmAudioSourcies[count].loop = true;
        BgmAudioSourcies[count].Play();
    }
    public void PlayBgmAdd(AudioClip clip,bool loop)
    {
        if (muteBgm) { return; }
        if (IsPlayingBGM(clip)) { return; }

        int count = 1;
        while (true)
        {
            if (BgmAudioSourcies.Length <= count)
            {
                Debug.LogError("AudioSourceの数以上のBGMを再生しようとしています");
                return;
            }
            if (!BgmAudioSourcies[count].isPlaying)
            {
                break;
            }
            else
            {
                count++;
            }
        }
        BgmAudioSourcies[count].clip = clip;
        BgmAudioSourcies[count].loop = loop;
        BgmAudioSourcies[count].Play();
    }

    /// <summary>
    /// 今流れているのは止めてbgm再生
    /// </summary>
    /// <param name="clip"></param>
    public void PlayBgmSingle(AudioClip clip)
    {
        if (muteBgm) { return; }
        if (IsPlayingBGM(clip)) { return; }

        StopAllBGM();
        BgmAudioSourcies[0].clip = clip;
        BgmAudioSourcies[0].loop = true;
        BgmAudioSourcies[0].Play();
    }
    public void PlayBgmSingle(AudioClip clip,bool loop)
    {
        if (muteBgm) { return; }
        if (IsPlayingBGM(clip)) { return; }

        StopAllBGM();
        BgmAudioSourcies[0].clip = clip;
        BgmAudioSourcies[0].loop = loop;
        BgmAudioSourcies[0].Play();
    }

    public bool IsPlayingBGM(AudioClip clip)
    {
        foreach(var audioSource in BgmAudioSourcies)
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
        foreach (var audioSource in BgmAudioSourcies)
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
        foreach(var audioSource in BgmAudioSourcies)
        {
            if (audioSource.clip == clip)
            {
                audioSource.clip = null;
                audioSource.Stop();
                return;
            }
        }
        Debug.LogWarning("止めるBGMがありません");
    }

    public void StopAllBGM()
    {
        foreach (var audioSource in BgmAudioSourcies)
        {
            if (audioSource.isPlaying)
            {
                audioSource.clip = null;
                audioSource.Stop();
            }
        }
    }

    const float DefaultFadeTime = 1;
    
    public void FadeInBGM(AudioClip clip,Action action=null,float fadeTime= DefaultFadeTime)
    {
        if (muteBgm) { return; }
        foreach (var audioSource in BgmAudioSourcies)
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
        foreach (var audioSource in BgmAudioSourcies)
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
