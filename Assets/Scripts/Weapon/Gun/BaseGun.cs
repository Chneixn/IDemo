using UnityEngine;
using InventorySystem;
using System;
using UnityUtils;
using Random = UnityEngine.Random;

public enum GunFireMode
{
    SemiAutomatic,
    Automatic,
    Burst
}

public class BaseGun : IWeapon
{
    public GunSetting set;

    [SerializeField, ReadOnly] protected int totalBulletsLeft = 0;        // 背包中剩余弹药数
    [SerializeField, ReadOnly] protected int currentBulletsCount = 0;     // 当前弹匣剩余子弹
    [SerializeField, ReadOnly] protected int bulletsShotted = 0;          // 已射击的子弹
    public int TotalBulletsLeft => totalBulletsLeft;
    public int CurrentBulletsCount => currentBulletsCount;

    protected GunFireMode currentFireMod;
    [SerializeField, ReadOnly] protected float timeBetweenShoot; // 依据每分钟射速计算每次最低开火间隔

    [Header("瞄准参数")]
    protected bool isAiming = false;
    [SerializeField, ReadOnly] protected bool readyToAim = true;
    [SerializeField, ReadOnly] protected bool readyToReload = true;
    [SerializeField, ReadOnly] protected bool readyToShoot = false;

    protected Camera cam;
    protected Transform _targetTransform;
    protected Vector3 centerPoint = new(0.5f, 0.5f, 0f);
    [Header("位置绑定")]
    [Tooltip("枪口位置, 用于子弹生成和开火特效")]
    [SerializeField] protected Transform muzzlePoint;           // 子弹射击点

    /// <summary>
    /// 如果击中某物的击中信息
    /// </summary>
    protected RaycastHit hitInfo;

    #region Action
    public Action OnEnableWeapon;
    public Action OnDisableWeapon;
    public Action<bool> OnShot;
    public Action<bool> OnShotFinshed;
    public Action OnAim;
    public Action OnAimFinshed;
    public Action<bool> OnReloadBegin;
    public Action OnReloadFinshed;
    #endregion

    protected Timer reloadTimer;
    protected Timer fireTimer;

    protected bool activated = false;

    /// <summary>
    /// 武器属性初始化赋值, 只在第一次EnableWeapon时调用一次
    /// </summary>
    /// <param name="cam"></param>
    protected virtual void ActivateWeapon(Camera cam)
    {
        if (activated) return;
        activated = true;

        if (visualModel == null) Debug.LogError("ViusalModel not set!");

        //初始化枪械状态
        this.cam = cam;
        fireTimer = TimerManager.CreateTimer();

        // 检查GunSet
        if (set == null) Debug.LogError("没有武器设置！", gameObject);
        currentFireMod = set.defaultFireMod;
        timeBetweenShoot = set.RPM / 3600f;
        if (set.isShotgun && set.isTraceableBullet) Debug.LogWarning("霰弹枪功能不能与追踪子弹共用！", gameObject);

        // 检查库存组件
        if (PlayerManager.Instance.PlayerInventory.PrimaryStorage == null) Debug.LogError("Can't find player's inventory!");
    }

    public override void EnableWeapon(Camera cam)
    {
        ActivateWeapon(cam);
        SetVisualModel(true);
        UpdateBulletCount();
        if (set.autoReloadWhenEmpty && currentBulletsCount == 0)
        {
            // 在换起武器时，若当前弹匣为空，自动尝试换弹匣
            readyToReload = true;
            Reload();
        }
        OnEnableWeapon?.Invoke();
    }

    public override void DisableWeapon()
    {
        // 在换弹匣时切换武器,重新换子弹
        SetVisualModel(false);
        if (!readyToReload && reloadTimer.IsActive) reloadTimer.PauseTimer();
        OnDisableWeapon?.Invoke();
    }

    public override void HandleInput(ref WeaponInput inputs)
    {
        if (inputs.reload) Reload();
        else if (inputs.fire) Shoot();
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

    protected virtual void ChangeFireMode()
    {
        if (currentFireMod == GunFireMode.SemiAutomatic && set.hasAuto)
            currentFireMod = GunFireMode.Automatic;
        else if (currentFireMod == GunFireMode.Burst && set.hasBrust)
            currentFireMod = GunFireMode.Burst;
        else
            currentFireMod = GunFireMode.SemiAutomatic;
    }

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
            if (set.autoReloadWhenEmpty) Reload();
            return;
        }

        readyToShoot = false;
        readyToReload = false;

        //从屏幕中间射出射线
        Ray ray = cam.ViewportPointToRay(centerPoint);
        ray.direction = cam.transform.forward;

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
                hitInfo = _hitInfo;
                //以射击即击中的方式传入伤害，忽略子弹飞行时间
                if (_hitInfo.collider.TryGetComponent(out IDamageable t))
                {
                    t.TakeDamage(set.weaponDamage, DamageType.Bullet, ray.direction);
                }
            }
        }

        if (set.needInstantiate)
        {
            var bullet = InstantiateBullet();
            // 赋予子弹速度
            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * set.shootVelocity;
        }

        // 霰弹枪功能
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

    protected virtual Vector2 CalculateSpread()
    {
        // 增加击中点的偏移值
        float x = Random.Range(-set.spread, set.spread);
        float y = Random.Range(-set.spread, set.spread);

        return new Vector2(x, y);
    }

    protected virtual GameObject InstantiateBullet()
    {
        // TODO: 接入对象池系统
        if (set.bulletData.item_prefab == null)
        {
            Debug.LogWarning("未指定子弹预制体！");
            return null;
        }
        else return Instantiate(set.bulletData.item_prefab, muzzlePoint.position, Quaternion.identity);
    }

    protected virtual void ShootFinished()
    {
        readyToShoot = true;
        readyToReload = true;
        bool isEmpty = currentBulletsCount == 0;
        if (isEmpty && set.autoReloadWhenEmpty) Reload();
        OnShotFinshed?.Invoke(isEmpty);
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
        if (reloadTimer != null)
            reloadTimer.StartTiming(set.reloadTime, repeateTime: 1, onCompleted: ReloadFinished);
        else
        {
            reloadTimer = TimerManager.CreateTimer();
            reloadTimer.StartTiming(set.reloadTime, repeateTime: 1, onCompleted: ReloadFinished);
        }
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
        UpdateBulletCount();
        OnReloadFinshed?.Invoke();
    }
    #endregion

    #region Handle Ammo
    /// <summary>
    /// 获取背包中剩余弹药
    /// </summary>
    protected void UpdateBulletCount()
    {
        totalBulletsLeft = PlayerManager.Instance.PlayerInventory.PrimaryStorage.GetItemCountFormInventory(set.bulletData);
        //Debug.Log($"update bullet count! total bullet left {totalBulletsLeft}");
    }

    /// <summary>
    /// 从背包中移除弹药
    /// </summary>
    /// <param name="amount"></param>
    protected void RemoveBulletFormInventory(int amount)
    {
        bool success = PlayerManager.Instance.PlayerInventory.PrimaryStorage.RemoveItemsFromInventory(set.bulletData, amount, out _);
        //if (!success)
        //{
        //    Debug.Log($"Remove ammo [{set.bulletData.displayName}] failed!");
        //}
        //else Debug.Log($"Remove ammo [{set.bulletData.displayName}] {amount}!");
    }
    #endregion
}
