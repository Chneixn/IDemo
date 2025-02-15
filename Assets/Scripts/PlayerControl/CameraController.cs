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
    public bool switchCamState;
}

//public interface ICameraState
//{
//    CamState CamState { get; }
//    void HandleInput(ref CameraInput inputs);
//}

/// <summary>
/// 实现玩家视角操控和操作输入传入
/// </summary>
public class CameraController : CinemachineCameraManagerBase
{
    public static CameraController Instance;
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

    public CinemachineCamera f_cam;
    private CinemachinePanTilt f_POV;
    public CinemachineCamera t_cam;
    public CinemachineCamera free_cam;

    private CinemachineBrain brain;

    private void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
        brain = GetComponent<CinemachineBrain>();

        for (int i = 0; i < ChildCameras.Count; ++i)
        {
            var cam = ChildCameras[i];
            if (!cam.isActiveAndEnabled)
                continue;
            if (f_cam == null && cam.TryGetComponent<CinemachinePanTilt>(out _))
            {
                f_cam = (CinemachineCamera)cam;
            }
            else if (t_cam == null && cam.TryGetComponent<CinemachineThirdPersonAim>(out _))
            {
                t_cam = (CinemachineCamera)cam;
            }
            else if (free_cam == null)
            {
                free_cam = (CinemachineCamera)cam;
            }
        }

        f_POV = f_cam.GetComponent<CinemachinePanTilt>();
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
        if (inputs.switchCamState)
            currentCamState = currentCamState == CamState.FPS ? CamState.TPS : CamState.FPS;

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

        // 相机更新
        brain.ManualUpdate();
    }

    protected override CinemachineVirtualCameraBase ChooseCurrentCamera(Vector3 worldUp, float deltaTime)
    {
        var newCam = currentCamState switch
        {
            CamState.FPS => f_cam,
            CamState.TPS => t_cam,
            CamState.FreeLook => free_cam,
            _ => null
        };
        return newCam;
    }

    public void UpdateCamState(CamState newstate)
    {
        if (currentCamState == newstate) return;
        currentCamState = newstate;
        OnCamStateChange?.Invoke(currentCamState);
    }
}
