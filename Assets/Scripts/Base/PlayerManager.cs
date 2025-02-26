using DRockInputBridge;
using UnityEngine;
using InventorySystem;
using System;

[DisallowMultipleComponent]
public class PlayerManager : SameSceneSingleMono<PlayerManager>
{
    public int PlayerIndex = 0;

    [SerializeField] private bool _gamePause;
    public Action<bool> OnGamePause;

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

    public void SetGamePasue(bool pause)
    {
        _gamePause = pause;
        OnGamePause?.Invoke(pause);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        OnGamePause?.Invoke(pauseStatus);
    }
}
