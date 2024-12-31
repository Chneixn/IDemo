using System;
using System.Collections.Generic;
using KinematicCharacterController;
using UnityEngine;
using MState = CharacterMovementState;

public enum CharacterMovementState
{
    Freeze,
    Idle,
    Walking,
    Running,
    Jumping,
    Crouching,
    Sliding,
    Swing,
    WallRunning,
    Swimming,
    InAir,
    Fly,
    ClimbingLadder
}

public struct PlayerCharacterInput
{
    public Vector2 moveDirection;
    public Vector3 lookDirection;
    public bool tryJump;
    public bool tryRun;
    public bool tryCrouch;
    public bool trySlide;
    public bool tryFly;
    public bool tryDodge; //闪避
}

public class CharacterControl : MonoBehaviour, ICharacterController
{
    private KinematicCharacterMotor motor;
    private PlayerBodyAnimation bodyAnimation;

    #region Public
    public Vector3 MoveDirection => _moveDirection;
    public Vector3 CurrentSpeed => currentVelocity;
    public bool IsStableGround => motor.GroundingStatus.IsStableOnGround;
    public bool IsAnyGround => motor.GroundingStatus.FoundAnyGround;
    public MState CurrentMovementState => currentMovementState;
    #endregion

    [Header("相机绑定")]
    [SerializeField]
    private Transform characterCamPos;
    [SerializeField, ReadOnly]
    private float camPosCache; //缓存相机原始Y坐标
    [SerializeField, ReadOnly]
    private float camTargetPos; //缓存相机即将移动到的Y坐标
    [SerializeField, ReadOnly]
    private float crouch_cam_velocity; //用于平滑摄像机移动
    [SerializeField, ReadOnly]
    private bool camPosUpdateRequested = false;

    [Header("MovementState移动状态")]
    [SerializeField, ReadOnly] private MState currentMovementState;
    public Action<MState> OnMovementStateChanged;

    [Header("DirectionInfo方向信息")]
    [SerializeField, ReadOnly] private Quaternion _camRotation;
    [SerializeField, ReadOnly] private Vector3 _moveDirection;
    [SerializeField, ReadOnly] private Vector3 _lookDirection;
    [SerializeField, ReadOnly] private Vector3 _moveDirectionInput;

    #region Setting
    #region Speed
    [Header("MovementSpeed移动速度")]
    [SerializeField]
    [ReadOnly]
    private Vector3 currentVelocity;


    [SerializeField]
    [ReadOnly]
    private Vector3 targetMovementVelocity;

    [SerializeField]
    private float maxSpeed = 0f; //当前最大速度

    [SerializeField]
    private float walkSpeed = 4f; //行走速度

    [SerializeField]
    private float freeFlyCamSpeed = 10f; //自由视角的移动速度

    [SerializeField]
    private float speedChangeTime = 3f; //改变速度所需时间

    [SerializeField]
    private float stableMovementSharpness = 15f; //速度改变的敏锐度

    [SerializeField]
    private float mult_speed = 1f; //移动速度的倍率
    private Vector3 _internalVelocity = Vector3.zero; //下一帧的附加突变速度

    // [SerializeField]
    // private float orientationSharpnes = 10f; //转向的敏锐度

    public Vector3 InternalVelocity
    {
        set { _internalVelocity = value; }
    }
    public float Mult_Speed
    {
        get { return mult_speed; }
        set { mult_speed = value; }
    }

    [Header("RunningSet奔跑")]
    [SerializeField]
    private float runSpeed = 6f; //奔跑速度
    private bool _allowToRun = true;

    #endregion
    #region Jump

    [Header("JumpSet跳跃")]
    [SerializeField] private float jumpSpeed = 8f;

    [SerializeField] private float jumpCooldown = 0.25f;

    [SerializeField, ReadOnly] private Vector3 jumpDirection;

    [SerializeField, ReadOnly] float heightCounter = 0f;

    [SerializeField, ReadOnly] private bool _readyToJump = true;

    [SerializeField, ReadOnly] private Vector3 groundLeavePosition;
    private Timer airTimer;

    #endregion
    #region Crouch

    [Header("CrouchSet下蹲")]
    [SerializeField] private float crouchSpeed = 2f; //下蹲后的移动速度

    [SerializeField] private float crouchTime = 0.8f; //下蹲动作完成时间

    [SerializeField] private float crouchCoolDown = 0.8f; //下蹲操作的冷却时间

    [SerializeField] private float crouchCamHeight = 0.5f; //下蹲后相机的高度相对于原本高度的比例

    [SerializeField][ReadOnly] private bool _readyToCrouch = true;
    private readonly Collider[] _probedColliders = new Collider[8]; //取消下蹲前的碰撞体检测缓存

    #endregion
    #region Slide
    [Header("SlideSet滑铲")]
    [SerializeField] private float slidingSpeed = 8f;

    // [SerializeField] private float slidingCoolTime = 0.2f;

    // [SerializeField] private float slideEndDely = 1.5f;

    [SerializeField][ReadOnly] private Vector3 _directionCache;

    [SerializeField][ReadOnly] private bool _readyToSlide = false;

    #endregion

    #region WallRun

    [Header("WallRunning参数")]
    [SerializeField] private bool enableWallRun = true;
    [SerializeField] private LayerMask runableWall;
    [SerializeField] private float wallRunSpeed = 200f;
    // [SerializeField] private float maxWallRunTime = 1.5f;
    [SerializeField] private float dragToWall = 0.5f;
    private bool wallRunEnter = false;

    [Header("Detection")]
    [SerializeField] private float wallCheckDistance = 0.7f;
    [SerializeField][ReadOnly] private bool wallAtLeft;
    [SerializeField][ReadOnly] private bool wallAtRight;
    [SerializeField][ReadOnly] private Vector3 usingWallNormal;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;

    [Header("WallRunDebug")]
    [SerializeField] private bool isWallRunDebug;
    // 预制体
    [SerializeField] private GameObject debugSphere_pre;
    [SerializeField] private GameObject debugCube_pre;
    // 实体引用
    private GameObject debugSphere;
    private GameObject debugCube;

    #endregion

    #region Ladder

    [Header("InLadder爬梯")]
    [SerializeField]
    private Transform _ladderBottomPos;

    [SerializeField]
    private float _ladderHeight;
    private bool _readyToClimb = true;
    private float _climbSpeed = 3f;
    #endregion

    #region Gravity
    [Header("Gravity重力")]
    [SerializeField]
    private bool enableGravity = true;
    [SerializeField, ReadOnly]
    private bool shouldUseGravity = false;
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
    public float MaxAirSpeed = 4f;
    public SpringCalculater sc;

    #endregion

    [Header("Colliders碰撞体")]
    [SerializeField]
    private List<Collider> IgnoredColliders = new(); //忽视碰撞体的列表

    [SerializeField] private Transform meshRoot;

    #region 胶囊体信息

    public struct CapsuleInfo
    {
        public float radius;
        public float height;
        public float yOffset;
    }
    // 存储初始与下顿时的胶囊体数据
    private CapsuleInfo initialCapusle;
    private CapsuleInfo crouchCapsule;

    #endregion

    #endregion

    #region 初始化配置
    private void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
        motor.CharacterController = this;
        if (meshRoot == null) Debug.Log("Mesh Root not set!");

        bodyAnimation = GetComponent<PlayerBodyAnimation>();
        //动画脚本初始化
        bodyAnimation.Init(this);
        ResetState();
    }

    /// <summary>
    /// 初始化移动状态
    /// </summary>
    private void ResetState()
    {
        //胶囊体信息
        CapsuleCollider collider = GetComponent<CapsuleCollider>();
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

        camPosCache = characterCamPos.localPosition.y;
        using_gravity = default_gravity;
        currentMovementState = MState.Walking;
    }

    private void Start()
    {
        airTimer = TimerManager.CreateTimer();

        TransitionToState(MState.Idle);
    }

    #region ChangeStateFSM
    /// <summary>
    /// 跳转下一个状态
    /// </summary>
    public void TransitionToState(MState nextState)
    {
        MState lastState = currentMovementState;
        OnStateExit(lastState, nextState);
        currentMovementState = nextState;
        OnStateEnter(nextState, lastState);
        OnMovementStateChanged?.Invoke(nextState);
        bodyAnimation.UpdateAnimations(nextState, lastState);
    }

    /// <summary>
    /// 进入新状态前
    /// </summary>
    public void OnStateEnter(MState nextState, MState lastState)
    {
        switch (nextState)
        {
            case MState.Freeze:
                {
                    enableGravity = false;
                }
                break;
            case MState.Idle:
                {
                    break;
                }
            case MState.Walking:
                break;
            case MState.Running:
                {
                    maxSpeed = runSpeed;
                }
                break;
            case MState.Jumping:
                {
                    _readyToCrouch = false;
                    _readyToJump = false;
                }
                break;
            case MState.Crouching:
                {
                    //状态更新
                    _readyToCrouch = false;
                    _readyToJump = false;
                    _readyToSlide = false;
                    //下蹲CD
                    TimerManager.CreateTimeOut(crouchCoolDown, () => _readyToCrouch = true);

                    //更新为下蹲时胶囊体
                    motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.yOffset);
                    //更新摄像机位置
                    camTargetPos = camPosCache * crouchCamHeight;
                    camPosUpdateRequested = true;
                }
                break;
            case MState.Sliding:
                {
                    maxSpeed = slidingSpeed;
                    _allowToRun = false;
                    _readyToSlide = false;
                    _readyToCrouch = false;

                    //更新胶囊体
                    motor.SetCapsuleDimensions(initialCapusle.radius, initialCapusle.height, initialCapusle.yOffset);
                    //更新摄像机位置
                    camTargetPos = camPosCache * crouchCamHeight;
                    camPosUpdateRequested = true;
                }
                break;
            case MState.WallRunning:
                {
                    enableGravity = false;
                    wallRunEnter = true;
                    break;
                }
            case MState.Swimming:
                break;
            case MState.InAir:
                {
                    if (!airTimer.IsActive)
                    {
                        airTimer.StartCounting();
                        groundLeavePosition = transform.position;
                    }
                }
                break;
            case MState.Fly:
                {
                    motor.SetCapsuleCollisionsActivation(false);
                    motor.SetMovementCollisionsSolvingActivation(false);
                    motor.SetGroundSolvingActivation(false);
                    break;
                }

            case MState.ClimbingLadder:
                {
                    motor.SetMovementCollisionsSolvingActivation(false);
                    motor.SetGroundSolvingActivation(false);
                    break;
                }
            default:
                break;
        }
    }

    /// <summary>
    /// 退出状态前
    /// </summary>
    public void OnStateExit(MState nowState, MState nextState)
    {
        switch (nowState)
        {
            case MState.Freeze:
                {
                    enableGravity = true;
                }
                break;
            case MState.Walking:
                break;
            case MState.Running:
                break;
            case MState.Jumping:
                {
                    _readyToCrouch = true;
                }
                break;
            case MState.Crouching:
                {
                    if (nextState == MState.Crouching)
                        return;

                    //恢复为初始胶囊体
                    motor.SetCapsuleDimensions(initialCapusle.radius, initialCapusle.height, initialCapusle.yOffset);

                    if (motor.CharacterCollisionsOverlap(motor.TransientPosition, motor.TransientRotation, _probedColliders) > 0)
                    {
                        // //如果胶囊体有重叠碰撞，进入下蹲状态
                        // motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.center.y);
                        TransitionToState(MState.Crouching);
                    }
                    else
                    {
                        //恢复摄像机位置
                        camTargetPos = camPosCache;
                        camPosUpdateRequested = true;
                        _readyToSlide = true;
                        //状态恢复
                        _readyToJump = true;
                    }
                }
                break;
            case MState.Sliding:
                {
                    _allowToRun = true;
                    _readyToSlide = true;
                    _readyToCrouch = true;

                    //恢复为初始胶囊体
                    motor.SetCapsuleDimensions(initialCapusle.radius, initialCapusle.height, initialCapusle.yOffset);

                    if (motor.CharacterCollisionsOverlap(motor.TransientPosition, motor.TransientRotation, _probedColliders) > 0)
                    {
                        // //如果胶囊体有重叠碰撞，恢复为下蹲时胶囊体，并进入下蹲状态
                        // motor.SetCapsuleDimensions(crouchCapsule.radius, crouchCapsule.height, crouchCapsule.center.y);
                        TransitionToState(MState.Crouching);
                    }
                    else
                    {
                        //恢复摄像机位置
                        camTargetPos = camPosCache;
                        camPosUpdateRequested = true;
                    }
                }
                break;
            case MState.WallRunning:
                {
                    enableGravity = true;
                    break;
                }
            case MState.Swimming:
                break;
            case MState.InAir:
                {
                    if (airTimer.IsActive)
                    {
                        heightCounter = airTimer.PauseTimer();
                    }
                }
                break;

            case MState.Idle:
                {
                    break;
                }
            case MState.Fly:
                {
                    motor.SetCapsuleCollisionsActivation(true);
                    motor.SetMovementCollisionsSolvingActivation(true);
                    motor.SetGroundSolvingActivation(true);
                    break;
                }
            case MState.ClimbingLadder:
                {
                    motor.SetMovementCollisionsSolvingActivation(true);
                    motor.SetGroundSolvingActivation(true);
                    break;
                }
            default:
                break;
        }
    }
    #endregion

    //public PlayerBaseState currentPlayerState;
    //public void TransitionToState(PlayerBaseState newState)
    //{
    //    PlayerBaseState nowState = currentPlayerState; ;
    //    currentPlayerState.OnStateExit(currentMovementState, nextState);
    //    currentPlayerState = newState;
    //    currentPlayerState.OnStateEnter(nextState, nowState);
    //}
    #endregion

    public void AfterCharacterUpdate(float deltaTime)
    {
        if (camPosUpdateRequested)
        {
            // 将摄像机位置平滑移动至下蹲后应有高度位置
            float height = Mathf.SmoothDamp(characterCamPos.localPosition.y, camTargetPos, ref crouch_cam_velocity, crouchTime);
            characterCamPos.localPosition = new Vector3(0f, height, 0f);
            if (characterCamPos.localPosition.y == camTargetPos)
                camPosUpdateRequested = false;
        }
    }

    public void BeforeCharacterUpdate(float deltaTime) { }

    public bool IsColliderValidForCollisions(Collider coll)
    {
        //处理碰撞：在列表中的物体不计算物体碰撞
        if (IgnoredColliders.Contains(coll))
        {
            return false;
        }
        return true;
    }

    public void OnDiscreteCollisionDetected(Collider hitCollider) { }

    public void OnGroundHit(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport
    )
    { }

    public void OnMovementHit(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        ref HitStabilityReport hitStabilityReport
    )
    {
        //Debug.Log("OnMovementHit");
    }

    public void PostGroundingUpdate(float deltaTime)
    {
        // Handle landing and leaving ground
        if (IsStableGround && !motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
        else if (!IsStableGround && motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLeaveStableGround();
        }
    }

    #region 落地和离地回调
    private void OnLanded()
    {
        TransitionToState(MState.Idle);
    }

    private void OnLeaveStableGround() { }
    #endregion

    public void ProcessHitStabilityReport(
        Collider hitCollider,
        Vector3 hitNormal,
        Vector3 hitPoint,
        Vector3 atCharacterPosition,
        Quaternion atCharacterRotation,
        ref HitStabilityReport hitStabilityReport
    )
    { }

    public void OnCameraUpdate(float deltaTime, Vector3 camForward)
    {
        camForward = Vector3.ProjectOnPlane(camForward, motor.CharacterUp);
        //Frame Perfect rotation实现，在更新相机旋转的同一帧，更新角色模型子物体的旋转
        meshRoot.rotation = Quaternion.LookRotation(camForward, motor.CharacterUp);
    }

    /// <summary>
    /// 能且仅能在这里控制角色的转向
    /// </summary>
    /// <param name="currentRotation">玩家应有的rotation</param>
    /// <param name="deltaTime">当前KCC刷新时间，默认与fixedUpdate同步为0.02s</param>
    public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
    {
        switch (currentMovementState)
        {
            case MState.Freeze:
                break;
            case MState.Idle:
                {
                    //角色旋转直接与摄像机看向的方向同步
                    if (_lookDirection != Vector3.zero)
                        currentRotation = Quaternion.LookRotation(_lookDirection, motor.CharacterUp);

                    //考虑转向延迟
                    //Vector3 reorientedDirection = _lookDirection;
                    //Quaternion targetRotation = Quaternion.LookRotation(reorientedDirection, motor.CharacterUp);
                    //currentRotation = Quaternion.Lerp(currentRotation, targetRotation, orientationSharpnes);
                }
                break;
            case MState.Running:
                goto case MState.Idle;
            case MState.Jumping:
                goto case MState.Idle;
            case MState.Crouching:
                goto case MState.Idle;
            case MState.Sliding:
                goto case MState.Idle;
            case MState.WallRunning:
                goto case MState.Idle;
            case MState.InAir:
                goto case MState.Idle;
            case MState.Walking:
                goto case MState.Idle;
            case MState.Fly:
                goto case MState.Idle;
            case MState.Swimming:
                break;
            case MState.ClimbingLadder:
                {
                    break;
                }
            default:
                break;
        }

        if (_walkOnAnyGround)
        {
            float angleBetweenUpDirections = Vector3.Angle(-using_gravity, motor.CharacterUp);
            //float angleThreshold = 0.001f;

            if (angleBetweenUpDirections < 0.001f) { return; }

            Quaternion retationDifference = Quaternion.FromToRotation(motor.CharacterUp, -using_gravity);
            currentRotation = retationDifference * currentRotation;
            //currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -gravity) * currentRotation;
        }
    }

    #region UpdateVelocity
    /// <summary>
    /// 能且仅能在这里控制角色移动
    /// </summary>
    /// <param name="currentVelocity">需要被设置的玩家速度</param>
    /// <param name="deltaTime">0.02f</param>
    public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        shouldUseGravity = !IsAnyGround;

        switch (currentMovementState)
        {
            case MState.Freeze:
                {
                    // 停止速度，彻底冻结
                    currentVelocity = motor.CharacterForward * maxSpeed;
                    break;
                }
            case MState.Idle:
                {
                    goto case MState.Running;
                }
            case MState.Walking:
                {
                    goto case MState.Running;
                }
            case MState.Crouching:
                {
                    goto case MState.Running;
                }
            case MState.Sliding:
                {
                    //在滑铲持续时，锁定角色移动方向为滑铲开始时方向
                    _moveDirection = _directionCache;

                    // //禁止在上坡时滑铲滑铲
                    // if (currentVelocity.y > 0.1f)
                    //     TransitionToState(MState.Idle);

                    // 角色与地面法线角度越小则降速越快
                    float angle = Vector3.Angle(motor.CharacterUp, motor.GroundingStatus.GroundNormal);
                    maxSpeed = Mathf.Lerp(slidingSpeed, crouchSpeed, deltaTime * (1 - angle * angle / 180f));

                    goto case MState.Running;
                }
            case MState.Running:
                {
                    //获得上一次的速度的方向，GetDirectionTangentToSurface可以求出玩家移动向量相对于地表法线的切线
                    //motor.GroundingStatus.GroundNormal传出当前角色所在地面法线
                    currentVelocity =
                        motor.GetDirectionTangentToSurface(
                            currentVelocity,
                            motor.GroundingStatus.GroundNormal
                        ) * currentVelocity.magnitude;

                    //计算玩家的速度向量（实现在非水平平面以准确速度移动）
                    //1.需要移动的方向叉乘玩家向上方向，求出需要移动的方向的右侧向量
                    Vector3 inputRight = Vector3.Cross(_moveDirection, motor.CharacterUp);
                    //2.用地表法线方向叉乘右侧向量，根据当前的地表状况重新计算出移动方向的前方，再乘玩家移动向量的归一化，得出考虑了地表情况的实际移动向量
                    Vector3 reorientedInput =
                        Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized
                        * _moveDirection.magnitude;
                    //3.实际移动向量乘上当前移动速度，得出当前被设置的玩家速度
                    targetMovementVelocity = reorientedInput * maxSpeed;
                    //4.使用Lerp插值计算，使当前速度平滑变为目标速度
                    //currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, speedChangeTime);
                    currentVelocity = Vector3.Lerp(
                        currentVelocity,
                        targetMovementVelocity,
                        1 - Mathf.Exp(-stableMovementSharpness * deltaTime)
                    );

                    if (shouldUseGravity)
                    {
                        TransitionToState(MState.InAir);
                    }
                }
                break;
            case MState.Jumping:
                {
                    //设置跳跃方向
                    jumpDirection = motor.CharacterUp;
                    //允许在非水平地面上沿地面法线跳跃，即某些移动平台或斜坡
                    if (
                        motor.GroundingStatus.FoundAnyGround
                        && !motor.GroundingStatus.IsStableOnGround
                    )
                    {
                        jumpDirection = motor.GroundingStatus.GroundNormal;
                    }

                    //在下一帧中脱离地面(否则KCC会将角色拉回地面)
                    motor.ForceUnground(0.1f);

                    //输出附加跳跃速度后的正确速度(原来的速度带有在角色上方的分速度，使用投影方法将其减去以获得正确的跳跃速度)
                    currentVelocity += (jumpDirection * jumpSpeed);// - Vector3.Project(currentVelocity, motor.CharacterUp);

                    //在jumpCooldown的时间后重置跳跃允许状态
                    TimerManager.CreateTimeOut(
                        jumpCooldown,
                        () =>
                        {
                            _readyToJump = true;
                        }
                    );
                    TransitionToState(MState.InAir);
                }
                break;
            case MState.InAir:
                {
                    // 实现摆动时受限运动
                    if (sc.active)
                    {
                        sc.SelfPosition = transform.position;
                        sc.SelfVelocity = currentVelocity;
                        sc.UpdateValue(deltaTime);
                        currentVelocity = sc.Value;
                    }

                    //空中移动
                    if (_moveDirection.sqrMagnitude > 0.01f)
                    {
                        targetMovementVelocity = _moveDirection * MaxAirSpeed;
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, deltaTime * speedChangeTime);
                    }
                }
                break;
            case MState.WallRunning:
                {
                    // 获取在墙上奔跑时前进方向
                    Vector3 wallForward = Vector3.Cross(usingWallNormal, motor.CharacterUp).normalized;

                    // 依据移动方向重新定位墙跑的前进方向
                    if ((_moveDirection - wallForward).magnitude > (_moveDirection - -wallForward).magnitude)
                        wallForward = -wallForward;

                    if (isWallRunDebug) debugCube.transform.forward = wallForward;

                    // 进入墙跑瞬间重置速度
                    if (wallRunEnter)
                    {
                        currentVelocity = wallForward * wallRunSpeed;
                        wallRunEnter = !wallRunEnter;
                    }
                    else targetMovementVelocity = wallForward * wallRunSpeed;

                    // 赋予玩家贴近墙壁的方向
                    targetMovementVelocity += -usingWallNormal * dragToWall;

                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, deltaTime * speedChangeTime);
                }
                break;
            case MState.Swimming:
                break;
            case MState.Fly:
                {
                    targetMovementVelocity = _moveDirection * freeFlyCamSpeed;
                    currentVelocity = Vector3.Lerp(
                        currentVelocity,
                        targetMovementVelocity,
                        speedChangeTime
                    );

                    break;
                }
            case MState.ClimbingLadder:
                {
                    shouldUseGravity = false;
                    currentVelocity = Vector3.zero;
                    int isUp = _moveDirection.x > 0f ? 1 : -1;
                    targetMovementVelocity = isUp * _climbSpeed * motor.CharacterUp;
                    currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, deltaTime);
                    break;
                }
            default:
                break;
        }

        // 实现瞬时突变速度
        if (_internalVelocity.sqrMagnitude > 0f)
        {
            currentVelocity += _internalVelocity;
            _internalVelocity = Vector3.zero;
        }

        //重力实现
        if (shouldUseGravity && enableGravity)
        {
            currentVelocity += using_gravity * deltaTime;
        }

        //获取当前直观速度大小
        this.currentVelocity = currentVelocity;
    }
    #endregion

    #region 玩家输入
    /// <summary>
    /// 获取玩家的操作输入
    /// </summary>
    /// <param name="inputs"></param>
    public void ApplyInput(ref PlayerCharacterInput inputs)
    {
        #region 移动向量与视角向量处理
        //玩家输入的世界坐标下移动向量
        _moveDirectionInput = new Vector3(inputs.moveDirection.x, 0f, inputs.moveDirection.y);

        //玩家当前的视角在水平面投影的方向向量
        _lookDirection = Vector3.ProjectOnPlane(inputs.lookDirection, motor.CharacterUp).normalized;

        //已经在cam处限制向上看的角度，不会出现垂直
        //if (_lookDirection == Vector3.zero)  //正在向上看
        //{
        //    _lookDirection = new Vector3(inputs.lookDirection.x, inputs.lookDirection.z, -inputs.lookDirection.y).normalized;
        //}

        //玩家在自身视角坐标系下的移动向量
        _moveDirection = Quaternion.LookRotation(_lookDirection) * _moveDirectionInput;

        #endregion

        switch (currentMovementState)
        {
            case MState.Freeze:
                {
                    maxSpeed = 0;
                }
                break;
            case MState.Idle:
                {
                    maxSpeed = 0f;

                    if (_moveDirection.sqrMagnitude > 0.01f) // Walk
                    {
                        TransitionToState(MState.Walking);
                    }
                    else if (inputs.tryCrouch && IsStableGround && _readyToCrouch) // Crouch
                    {
                        TransitionToState(MState.Crouching);
                    }
                    else if (inputs.tryJump && _readyToJump && IsAnyGround) // Jump
                    {
                        TransitionToState(MState.Jumping);
                    }
                    else if (inputs.tryFly)
                    {
                        TransitionToState(MState.Fly);
                    }
                }
                break;
            case MState.Walking:
                {
                    maxSpeed = walkSpeed;

                    if (_moveDirection == Vector3.zero)
                    {
                        TransitionToState(MState.Idle);
                    }
                    else if (inputs.tryRun && _allowToRun && inputs.moveDirection.y > 0.1f)
                    {
                        TransitionToState(MState.Running);
                    }
                    else if (inputs.tryCrouch && IsStableGround && _readyToCrouch)
                    {
                        TransitionToState(MState.Crouching);
                    }
                    else if (inputs.tryJump && _readyToJump && IsAnyGround)
                    {
                        TransitionToState(MState.Jumping);
                    }
                    else if (inputs.tryFly)
                    {
                        TransitionToState(MState.Fly);
                    }
                }
                break;
            case MState.Running:
                {
                    maxSpeed = runSpeed;

                    if (_moveDirection == Vector3.zero)
                    {
                        TransitionToState(MState.Idle);
                    }
                    //else if (inputs.moveDirection.y < -0.1f)
                    //{
                    //    TransitionToState(MState.Walking);
                    //}
                    else if (inputs.tryCrouch && IsStableGround)
                    {
                        if (currentVelocity.magnitude > walkSpeed && _readyToSlide)
                        {
                            //缓存滑铲开始时方向向量
                            _directionCache = _moveDirection;
                            TransitionToState(MState.Sliding);
                        }
                        else if (_readyToCrouch)
                        {
                            TransitionToState(MState.Crouching);
                        }
                    }
                    else if (inputs.tryJump && _readyToJump && IsAnyGround)
                    {
                        TransitionToState(MState.Jumping);
                    }
                    else if (inputs.tryFly)
                    {
                        TransitionToState(MState.Fly);
                    }
                }
                break;
            case MState.Jumping:
                // 过渡状态
                break;
            case MState.Crouching:
                {
                    maxSpeed = crouchSpeed;

                    if (!inputs.tryCrouch)
                    {
                        if (_moveDirection == Vector3.zero)
                            TransitionToState(MState.Idle);
                        else
                            TransitionToState(MState.Walking);
                    }
                }
                break;
            case MState.Sliding:
                {
                    if (_moveDirection == Vector3.zero)
                    {
                        TransitionToState(MState.Idle);
                    }
                    // else if (maxSpeed <= crouchSpeed)
                    // {
                    //     TransitionToState(MState.Crouching);
                    // }
                    else if (!inputs.tryCrouch && maxSpeed <= runSpeed)
                    {
                        if (inputs.tryRun)
                            TransitionToState(MState.Running);
                        else
                            TransitionToState(MState.Walking);
                    }
                    else if (inputs.tryJump && _readyToJump && IsAnyGround)
                    {
                        TransitionToState(MState.Jumping);
                    }
                }
                break;
            case MState.Swimming:
                break;
            case MState.InAir:
                {
                    if (_moveDirection.sqrMagnitude > 0.01f && enableWallRun)
                    {
                        Vector3 detcetPos = transform.position + initialCapusle.yOffset * motor.CharacterUp;
                        if (CheckForWall(detcetPos, motor.CharacterRight))
                        {
                            // 当移动方向与检测到的允许墙跑的墙法线反向夹角小于60°时，认为想要墙跑
                            if (Vector3.Angle(_moveDirection, -GetWallNormal(detcetPos)) < 60f)
                            {
                                TransitionToState(MState.WallRunning);
                            }
                        }
                    }
                    else if (inputs.tryFly)
                    {
                        TransitionToState(MState.Fly);
                    }
                }
                break;
            case MState.WallRunning:
                {
                    if (_moveDirection.sqrMagnitude > 0.01f)
                    {
                        Vector3 detcetPos = transform.position + initialCapusle.yOffset * motor.CharacterUp;
                        if (inputs.tryJump && CheckForWall(detcetPos, motor.CharacterUp))
                        {
                            // 增加一个相对于墙反向的瞬时速度
                            InternalVelocity = GetWallNormal(detcetPos) * jumpSpeed;
                            TransitionToState(MState.Jumping);
                        }
                        else if (CheckForWall(detcetPos, motor.CharacterRight)
                            && Vector3.Angle(_moveDirection, -GetWallNormal(detcetPos)) < 60f)
                        {
                            // 当移动方向与检测到的允许墙跑的墙法线反向夹角小于60°时，认为想要墙跑

                        }
                        // 没有墙壁可供使用
                        else TransitionToState(MState.InAir);
                    }
                    else TransitionToState(MState.InAir);
                }
                break;
            case MState.Fly:
                {
                    // 移动方向与玩家视角方向相同
                    _moveDirection = inputs.lookDirection + _moveDirectionInput;
                    if (!inputs.tryFly)
                    {
                        TransitionToState(MState.Walking);
                    }
                }
                break;
            case MState.ClimbingLadder:
                {
                    _moveDirection = new(0f, _moveDirectionInput.x, 0f);

                    if (inputs.tryJump && _readyToJump)
                    {
                        TransitionToState(MState.Jumping);
                    }
                }
                break;
            default:
                break;
        }
    }

    #region InLabber爬梯状态

    public bool StartClimbLabber(Transform ladderBottomPos, float ladderHeight)
    {
        if (_readyToClimb)
        {
            this._ladderBottomPos = ladderBottomPos;
            this._ladderHeight = ladderHeight;
            // 判断玩家位置是否比梯子高
            if (transform.position.y - ladderBottomPos.position.y > ladderHeight)
                return false;
            // 移动玩家至梯子
            motor.MoveCharacter(
                new(ladderBottomPos.position.x, transform.position.y, ladderBottomPos.position.z)
            );
            _readyToClimb = false;
            TransitionToState(MState.ClimbingLadder);
            return true;
        }
        return false;
    }
    #endregion

    #endregion

    public void ForceUnground()
    {
        motor.ForceUnground();
    }

    #region WallRun
    /// <summary>
    /// 检测左右是否有可用于墙跑的墙体
    /// </summary>
    /// <param name="detectPos"></param>
    /// <param name="characterRight"></param>
    /// <returns>是否有</returns>
    public bool CheckForWall(Vector3 detectPos, Vector3 characterRight)
    {
        wallAtRight = Physics.Raycast(detectPos, characterRight, out rightWallhit, wallCheckDistance, runableWall);
        wallAtLeft = Physics.Raycast(detectPos, -characterRight, out leftWallhit, wallCheckDistance, runableWall);

        return wallAtLeft || wallAtRight;
    }

    public Vector3 GetWallNormal(Vector3 selfPosition)
    {
        if (wallAtLeft && wallAtRight)
        {
            if (Vector3.Distance(leftWallhit.point, selfPosition) > Vector3.Distance(rightWallhit.point, selfPosition))
            {
                usingWallNormal = rightWallhit.normal;
            }
            else usingWallNormal = leftWallhit.normal;
        }
        else usingWallNormal = wallAtRight ? rightWallhit.normal : leftWallhit.normal;

        return usingWallNormal;
    }

    #endregion

    /// <summary>
    /// 脱离卡死
    /// </summary>
    [ContextMenu("脱离卡死")]
    public void GetOutOfJam()
    {
        do
        {
            motor.SetPositionAndRotation(new(transform.position.x, transform.position.y + 1f, transform.position.z),
                            Quaternion.identity);
        } while (motor.CharacterCollisionsOverlap(motor.TransientPosition, motor.TransientRotation, _probedColliders) > 0);
        Debug.Log("Done!");
    }
}
