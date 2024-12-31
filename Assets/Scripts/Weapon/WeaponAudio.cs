using UnityEngine;

[System.Serializable]
public class WeaponAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [Header("AudioSetting音频")]
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip fireEmptySound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip reloadSound_drop;
    [SerializeField] private AudioClip reloadSound_input;
    [SerializeField] private AudioClip reloadSound_lock;
    [SerializeField] private AudioClip reloadEmptySound;

    public void Init(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    public void PlayShootSound(bool isEmpty = false)
    {
        if (!isEmpty && fireEmptySound != null)
            audioSource.clip = fireEmptySound;
        else if (fireSound != null)
            audioSource.clip = fireSound;

        audioSource.Play();
    }

    public void PlayReloadSounds(float reloadTime, float time, bool isEmpty = false)
    {
        if (reloadSound_drop != null)
        {
            //换弹声音为分段式
            if (time <= reloadTime && time >= reloadTime / 3 * 2f)
            {
                audioSource.clip = reloadSound_drop;
                audioSource.Play();
            }
            if (time >= reloadTime / 3f && time <= reloadTime / 3 * 2f)
            {
                audioSource.clip = reloadSound_input;
                audioSource.Play();
            }
            if (time <= reloadTime / 3f)
            {
                audioSource.clip = reloadSound_lock;
                audioSource.Play();
            }
        }
        else
        {
            //换弹声音为一整段
            if (isEmpty) audioSource.clip = reloadEmptySound;
            else audioSource.clip = reloadSound;
            if (time >= reloadTime)
                audioSource.Play();
        }
    }
}
