using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    public List<BGMSoundData> bgmSoundDatas = new();
    private readonly Dictionary<string, AudioClip> _bgmSounds = new();

    public List<FxSoundData> fxSoundDatas = new();
    private readonly Dictionary<string, AudioClip> _fxSounds = new();

    private void SetSoundData()
    {
        if (_bgmSounds.Count <= 0)
        {
            foreach (var bgmSoundData in bgmSoundDatas)
            {
                _bgmSounds.Add(bgmSoundData.SoundName, bgmSoundData.SoundClip);
            }
        }

        if (_fxSounds.Count <= 0)
        {
            foreach (var fxSoundData in fxSoundDatas)
            {
                _fxSounds.Add(fxSoundData.SoundName, fxSoundData.SoundClip);
            }
        }
    }

    public AudioClip GetBGMSound(string soundName)
    {
        SetSoundData();
        _bgmSounds.TryGetValue(soundName, out var bgmSound);
        return bgmSound;
    }

    public AudioClip GetFXSound(string soundName)
    {
        SetSoundData();
        _fxSounds.TryGetValue(soundName, out var fxSound);
        return fxSound;
    }
}

[Serializable]
public class BGMSoundData
{
    public string SoundName;
    public AudioClip SoundClip;
}

[Serializable]
public class FxSoundData
{
    public string SoundName;
    public AudioClip SoundClip;
}