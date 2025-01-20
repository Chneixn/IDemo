using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CapsuleInfo
{
    public float radius;
    public float height;
    public float yOffset;
}

[Serializable]
public class Crouching : IMovementState
{
    public MovementState State => state;
    private readonly MovementState state = MovementState.Crouching;

    private CharacterControl character;
    public Crouching(CharacterControl character)
    {
        this.character = character;
    }

    public float crouchSpeed = 2f; //下蹲后的移动速度

    public float crouchTime = 0.8f; //下蹲动作完成时间

    public float crouchCoolDown = 0.8f; //下蹲操作的冷却时间

    public float crouchCamHeight = 0.5f; //下蹲后相机的高度相对于原本高度的比例

    private bool readyToCrouch = true;
    private readonly Collider[] _probedColliders = new Collider[8]; //取消下蹲前的碰撞体检测缓存

    // 存储初始与下蹲时的胶囊体数据
    private CapsuleInfo initialCapusle;
    private CapsuleInfo crouchCapsule;

    public void OnStateEnter(MovementState lastState)
    {
        ////状态更新
        //readyToCrouch = false;
        ////下蹲CD
        //TimerManager.CreateTimeOut(crouchCoolDown, () => readyToCrouch = true);

        ////更新为下蹲时胶囊体
        //motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.yOffset);
        ////更新摄像机位置
        //camTargetPos = camPosCache * crouchCamHeight;
        //camPosUpdateRequested = true;
    }

    public void OnStateExit(MovementState newState)
    {

    }

    public void AfterCharacterUpdate(float deltaTime)
    {

    }

    public void BeforeCharacterUpdate(float deltaTime)
    {

    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {

    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void PostGroundingUpdate(float deltaTime)
    {

    }

    public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
    {

    }

    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {

    }

    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {

    }
}
