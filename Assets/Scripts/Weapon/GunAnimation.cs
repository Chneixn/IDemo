using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GunAnimation : MonoBehaviour, IGunAnimation
{
    private Animator m_Animator;

    /// <summary>
    /// 开火动画
    /// </summary>
    public void HandleShootingAnimation()
    {

    }

    public void AimAnimationStart()
    {
        m_Animator.SetBool("Aiming", true);
        m_Animator.CrossFadeInFixedTime("ADS_Anim", 0.3f);
    }

    public void AimAnimationEnd()
    {
        m_Animator.SetBool("Aiming", false);
        m_Animator.CrossFadeInFixedTime("Idle_Anim", 0.3f);
    }

    public void ReloadAnimationEnd()
    {

    }

    public void ReloadAnimationStart(bool isEmpty)
    {
        if (!isEmpty)
        {
            m_Animator.CrossFadeInFixedTime("Reload_Anim", 0.01f);
        }
        else
        {
            m_Animator.CrossFadeInFixedTime("Reload_Empty_Anim", 0.01f);
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

    public void ShootingAnimationEnd()
    {

    }

    public void ShootingAnimationStart(bool aimState)
    {
        if (aimState)
        {
            m_Animator.CrossFadeInFixedTime("ADS_Fire_Anim", 0.01f);
        }
        else
        {
            m_Animator.CrossFadeInFixedTime("Fire_Anim", 0.01f);
        }
    }
}
