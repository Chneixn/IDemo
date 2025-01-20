using System;
using Unity.Cinemachine;
using KinematicCharacterController;
using UnityEngine;

public enum CamState { FPS, TPS }
public enum UpModes { Player, World };

public struct CameraInput
{
    public float mouseX;
    public float mouseY;
    public float zoomValue;
    public CamState activeCamState;
}

//public interface ICameraState
//{
//    CamState CamState { get; }
//    void HandleInput(ref CameraInput inputs);
//}

/// <summary>
/// 实现玩家视角操控和操作输入传入
/// </summary>
public class CameraController : SingleMonoBase<CameraController>
{
    [SerializeField, ReadOnly] private CamState currentCamState;
    public CamState CurrentCamState => currentCamState;
    public Action<CamState> OnCamStateChange;
    public Transform CameraPosition;

    [SerializeField, Header("MouseSensitivity鼠标灵敏度")]
    private float XmouseSensitivity = 1f;

    [SerializeField]
    private float YmouseSensitivity = 1f;

    [SerializeField]
    private bool isMixXYSensitivity;
    [Tooltip("只有在isMixXYSensitivity开启时生效")]
    [SerializeField, Range(0f, 5f)]
    private float mouseMixSensitivity = 1f;

    [Header("鼠标移动系数")]
    [SerializeField, Tooltip("数值越大，视角速度越慢")]
    private float X_Mult = 25f;

    [SerializeField, Tooltip("数值越大，视角速度越慢")]
    private float Y_Mult = 25f;

    [SerializeField, ReadOnly]
    private float _mouseX;

    [SerializeField, ReadOnly]
    private float _mouseY;

    [Header("FPS Settings")]
    [SerializeField]
    private CinemachineCamera f_cam;
    private CinemachinePanTilt f_POV;

    [SerializeField]
    private float f_maxVerticalValue = 90f;

    [SerializeField]
    private float f_minVerticalValue = -80f;

    [Header("TPS Settings")]
    [SerializeField]
    private CinemachineCamera t_cam;
    private CinemachinePositionComposer t_framingTransposer;
    private CinemachinePanTilt t_camPOV;

    [SerializeField, Range(0f, 10f)]
    private float t_defaultCamDistance = 6f;

    [SerializeField, Range(0f, 10f)]
    private float t_maxCamDistance = 1f;

    [SerializeField, Range(0f, 10f)]
    private float t_minCamDistance = 6f;

    [SerializeField, Range(0f, 10f)]
    private float t_smooth_mult = 4f;

    [SerializeField, Range(0f, 10f)]
    private float t_zoomSensitivity = 1f;
    private float t_currentTargetDistance;

    [SerializeField]
    LayerMask t_renderIgnore;

    private CinemachineBrain brain;
    private Vector3 lastRawInput = new(0f, 0f, 0f);

    protected override void Awake()
    {
        base.Awake();
        CheckCamSet();
    }

    private void CheckCamSet()
    {
        brain = GetComponent<CinemachineBrain>();

        // FPS Set

        if (f_cam.TryGetComponent(out f_POV))
        {
            f_POV.TiltAxis.Range = new Vector2(f_maxVerticalValue, f_minVerticalValue);
        }
        else
            Debug.LogError("POV component not foud!");

        // TPS Set
        //t_camPOV = t_cam.GetCinemachineComponent<CinemachinePOV>();
        //t_framingTransposer = t_cam.GetCinemachineComponent<CinemachineFramingTransposer>();
        t_currentTargetDistance = t_defaultCamDistance;
    }

    public void ApplyInput(ref CameraInput inputs)
    {
        transform.parent.rotation = CameraPosition.transform.rotation;

        // 读取鼠标移动数值
        if (isMixXYSensitivity)
        {
            _mouseX += inputs.mouseX * mouseMixSensitivity / X_Mult;
            _mouseY -= inputs.mouseY * mouseMixSensitivity / Y_Mult;
        }
        else
        {
            _mouseX += inputs.mouseX * XmouseSensitivity / X_Mult;
            _mouseY -= inputs.mouseY * YmouseSensitivity / Y_Mult;
        }

        // 读取控制表的摄像机状态
        if (currentCamState != inputs.activeCamState)
            UpdateCamState(inputs.activeCamState);

        //// Get the reference frame for the input
        //var rawInput = new Vector3(inputs.mouseX, 0, inputs.mouseY);
        //var inputFrame = GetInputFrame(Vector3.Dot(rawInput, lastRawInput) < 0.8f);
        //lastRawInput = rawInput;

        switch (currentCamState)
        {
            case CamState.FPS:
                {
                    f_POV.TiltAxis.Value = _mouseY;
                    f_POV.PanAxis.Value = _mouseX;
                }
                break;
            case CamState.TPS:
                {
                    // Handle Zoom
                    inputs.zoomValue *= t_zoomSensitivity;
                    t_currentTargetDistance = Mathf.Clamp(
                        t_currentTargetDistance + inputs.zoomValue,
                        t_minCamDistance,
                        t_maxCamDistance
                    );
                    //float currentDistance = t_framingTransposer.m_CameraDistance;
                    //if (currentDistance != t_currentTargetDistance)
                    //{
                    //    float lerpedZoomValue = Mathf.Lerp(
                    //        currentDistance,
                    //        t_currentTargetDistance,
                    //        t_smooth_mult * Time.deltaTime
                    //    );
                    //    t_framingTransposer.m_CameraDistance = lerpedZoomValue;
                    //}

                    //// Handle POV
                    //t_camPOV.m_VerticalAxis.Value = _mouseY;
                    //t_camPOV.m_HorizontalAxis.Value = _mouseX;
                }
                break;
        }

        brain.ManualUpdate();
    }

    private void UpdateCamState(CamState camState)
    {
        if (camState == CamState.FPS)
        {
            f_cam.enabled = true;
            t_cam.enabled = false;
        }
        else if (camState == CamState.TPS)
        {
            t_cam.enabled = true;
            f_cam.enabled = false;
        }
        currentCamState = camState;
        OnCamStateChange?.Invoke(currentCamState);
    }

    //// Get the reference frame for the input.  The idea is to map camera fwd/right
    //// to the player's XZ plane.  There is some complexity here to avoid
    //// gimbal lock when the player is tilted 180 degrees relative to the input frame.
    //Quaternion GetInputFrame(bool inputDirectionChanged)
    //{
    //    // Get the raw input frame, depending of forward mode setting
    //    var frame = Quaternion.identity;
    //    frame = transform.rotation;

    //    // Map the raw input frame to something that makes sense as a direction for the player
    //    var playerUp = transform.up;
    //    var up = frame * Vector3.up;

    //    // Is the player in the top or bottom hemisphere?  This is needed to avoid gimbal lock,
    //    // but only when the player is upside-down relative to the input frame.
    //    const float BlendTime = 2f;
    //    m_TimeInHemisphere += Time.deltaTime;
    //    bool inTopHemisphere = Vector3.Dot(up, playerUp) >= 0;
    //    if (inTopHemisphere != m_InTopHemisphere)
    //    {
    //        m_InTopHemisphere = inTopHemisphere;
    //        m_TimeInHemisphere = Mathf.Max(0, BlendTime - m_TimeInHemisphere);
    //    }

    //    // If the player is untilted relative to the input frmae, then early-out with a simple LookRotation
    //    var axis = Vector3.Cross(up, playerUp);
    //    if (axis.sqrMagnitude < 0.001f && inTopHemisphere)
    //        return frame;

    //    // Player is tilted relative to input frame: tilt the input frame to match
    //    var angle = UnityVectorExtensions.SignedAngle(up, playerUp, axis);
    //    var frameA = Quaternion.AngleAxis(angle, axis) * frame;

    //    // If the player is tilted, then we need to get tricky to avoid gimbal-lock
    //    // when player is tilted 180 degrees.  There is no perfect solution for this,
    //    // we need to cheat it :/
    //    Quaternion frameB = frameA;
    //    if (!inTopHemisphere || m_TimeInHemisphere < BlendTime)
    //    {
    //        // Compute an alternative reference frame for the bottom hemisphere.
    //        // The two reference frames are incompatible where they meet, especially
    //        // when player up is pointing along the X axis of camera frame. 
    //        // There is no one reference frame that works for all player directions.
    //        frameB = frame * m_Upsidedown;
    //        var axisB = Vector3.Cross(frameB * Vector3.up, playerUp);
    //        if (axisB.sqrMagnitude > 0.001f)
    //            frameB = Quaternion.AngleAxis(180f - angle, axisB) * frameB;
    //    }
    //    // Blend timer force-expires when user changes input direction
    //    if (inputDirectionChanged)
    //        m_TimeInHemisphere = BlendTime;

    //    // If we have been long enough in one hemisphere, then we can just use its reference frame
    //    if (m_TimeInHemisphere >= BlendTime)
    //        return inTopHemisphere ? frameA : frameB;

    //    // Because frameA and frameB do not join seamlessly when player Up is along X axis,
    //    // we blend them over a time in order to avoid degenerate spinning.
    //    // This will produce weird movements occasionally, but it's the lesser of the evils.
    //    if (inTopHemisphere)
    //        return Quaternion.Slerp(frameB, frameA, m_TimeInHemisphere / BlendTime);
    //    return Quaternion.Slerp(frameA, frameB, m_TimeInHemisphere / BlendTime);
    //}

#if UNITY_EDITOR
    [ContextMenu("同步FPS相机位置")]
    public void SysncCamPosition()
    {
        if (f_cam != null)
        {
            transform.localPosition = f_cam.Target.TrackingTarget.position;
        }
    }

#endif
}
