using System;
using UnityEngine;

public class PlayerFootstepLisener : MonoBehaviour
{
    public bool enableFootstep;
    [SerializeField] private FootStepAudioData AudioData = new();
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private string audioTag;

    private float nextPlayTime;
    private float audioDelay = 0f;
    [SerializeField, Range(0f, 1f)] private float running_delay = 0.3f;
    [SerializeField, Range(0f, 1f)] private float walking_delay = 0.5f;
    [SerializeField, Range(0f, 1f)] private float crouching_delay = 0.7f;


    // 角色发出脚步声条件
    // 实现角色踩踏位置的对应材质声音, 从player control回传

    private void Start()
    {
        PlayerManager.Instance.CharacterControl.OnMovementStateChanged += OnStateChange;
    }

    private void OnStateChange(IMovementState state)
    {
        enableFootstep = true;
        if (state is Walking)
        {
            audioDelay = walking_delay;
            audioTag = "Walking";
        }
        else if (state is Running)
        {
            audioDelay = running_delay;
            audioTag = "Running";
        }
        else if (state is Crouching)
        {
            audioDelay = crouching_delay;
            audioTag = "Crouching";
        }
        else enableFootstep = false;
    }

    private void Update()
    {
        // var state = characterControl.CurrentMovementState;
        if (!enableFootstep) return;

        nextPlayTime += Time.deltaTime;

        if (nextPlayTime >= audioDelay)
        {
            // 播放移动声音
            AudioClip audioClip = AudioData.GetRandomAudio(audioTag);
            audioSource.clip = audioClip;
            audioSource.Play();
            nextPlayTime = 0;   // reset nextPlayTime
        }

    }
}
