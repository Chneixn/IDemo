using System;
using UnityEngine;

namespace InventorySystem
{
    public abstract class ItemSlot
    {
        [NonSerialized] protected ItemData itemDate; // 插槽内物品数据
        [SerializeField] protected int itemID = -1;
        [SerializeField] protected int stackSize; // 插槽内物品的实际数量

        public ItemData ItemDate => itemDate;
        public int StackSize => stackSize;

        public void ClearSlot()
        {
            itemDate = null;
            itemID = -1;
            stackSize = -1;
        }

        public void AssignItem(ItemData data, int amount)
        {
            if (itemDate == data) AddToStack(amount);
            else
            {
                itemDate = data;
                itemID = data.ID;
                stackSize = 0;
                AddToStack(amount);
            }
        }

        /// <summary>
        /// 增加当前插槽物品数量(无保护)
        /// </summary>
        /// <param name="amount"></param>
        public void AddToStack(int amount)
        {
            stackSize += amount;
        }

        /// <summary>
        /// 减少当前插槽物品数量(无保护)
        /// </summary>
        /// <param name="amount"></param>
        public void RemoveFromStack(int amount)
        {
            stackSize -= amount;
            if (stackSize <= 0) ClearSlot();
        }
    }
}