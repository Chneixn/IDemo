using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Crouching : IMovementState
{
    public float crouchSpeed; //下蹲后的移动速度
    public float crouchTime; //下蹲动作完成时间
    public float crouchCoolDownf; //下蹲操作的冷却时间
    public float crouchCamHeight; //下蹲后相机的高度相对于原本高度的比例
    // private bool _readyToCrouch = true;
    private readonly Collider[] _probedColliders = new Collider[8]; //取消下蹲前的碰撞体检测缓存
    public bool DebugCrouch;
    public struct CapsuleInfo
    {
        public float radius;
        public float height;
        public float yOffset;
    }
    // 存储初始与下蹲时的胶囊体数据
    private CapsuleInfo initialCapusle;
    private CapsuleInfo crouchCapsule;
    private bool saveCapsuleDate = false;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (!inputs.TryCrouch)
        {
            if (DebugCrouch) Debug.Log("尝试取消下蹲");
            // 恢复为初始胶囊体,检测是否有碰撞体重叠,若有则不结束下蹲
            CC.Motor.SetCapsuleDimensions(initialCapusle.radius, initialCapusle.height, initialCapusle.yOffset);

            if (CC.Motor.CharacterCollisionsOverlap(CC.Motor.TransientPosition, CC.Motor.TransientRotation, _probedColliders) > 0)
            {
                CC.Motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.yOffset);
                if (DebugCrouch) Debug.Log("取消下蹲失败，有碰撞体重叠");
                return;
            }
            if (DebugCrouch) Debug.Log("取消下蹲成功");

            if (CC.MoveDirection.sqrMagnitude >= 0.001f)
            {
                CC.ChangeMovementState(typeof(Walking));
            }
            else
            {
                CC.ChangeMovementState(typeof(Idle));
            }
        }
    }

    public override void OnStateEnter()
    {
        if (!saveCapsuleDate)
        {
            if (DebugCrouch) Debug.Log("初始化下蹲胶囊体信息");
            saveCapsuleDate = true;
            // 缓存胶囊体信息
            CapsuleCollider collider = CC.GetComponent<CapsuleCollider>();
            initialCapusle = new()
            {
                radius = collider.radius,
                yOffset = collider.center.y,
                height = collider.height
            };
            crouchCapsule = new()
            {
                radius = collider.radius,
                yOffset = collider.center.y / 2f,
                height = collider.height / 2f
            };
        }
        if (DebugCrouch) Debug.Log("开始下蹲");
        //更新为下蹲时胶囊体
        CC.Motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.yOffset);
        CC.ChangeCamHight(crouchCamHeight, crouchTime);

        CC.MaxSpeed = crouchSpeed;
    }

    public override void OnStateExit(IMovementState newState)
    {
        if (DebugCrouch) Debug.Log("退出下蹲完毕");
        //恢复摄像机位置
        CC.RecoverCamHight();
    }
}
