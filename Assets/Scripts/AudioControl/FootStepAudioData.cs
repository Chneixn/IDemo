using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Footstep Audio Data")]
public class FootStepAudioData : ScriptableObject
{
    public List<FootstepAudio> FootstepAudios = new();

    public AudioClip GetRandomAudio(string Tag)
    {
        var result = FootstepAudios.FirstOrDefault(s => s.Tag == Tag);
        int index = Random.Range(0, result.audioClips.Count - 1);

        return result.audioClips[index];
    }
}

[System.Serializable]
public class FootstepAudio
{
    public string Tag;
    public List<AudioClip> audioClips = new();
}
