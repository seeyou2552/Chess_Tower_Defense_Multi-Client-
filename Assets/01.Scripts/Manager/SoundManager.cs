using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundEntry
{
    public string Key;
    public AudioClip Clip;
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("BGM")]
    public AudioSource BgmSource;
    public List<SoundEntry> BgmList = new List<SoundEntry>();

    [Header("SFX")]
    public AudioSource SfxSource;
    public List<SoundEntry> SfxList = new List<SoundEntry>();
    public int SfxPoolSize = 5;

    private AudioSource[] _sfxPool;
    private Coroutine _bgmFadeCoroutine;

    private bool _isBgmMute;
    private bool _isSfxMute;
    private string _currentBgmKey;

    private float START_VOLUME = 0.5f;

    protected override void Awake()
    {
        base.Awake();
        InitializeAudioSources();
    }

    private void InitializeAudioSources()
    {
        if (BgmSource == null)
        {
            BgmSource = gameObject.AddComponent<AudioSource>();
            BgmSource.loop = true;
        }

        if (SfxSource == null)
        {
            SfxSource = gameObject.AddComponent<AudioSource>();
            SfxSource.loop = false;
        }

        _sfxPool = new AudioSource[SfxPoolSize];
        for (int i = 0; i < SfxPoolSize; i++)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            _sfxPool[i] = source;
        }

        // 시작 볼륨 설정
        SetBGMVolume(START_VOLUME);
        SetSFXVolume(START_VOLUME);
    }

    public void PlayBGM(string key, float fadeTime = 0.5f)
    {
        // if (isBgmMute)
        //     return;

        AudioClip clip = GetClipFromList(BgmList, key);
        if (clip == null)
        {
            Debug.LogWarning($"SoundManager: BGM key not found: {key}");
            return;
        }

        _currentBgmKey = key;

        if (_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        _bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeTime));
    }

    public void PlayBGM(AudioClip clip, float fadeTime = 0.5f)
    {
        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlayBGM clip is null.");
            return;
        }

        if (_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        _bgmFadeCoroutine = StartCoroutine(FadeBGM(clip, fadeTime));
    }

    public void StopBGM(float fadeTime = 0.5f)
    {
        if (_bgmFadeCoroutine != null)
            StopCoroutine(_bgmFadeCoroutine);

        _bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeTime));
    }

    public void PauseBGM()
    {
        if (BgmSource.isPlaying)
            BgmSource.Pause();
    }

    public void ResumeBGM()
    {
        if (BgmSource.isPlaying && BgmSource.clip != null)
            BgmSource.UnPause();
    }

    public void SetBGMVolume(float volume)
    {
        BgmSource.volume = Mathf.Clamp01(volume);
    }

    public void SetSFXVolume(float volume)
    {
        SfxSource.volume = Mathf.Clamp01(volume);
        for (int i = 0; i < _sfxPool.Length; i++)
            _sfxPool[i].volume = Mathf.Clamp01(volume);
    }

    public void ToggleBgmMute()
    {
        _isBgmMute = !_isBgmMute;
        if (BgmSource != null)
            BgmSource.mute = _isBgmMute;
    }

    public void ToggleSfxMute()
    {
        SetSfxEnabled(!IsSfxEnabled());
    }

    public bool IsBgmEnabled()
    {
        return !_isBgmMute;
    }

    public bool IsSfxEnabled()
    {
        return !_isSfxMute;
    }

    public void SetBgmEnabled(bool enabled)
    {
        _isBgmMute = !enabled;
        if (BgmSource != null)
            BgmSource.mute = _isBgmMute;

        if (enabled && !string.IsNullOrEmpty(_currentBgmKey) && !BgmSource.isPlaying)
        {
            PlayBGM(_currentBgmKey);
        }
    }

    public void SetSfxEnabled(bool enabled)
    {
        _isSfxMute = !enabled;
        if (SfxSource != null)
            SfxSource.mute = _isSfxMute;

        if (_sfxPool != null)
        {
            for (int i = 0; i < _sfxPool.Length; i++)
                if (_sfxPool[i] != null)
                    _sfxPool[i].mute = _isSfxMute;
        }
    }

    public void ApplySettings(SettingData settingData)
    {
        if (settingData == null)
            return;

        SetBgmEnabled(settingData.IsBGMOn);
        SetSfxEnabled(settingData.IsSFXOn);
    }

    /// <summary>
    /// SFX 재생 - 키로 재생
    /// </summary>
    /// <param name="key">sfxList에서 등록된 키</param>
    public void PlaySFX(string key, float volume = 0.5f, float pitchVariance = 0f)
    {
        AudioClip clip = GetClipFromList(SfxList, key);
        if (clip == null)
        {
            Debug.LogWarning($"SoundManager: SFX key not found: {key}");
            return;
        }

        PlaySFX(clip, volume, pitchVariance);
    }

    public void PlaySFX(AudioClip clip, float volume = 0.5f, float pitchVariance = 0f)
    {
        if (_isSfxMute)
            return;

        if (clip == null)
        {
            Debug.LogWarning("SoundManager: PlaySFX clip is null.");
            return;
        }

        AudioSource source = GetAvailableSFXSource();
        if (source == null)
        {
            source = SfxSource;
        }

        source.clip = clip;
        source.volume = Mathf.Clamp01(volume);
        source.pitch = 1f + Random.Range(-pitchVariance, pitchVariance);
        source.Play();
    }

    public void PlayOneShot(AudioClip clip, float volumeScale = 1f)
    {
        if (_isSfxMute)
            return;

        if (clip == null)
            return;

        SfxSource.PlayOneShot(clip, Mathf.Clamp01(volumeScale));
    }

    public void StopAllSFX()
    {
        SfxSource.Stop();
        foreach (var source in _sfxPool)
            source.Stop();
    }

    private IEnumerator FadeBGM(AudioClip newClip, float fadeTime)
    {
        float startVolume = BgmSource.volume;
        float halfTime = fadeTime * 0.5f;

        for (float time = 0f; time < halfTime; time += Time.unscaledDeltaTime)
        {
            BgmSource.volume = Mathf.Lerp(startVolume, 0f, time / halfTime);
            yield return null;
        }

        BgmSource.clip = newClip;
        BgmSource.Play();

        for (float time = 0f; time < halfTime; time += Time.unscaledDeltaTime)
        {
            BgmSource.volume = Mathf.Lerp(0f, startVolume, time / halfTime);
            yield return null;
        }

        BgmSource.volume = startVolume;
        _bgmFadeCoroutine = null;
    }

    private IEnumerator FadeOutBGM(float fadeTime)
    {
        float startVolume = BgmSource.volume;
        for (float time = 0f; time < fadeTime; time += Time.unscaledDeltaTime)
        {
            BgmSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeTime);
            yield return null;
        }

        BgmSource.Stop();
        BgmSource.volume = startVolume;
        _bgmFadeCoroutine = null;
    }

    private AudioSource GetAvailableSFXSource()
    {
        for (int i = 0; i < _sfxPool.Length; i++)
        {
            if (!_sfxPool[i].isPlaying)
                return _sfxPool[i];
        }

        return _sfxPool[0];
    }

    private AudioClip GetClipFromList(List<SoundEntry> list, string key)
    {
        if (string.IsNullOrEmpty(key))
            return null;

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] != null && list[i].Key == key)
                return list[i].Clip;
        }

        return null;
    }
}
