using Unity.Collections;
using UnityEngine;
using System;

[Serializable]
public class Health : ICharacterState
{
    [Header("生命值")]
    public bool isDamageable = true;
    public float maxHealth = 100f;
    [SerializeField][ReadOnly] private float currentHealth = 100f;
    public float CurrentHealth => currentHealth;
    [SerializeField][Range(0f, 1f)] private float defend_mult = 0f;
    [SerializeField][Range(0f, 1f)] private float explosion_mult = 0f;
    [SerializeField][ReadOnly] private bool isDead = false;
    public bool IsDead => isDead;
    public Action<Vector3> OnCharacterDead;
    public event Action OnStateUpdate;

    /// <summary>
    /// 增加防御系数
    /// </summary>
    /// <param name="amount">增加的百分比</param>
    /// <param name="type">防御的伤害类型</param>
    public void AddDefend(float amount, DamageType type)
    {
        switch (type)
        {
            case DamageType.Bullet:
                {
                    defend_mult += amount;
                    Mathf.Clamp(defend_mult, 0f, 1f);
                }
                break;
            case DamageType.Explosion:
                {
                    explosion_mult += amount;
                    Mathf.Clamp(explosion_mult, 0f, 1f);
                }
                break;
            case DamageType.Knife:
                break;
        }
    }

    /// <summary>
    /// 降低防御系数
    /// </summary>
    /// <param name="amount">降低的百分比</param>
    /// <param name="type">防御的伤害类型</param>
    public void RemoveDefend(float amount, DamageType type)
    {
        switch (type)
        {
            case DamageType.Bullet:
                {
                    defend_mult -= amount;
                    Mathf.Clamp(defend_mult, 0f, 1f);
                }
                break;
            case DamageType.Explosion:
                {
                    explosion_mult -= amount;
                    Mathf.Clamp(explosion_mult, 0f, 1f);
                }
                break;
            case DamageType.Knife:
                break;
        }
    }

    #region Health 生命值相关逻辑
    /// <summary>
    /// 承受攻击时
    /// </summary>
    /// <param name="damage">伤害的数值</param>
    /// <param name="type">伤害的类型</param>
    /// <param name="direction">受击力的方向(用于布娃娃)</param>
    public void TakeDamage(float damage, DamageType type, Vector3 direction)
    {
        if (!isDamageable || isDead) return;
        switch (type)
        {
            case DamageType.Bullet:
                {
                    currentHealth -= damage * (1 - defend_mult);
                    break;
                }
            case DamageType.Explosion:
                {
                    currentHealth -= damage * (1 - defend_mult);
                    break;
                }
            case DamageType.Knife:
                {
                    currentHealth -= damage;
                    break;
                }
        }

        currentHealth = Mathf.Clamp(CurrentHealth, 0, maxHealth);

        if (currentHealth == 0)
        {
            isDead = true;
            OnCharacterDead?.Invoke(direction);
        }
        OnDamage();
        OnStateUpdate?.Invoke();
    }

    private void OnDamage()
    {

    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnStateUpdate?.Invoke();
    }
    #endregion
}
