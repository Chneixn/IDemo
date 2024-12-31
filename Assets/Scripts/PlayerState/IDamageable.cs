using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    Bullet,
    Explosion,
    Knife
}

public interface IDamageable
{
    /// <summary>
    /// 接受伤害
    /// direction用于计算布娃娃受力方向
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="type"></param>
    /// <param name="direction">用于计算布娃娃受力方向</param>
    public void TakeDamage(float damage, DamageType type, Vector3 direction);
}
