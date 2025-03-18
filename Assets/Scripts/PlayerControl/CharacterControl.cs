using System;
using System.Collections.Generic;
using KinematicCharacterController;
using Unity.Cinemachine;
using UnityEngine;

public enum ForwardModes { Camera, Player, World };

public struct PlayerCharacterInput
{
    public Vector2 MoveDirection;
    public Quaternion CamRotation;
    public Vector3 LookDirection;
    public CamState CameraState;
    public bool TryRun;
    public bool TryDodge;
    public bool TryJump;
    public bool TryCrouch;
    public bool TryFly;
}

public class CharacterControl : MonoBehaviour, ICharacterController
{
    [HideInInspector]
    public KinematicCharacterMotor Motor;
    //private PlayerBodyAnimation bodyAnimation;

    #region Public
    public Vector3 CurrentSpeed;
    public bool IsStableGround => Motor.GroundingStatus.IsStableOnGround;
    public bool IsAnyGround => Motor.GroundingStatus.FoundAnyGround;
    #endregion

    [Header("Camera")]
    public Transform CharacterCamPos;
    [SerializeField, ReadOnly] private float camYPosCache; //缓存相机原始Y坐标
    [SerializeField, ReadOnly] private float camTargetPos; //缓存相机即将移动到的Y坐标
    [SerializeField, ReadOnly] private bool camPosUpdateRequested = false;
    [SerializeField, ReadOnly] private float camVelocity; //用于平滑摄像机移动
    [SerializeField, ReadOnly] private float camMoveTime;

    [Header("Direction")]
    [ReadOnly] public Vector3 MoveDirection;
    [ReadOnly] public Vector3 FaceDirection;
    [ReadOnly] public Vector3 MoveDirectionInput;

    #region State
    private FSM<IMovementState> FSM;
    private IMovementState CurrentState => FSM.CurState;
    public Action<IMovementState> OnMovementStateChanged;

    [Header("MovementState移动状态")]
    public Freeze freeze = new();
    public Idle idle = new();
    public Walking walking = new();
    public Running run = new();
    public Jump jump = new();
    public Crouching crouching = new();
    public InAir inAir = new();
    public Fly fly = new();
    #endregion

    #region Setting
    [Header("Setting")]
    [Tooltip("当前最大限制速度")]
    public float MaxSpeed = 0f;
    [Tooltip("速度改变的敏锐度")]
    public float StableMovementSharpness = 15f;
    [Tooltip("是否平滑旋转")]
    public bool SmoothRotation = true;
    [Tooltip("平滑转向的敏锐度")]
    public float OrientationSharpness = 10f;

    [SerializeField, ReadOnly] private Vector3 internalVelocity = new(0, 0, 0); // 下一帧的附加突变速度
    public Vector3 InternalVelocity
    {
        set { internalVelocity = value; }
    }

    public GravityConfig Gravity = new();

    [Header("Colliders碰撞体")]
    public LayerMask IgnoredCollidersMask;
    public List<Collider> IgnoredColliders = new(); //忽视碰撞体的列表
    [SerializeField] private Transform meshRoot;

    #endregion

    private void Awake()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        Motor.CharacterController = this;
        camYPosCache = CharacterCamPos.localPosition.y;

        CreateFSM();
    }

    private void CreateFSM()
    {
        // 在创建状态机时输入第一个状态
        FSM = new(idle, this);
        FSM.AddState(fly);
        FSM.AddState(inAir);
        FSM.AddState(crouching);
        FSM.AddState(jump);
        FSM.AddState(walking);
        FSM.AddState(run);
        FSM.AddState(freeze);
    }

    public void ChangeMovementState(System.Type stateType)
    {
        FSM.ChangeState(stateType);
        OnMovementStateChanged?.Invoke(CurrentState);
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        if (camPosUpdateRequested)
        {
            // 将摄像机位置平滑移动至下蹲后应有高度位置
            float height = Mathf.SmoothDamp(CharacterCamPos.localPosition.y, camTargetPos, ref camVelocity, camMoveTime);
            CharacterCamPos.localPosition = new Vector3(0f, height, 0f);
            if (CharacterCamPos.localPosition.y == camTargetPos)
                camPosUpdateRequested = false;
        }

        CurrentState.AfterCharacterUpdate(deltaTime);
    }

    /// <summary>
    /// 改变角色摄像机高度
    /// </summary>
    /// <param name="newHight"></param>
    /// <param name="moveTime"></param>
    public void ChangeCamHight(float newHight, float moveTime)
    {
        camPosUpdateRequested = true;
        camTargetPos = camYPosCache * newHight;
        camMoveTime = moveTime;
    }

    public void RecoverCamHight()
    {
        camPosUpdateRequested = true;
        camTargetPos = camYPosCache;
    }

    public void BeforeCharacterUpdate(float deltaTime)
    {
        CurrentState.BeforeCharacterUpdate(deltaTime);
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        if (coll.gameObject.layer == IgnoredCollidersMask) return true;
        return CurrentState.IsColliderValidForCollisions(coll);
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        CurrentState.OnDiscreteCollisionDetected(hitCollider);
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        CurrentState.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        CurrentState.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        CurrentState.PostGroundingUpdate(deltaTime);
    }

    public void ProcessHitStabilityReport(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        Vector3 atCharacterPosition,
        Quaternion atCharacterRotation,
        ref HitStabilityReport hitStabilityReport
    )
    {
        CurrentState.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
    }

    public void OnCameraUpdate(float deltaTime, Vector3 camForward)
    {
        camForward = Vector3.ProjectOnPlane(camForward, Motor.CharacterUp);
        //Frame Perfect rotation实现，在更新相机旋转的同一帧，更新角色模型子物体的旋转
        if (meshRoot != null)
            meshRoot.rotation = Quaternion.LookRotation(camForward, Motor.CharacterUp);
    }

    /// <summary>
    /// 能且仅能在这里控制角色的转向
    /// </summary>
    /// <param name="currentRotation">玩家应有的rotation</param>
    /// <param name="deltaTime">当前KCC刷新时间，默认与fixedUpdate同步为0.02s</param>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        CurrentState.UpdateRotation(ref currentRotation, deltaTime);

        // 控制相对于 Gravity.Value 的旋转
        if (Gravity.walkOnAnyGround)
        {
            // 当前 CharacterUp 与 Gravity.value 是否存在角度, 
            float angleBetweenUpDirections = Vector3.Angle(Gravity.Value, Motor.CharacterUp);
            // float angleThreshold = 0.001f;
            // 角度计算存在误差
            if (angleBetweenUpDirections < 0.001f) return;
            // 这里的转向不考虑旋转的方向
            Quaternion retationDifference = Quaternion.FromToRotation(Motor.CharacterUp, Gravity.Value);
            currentRotation = retationDifference * currentRotation;
        }
    }

    /// <summary>
    /// 能且仅能在这里控制角色移动
    /// </summary>
    /// <param name="currentVelocity">需要被设置的玩家速度</param>
    /// <param name="deltaTime">0.02f</param>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        CurrentState.UpdateVelocity(ref currentVelocity, deltaTime);

        // 实现瞬时突变速度
        if (internalVelocity.sqrMagnitude > 0f)
        {
            currentVelocity += internalVelocity;
            internalVelocity = Vector3.zero;
        }

        //重力实现
        if (!IsAnyGround && Gravity.Enable)
        {
            currentVelocity += Gravity.Value * deltaTime;
        }

        //获取当前直观速度大小
        CurrentSpeed = currentVelocity;
    }

    #region 玩家输入
    /// <summary>
    /// 获取玩家的操作输入, 处理移动向量与视角向量处理
    /// </summary>
    /// <param name="inputs"></param>
    public void HandleInput(ref PlayerCharacterInput inputs)
    {
        // 玩家想要移动的按键输入方向
        MoveDirectionInput = new Vector3(inputs.MoveDirection.x, 0f, inputs.MoveDirection.y);

        switch (inputs.CameraState)
        {
            case CamState.FPS:
                FaceDirection = inputs.LookDirection.ProjectOntoPlane(Motor.CharacterUp).normalized;
                break;
            case CamState.TPS: goto case CamState.FPS;
            case CamState.FreeLook:
                FaceDirection = Motor.CharacterForward;
                break;
        }

        // 玩家移动向量为当前
        MoveDirection = Motor.TransientRotation * MoveDirectionInput;

        if (inputs.TryFly && fly.allowFly)
        {
            ChangeMovementState(typeof(Fly));
        }

        CurrentState.HandleStateChange(ref inputs);
    }
    #endregion
}