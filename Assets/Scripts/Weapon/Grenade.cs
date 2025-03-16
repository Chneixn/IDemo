using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : ExplosionBullet
{
    [Tooltip("是否允许碰撞次数引爆")]
    public bool allowCollisionsToExplode;
    [Range(0f, 1f), Tooltip("子弹弹力")]
    public float bounciness;
    [Tooltip("最大弹跳次数")]
    public int maxCollisions;
    int collisions = 0; // 当前碰撞次数
    PhysicMaterial _physicsMaterial;

    protected override void Awake()
    {
        base.Awake();
        // 创建新物理材质，初始化材质设置
        _physicsMaterial = new PhysicMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        // PhysicMaterial.frictionCombine 确定摩擦力的组合方式
        // PhysicMaterial.bounceCombine 表面的弹性有多大？值为0时不会反弹。值为1将反弹而不会损失任何能量

        // 应用物理材质设置
        GetComponent<Collider>().material = _physicsMaterial;

        if (allowCollisionsToExplode)
            rb.useGravity = true;
    }

    public override void OnGet()
    {
        base.OnGet();

    }

    public override void OnRecycle()
    {
        base.OnRecycle();
        collisions = 0;
    }

    protected void OnCollisionEnter(Collision other)
    {
        if (allowCollisionsToExplode)
        {
            collisions++;
            if (collisions > maxCollisions)
            {
                // 碰撞次数达到最大
                Explode();
            }
        }
    }
}