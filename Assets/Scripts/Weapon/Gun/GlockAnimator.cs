using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlockAnimator : GunAnimation
{
    public override void Start()
    {
        base.Start();
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
        if (isEmpty)
            animator.SetBool("Empty", true);
    }

    public void ShotStart(bool aimState)
    {
        if (!aimState)
            animator.SetBool("Shot", true);
        else
            animator.SetBool("ShotWithAim", true);
    }

    public void ReloadEnd()
    {
        animator.SetBool("EmptyReload", false);
        animator.SetBool("Reloading", false);
        animator.SetBool("Empty", false);
    }

    public void ReloadStart(bool isEmpty)
    {
        if (!isEmpty)
        {
            animator.SetBool("EmptyReload", true);
        }
        else
        {
            animator.SetBool("Reloading", true);
        }
    }
}
