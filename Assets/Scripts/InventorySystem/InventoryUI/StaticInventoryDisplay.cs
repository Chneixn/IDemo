using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

/// <summary>
/// 静态库存显示, 用于制作玩家HotBar
/// </summary>
public class StaticInventoryDisplay : InventoryDisplay
{
    [SerializeField] private PlayerInventoryHolder playerInventory;

    // 存放插槽UI预制体
    [Tooltip("存放插槽UI预制体")]
    [SerializeField] private InventorySlot_UI slotPrefab;

    // HotBar界面的插槽UI元素组
    [Tooltip("HotBar界面的插槽UI元素组")]
    [SerializeField] protected InventorySlot_UI[] slots;

    protected virtual void OnEnable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChange += RefreshStaticDisplay;
    }

    protected virtual void OnDisable()
    {
        PlayerInventoryHolder.OnPlayerInventoryChange -= RefreshStaticDisplay;
    }

    /// <summary>
    /// 刷新静态库存UI界面
    /// </summary>
    private void RefreshStaticDisplay()
    {
        if (playerInventory != null)
        {
            // 获取库存系统，注册库存变更时事件
            inventorySystem = playerInventory.PrimaryStorage;
            inventorySystem.OnInventorySlotChanged += UpdateSlot;
        }
        else Debug.LogWarning($"No Player's Inventory assigned to {this.gameObject}");

        AssignSlot(inventorySystem, playerInventory.HotBarCount);
    }

    protected override void Start()
    {
        base.Start();

        RefreshStaticDisplay();
    }

    /// <summary>
    /// 注册UI元素，并绑定对应库存插槽(静态库存显示只会进行一次注册)
    /// </summary>
    /// <param name="invToDisplay"></param>
    /// <param name="offest"></param>
    public override void AssignSlot(InventoryStorage invToDisplay, int offest)
    {
        // 字典进行数据绑定
        slotDictionary = new Dictionary<InventorySlot_UI, InventorySlot>();

        // 逐一绑定
        for (int i = 0; i < playerInventory.HotBarCount; i++)
        {
            slotDictionary.Add(slots[i], inventorySystem.InventorySlots[i]);
            slots[i].Init(inventorySystem.InventorySlots[i]);
            slots[i].UpdateUISlot();
        }
    }

}
