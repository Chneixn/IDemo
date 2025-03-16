using System;
using Unity.Cinemachine;
using UnityEngine;

public enum CamState
{
    FPS,
    TPS,
    FreeLook
}

public struct CameraInput
{
    public float mouseX;
    public float mouseY;
    public float zoomValue;
    /// <summary>
    /// 切换摄像机状态 true为FPS false为TPS
    /// </summary>
    public bool switchCamState;
    public bool freeLook;
}

/// <summary>
/// 实现玩家视角操控和操作输入传入
/// </summary>
public class CameraController : CinemachineCameraManagerBase
{
    public CinemachineBrain brain;
    [SerializeField] private CamState currentCamState;
    public CamState CurrentCamState => currentCamState;
    public Action<CamState> OnCamStateChange;

    [Header("MouseSensitivity鼠标灵敏度")]
    public float XmouseSensitivity = 1f;
    public float YmouseSensitivity = 1f;
    public bool isMixXYSensitivity;
    [Tooltip("只有在isMixXYSensitivity开启时生效")]
    public float mouseMixSensitivity = 1f;

    [Header("鼠标灵敏度系数")]
    public float X_Mult = 25f;
    public float Y_Mult = 25f;

    private float _mouseX;
    private float _mouseY;

    public CinemachineVirtualCameraBase F_cam;
    public InputAxis F_Input = new();
    private CinemachinePanTilt F_POV;
    public CinemachineVirtualCameraBase t_cam;
    private bool isFreeLook = false;
    public CinemachineVirtualCameraBase free_cam;
    public Vector3 lastLookDirection;

    protected override void Start()
    {
        base.Start();
        lastLookDirection = transform.forward;
        if (brain == null)
        {
            brain = FindFirstObjectByType<CinemachineBrain>();
            Debug.Log($"Auto find CinemachineBrain at {brain.gameObject.name}");
        }

        for (int i = 0; i < ChildCameras.Count; ++i)
        {
            var cam = ChildCameras[i];
            if (!cam.isActiveAndEnabled)
                continue;
            if (F_cam == null && cam.TryGetComponent<CinemachinePanTilt>(out _))
            {
                F_cam = cam;
            }
            else if (t_cam == null && cam.TryGetComponent<CinemachineThirdPersonAim>(out _))
                t_cam = cam;

            else if (free_cam == null)
            {
                free_cam = cam;
            }
        }

        F_POV = F_cam.GetComponent<CinemachinePanTilt>();
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

        // 读取控制模式的摄像机状态
        if (inputs.switchCamState)
            UpdateCamState(CamState.TPS);
        else UpdateCamState(CamState.FPS);


        switch (currentCamState)
        {
            // FIXME: 摄像机输入没有限制
            case CamState.FPS:
                {
                    F_POV.TiltAxis.Value = _mouseY;
                    F_POV.PanAxis.Value = _mouseX;
                    F_POV.PanAxis.Validate();
                    F_POV.TiltAxis.Validate();
                }
                break;
            case CamState.TPS:
                {
                    // Handle Zoom
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

                    isFreeLook = inputs.freeLook;
                    if (isFreeLook) UpdateCamState(CamState.FreeLook);
                    else UpdateCamState(CamState.TPS);
                }
                break;
        }

        // 相机更新
        brain.ManualUpdate();
    }

    protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
    {
        var newCam = currentCamState switch
        {
            CamState.FPS => F_cam,
            CamState.TPS => t_cam,
            CamState.FreeLook => free_cam,
            _ => null
        };
        return newCam;
    }

    /// <summary>
    /// 改变相机状态，若同状态不切换
    /// </summary>
    /// <param name="state"></param>
    public void UpdateCamState(CamState state)
    {
        if (currentCamState == state) return;
        if (state == CamState.FreeLook) lastLookDirection = transform.forward;
        currentCamState = state;
        OnCamStateChange?.Invoke(currentCamState);
    }
}
