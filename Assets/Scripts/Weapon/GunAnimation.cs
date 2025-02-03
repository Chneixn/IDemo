using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    protected Animator animator;
    protected BaseGun Gun;

    public virtual void Start()
    {
        if (Gun == null)
        {
            if (gameObject.TryGetComponent(out BaseGun Gun))
            {
                this.Gun = Gun;
            }
        }

        if (animator == null)
        {
            if (gameObject.TryGetComponent(out Animator animator))
            {
                this.animator = animator;
            }
        }
    }

    ///// <summary>
    ///// 依据BasicGun设置的ReloadTime调整换弹动画片段的速度
    ///// </summary>
    ///// <remarks>Problem:无法依据不同武器获取到不同的换弹动画片段速度</remarks>
    //private void SetReloadAnimationTime(float currentReloadTime)
    //{
    //    //在此处获取对应换弹动画的时长
    //    float _animTime = MyExtension.GetAnimationClipLength(animator, "Reload_Anim");
    //    if (_animTime != 0f)
    //    {
    //        float _mult = _animTime / currentReloadTime;
    //        animator.SetFloat("Reload_Mult", _mult);
    //    }
    //    else animator.SetFloat("Reload_Mult", 1f);
    //}
}
