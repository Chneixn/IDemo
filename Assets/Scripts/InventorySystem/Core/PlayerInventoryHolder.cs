using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public class PlayerInventoryHolder : InventoryHolder
    {
        public static UnityAction OnPlayerInventoryChange;

        public static UnityAction<InventorySystem, int> OnPlayerInventoryDisplayRequested;

        public static UnityAction<InventorySystem, int> WithChestInteraction;

        // 玩家hotbar的数量，HotBar插槽不会在玩家背包显示，仅在hotbar显示
        [SerializeField] private int hotBarCount = 2;
        public int HotBarCount => hotBarCount;

        //private void Start()
        //{
        //    SaveGameManager.data.playerInventory = new InventorySaveData(primaryInventorySystem);
        //}

        //private void OnEnable()
        //{
        //    ChestInventory.OnChestInteraction += DisplayPlayerInventoryOnChestInteraction;
        //}

        //private void OnDisable()
        //{
        //    ChestInventory.OnChestInteraction -= DisplayPlayerInventoryOnChestInteraction;
        //}

        ///// <summary>
        ///// 读取玩家库存的存档数据
        ///// </summary>
        ///// <param name="data"></param>
        //protected override void LoadInventory(SaveDate data)
        //{
        //    // 检查存档数据是否存在，存在则导入数据
        //    if (data.playerInventory.InvSystem != null)
        //    {
        //        this.primaryInventorySystem = data.playerInventory.InvSystem;
        //        OnPlayerInventoryChange?.Invoke();
        //    }
        //}

        /// <summary>
        /// 向玩家库存添加物品
        /// </summary>
        /// <param name="data"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int AddToInventory(ItemData data, int amount)
        {
            int amountLeft = primaryInventorySystem.AddToInventory(data, amount);
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
            bool s = primaryInventorySystem.RemoveItemsFromInventory(data, amount, out _);
            return s;
        }

        /// <summary>
        /// 当与箱子交互时显示玩家库存界面
        /// </summary>
        /// <param name="chestInv"></param>
        /// <param name="_offest"></param>
        public void DisplayPlayerInventoryOnChestInteraction(InventorySystem chestInv, int _offest)
        {
            WithChestInteraction?.Invoke(chestInv, _offest);
            OnPlayerInventoryDisplayRequested?.Invoke(primaryInventorySystem, hotBarCount);
        }
    }
}