using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class FootStepAudioData
{
    public FootStepAudioData()
    {
        FootstepAudios.Add(new FootStepAudio("Walking"));
        FootstepAudios.Add(new FootStepAudio("Running"));
        FootstepAudios.Add(new FootStepAudio("Crouching"));
    }

    public List<FootStepAudio> FootstepAudios = new();

    public AudioClip GetRandomAudio(string Tag)
    {
        var result = FootstepAudios.FirstOrDefault(s => s.Tag == Tag);
        if (result == null || result.audioClips == null || result.audioClips.Count == 0) return null;
        
        int index = UnityEngine.Random.Range(0, result.audioClips.Count - 1);
        return result.audioClips[index];
    }
}

[System.Serializable]
public class FootStepAudio
{
    public FootStepAudio() { }
    public FootStepAudio(string tag)
    {
        Tag = tag;
    }
    public string Tag;
    public List<AudioClip> audioClips = new();
}
