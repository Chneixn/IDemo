using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using System;

[Serializable]
public class EXPState : ICharacterState
{
    public event OnStateChange OnStateUpdate;

    [Header("经验值")]
    [SerializeField][ReadOnly] private int currentLevel = 0;
    [SerializeField] private int maxLevel = 100;
    [SerializeField][ReadOnly] private int currentExp = 0;
    [SerializeField][ReadOnly] private int maxExp = 100;

    #region 经验
    private void AddExp(int newExp)
    {
        if (currentLevel >= maxLevel) return;
        currentExp += newExp;
        if (currentExp >= maxExp)
        {
            LevelUp();
        }

        OnStateUpdate.Invoke();
    }

    private void LevelUp()
    {
        currentLevel++;
        currentExp = 0;
        //升级奖励
        //maxHealth +=10; currentHealth = maxHealth;
        //升级惩罚
        maxExp += 100;
    }
    #endregion
}
