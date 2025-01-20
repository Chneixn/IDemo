using UnityEngine;
using InventorySystem;

// TODO:已关闭动画系统

public enum GunFireMode
{
    Single,
    Burst,
    Auto
}

// 子弹扩散
// 使用库存系统的子弹
[RequireComponent(typeof(WeaponAnimation), typeof(WeaponAudio))]
public class BaseGun : MonoBehaviour, IWeapon
{
    public GunSetting set;

    [SerializeField, ReadOnly] private int totalBulletsLeft = 0;        // 背包中剩余弹药数
    [SerializeField, ReadOnly] private int currentBulletsCount = 0;     // 当前弹匣剩余子弹
    [SerializeField, ReadOnly] private int bulletsShotted = 0;          // 已射击的子弹
    public int TotalBulletsLeft => totalBulletsLeft;
    public int CurrentBulletsCount => currentBulletsCount;

    private GunFireMode currentFireMod;
    [SerializeField, ReadOnly] private float timeBetweenShoot; // 依据每分钟射速计算每次最低开火间隔

    [Header("瞄准参数")]
    [SerializeField, ReadOnly] private bool readyToAim;
    [SerializeField, ReadOnly] private bool readyToReload;
    [SerializeField, ReadOnly] private bool readyToShoot;


    [Header("位置绑定")]
    [SerializeField] private Transform attackPoint;           // 子弹射击点
    [SerializeField] private Transform casingPoint;           // 弹壳抛出点

    private Camera cam;
    private Transform _targetTransform;
    private Vector3 centerPoint = new(0.5f, 0.5f, 0f);

    [Header("GunVFX视觉效果")]
    [SerializeField] private ParticleSystem muzzleFlash;      // 枪口火焰 （粒子效果）
    [SerializeField] private ParticleSystem casingParticle;   // 弹壳抛出 （粒子效果）


    [Header("Sound声音效果")]
    [SerializeField] private WeaponAudio weaponAudio;

    //private IGunAnimation gunAnimation;
    private Timer reloadTimer;
    private Timer fireTimer;

    private bool activated = false;
    public bool Activated => activated;

    // 锁定模式参数
    private RaycastHit[] hitInfo;
    private int lockCount = 0;


    /// <summary>
    /// 武器初始化
    /// </summary>
    /// <param name="cam"></param>
    public void ActivateWeapon(Camera cam)
    {
        if (activated) return;
        activated = true;

        //初始化枪械状态
        this.cam = cam;
        hitInfo ??= new RaycastHit[set.maxLockCount];
        fireTimer = TimerManager.CreateTimer();

        // 检查GunSet
        if (set == null) Debug.LogError("没有武器设置！", gameObject);
        currentFireMod = set.defaultFireMod;
        timeBetweenShoot = set.RPM / 3600f;
        if (set.isShotgun && set.isTraceableBullet) Debug.LogWarning("霰弹枪功能不能与追踪子弹共用！", gameObject);

        // 检查音频组件
        if (TryGetComponent<WeaponAudio>(out var audio))
            weaponAudio = audio;
        else Debug.LogError("Audio not found!");

        // 检查库存组件
        if (GameManager.Instance.PlayerInventory.PrimaryInventorySystem == null) Debug.LogError("Can't find player's inventory!");
    }

    public void EnableWeapon()
    {
        UpdateBulletCount();
        readyToShoot = true;
        readyToAim = true;
        // 在换起武器时，若当前弹匣为空，自动尝试换弹匣
        if (currentBulletsCount == 0)
        {
            readyToReload = true;
            Reload();
        }
        //Debug.Log("gun enable");
    }

    public void DisableWeapon()
    {
        // 在换弹匣时切换武器,重新换子弹
        // TODO: 分段换弹匣动画
        if (!readyToReload && reloadTimer.IsActive) reloadTimer.PauseTimer();
        //Debug.Log("gun disable");
    }

    public void ChangeFireMode()
    {
        if (currentFireMod == GunFireMode.Single && set.hasAuto)
            currentFireMod = GunFireMode.Auto;
        else if (currentFireMod == GunFireMode.Burst && set.hasBrust)
            currentFireMod = GunFireMode.Burst;
        else
            currentFireMod = GunFireMode.Single;
    }

    #region Handle Aim
    public void AimStart()
    {
        // 锁定敌人功能
        if (!set.lockMode) return;

        //从屏幕中间射出射线
        Ray _aimRay = cam.ViewportPointToRay(centerPoint);
        _aimRay.direction = cam.transform.forward;

        int _lockCount = Physics.SphereCastNonAlloc(_aimRay, set.lockRange, hitInfo, set.weaponRange, set.lockableLayer);
        if (_lockCount > 0)
        {
            lockCount = _lockCount;
            // 显示锁定标志
        }
        else lockCount = 0;
    }

    public void AimEnd()
    {
        lockCount = 0;
        // 清除锁定标志
    }
    #endregion

    #region HandleShoot
    public void Shoot()
    {
        if (!readyToShoot) return;
        else if (currentBulletsCount <= 0)
        {
            Reload();
        }

        readyToShoot = false;
        readyToReload = false;

        //从屏幕中间射出射线
        Ray ray = cam.ViewportPointToRay(centerPoint);
        ray.direction = cam.transform.forward;

        // 初始化击中点
        Vector3 _targetPoint = Vector3.zero;

        if (set.lockMode)
        {
            // 攻击已经锁定的敌人
            if (set.isTraceableBullet && lockCount >= 0)
            {
                for (int i = lockCount; i > 0; i--)
                {
                    var bullet = InstantiateBullet();
                    if (bullet.TryGetComponent(out ExplosionBulletSet t))
                        t.GetTarget(hitInfo[i].collider.transform);
                    else
                    {
                        Debug.LogError("子弹预制体不带有追踪控制脚本！");
                    }
                }
            }
        }
        else    // 正常射击
        {
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
                    }

                    //尝试生成击中效果
                    if (_hitInfo.collider != null)
                        HandleHitEffect(ref _hitInfo);
                }
            }

            if (set.needInstantiate)
            {
                var bullet = InstantiateBullet();
                // 旋转子弹前方向为射击方向
                bullet.transform.forward = ray.direction;

                //Way-1 使用力使子弹获得速度
                //Bullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * shootForce, ForceMode.Impulse);
                //Bullet.GetComponent<Rigidbody>().AddForce(fpsCam.transform.up * upwardForce, ForceMode.Impulse); //增加子弹上跳

                // Way-2 直接赋予子弹速度
                bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * set.shootVelocity;
            }
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
            PlayShootEffects(true);
            fireTimer.StartTiming(timeBetweenShoot, repeateTime: 1, onCompleted: ShootFinished);
        }
    }

    private Vector2 CalculateSpread()
    {
        // 增加击中点的偏移值
        float x = Random.Range(-set.spread, set.spread);
        float y = Random.Range(-set.spread, set.spread);

        return new Vector2(x, y);
    }

    public void ShootFinished()
    {
        readyToShoot = true;
        readyToReload = true;
        PlayShootEffects(false);
        if (currentBulletsCount == 0)
        {
            Reload();
        }
    }

    /// <summary>
    /// 子弹实例化
    /// </summary>
    /// <param name="targetPoint"></param>
    private GameObject InstantiateBullet()
    {
        if (set.bulletData.item_prefab == null) { Debug.LogWarning("未指定子弹预制体！"); return null; }

        return GameObjectPoolManager.SpawnObject(set.bulletData.item_prefab, attackPoint.position, Quaternion.identity, GameObjectPoolManager.PoolType.Bullets);
    }

    private void PlayShootEffects(bool active)
    {
        if (active)
        {
            if (muzzleFlash != null) muzzleFlash.Play();
            weaponAudio.PlayShootSound();
        }
    }

    /// <summary>
    /// 处理击中某物后的效果实现
    /// </summary>
    /// <param name="hit">射线返回的信息</param>
    public void HandleHitEffect(ref RaycastHit hit)
    {
        if (set.bulletHole == null) { Debug.LogWarning("未指定弹痕预制体！"); return; }

        GameObject spawnedDecal = GameObjectPoolManager.SpawnObject(set.bulletHole, hit.point, Quaternion.LookRotation(-hit.normal));
        //将贴花作为被击中物体的子物体
        spawnedDecal.transform.SetParent(hit.collider.transform);

        ////检测击中物体的Tag，传入对应贴花预制体
        //if (hit.collider.CompareTag("Wall"))
        //    SpawnDecal(hit, bulletHole);
        //检测击中的物体是否有材质
        //if (hit.collider.sharedMaterial != null)
        //{
        //    //缓存材质的命名
        //    string materialName = hit.collider.sharedMaterial.name;
        //    if (bulletHole == null) return;
        //    //判断不同材质名生成对应贴花
        //    switch (materialName)
        //    {
        //        case "Metal":
        //            {

        //            }
        //            break;
        //        default:
        //            {

        //            }
        //            break;
        //    }
        //}
    }
    #endregion

    #region HandleReload

    public void Reload()
    {
        if (!readyToReload && totalBulletsLeft <= 0) return;
        if (currentBulletsCount >= set.defaultMagazineSize) return;

        readyToReload = false;
        readyToShoot = false;
        bool isEmpty = currentBulletsCount == 0;

        // 换子弹计时器
        if (reloadTimer != null)
            reloadTimer.StartTiming(set.reloadTime, repeateTime: 1, onCompleted: ReloadFinished, onUpdate: OnReloading);
        else
        {
            reloadTimer = TimerManager.CreateTimer();
            reloadTimer.StartTiming(set.reloadTime, repeateTime: 1, onCompleted: ReloadFinished, onUpdate: OnReloading);
        }
        //Debug.Log("reload!");
        //gunAnimation.ReloadAnimationStart(isEmpty);
    }

    private void OnReloading(float t)
    {
        bool isEmpty = currentBulletsCount == 0;
        weaponAudio.PlayReloadSounds(set.reloadTime, t, isEmpty);
    }

    private void ReloadFinished()
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
        //Debug.Log("reload finished!");
    }
    #endregion

    /// <summary>
    /// 获取背包中剩余弹药
    /// </summary>
    public void UpdateBulletCount()
    {
        totalBulletsLeft = GameManager.Instance.PlayerInventory.PrimaryInventorySystem.GetItemCountFormInventory(set.bulletData);
        //Debug.Log($"update bullet count! total bullet left {totalBulletsLeft}");
    }

    /// <summary>
    /// 从背包中移除弹药
    /// </summary>
    /// <param name="amount"></param>
    private void RemoveBulletFormInventory(int amount)
    {
        bool success = GameManager.Instance.PlayerInventory.PrimaryInventorySystem.RemoveItemsFromInventory(set.bulletData, amount, out _);
        //if (!success)
        //{
        //    Debug.Log($"Remove ammo [{set.bulletData.displayName}] failed!");
        //}
        //else Debug.Log($"Remove ammo [{set.bulletData.displayName}] {amount}!");
    }
}
