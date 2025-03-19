using System;
using UnityEngine;
using InventorySystem;

[Serializable]
[CreateAssetMenu(menuName = "Weapon/Create new GunSetting", fileName = "New GunSetting")]
public class GunSetting : ScriptableObject
{
    public ItemData gunData;
    [Header("Gun Parameter枪械参数")]
    public float weaponDamage;          //枪械伤害

    [Tooltip("每分钟射速Revolutions per minute")]
    public float RPM;

    [Tooltip("是否有射击扩散")]
    public bool allowSpread;            // 是否有射击扩散
    public float spread;                // 子弹扩散大小
    public float spreadMult;            // 扩散系数
    public float spreadMultWithAim;     // 瞄准时扩散系数

    public bool autoReloadWhenEmpty;
    public float reloadTime;            // 弹匣填装时间
    public float reloadEmptyTime;       // 空仓后弹匣填装时间
    public int defaultMagazineSize;     // 弹匣容量
    public float weaponRange;           // 枪械射程

    [Header("枪械射击模式")]
    [Tooltip("枪械默认射击模式")]
    public GunFireMode defaultFireMod;
    public bool hasAuto;
    public bool hasBrust;
    public int burstBulletCount;
    public float timeBetweenShootOnBrust;

    public bool isShotgun;
    [Tooltip("单次射击弹丸数")]
    public int bulletsPerTap;           // 单次射击弹丸数

    [Header("BulletParameter子弹参数")]
    public bool fireIsHit;
    public ItemData bulletData;
    public Bullet bulletPrefab;
    [Tooltip("子弹初速")]
    public float shootVelocity;
    [Tooltip("是否为追踪子弹(不可与霰弹枪功能同用)")]
    public bool isTraceableBullet;

    [Header("动画延迟")]
    [Tooltip("拿起武器的时间")]
    public float timeToDraw;
    [Tooltip("收起武器的时间")]
    public float timeToHolster;
}
