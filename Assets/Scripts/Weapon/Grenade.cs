using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Grenade : ExplosionBullet
{
    [Header("弹力")]
    [Range(0f, 1f), Tooltip("子弹弹力")]
    public float bounciness;
    public bool useGravity = true;

    [Tooltip("是否允许碰撞次数引爆")]
    public bool allowCollisionsToExplode;
    [Tooltip("最大弹跳次数")]
    public int maxCollisions;
    private Rigidbody rb;
    int collisions = 0; //当前碰撞次数
    PhysicMaterial _physicsMaterial;

    protected override void Awake()
    {
        base.Awake();
        //创建新物理材质，初始化材质设置
        _physicsMaterial = new PhysicMaterial
        {
            bounciness = bounciness,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Maximum
        };
        //PhysicMaterial.frictionCombine确定摩擦力的组合方式
        //PhysicMaterial.bounceCombine表面的弹性有多大？值为0时不会反弹。值为1将反弹而不会损失任何能量

        //应用物理材质设置
        GetComponent<Collider>().material = _physicsMaterial;
        rb = GetComponent<Rigidbody>();
        //设置重力使用
        rb.useGravity = useGravity;
    }

    public override void OnGet()
    {
        base.OnGet();
        collisions = 0;
    }

    protected void OnCollisionEnter(Collision other)
    {//记录每次碰撞
        collisions++;
        // 碰撞次数达到最大
        if (allowCollisionsToExplode)
        {
            if (collisions > maxCollisions)
            {
                Explode();
            }
        }
    }
}