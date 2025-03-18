using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimation : MonoBehaviour
{
    [SerializeField] protected Animator animator;
    protected Gun Gun;

    public void Awake()
    {
        if (Gun == null) Gun = GetComponent<Gun>();

        if (animator == null) GetComponentInChildren<Animation>(true);
    }

    private void Start()
    {
        Gun.OnShot += ShotStart;
        Gun.OnShotFinshed += ShotEnd;
        Gun.OnAim += AimStart;
        Gun.OnAimFinshed += AimEnd;
        Gun.OnDisableWeapon += DisableGun;
        Gun.OnReloadBegin += ReloadStart;
        Gun.OnReloadFinshed += ReloadEnd;
    }

    public void DisableGun()
    {
        animator.SetBool("Hiding", true);
    }

    public void AimStart()
    {
        animator.SetBool("Aiming", true);
    }

    public void AimEnd()
    {
        animator.SetBool("Aiming", false);
    }

    public void ShotEnd(bool isEmpty)
    {
        animator.SetBool("Shot", false);
        animator.SetBool("ShotWithAim", false);
        if (isEmpty) animator.SetBool("Empty", true);
    }

    public void ShotStart(bool aimState)
    {
        if (animator.GetBool("Aiming"))
            animator.SetBool("ShotWithAim", true);
        else
            animator.SetBool("Shot", true);
    }

    public void ReloadEnd()
    {
        animator.SetBool("Empty", false);
    }

    public void ReloadStart(bool empty)
    {
        if (!animator.GetBool("Empty"))
        {
            animator.SetTrigger("ReloadEmpty");
        }
        else
        {
            animator.SetTrigger("Reloading");
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
