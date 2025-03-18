using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System;

[Serializable]
public class EnergyPower : ICharacterState
{
    [Header("体力值")]
    [SerializeField] private float maxPower = 100f;
    [SerializeField][ReadOnly] private float currentPower = 100f;
    public float CurrentPower => currentPower;

    public event Action OnStateUpdate;

    public void LowerPower(float amount)
    {
        currentPower -= amount;
        currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
        OnStateUpdate?.Invoke();
    }

    public void RecoverPower(float amount)
    {
        currentPower += amount;
        currentPower = Mathf.Clamp(currentPower, 0f, maxPower);
        OnStateUpdate?.Invoke();
    }
}
