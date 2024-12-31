using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using InventorySystem;

public class UIManager : SingleMonoBase<UIManager>
{
    [SerializeField] private GameObject pasueMenu;

    [Header("游戏内UI")]
    [SerializeField] private GameObject inGamePanel;

    [Header("状态栏PlayerStateBar")]
    [SerializeField] private PlayerStateBar playerStateBar;
    [SerializeField] private CharacterStateEventSO stateEvent;  //事件监听更新UI

    [Header("SpeedTextUI")]
    public TextMeshProUGUI speedDisplay_text;
    [Header("AmmoTextUI")]
    public TextMeshProUGUI AmmoTextUI;
    [Header("CrossHairUI")]
    public Image crossHairUI;

    [SerializeField] private CharacterControl character;
    [SerializeField] private ShopKeeperDisplay _shopKeeperDisplay;

    private void OnEnable()
    {
        stateEvent.OnEventRaised += OnStateEvent;

        ShopKeeper.OnShopWindowRequested += DisplayShopWindow;
    }

    private void OnDisable()
    {
        stateEvent.OnEventRaised -= OnStateEvent;

        ShopKeeper.OnShopWindowRequested -= DisplayShopWindow;
    }

    #region 商店界面逻辑
    private void DisplayShopWindow(ShopSystem shopSystem, PlayerInventoryHolder playerInventory)
    {
        DisplayShopWindow(true);
        _shopKeeperDisplay.WhenDisplayShopWindow(shopSystem, playerInventory);
    }

    private void DisplayShopWindow(bool active)
    {
        if (_shopKeeperDisplay.gameObject.activeSelf != active)
            _shopKeeperDisplay.gameObject.SetActive(active);
    }
    #endregion

    private void OnStateEvent(CharacterStateHolder stateHolder)
    {
        float h = stateHolder.healthState.CurrentHealth;
        playerStateBar.OnHealthChange(h);

        float p = stateHolder.powerState.CurrentPower;
        playerStateBar.OnPowerChange(p);
    }

    #region 暂停界面

    private void DisPlayPasueMenu(bool active)
    {
        if (!active)
        {
            //关闭暂停面板
            LockCursor(true);
        }
        else
        {
            //开启暂停面板
            LockCursor(false);
        }
        pasueMenu.SetActive(active);
    }

    #endregion

    public void LockCursor(bool isLock)
    {
        if (isLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void Update()
    {
        // //在暂停界面返回游戏
        // if (pasueMenu.activeSelf && userInput.GetKeyDown("CloseMenu"))
        // {
        //     GameManager.Instance.PDFSM.Pop();
        //     DisPlayPasueMenu(false);
        //     GameManager.Instance.SetGamePasue(false);
        // }

        // //在商店界面返回游戏
        // if (_shopKeeperDisplay.gameObject.activeSelf
        //     && userInput.GetKeyDown("CloseMenu"))
        // {
        //     DisplayShopWindow(false);
        //     LockCursor(true);
        //     GameManager.Instance.SetGamePasue(false);
        // }

        speedDisplay_text.text = "Speed:" + character.CurrentSpeed.ToString("0.00");
    }
}
