using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

[Serializable]
[CreateAssetMenu(menuName = "Weapon/Create new GunSetting", fileName = "New_GunSetting")]
public class GunSetting : ScriptableObject
{
    [Header("Gun Parameter枪械参数")]
    public float weaponDamage;          //枪械伤害
    public bool isShotgun;

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

    public int bulletsPerTap;           // 单次射击弹丸数
    public float weaponRange;           // 枪械射程

    [Header("枪械射击模式")]
    [Tooltip("枪械默认射击模式")]
    public GunFireMode defaultFireMod;
    public bool hasAuto;
    public bool hasBrust;

    public bool fireIsHit;
    public float timeToHolster;         // 收起武器的时间

    [Header("BulletParameter子弹参数")]
    public ItemData bulletData;

    [Tooltip("正常射击时是否需要实例化子弹")]
    public bool needInstantiate;

    [Tooltip("子弹初速")]
    public float shootVelocity;

    [Tooltip("是否为追踪子弹(不可与霰弹枪功能同用)")]
    public bool isTraceableBullet;
}
