using UnityEngine;

[RequireComponent(typeof(CharacterControl))]
public class PlayerFootstepLisener : MonoBehaviour
{
    public bool enableFootstep;
    public FootStepAudioData AudioData;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private CharacterControl characterControl;

    private float nextPlayTime;
    [SerializeField, Range(0f, 1f)] private float running_delay = 0.3f;
    [SerializeField, Range(0f, 1f)] private float walking_delay = 0.5f;
    [SerializeField, Range(0f, 1f)] private float crouching_delay = 0.7f;


    // 角色发出脚步声条件
    // 实现角色踩踏位置的对应材质声音, 从player control回传

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterControl = PlayerManager.Instance.CharacterControl;
    }

    private void Update()
    {
        float audioDelay = 0f;

        //var state = characterControl.CurrentMovementState;


        if (!enableFootstep) return;

        nextPlayTime += Time.deltaTime;

        //TODO:获取地面信息从而决定使用那种脚步声音
        if (nextPlayTime >= audioDelay)
        {
            //播放移动声音
            //AudioClip audioClip = AudioData.GetRandomAudio();
            //audioSource.clip = audioClip;
            audioSource.Play();
            nextPlayTime = 0;   //reset nextPlayTime
        }

    }
}
