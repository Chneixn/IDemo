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
        var m = GameManager.Instance;
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
        characterInputs.moveDirection = input.Movement.ReadValue<Vector2>();
        characterInputs.lookDirection = playerCam.transform.forward;

        if (holdToJump)
        {
            characterInputs.tryJump = input.Jump.IsPressed();
        }
        else
        {
            characterInputs.tryJump = input.Jump.triggered;
        }

        if (holdToRun)
        {
            characterInputs.tryRun = input.Run.IsPressed();
        }
        else if (input.Run.triggered)
        {
            characterInputs.tryRun = !characterInputs.tryRun;
        }

        if (holdToCrouch)
        {
            characterInputs.tryCrouch = input.Crouch.IsPressed();
        }
        else if (input.Crouch.triggered)
        {
            characterInputs.tryCrouch = !characterInputs.tryCrouch;
        }

        if (input.Fly.triggered)
            characterInputs.tryFly = !characterInputs.tryFly;

        //闪避
        characterInputs.tryDodge = input.Dodge.triggered;

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

        if (input.SwitchCamState.triggered)
        {
            cameraInput.activeCamState = cameraInput.activeCamState == CamState.FPS ? CamState.TPS : CamState.FPS;
        }

        playerCam.ApplyInput(ref cameraInput);
        //characterControl.OnCameraUpdate(Time.deltaTime, playerCam.transform.forward);
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
        weaponInput.quick0 = input.QuickUse_0.triggered;
        weaponInput.quick1 = input.QuickUse_1.triggered;
        weaponInput.quick2 = input.QuickUse_2.triggered;
        weaponInput.quick3 = input.QuickUse_3.triggered;
        weaponInput.quick4 = input.QuickUse_4.triggered;
        weaponInput.switchWeapon = input.SwitchLastWeapon.triggered;
        weaponInput.fire = input.Fire.triggered;

        if (holdToAim)
            weaponInput.aim = input.Aim.IsPressed();
        else if (input.Aim.triggered)
            weaponInput.aim = !weaponInput.aim;

        weaponInput.reload = input.Reload.triggered;
        weaponInput.switchFireMod = input.FireModeSwitch.triggered;
        weaponHolder.ApplyInput(ref weaponInput);
    }
    #endregion
}
