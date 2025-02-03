using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:锁定模式视觉反馈
public class LockGun : BaseGun
{
    [Header("Gun Setting")]
    [Header("锁定模式")]
    public bool enableLock;
    public float lockRange;
    public float lockRadius;
    public float lockTime;
    public bool multilock;
    public int maxLockCount;
    [Tooltip("可以被锁定目标所在层级以及可以阻挡锁定的层级")]
    public LayerMask scanLayer;

    private RaycastHit[] hitInfos;
    /// <summary>
    /// 正在锁定的目标
    /// </summary>
    private List<LockInfo> lockingTarget = new();
    /// <summary>
    /// 已经被锁定的目标
    /// </summary>
    private List<Transform> lockedTargets = new();
    private Coroutine lockingCoroutine;

    public class LockInfo
    {
        public Transform Target;
        public float StartTime;
        public float Progress;
    }

    public override void ActivateWeapon(Camera cam)
    {
        base.ActivateWeapon(cam);
        hitInfos ??= new RaycastHit[50];
    }

    public override void AimEnd()
    {
        if (lockingCoroutine != null) StopCoroutine(lockingCoroutine);
        // 清除锁定标志
        base.AimEnd();
    }

    public override void AimStart()
    {
        lockingCoroutine = StartCoroutine(LockingTarget());
        base.AimStart();
    }

    private IEnumerator LockingTarget()
    {
        //从屏幕中间射出射线
        Ray _aimRay = cam.ViewportPointToRay(centerPoint);
        _aimRay.direction = cam.transform.forward;

        if (multilock)
        {
            int _lockCount = Physics.SphereCastNonAlloc(_aimRay, lockRange, hitInfos, set.weaponRange, scanLayer);
            for (int i = 0; i < _lockCount; i++)
            {
                LockInfo info = lockingTarget.Find((l) => { return l.Target == hitInfos[i].collider.transform; });

                if (info != null)
                {
                    // 持续锁中
                    info.Progress = Mathf.Clamp01((Time.time - info.StartTime) / lockTime);
                    if (info.Progress >= 1f)
                    {
                        lockedTargets.Add(info.Target);
                        lockingTarget.Remove(info);
                    }
                }
                else
                {
                    // 第一次锁中
                    LockInfo newInfo = new()
                    {
                        Target = hitInfos[i].collider.transform,
                        StartTime = Time.time,
                        Progress = 0f
                    };
                    lockingTarget.Add(newInfo);
                }
            }
        }
        else
        {
            if (!Physics.SphereCast(_aimRay, lockRange, out var hitInfo, set.weaponRange, scanLayer))
            {
                // 视野中没有能锁定的物体
                lockingTarget.Clear();
                yield return null;
            }

            if (lockingTarget.Count <= 0)
            {
                if (hitInfo.collider.TryGetComponent(out ILockable target))
                {
                    // 开始锁定新目标
                    LockInfo newInfo = new()
                    {
                        Target = hitInfo.collider.transform,
                        StartTime = Time.time,
                        Progress = 0f
                    };
                    lockingTarget.Add(newInfo);
                }
            }
            else
            {
                // 持续锁定目标
                LockInfo info = lockingTarget[0];
                if (info.Target == hitInfo.collider.transform)
                {
                    // 持续锁中
                    info.Progress = Mathf.Clamp01((Time.time - info.StartTime) / lockTime);
                    // 完成锁中
                    if (info.Progress >= 1f)
                    {
                        lockedTargets.Add(info.Target);
                        lockingTarget.Clear();
                    }
                }
            }
        }
        yield return null;
    }

    public override void Shoot()
    {
        if (!readyToShoot) return;
        else if (set.autoReloadWhenEmpty && currentBulletsCount <= 0)
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

        _targetPoint += ray.GetPoint(set.weaponRange);

        // 攻击已经锁定的敌人
        if (lockedTargets.Count > 0)
        {
            for (int i = lockedTargets.Count; i > 0; i--)
            {
                var bullet = InstantiateBullet();
                if (bullet.TryGetComponent(out ExplosionBulletSet t))
                    t.GetTarget(lockedTargets[i].transform);
                else
                {
                    Debug.LogError("子弹缺失追踪控制脚本!", bullet);
                }
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
            fireTimer.StartTiming(timeBetweenShoot, repeateTime: 1, onCompleted: ShootFinished);
            OnShot?.Invoke(isAiming);
        }


    }
}
