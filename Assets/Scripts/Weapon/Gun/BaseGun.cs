using UnityEngine;
using InventorySystem;
using System;
using Random = UnityEngine.Random;
using UnityGameObjectPool;
using System.Collections;

public enum GunFireMode
{
    SemiAutomatic,
    Automatic,
    Burst
}

public class Gun : IWeapon
{
    [SerializeField] protected GunSetting set;
    public GunSetting Setting => set;

    [SerializeField, ReadOnly] protected int totalBulletsLeft = 0;        // 背包中剩余弹药数
    [SerializeField, ReadOnly] protected int currentBulletsCount = 0;     // 当前弹匣剩余子弹
    [SerializeField, ReadOnly] protected int bulletsShotted = 0;          // 已射击的子弹
    public int TotalBulletsLeft => totalBulletsLeft;
    public int CurrentBulletsCount => currentBulletsCount;

    protected GunFireMode fireMod;
    [SerializeField, ReadOnly] protected float timeBetweenShoot; // 依据每分钟射速计算每次最低开火间隔

    [Header("控制参数")]
    protected bool isAiming = false;
    protected bool triggerPressed = false;
    [SerializeField, ReadOnly] protected bool readyToInput = false;
    [SerializeField, ReadOnly] protected bool readyToAim = true;
    [SerializeField, ReadOnly] protected bool readyToReload = true;
    [SerializeField, ReadOnly] protected bool readyToShoot = false;

    protected Transform _targetTransform;
    protected Vector3 centerPoint = new(0.5f, 0.5f, 0f);
    [Header("位置绑定")]
    [Tooltip("枪口位置, 用于子弹生成和开火特效")]
    [SerializeField] protected Transform muzzlePoint;           // 子弹射击点

    #region Action
    public Action OnEnableWeapon;
    public Action OnDisableWeapon;
    public Action<bool> OnShot;
    public Action OnShotEmpty;
    public Action<bool> OnShotFinshed;
    public Action<RaycastHit> OnHit;
    public Action OnAim;
    public Action OnAimFinshed;
    public Action<bool> OnReloadBegin;
    public Action OnReloadFinshed;
    #endregion

    protected Timer reloadTimer;
    protected Timer fireTimer;

    public override void ActivateWeapon()
    {
        fireTimer = TimerManager.CreateTimer();
        reloadTimer = TimerManager.CreateTimer();

        // 检查GunSet
        if (set == null) Debug.LogError("没有武器设置！", gameObject);
        fireMod = set.defaultFireMod;
        timeBetweenShoot = set.RPM / 3600f;
    }

    public override bool EnableWeapon()
    {
        holder.playerStorage.OnStorageUpdated += UpdateBulletCount;
        UpdateBulletCount();
        OnEnableWeapon?.Invoke();
        if (set.autoReloadWhenEmpty && currentBulletsCount == 0)
        {
            // 在换起武器时，若当前弹匣为空，自动尝试换弹匣
            readyToReload = true;
            Reload();
        }
        DelayEnableInput(set.timeToDraw);
        return true;
    }

    public override void DisableWeapon()
    {
        holder.playerStorage.OnStorageUpdated -= UpdateBulletCount;
        // 在换弹匣时切换武器,重新换子弹
        if (!readyToReload && reloadTimer.IsActive) reloadTimer.PauseTimer();

        OnDisableWeapon?.Invoke();
    }

    #region Handle Input
    public override void HandleInput(ref WeaponInput inputs)
    {
        if (!readyToInput) return;
        if (inputs.reload) Reload();
        else if (inputs.fire)
        {
            switch (fireMod)
            {
                case GunFireMode.SemiAutomatic:
                    {
                        if (!triggerPressed)
                        {
                            Shoot();
                            triggerPressed = true;
                        }
                    }
                    break;
                case GunFireMode.Automatic:
                    Shoot();
                    break;
                case GunFireMode.Burst:
                    {
                        StartCoroutine(BrustShoot());
                        OnDisableWeapon += StopAllCoroutines;
                    }
                    break;
            }
        }
        else if (!inputs.fire) triggerPressed = false;
        else if (inputs.aim)
        {
            AimStart();
        }
        else if (isAiming)
        {
            AimEnd();
        }
        else if (inputs.switchFireMod) ChangeFireMode();
    }

    protected virtual IEnumerator BrustShoot()
    {
        readyToInput = false;
        int count = set.burstBulletCount;
        while (currentBulletsCount > 0 && count > 0)
        {
            Shoot();
            count--;
            yield return new WaitForSeconds(set.timeBetweenShootOnBrust);
        }
        readyToInput = true;
    }

    protected virtual void DelayEnableInput(float duration)
    {
        readyToInput = false;
        TimerManager.CreateTimeOut(duration, () => readyToInput = true);
    }

    protected virtual void ChangeFireMode()
    {
        if (fireMod == GunFireMode.SemiAutomatic && set.hasAuto)
            fireMod = GunFireMode.Automatic;
        else if (fireMod == GunFireMode.Burst && set.hasBrust)
            fireMod = GunFireMode.Burst;
        else
            fireMod = GunFireMode.SemiAutomatic;
    }
    #endregion

    #region Handle Aim
    protected virtual void AimStart()
    {
        isAiming = true;
        OnAim?.Invoke();
    }

    protected virtual void AimEnd()
    {
        isAiming = false;
        OnAimFinshed?.Invoke();
    }
    #endregion

    #region HandleShoot
    protected virtual void Shoot()
    {
        if (!readyToShoot) return;
        else if (currentBulletsCount <= 0)
        {
            ShootEmpty();
            return;
        }

        readyToShoot = false;
        readyToReload = false;

        //从屏幕中间射出射线
        Ray ray = holder.Cam.ViewportPointToRay(centerPoint);
        ray.direction = holder.Cam.transform.forward;

        // 初始化击中点
        Vector3 _targetPoint = Vector3.zero;

        _targetPoint += ray.GetPoint(set.weaponRange);

        if (set.allowSpread)
        {
            //当玩家尝试移动时计算扩散后的射击方向
            _targetPoint += (Vector3)CalculateSpread();
            ray.direction = (_targetPoint - ray.origin).normalized;
        }

        if (set.fireIsHit)
        {
            if (Physics.Raycast(ray, out RaycastHit _hitInfo, set.weaponRange))
            {
                //以射击即击中的方式传入伤害，忽略子弹飞行时间
                if (_hitInfo.collider.TryGetComponent(out IDamageable t))
                {
                    t.TakeDamage(set.weaponDamage, DamageType.Bullet, ray.direction);
                    OnHit?.Invoke(_hitInfo);
                }
            }
        }

        if (set.needInstantiate) InstantiateBullet(ray.direction);

        // 霰弹枪功能, 同一帧内多发弹丸
        if (set.isShotgun && bulletsShotted < set.bulletsPerTap)
        {
            bulletsShotted++;  // 记录弹丸数
            readyToShoot = true;
            Shoot();
        }
        else
        {
            currentBulletsCount--;  // 每次射击，剩余子弹-1
            fireTimer.StartTimeOut(timeBetweenShoot, onCompleted: ShootFinished);
            OnShot?.Invoke(isAiming);
        }
    }

    protected virtual void ShootFinished()
    {
        readyToShoot = true;
        readyToReload = true;
        bulletsShotted = 0;
        bool isEmpty = currentBulletsCount == 0;
        if (isEmpty && set.autoReloadWhenEmpty) Reload();
        OnShotFinshed?.Invoke(isEmpty);
    }

    protected virtual void ShootEmpty()
    {
        if (set.autoReloadWhenEmpty) Reload();
        OnShotEmpty?.Invoke();
    }

    protected virtual Vector2 CalculateSpread()
    {
        // 增加击中点的偏移值
        float x = Random.Range(-set.spread, set.spread);
        float y = Random.Range(-set.spread, set.spread);

        return new Vector2(x, y);
    }

    protected virtual GameObject InstantiateBullet(Vector3 direction)
    {
        if (set.bulletData.prefab == null) return null;

        var bullet = GameObjectPoolManager.GetItem<Bullet>(set.bulletData.prefab.GetComponent<Bullet>(), muzzlePoint.position, muzzlePoint.rotation);
        bullet.SetVelocity(set.shootVelocity);
        return bullet.gameObject;
    }

    #endregion

    #region Handle Reload

    protected virtual void Reload()
    {
        if (!readyToReload || totalBulletsLeft <= 0) return;
        else if (currentBulletsCount >= set.defaultMagazineSize) return;

        readyToReload = false;
        readyToShoot = false;
        bool isEmpty = currentBulletsCount == 0;

        // 换子弹计时器
        reloadTimer.StartTiming(set.reloadTime, repeateTime: 1, onCompleted: ReloadFinished);

        //Debug.Log("reload!");
        OnReloadBegin?.Invoke(isEmpty);
    }

    protected virtual void ReloadFinished()
    {
        int need = set.defaultMagazineSize - currentBulletsCount;
        if (totalBulletsLeft < need)
        {
            // 剩余子弹不足补充满整个弹匣
            currentBulletsCount += totalBulletsLeft;
            RemoveBulletFormInventory(totalBulletsLeft);
        }
        else if (totalBulletsLeft >= need)
        {
            // 剩余子弹足够补充整个弹匣
            currentBulletsCount += need;
            RemoveBulletFormInventory(need);
        }
        readyToReload = true;
        readyToShoot = true;
        OnReloadFinshed?.Invoke();
    }
    #endregion

    #region Handle Ammo

    public virtual void UnLoadAmmo()
    {
        if (currentBulletsCount > 0)
        {
            currentBulletsCount = holder.playerStorage.AddToInventory(set.bulletData, currentBulletsCount);
        }
    }

    /// <summary>
    /// 获取背包中剩余弹药
    /// </summary>
    protected virtual void UpdateBulletCount()
    {
        totalBulletsLeft = holder.playerStorage.GetItemCountFormInventory(set.bulletData);
        //Debug.Log($"update bullet count! total bullet left {totalBulletsLeft}");
    }

    /// <summary>
    /// 从背包中移除弹药
    /// </summary>
    /// <param name="amount"></param>
    protected virtual bool RemoveBulletFormInventory(int amount)
    {
        return holder.playerStorage.RemoveItemsFromInventory(set.bulletData, amount, out _);
        //if (!success)
        //{
        //    Debug.Log($"Remove ammo [{set.bulletData.displayName}] failed!");
        //}
        //else Debug.Log($"Remove ammo [{set.bulletData.displayName}] {amount}!");
    }
    #endregion
}
