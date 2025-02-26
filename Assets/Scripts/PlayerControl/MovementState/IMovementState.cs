using KinematicCharacterController;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class IMovementState : ICharacterController
{
    [HideInInspector] public CharacterControl CC;
    public abstract void OnStateExit(IMovementState newState);
    public abstract void OnStateEnter();
    public abstract void HandleStateChange(ref PlayerCharacterInput inputs);
    public virtual void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        if (CC.FaceDirection != Vector3.zero) return;
        if (CC.SmoothRotation && CC.OrientationSharpness > 0f)
        {
            var smoothedFaceDirection = Vector3.Slerp(CC.Motor.CharacterForward, CC.FaceDirection, 1 - Mathf.Exp(-CC.OrientationSharpness * deltaTime)).normalized;
            currentRotation = Quaternion.LookRotation(smoothedFaceDirection, CC.Motor.CharacterUp);
        }
        else
        {
            currentRotation = Quaternion.LookRotation(CC.FaceDirection, CC.Motor.CharacterUp);
        }
    }
    public virtual void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        //获得上一次的速度的方向，GetDirectionTangentToSurface可以求出玩家移动向量相对于地表法线的切线
        //motor.GroundingStatus.GroundNormal传出当前角色所在地面法线
        currentVelocity =
            CC.Motor.GetDirectionTangentToSurface(currentVelocity, CC.Motor.GroundingStatus.GroundNormal)
            * currentVelocity.magnitude;

        //计算玩家的速度向量（实现在非水平平面以准确速度移动）
        //1.需要移动的方向叉乘玩家向上方向，求出需要移动的方向的右侧向量
        Vector3 inputRight = Vector3.Cross(CC.MoveDirection, CC.Motor.CharacterUp);
        //2.用地表法线方向叉乘右侧向量，根据当前的地表状况重新计算出移动方向的前方，再乘玩家移动向量的归一化，得出考虑了地表情况的实际移动向量
        Vector3 reorientedInput =
            Vector3.Cross(CC.Motor.GroundingStatus.GroundNormal, inputRight).normalized
            * CC.MoveDirection.magnitude;
        //3.实际移动向量乘上当前移动速度，得出当前被设置的玩家速度
        var targetMovementVelocity = reorientedInput * CC.MaxSpeed;
        //4.使用Lerp插值计算，使当前速度平滑变为目标速度
        //currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, speedChangeTime);
        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
            1 - Mathf.Exp(-CC.StableMovementSharpness * deltaTime)
        );
    }
    public virtual void BeforeCharacterUpdate(float deltaTime) { }
    public virtual void PostGroundingUpdate(float deltaTime)
    {
        if (!CC.IsStableGround && CC.Motor.LastGroundingStatus.IsStableOnGround)
        {
            CC.ChangeMovementState(typeof(InAir));
        }
    }
    public virtual void AfterCharacterUpdate(float deltaTime) { }
    public virtual bool IsColliderValidForCollisions(Collider coll)
    {
        if (CC.IgnoredColliders.Contains(coll)) return false;
        else return true;
    }
    public virtual void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public virtual void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport) { }
    public virtual void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport) { }
    public virtual void OnDiscreteCollisionDetected(Collider hitCollider) { }
}
