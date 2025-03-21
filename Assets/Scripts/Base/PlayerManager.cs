using DRockInputBridge;
using UnityEngine;
using InventorySystem;
using System;
using UISystem;

[DisallowMultipleComponent]
public class PlayerManager : SameSceneSingleMono<PlayerManager>
{
    public int PlayerIndex = 0;

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

    [Header("UI")]
    public PlayerHUD playerHUD;

    void Start()
    {
        UIManager.Instance.PushUI(playerHUD);
        EnablePlayerInput(true);
    }

    public void EnablePlayerInput(bool enable)
    {
        if (enable) InputManager.Instance.Push(PlayerInput);
        else if (InputManager.Instance.FSM.CurrentReceiver != PlayerInput as IInputReceiver)
        {
            Debug.LogError("Pop PlayerInput Failed! Current Receiver is not PlayerInput!");
            return;
        }
        InputManager.Instance.Pop();
    }
}
