using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GrabbingAbility : BaseAbility
{
    [Header("Reference引用绑定")]
    public CharacterControl cc;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    [SerializeField] private LineRenderer rope_lr;
    [SerializeField] private LineRenderer swing_lr;

    [Header("Grappling Setting参数设置")]
    public float maxGrappleDistance = 25f;
    public float grappleDelayTime = 0.25f;
    public float overshootYAxis = 2f;
    [SerializeField] private bool allowStop = false;
    [SerializeField] private bool allowGrappling = true;

    [Header("Swing摆动设置")]
    public float UAF = 4.5f;    // 弹簧频率
    public float DR = 0.5f;     // 阻尼比
    public float Mass = 1f;     // 玩家重量
    public float Length = 8f;   // 绳索长度
    public float SwingSpeed = 15f; // 摆动时最大速度

    [Header("Cooldown冷却时间")]
    public float grapplingCd = 1f;

    [Header("VFX")]
    public bool enabledRope;    // 是否绘制钩爪线条
    public bool enabledGrabblingPath = false;   // 是否绘制摇摆路径
    public GrapplingRope rope;
    public AnimationCurve affectCurve;
    private bool needDraw = false;

    [Header("Log")]
    public bool isLog = false;
    public GameObject debugSphere;

    private Vector3 grapplePoint = Vector3.zero;
    public Vector3 GrapplePoint => grapplePoint;
    private Vector3 gcache;
    private float vcache;
    private readonly SpringCalculater joint = new();

    private void Start()
    {
        gcache = cc.Gravity;
        vcache = cc.MaxAirSpeed;
        rope_lr = GetComponent<LineRenderer>();
        // swing_lr = cc.AddComponent<LineRenderer>();
        rope = new(this, rope_lr, swing_lr, affectCurve);
    }

    private void LateUpdate()
    {
        if (needDraw && enabledRope)
            rope.DrawRope();
    }

    public override void StartAbility()
    {
        if (!allowGrappling)
            return;

        if (!needDraw)
        {
            needDraw = true;    // 绘制钩爪线   
            rope.ClearDraw();   // 钩爪线初始化
        }

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit _hit, maxGrappleDistance, whatIsGrappleable))
        {
            rope.enabledGrabblingPath = enabledGrabblingPath;   // 是否绘制玩家轨迹
            grapplePoint = _hit.point;  // 获取击中点

            TimerManager.CreateTimeOut(grappleDelayTime, () => ExecutGrapple());

            // cc.TransitionToState(CharacterMovementState.Freeze);
            if (isLog)
            {
                Instantiate(debugSphere, grapplePoint, Quaternion.identity);
                Debug.Log("WillSwing!");
            }
        }
        else
        {
            grapplePoint = transform.position + transform.forward * maxGrappleDistance;
            TimerManager.CreateTimeOut(grappleDelayTime, () => { needDraw = false; rope.ClearDraw(); allowGrappling = true; });
        }
        allowGrappling = false;
        if (isLog) Debug.Log("TrySwing!");
    }

    public void ExecutGrapple()
    {
        //计算在抓钩过程中是否需要上升高度，当玩家低于抓钩点时，抓钩轨迹最高点即为抓钩点
        // float grapplePointRelativeYPos = grapplePoint.y - cc.transform.position.y;
        // float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        // if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        // cc.ForceUnground(); //使玩家下一帧能脱离地面
        gcache = cc.Gravity;        //缓存重力
        cc.Gravity = Vector3.zero;  //关闭重力

        cc.TransitionToState(CharacterMovementState.Jumping);
        vcache = cc.MaxAirSpeed;    //缓存空速
        cc.MaxAirSpeed = 20f;
        // JumpingToPosition(grapplePoint, highestPointOnArc);
        // cc.MaxAirSpeed = JumpingToPosition(grapplePoint, highestPointOnArc).magnitude;  //提高空速为跳跃速度

        // 引用传递
        cc.sc = joint;

        // 参数赋值
        joint.TargetPosition = grapplePoint;
        joint.UAF = UAF;
        joint.DR = DR;
        joint.Mass = Mass;
        joint.Length = Length;
        joint.gravity = gcache;
        joint.active = true;

        // // 修改空速
        // cc.MaxAirSpeed = SwingSpeed;
        allowStop = true;

        if (isLog) Debug.Log("Swing!");
    }

    public override void StopAbility()
    {
        if (!allowStop) return;

        allowStop = false;
        needDraw = false;
        cc.Gravity = gcache;    //恢复重力
        cc.MaxAirSpeed = vcache;    //恢复空速
        rope.ClearDraw();
        joint.active = false;   // 停止弹簧计算器
        TimerManager.CreateTimeOut(grappleDelayTime, () => { allowGrappling = true; });

        if (isLog) Debug.Log("StopSwing!");
    }

    /// <summary>
    /// 将玩家跳跃至指定地点
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <param name="trajectoryHeight"></param>
    /// <returns>当前玩家速度</returns>
    public Vector3 JumpingToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        Vector3 r = CalculateJumpVelocity(cc.transform.position, targetPosition, trajectoryHeight) - cc.CurrentSpeed;
        cc.InternalVelocity = r;
        if (isLog) Debug.Log("InternalVelocity: " + r);
        return r;
    }

    /// <summary>
    /// 计算完成抛物线运动所需初速度
    /// </summary>
    /// <param name="startPoint"></param>
    /// <param name="endPoint"></param>
    /// <param name="trajectoryHeight"></param>
    /// <returns>初速度</returns>
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = gcache.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
}
