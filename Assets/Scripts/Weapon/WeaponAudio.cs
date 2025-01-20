using System;
using UnityEngine;

[Serializable, RequireComponent(typeof(AudioSource))]
public class WeaponAudio : MonoBehaviour
{
    private AudioSource audioSource;
    [Header("AudioSetting音频")]

    [SerializeField, Tooltip("收起武器声音")]
    private AudioClip holsterSound;

    [SerializeField] private AudioClip shot;
    [SerializeField] private AudioClip silencedShot;
    [SerializeField] private AudioClip dryShot;
    [SerializeField] private AudioClip reload;
    [SerializeField] private AudioClip reloadEmpty;

    [SerializeField] private bool isBreakOut;
    [SerializeField] private AudioClip reload_drop;
    [SerializeField] private AudioClip reload_input;
    [SerializeField] private AudioClip reload_lock;

    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlayShootSound()
    {
        if (shot != null)
        {
            audioSource.clip = shot;
            audioSource.Play();
        }
    }

    public void PlaySilencedShot()
    {
        if (silencedShot != null)
        {
            audioSource.clip = silencedShot;
            audioSource.Play();
        }
    }

    public void PlayEmptyShootSound()
    {
        if (dryShot != null)
        {
            audioSource.clip = dryShot;
            audioSource.Play();
        }
    }

    public void PlayReloadSounds(float reloadTime, float runtime, bool isEmpty = false)
    {
        if (isBreakOut)
        {
            // 换弹声音为分段式
            if (runtime <= reloadTime / 3)
            {
                if (audioSource.clip != reload_drop)
                {
                    audioSource.clip = reload_drop;
                    audioSource.Play();
                }
            }
            else if (runtime <= reloadTime / 3 * 2f)
            {
                if (audioSource.clip != reload_input)
                {
                    audioSource.clip = reload_input;
                    audioSource.Play();
                }
            }
            else
            {
                if (audioSource.clip != reload_lock)
                {
                    audioSource.clip = reload_lock;
                    audioSource.Play();
                }
            }
        }
        else
        {
            // 换弹声音为一整段
            if (isEmpty) audioSource.clip = reloadEmpty;
            else audioSource.clip = reload;

            if (runtime == 0f)
                audioSource.Play();
        }
    }

    public void PlayHolsterSound()
    {
        if (holsterSound != null)
        {
            audioSource.clip = holsterSound;
            audioSource.Play();
        }
    }
}
