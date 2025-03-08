using System;
using UnityEngine;
using UnityUtils;

public class Knife : IWeapon
{
    public KnifeSetting setting;
    private Timer cdTimer;

    #region Action

    public Action OnEnableWeapon;
    public Action OnDisableWeapon;
    /// <summary>
    /// 攻击(true为轻)
    /// </summary>
    public Action<bool> OnAttack;

    #endregion

    public override void ActivateWeapon()
    {
        cdTimer = TimerManager.CreateTimer();
        if (visualModel == null) Debug.LogError("ViusalModel not set!");
        if (setting == null) Debug.LogError("没有武器设置！", gameObject);
    }

    public override void DisableWeapon()
    {

    }

    public override bool EnableWeapon()
    {
        return true;
    }

    public override void HandleInput(ref WeaponInput input)
    {
        if (cdTimer.IsActive) return;

        if (input.fire) LitAttack();
        else if (input.aim) HeavyAttack();
    }

    private void LitAttack()
    {
        Attack(setting.litDamage);
        cdTimer.StartTimeOut(setting.litCoolDown, null, null, false);
        OnAttack?.Invoke(true);
    }

    private void HeavyAttack()
    {
        Attack(setting.heavyDamage);
        cdTimer.StartTimeOut(setting.heavyCoolDown, null, null, false);
        OnAttack?.Invoke(false);
    }

    private void Attack(float damage)
    {
        // 以屏幕中心为射线起始点
        Ray aimRay = holder.Cam.ScreenPointToRay(new(0.5f, 0.5f, 0f));
        aimRay.direction = holder.Cam.transform.forward;

        if (Physics.Raycast(aimRay, out RaycastHit _hitInfo, setting.attackRange))
        {
            if (_hitInfo.collider.TryGetComponent(out IDamageable target))
            {
                target.TakeDamage(damage, DamageType.Knife, holder.Cam.transform.forward);
            }
            SpawnDecal(_hitInfo);
        }
    }

    private void SpawnDecal(RaycastHit hit)
    {
        if (setting.attackEffect != null)
        {
            // TODO:接入对象池
            GameObject spawnedDecal = Instantiate(setting.attackEffect, hit.point, Quaternion.LookRotation(-hit.normal));
            // 贴花偏移, 避免重叠时产生闪烁
            spawnedDecal.transform.localPosition += new Vector3(0f, 0f, -0.02f);
        }
    }
}