using UnityEngine;
using DRockInputBridge;
using System;

public class PlayerInputReceiver : UnityInputReceiver
{
    [Header("MovementControl运动控制")]
    [SerializeField] private bool holdToCrouch = false;
    [SerializeField] private bool holdToJump = false;
    [SerializeField] private bool holdToRun = false;
    [SerializeField] private bool allowFly = false;

    [Header("武器控制")]
    [SerializeField] private bool holdToAim = false;

    private CameraController playerCam;
    private CharacterControl characterControl;
    private Interactor interactor;
    private WeaponHolder weaponHolder;

    [SerializeField]
    private PlayerCharacterInput characterInputs;
    private CameraInput cameraInput;
    private InteractionInput interactionInput;
    private WeaponInput weaponInput;

    private void Awake()
    {
        // 初始化所有输入接口表，所有受控制的脚本从接口表读取布尔值
        characterInputs = new();
        cameraInput = new();
        interactionInput = new();
        weaponInput = new();
    }

    public override void Start()
    {
        base.Start();
        var m = PlayerManager.Instance;
        if (m == null) Debug.LogError("Missing GameManager!");
        playerCam = m.PlayerCam;
        characterControl = m.CharacterControl;
        interactor = m.Interactor;
        weaponHolder = m.WeaponHolder;
    }

    public override void SetActionMap(SourceInput sourceInput)
    {
        sourceInput.Disable();
        sourceInput.PlayerInput.Enable();
        sourceInput.WeaponInput.Enable();
    }

    public override void OnEnter()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void OnExit()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public override void OnUpdate()
    {
        PrassThroughChararcterInput();
        PrassThroughWeaponInput();
        PrassThroughInteractionInput();
    }

    public override void OnLateUpdate()
    {
        PrassThroughCameraInput();
    }

    public override void OnPause()
    {

    }

    public override void OnResume()
    {

    }

    #region 当控制器处于激活状态时, 对所有附属控件传入控制信息
    /// <summary>
    /// 传入角色控制
    /// </summary>
    private void PrassThroughChararcterInput()
    {
        var input = userInput.SourceInput.PlayerInput;
        characterInputs.MoveDirection = input.Movement.ReadValue<Vector2>();

        characterInputs.LookDirection = playerCam.brain.transform.position;
        characterInputs.CamRotation = playerCam.brain.transform.rotation;
        characterInputs.CameraState = playerCam.CurrentCamState;

        if (holdToJump)
            characterInputs.TryJump = input.Jump.inProgress;
        else
            characterInputs.TryJump = input.Jump.triggered;

        if (holdToRun)
            characterInputs.TryRun = input.Run.inProgress;
        else if (input.Run.triggered)
            characterInputs.TryRun = !characterInputs.TryRun;

        if (holdToCrouch)
            characterInputs.TryCrouch = input.Crouch.inProgress;
        else if (input.Crouch.triggered)
            characterInputs.TryCrouch = !characterInputs.TryCrouch;

        if (input.Fly.triggered)
            characterInputs.TryFly = !characterInputs.TryFly;

        //闪避
        characterInputs.TryDodge = input.Dodge.triggered;

        characterControl.HandleInput(ref characterInputs);
    }

    /// <summary>
    /// 传入相机控制
    /// </summary>
    private void PrassThroughCameraInput()
    {
        var input = userInput.SourceInput.PlayerInput;

        cameraInput.mouseX = input.Look.ReadValue<Vector2>().x;
        cameraInput.mouseY = input.Look.ReadValue<Vector2>().y;
        cameraInput.zoomValue = input.Zoom.ReadValue<float>();
        cameraInput.freeLook = input.FreeLook.inProgress;
        if (input.SwitchCamState.triggered)
        {
            cameraInput.switchCamState = !cameraInput.switchCamState;
        }

        playerCam.ApplyInput(ref cameraInput);
    }

    /// <summary>
    /// 传入交互控制
    /// </summary>
    private void PrassThroughInteractionInput()
    {
        var input = userInput.SourceInput.PlayerInput;

        interactionInput.tryInteraction = input.Interaction.triggered;
        interactor.ApplyInput(ref interactionInput);
    }

    private void PrassThroughWeaponInput()
    {
        var input = userInput.SourceInput.WeaponInput;
        weaponInput.quick1 = input.QuickUse_1.triggered;
        weaponInput.quick2 = input.QuickUse_2.triggered;
        weaponInput.quick3 = input.QuickUse_3.triggered;
        weaponInput.quick4 = input.QuickUse_4.triggered;
        weaponInput.reload = input.Reload.triggered;
        weaponInput.fire = input.Fire.inProgress;
        weaponInput.switchLastWeapon = input.SwitchLastWeapon.triggered;
        weaponInput.switchFireMod = input.FireModeSwitch.triggered;
        weaponInput.scrollSwitch = input.ScrollSwitch.ReadValue<float>();

        if (holdToAim)
            weaponInput.aim = input.Aim.IsPressed();
        else if (input.Aim.triggered)
            weaponInput.aim = !weaponInput.aim;

        weaponHolder.ApplyInput(ref weaponInput);
    }
    #endregion
}
