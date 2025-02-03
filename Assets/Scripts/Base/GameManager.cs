using DRockInputBridge;
using UnityEngine;
using InventorySystem;
using System;

[DisallowMultipleComponent]
public class GameManager : SingleMonoBase<GameManager>
{
    #region 游戏全局广播属性
    //全局GamePasue
    [SerializeField, ReadOnly] private bool _gamePause;
    public Action<bool> OnGamePause;

    //控制状态机
    private InputManager inputManager;
    public InputManager Input => inputManager;

    public bool AllowBackpackOpen;
    #endregion

    #region 玩家常用属性

    public CharacterControl CharacterControl;
    public CameraController PlayerCam;
    public Interactor Interactor;
    public WeaponHolder WeaponHolder;
    public PlayerInputReceiver PlayerInput;
    public CharacterCustomization CharacterCustomization;
    public PlayerInventoryHolder PlayerInventory;
    public ItemDatabase ItemDatabase;

    #endregion

    protected override void Awake()
    {
        base.Awake();
        inputManager = InputManager.Instance;
        // 临时测试用
        Application.targetFrameRate = 60;
        _gamePause = true;
    }

    private void Start()
    {
        // demo mode
        inputManager.Push(PlayerInput);
        CharacterCustomization.ChangeModelVisibility(PlayerCam.CurrentCamState);
        PlayerInventory.AddToInventory(ItemDatabase.GetItem(1), 50);
    }

    void Update()
    {
        inputManager.OnUpdate();
    }

    private void LateUpdate()
    {
        inputManager.OnLateUpdate();
    }

    public void SetGamePasue(bool pause)
    {
        _gamePause = pause;
        OnGamePause?.Invoke(pause);
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    SetGamePasue(pause);
    //}
}
