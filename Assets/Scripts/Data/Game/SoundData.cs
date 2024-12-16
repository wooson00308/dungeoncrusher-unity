using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "SoundData", menuName = "Data/SoundData")]
public class SoundData : ScriptableObject
{
    public List<BGMSoundData> bgmSoundDatas = new();
    private Dictionary<string, AudioClip> bgmSounds = new();

    public List<FxSoundData> fxSoundDatas = new();
    private Dictionary<string, AudioClip> fxSounds = new();

    private void SetSoundData()
    {
        if (bgmSounds.Count <= 0)
        {
            foreach (var bgmSoundData in bgmSoundDatas)
            {
                bgmSounds.Add(bgmSoundData.SoundName, bgmSoundData.SoundClip);
            }
        }

        if (fxSounds.Count <= 0)
        {
            foreach (var fxSoundData in fxSoundDatas)
            {
                fxSounds.Add(fxSoundData.SoundName, fxSoundData.SoundClip);
            }
        }
    }

    public AudioClip GetBGMSound(string soundName)
    {
        SetSoundData();
        bgmSounds.TryGetValue(soundName, out var bgmSound);
        return bgmSound;
    }

    public AudioClip GetFXSound(string soundName)
    {
        SetSoundData();
        fxSounds.TryGetValue(soundName, out var fxSound);
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