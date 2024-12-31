using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerStateSaveDate
{
    //生命值
    public float maxHealth;
    public float currentHealth;
    //体力值
    public float maxPower;
    //经验值
    public int currentExp;
    public int currentLevel;
    public int maxLevel;
    public int maxExp;

    public PlayerStateSaveDate(float maxHealth, float currentHealth, float maxPower, int currentExp,
                               int currentLevel, int maxLevel, int maxExp)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = currentHealth;
        this.maxPower = maxPower;
        this.currentExp = currentExp;
        this.currentLevel = currentLevel;
        this.maxLevel = maxLevel;
        this.maxExp = maxExp;
    }
}

public class PlayerStateHolder : CharacterStateHolder
{

}
