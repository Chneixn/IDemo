using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;

public enum ForwardModes { Camera, Player, World };

public struct PlayerCharacterInput
{
    public Vector2 MoveDirection;
    public Quaternion CamRotation;
    public Vector3 LookDirection;
    public ForwardModes inputForward;
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
    public Vector3 CurrentSpeed => currentVelocity;
    public bool IsStableGround => Motor.GroundingStatus.IsStableOnGround;
    public bool IsAnyGround => Motor.GroundingStatus.FoundAnyGround;
    #endregion

    [Header("Camera")]
    public Transform CharacterCamPos;
    [SerializeField, ReadOnly] private float camYPosCache; //缓存相机原始Y坐标
    [SerializeField, ReadOnly] private float camTargetPos; //缓存相机即将移动到的Y坐标
    [SerializeField, ReadOnly] private bool camPosUpdateRequested = false;
    [SerializeField, ReadOnly] private float cam_velocity; //用于平滑摄像机移动
    [SerializeField, ReadOnly] private float camMoveTime;

    [Header("Direction")]
    [ReadOnly] public Quaternion CamRotation;
    [ReadOnly] public Vector3 MoveDirection;
    [ReadOnly] public Vector3 FaceDirection;
    [ReadOnly] public Vector3 MoveDirectionInput;

    // These are part of a strategy to combat input gimbal lock when controlling a player
    // that can move freely on surfaces that go upside-down relative to the camera.
    // This is only used in the specific situation where the character is upside-down relative to the input frame,
    // and the input directives become ambiguous.
    // If the camera and input frame are travelling along with the player, then these are not used.
    private ForwardModes inputForward;
    bool m_InTopHemisphere = true;
    float m_TimeInHemisphere = 100;
    Vector3 m_LastRawInput;
    Quaternion m_Upsidedown = Quaternion.AngleAxis(180, Vector3.left);

    #region Setting
    private IMovementState CurrentMovementState;
    [Header("MovementState移动状态")]
    [ReadOnly] public MovementState MovementState;
    public Action<MovementState> OnMovementStateChanged;
    public Freeze freeze = new();
    public Idle idle = new();
    public Walking walking = new();
    public Run run = new();
    public Jump jump = new();
    public Crouching crouching = new();
    public InAir inAir = new();
    public Fly fly = new();

    [Header("Speed")]
    [SerializeField, ReadOnly] private Vector3 currentVelocity;
    [SerializeField, ReadOnly] private Vector3 targetMovementVelocity;
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

    #region Gravity
    [Header("Gravity重力")]
    public bool EnableGravity = true;
    [SerializeField]
    private bool _walkOnAnyGround = false;
    [SerializeField]
    private bool usePhysicsGravity = false;
    [SerializeField]
    private Vector3 default_gravity = new(0, -15f, 0);
    [SerializeField, ReadOnly]
    private Vector3 using_gravity = Vector3.zero;
    public Vector3 Gravity
    {
        get { return usePhysicsGravity == false ? using_gravity : Physics.gravity; }
        set { using_gravity = value; }
    }
    #endregion

    [Header("Colliders碰撞体")]
    public List<Collider> IgnoredColliders = new(); //忽视碰撞体的列表
    [SerializeField] private Transform meshRoot;

    #endregion

    private void Awake()
    {
        Motor = GetComponent<KinematicCharacterMotor>();
        Motor.CharacterController = this;
        camYPosCache = CharacterCamPos.localPosition.y;
        using_gravity = default_gravity;
        InitState();
    }

    private void Start()
    {
        CurrentMovementState = idle;
        ChangeMovementState(idle);
    }

    private void InitState()
    {
        freeze.CC = this;
        idle.CC = this;
        walking.CC = this;
        run.CC = this;
        jump.CC = this;
        crouching.CC = this;
        inAir.CC = this;
        fly.CC = this;
    }

    public void ChangeMovementState(IMovementState newState)
    {
        var lastState = CurrentMovementState.State;
        CurrentMovementState.OnStateExit(newState.State);
        CurrentMovementState = newState;
        MovementState = CurrentMovementState.State;
        OnMovementStateChanged?.Invoke(newState.State);
        newState.OnStateEnter(lastState);
    }

    public void AfterCharacterUpdate(float deltaTime)
    {
        if (camPosUpdateRequested)
        {
            // 将摄像机位置平滑移动至下蹲后应有高度位置
            float height = Mathf.SmoothDamp(CharacterCamPos.localPosition.y, camTargetPos, ref cam_velocity, camMoveTime);
            CharacterCamPos.localPosition = new Vector3(0f, height, 0f);
            if (CharacterCamPos.localPosition.y == camTargetPos)
                camPosUpdateRequested = false;
        }

        CurrentMovementState.AfterCharacterUpdate(deltaTime);
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
        CurrentMovementState.BeforeCharacterUpdate(deltaTime);
    }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        //处理碰撞：在列表中的物体不计算物体碰撞
        return CurrentMovementState.IsColliderValidForCollisions(coll);
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider)
    {
        CurrentMovementState.OnDiscreteCollisionDetected(hitCollider);
    }

    public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        CurrentMovementState.OnGroundHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
    {
        CurrentMovementState.OnMovementHit(hitCollider, hitNormal, hitPoint, ref hitStabilityReport);
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        CurrentMovementState.PostGroundingUpdate(deltaTime);
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
        CurrentMovementState.ProcessHitStabilityReport(hitCollider, hitNormal, hitPoint, atCharacterPosition, atCharacterRotation, ref hitStabilityReport);
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
        CurrentMovementState.UpdateRotation(ref currentRotation, deltaTime);

        #region 在其他重力平台自适应重力进行胶囊体旋转
        if (_walkOnAnyGround)
        {
            float angleBetweenUpDirections = Vector3.Angle(-using_gravity, Motor.CharacterUp);
            //float angleThreshold = 0.001f;

            if (angleBetweenUpDirections < 0.001f) { return; }

            Quaternion retationDifference = Quaternion.FromToRotation(Motor.CharacterUp, -using_gravity);
            currentRotation = retationDifference * currentRotation;
        }
        #endregion
    }

    /// <summary>
    /// 能且仅能在这里控制角色移动
    /// </summary>
    /// <param name="currentVelocity">需要被设置的玩家速度</param>
    /// <param name="deltaTime">0.02f</param>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        CurrentMovementState.UpdateVelocity(ref currentVelocity, deltaTime);

        // 实现瞬时突变速度
        if (internalVelocity.sqrMagnitude > 0f)
        {
            currentVelocity += internalVelocity;
            internalVelocity = Vector3.zero;
        }

        //重力实现
        if (!IsAnyGround && EnableGravity)
        {
            currentVelocity += using_gravity * deltaTime;
        }

        //获取当前直观速度大小
        this.currentVelocity = currentVelocity;
    }

    #region 玩家输入
    /// <summary>
    /// 获取玩家的操作输入
    /// </summary>
    /// <param name="inputs"></param>
    public void HandleInput(ref PlayerCharacterInput inputs)
    {
        /// 处理移动向量与视角向量处理
        // 玩家输入的世界坐标下移动向量
        MoveDirectionInput = new Vector3(inputs.MoveDirection.x, 0f, inputs.MoveDirection.y);

        // 玩家当前的视角在水平面投影的方向向量
        FaceDirection = Vector3.ProjectOnPlane(inputs.LookDirection, Motor.CharacterUp);

        // 已经在cam处限制向上看的角度，不会出现垂直
        if (FaceDirection == Vector3.zero)  //正在向上看
        {
            FaceDirection = new Vector3(inputs.LookDirection.x, inputs.LookDirection.z, -inputs.LookDirection.y).normalized;
        }

        CamRotation = inputs.CamRotation;

        //玩家在自身视角坐标系下的移动向量
        MoveDirection = Quaternion.LookRotation(FaceDirection) * MoveDirectionInput;

        if (inputs.TryFly && fly.allowFly)
        {
            ChangeMovementState(fly);
        }

        CurrentMovementState.HandleStateChange(ref inputs);
    }

    // Get the reference frame for the input.  The idea is to map camera fwd/right
    // to the player's XZ plane.  There is some complexity here to avoid
    // gimbal lock when the player is tilted 180 degrees relative to the input frame.
    Quaternion GetInputFrame(bool inputDirectionChanged)
    {
        // Get the raw input frame, depending of forward mode setting
        var frame = Quaternion.identity;
        switch (inputForward)
        {
            case ForwardModes.Camera: frame = CamRotation; break;
            case ForwardModes.Player: return transform.rotation;
            case ForwardModes.World: break;
        }

        // Map the raw input frame to something that makes sense as a direction for the player
        var playerUp = Motor.CharacterUp;
        var up = frame * Vector3.up;

        // Is the player in the top or bottom hemisphere?  This is needed to avoid gimbal lock,
        // but only when the player is upside-down relative to the input frame.
        const float BlendTime = 2f;
        m_TimeInHemisphere += Time.deltaTime;
        bool inTopHemisphere = Vector3.Dot(up, playerUp) >= 0;
        if (inTopHemisphere != m_InTopHemisphere)
        {
            m_InTopHemisphere = inTopHemisphere;
            m_TimeInHemisphere = Mathf.Max(0, BlendTime - m_TimeInHemisphere);
        }

        // If the player is untilted relative to the input frmae, then early-out with a simple LookRotation
        var axis = Vector3.Cross(up, playerUp);
        if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
            return frame;

        // Player is tilted relative to input frame: tilt the input frame to match
        var angle = SignedAngle(up, playerUp, axis);
        var frameA = Quaternion.AngleAxis(angle, axis) * frame;

        // If the player is tilted, then we need to get tricky to avoid gimbal-lock
        // when player is tilted 180 degrees.  There is no perfect solution for this,
        // we need to cheat it :/
        Quaternion frameB = frameA;
        if (!inTopHemisphere || m_TimeInHemisphere < BlendTime)
        {
            // Compute an alternative reference frame for the bottom hemisphere.
            // The two reference frames are incompatible where they meet, especially
            // when player up is pointing along the X axis of camera frame. 
            // There is no one reference frame that works for all player directions.
            frameB = frame * m_Upsidedown;
            var axisB = Vector3.Cross(frameB * Vector3.up, playerUp);
            if (axisB.sqrMagnitude > 0.001f)
                frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
        }
        // Blend timer force-expires when user changes input direction
        if (inputDirectionChanged)
            m_TimeInHemisphere = BlendTime;

        // If we have been long enough in one hemisphere, then we can just use its reference frame
        if (m_TimeInHemisphere >= BlendTime)
            return inTopHemisphere ? frameA : frameB;

        // Because frameA and frameB do not join seamlessly when player Up is along X axis,
        // we blend them over a time in order to avoid degenerate spinning.
        // This will produce weird movements occasionally, but it's the lesser of the evils.
        if (inTopHemisphere)
            return Quaternion.Slerp(frameB, frameA, m_TimeInHemisphere / BlendTime);
        return Quaternion.Slerp(frameA, frameB, m_TimeInHemisphere / BlendTime);
    }

    public static float SignedAngle(Vector3 v1, Vector3 v2, Vector3 up)
    {
        float num = Angle(v1, v2);
        if (Mathf.Sign(Vector3.Dot(up, Vector3.Cross(v1, v2))) < 0f)
        {
            return 0f - num;
        }

        return num;
    }

    public static float Angle(Vector3 v1, Vector3 v2)
    {
        v1.Normalize();
        v2.Normalize();
        return Mathf.Atan2((v1 - v2).magnitude, (v1 + v2).magnitude) * 57.29578f * 2f;
    }

    #endregion
}