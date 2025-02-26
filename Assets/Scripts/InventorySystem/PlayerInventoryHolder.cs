using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InventorySystem;

public class PlayerInventoryHolder : InventoryHolder
{
    public static UnityAction OnPlayerInventoryChange;

    public static UnityAction<InventoryStorage, int> OnPlayerInventoryDisplayRequested;

    public static UnityAction<InventoryStorage, int> WithChestInteraction;

    // 玩家hotbar的数量，HotBar插槽不会在玩家背包显示，仅在hotbar显示
    [SerializeField] private int hotBarCount = 2;
    public int HotBarCount => hotBarCount;

    /// <summary>
    /// 向玩家库存添加物品
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public int AddToInventory(ItemData data, int amount)
    {
        int amountLeft = primaryStorage.AddToInventory(data, amount);
        return amountLeft;
    }

    /// <summary>
    /// 从玩家库存移除物品
    /// </summary>
    /// <param name="data"></param>
    /// <param name="amount"></param>
    /// <returns></returns>
    public bool RemoveFormInventory(ItemData data, int amount)
    {
        bool s = primaryStorage.RemoveItemsFromInventory(data, amount, out _);
        return s;
    }

    /// <summary>
    /// 当与箱子交互时显示玩家库存界面
    /// </summary>
    /// <param name="chestInv"></param>
    /// <param name="_offest"></param>
    public void DisplayPlayerInventoryOnChestInteraction(InventoryStorage chestInv, int _offest)
    {
        WithChestInteraction?.Invoke(chestInv, _offest);
        OnPlayerInventoryDisplayRequested?.Invoke(primaryStorage, hotBarCount);
    }
}