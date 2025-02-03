using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class GlockAnimationPlayable : WeaponAnimation
{
    public AnimationClip IdleClip;

    PlayableGraph playableGraph;
    AnimationPlayableOutput playableOutput;

    public override void Start()
    {
        base.Start();
        playableGraph = PlayableGraph.Create();
        playableGraph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        playableOutput = AnimationPlayableOutput.Create(playableGraph, $"{name}:Animation", GetComponent<Animator>());

        Gun.OnEnableWeapon += OnEnableWeapon;
        Gun.OnDisableWeapon += OnDisableWeapon;
    }

    public void OnDisable()
    {
        //销毁该图创建的所有可播放项和 PlayableOutput。
        playableGraph.Destroy();
    }

    public virtual void OnEnableWeapon()
    {
        if (IdleClip != null)
        {
            // 将剪辑包裹在可播放项中
            AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(playableGraph, IdleClip);
            // 将可播放项连接到输出
            playableOutput.SetSourcePlayable(clipPlayable);

            // 播放该图。
            playableGraph.Play();
            Debug.Log("Glock Idle Animation.");
        }
    }
    public virtual void OnDisableWeapon()
    {
        playableGraph.Stop();
    }

    public virtual void OnShoot() { }
    public virtual void OnSootFinshed() { }
    public virtual void OnAim() { }
    public virtual void OnAimFinshed() { }
    public virtual void OnReloadBegin() { }
    public virtual void OnReloadFinshed() { }

}
