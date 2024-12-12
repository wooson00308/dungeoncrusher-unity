using System;
using UnityEngine;

public class SoundSystem : SingletonMini<SoundSystem>
{
    [SerializeField] private AudioSource _bgmAudioSource;
    [SerializeField] private AudioSource _fxAudioSource;

    [SerializeField] private SoundData _soundData;

    public void PlayBGM(string soundName)
    {
        var bgmClip = _soundData.GetBGMSound(soundName);
        if (_bgmAudioSource.clip == bgmClip) return;
        _bgmAudioSource.clip = bgmClip;
        _bgmAudioSource.Play();
    }

    public void PlayFx(string soundName)
    {
        var fxClip = _soundData.GetFXSound(soundName);
        _fxAudioSource.PlayOneShot(fxClip);
    }
}