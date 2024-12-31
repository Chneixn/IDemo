using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public AudioMixer mainMixer;
    public AudioClip clip;

    public GameObject musicOff;
    [SerializeField]private AudioSource source;
    private bool _isPlaying;

    private void Start()
    {
        _isPlaying = true;
        source.clip = clip;
        source.Play();
    }

    public void SetMasterVolume(float volume)    // 控制主音量的函数
    {
        mainMixer.SetFloat("Master", volume);
        // MasterVolume为我们暴露出来的Master的参数
    }

    public void SetMusicVolume(float volume)    // 控制背景音乐音量的函数
    {
        mainMixer.SetFloat("Music", volume);
        // MusicVolume为我们暴露出来的Music的参数
    }

    public void SetSoundEffectVolume(float volume)    // 控制音效音量的函数
    {
        mainMixer.SetFloat("SoundEffect", volume);
        // SoundEffectVolume为我们暴露出来的SoundEffect的参数
    }

    public void PlayBackGroundMusic()
    {
        if (_isPlaying)
        {
            source.Stop();
            _isPlaying = false;
            musicOff.SetActive(true);
        }
        else
        {
            source.Play();
            _isPlaying = true;
            musicOff.SetActive(false);
        }

    }
}
