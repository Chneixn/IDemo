using System;
using Unity.Cinemachine;
using KinematicCharacterController;
using UnityEngine;

public enum CamState
{
    FPS,
    TPS
}

public struct CameraInput
{
    public float mouseX;
    public float mouseY;
    public float zoomValue;
    public CamState activeCamState;
}

/// <summary>
/// 实现玩家视角操控和操作输入传入
/// </summary>
public class CameraController : SingleMonoBase<CameraController>
{
    [SerializeField, ReadOnly] private CamState currentCamState;
    public CamState CurrentCamState => currentCamState;
    public Action<CamState> OnCamStateChange;

    [Header("MouseSensitivity鼠标灵敏度")]
    [SerializeField]
    private float XmouseSensitivity = 1f;

    [SerializeField]
    private float YmouseSensitivity = 1f;

    [Range(0f, 5f)]
    [Tooltip("只有在isMixXYSensitivity开启时生效")]
    [SerializeField]
    private float mouseMixSensitivity = 1f;

    [SerializeField]
    private bool isMixXYSensitivity;

    [Header("鼠标移动系数")]
    [Tooltip("数值越大，视角速度越慢")]
    [SerializeField]
    private float X_Mult = 25f;

    [Tooltip("数值越大，视角速度越慢")]
    [SerializeField]
    private float Y_Mult = 25f;

    [SerializeField]
    [ReadOnly]
    private float _mouseX;

    [SerializeField]
    [ReadOnly]
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

    [SerializeField]
    [Range(0f, 10f)]
    private float t_defaultCamDistance = 6f;

    [SerializeField]
    [Range(0f, 10f)]
    private float t_maxCamDistance = 1f;

    [SerializeField]
    [Range(0f, 10f)]
    private float t_minCamDistance = 6f;

    [SerializeField]
    [Range(0f, 10f)]
    private float t_smooth_mult = 4f;

    [SerializeField]
    [Range(0f, 10f)]
    private float t_zoomSensitivity = 1f;
    private float t_currentTargetDistance;

    [SerializeField]
    LayerMask t_renderIgnore;

    private CinemachineBrain brain;

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
