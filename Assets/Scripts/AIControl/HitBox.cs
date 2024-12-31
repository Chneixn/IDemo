using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamageable
{
    public CharacterStateHolder stateHolder;

    public void TakeDamage(float damage, DamageType type, Vector3 direction)
    {
        stateHolder.healthState.TakeDamage(damage, type, direction);
    }
}
