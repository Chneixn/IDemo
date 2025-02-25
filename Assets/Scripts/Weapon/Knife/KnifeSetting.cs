using System;
using InventorySystem;
using UnityEngine;

[Serializable]
[CreateAssetMenu(menuName = "Weapon/Create new KnifeSetting", fileName = "New KnifeSetting")]
public class KnifeSetting : ScriptableObject
{
    [Header("武器参数")]
    public ItemData knifeData;
    public float litDamage;
    public float heavyDamage;
    public float attackRange;
    public float litCoolDown;
    public float heavyCoolDown;
    public bool allowButtonHold;
    public float timeToHolster;
    [Header("AttackEffect刀痕预制体")]
    public GameObject attackEffect;
}
