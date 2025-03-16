using UnityEngine;
using InventorySystem;
using System;
using UnityUtils;
using Random = UnityEngine.Random;
using Cysharp.Threading.Tasks;
using System.Threading;

public enum GunFireMode
{
    SemiAutomatic,
    Automatic,
    Burst
}

public class BaseGun : IWeapon
{
    public GunSetting setting;

    [SerializeField, ReadOnly] protected int totalBulletsLeft = 0;        // 背包中剩余弹药数
    [SerializeField, ReadOnly] protected int currentBulletsCount = 0;     // 当前弹匣剩余子弹
    [SerializeField, ReadOnly] protected int bulletsShotted = 0;          // 已射击的子弹
    public int TotalBulletsLeft => totalBulletsLeft;
    public int CurrentBulletsCount => currentBulletsCount;

    protected GunFireMode currentFireMod;
    [SerializeField, ReadOnly] protected float timeBetweenShoot; // 依据每分钟射速计算每次最低开火间隔

    [Header("控制参数")]
    protected bool isAiming = false;
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
        if (setting == null) Debug.LogError("没有武器设置！", gameObject);
        currentFireMod = setting.defaultFireMod;
        timeBetweenShoot = setting.RPM / 3600f;
    }

    public override bool EnableWeapon()
    {
        holder.playerStorage.OnStorageUpdated += UpdateBulletCount;
        UpdateBulletCount();
        OnEnableWeapon?.Invoke();
        if (setting.autoReloadWhenEmpty && currentBulletsCount == 0)
        {
            // 在换起武器时，若当前弹匣为空，自动尝试换弹匣
            readyToReload = true;
            Reload();
        }
        DelayEnableInput(setting.timeToDraw);
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
            Shoot();
        }
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

    protected virtual void DelayEnableInput(float duration)
    {
        readyToInput = false;
        TimerManager.CreateTimeOut(duration, () => readyToInput = true);
    }

    protected virtual void ChangeFireMode()
    {
        if (currentFireMod == GunFireMode.SemiAutomatic && setting.hasAuto)
            currentFireMod = GunFireMode.Automatic;
        else if (currentFireMod == GunFireMode.Burst && setting.hasBrust)
            currentFireMod = GunFireMode.Burst;
        else
            currentFireMod = GunFireMode.SemiAutomatic;
    }
    #endregion

    #region Handle Aim
    // FIXME:瞄准动画播放时可以开火
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
            if (setting.autoReloadWhenEmpty) Reload();
            return;
        }

        readyToShoot = false;
        readyToReload = false;

        //从屏幕中间射出射线
        Ray ray = holder.Cam.ViewportPointToRay(centerPoint);
        ray.direction = holder.Cam.transform.forward;

        // 初始化击中点
        Vector3 _targetPoint = Vector3.zero;

        _targetPoint += ray.GetPoint(setting.weaponRange);

        if (setting.allowSpread)
        {
            //当玩家尝试移动时计算扩散后的射击方向
            _targetPoint += (Vector3)CalculateSpread();
            ray.direction = (_targetPoint - ray.origin).normalized;
        }

        if (setting.fireIsHit)
        {
            if (Physics.Raycast(ray, out RaycastHit _hitInfo, setting.weaponRange))
            {
                //以射击即击中的方式传入伤害，忽略子弹飞行时间
                if (_hitInfo.collider.TryGetComponent(out IDamageable t))
                {
                    t.TakeDamage(setting.weaponDamage, DamageType.Bullet, ray.direction);
                    OnHit?.Invoke(_hitInfo);
                }
            }
        }

        if (setting.needInstantiate) InstantiateBullet(ray.direction);

        // 霰弹枪功能
        if (setting.isShotgun && bulletsShotted < setting.bulletsPerTap)
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

    protected virtual Vector2 CalculateSpread()
    {
        // 增加击中点的偏移值
        float x = Random.Range(-setting.spread, setting.spread);
        float y = Random.Range(-setting.spread, setting.spread);

        return new Vector2(x, y);
    }

    protected virtual GameObject InstantiateBullet(Vector3 direction)
    {
        if (setting.bulletData.prefab == null) return null;

        var bullet = GameObjectPoolManager.GetItem<Bullet>(setting.bulletData.prefab.GetComponent<Bullet>(), muzzlePoint.position, muzzlePoint.rotation);
        bullet.SetRigiBodyVelocity(setting.shootVelocity);
        return bullet.gameObject;
    }

    protected virtual void ShootFinished()
    {
        readyToShoot = true;
        readyToReload = true;
        bool isEmpty = currentBulletsCount == 0;
        if (isEmpty && setting.autoReloadWhenEmpty) Reload();
        OnShotFinshed?.Invoke(isEmpty);
    }

    #endregion

    #region Handle Reload

    protected virtual void Reload()
    {
        if (!readyToReload || totalBulletsLeft <= 0) return;
        else if (currentBulletsCount >= setting.defaultMagazineSize) return;

        readyToReload = false;
        readyToShoot = false;
        bool isEmpty = currentBulletsCount == 0;

        // 换子弹计时器
        reloadTimer.StartTiming(setting.reloadTime, repeateTime: 1, onCompleted: ReloadFinished);

        //Debug.Log("reload!");
        OnReloadBegin?.Invoke(isEmpty);
    }

    protected virtual void ReloadFinished()
    {
        int need = setting.defaultMagazineSize - currentBulletsCount;
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
            currentBulletsCount = holder.playerStorage.AddToInventory(setting.bulletData, currentBulletsCount);
        }
    }

    /// <summary>
    /// 获取背包中剩余弹药
    /// </summary>
    protected virtual void UpdateBulletCount()
    {
        totalBulletsLeft = holder.playerStorage.GetItemCountFormInventory(setting.bulletData);
        //Debug.Log($"update bullet count! total bullet left {totalBulletsLeft}");
    }

    /// <summary>
    /// 从背包中移除弹药
    /// </summary>
    /// <param name="amount"></param>
    protected virtual bool RemoveBulletFormInventory(int amount)
    {
        return holder.playerStorage.RemoveItemsFromInventory(setting.bulletData, amount, out _);
        //if (!success)
        //{
        //    Debug.Log($"Remove ammo [{set.bulletData.displayName}] failed!");
        //}
        //else Debug.Log($"Remove ammo [{set.bulletData.displayName}] {amount}!");
    }
    #endregion
}
