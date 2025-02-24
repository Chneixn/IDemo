using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace InventorySystem
{
    [Serializable]
    public class InventoryStorage
    {
        [SerializeField] private List<InventorySlot> inventorySlots;
        public List<InventorySlot> InventorySlots => inventorySlots;
        public int InventorySize => inventorySlots.Count;   // 库存容量大小
        private readonly Dictionary<ItemData, int> itemsRecord = new();
        public Dictionary<ItemData, int> ItemsRecord => itemsRecord;

        public UnityAction<InventorySlot> OnInventorySlotChanged;

        /// <summary>
        /// 创建一个插槽列表，输入所需的插槽数量
        /// </summary>
        /// <param name="size"></param>
        public InventoryStorage(int size)
        {
            inventorySlots = new List<InventorySlot>(size);

            for (int i = 0; i < size; i++)
            {
                inventorySlots.Add(new InventorySlot());
            }
        }

        /// <summary>
        /// 添加物品进入插槽列表
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="amountToAdd"></param>
        /// <returns>剩余未进入库存的物品数量</returns>
        public int AddToInventory(ItemData itemToAdd, int amountToAdd)
        {
            // 用于记录是否有未能放进库存的物品数量
            int left = amountToAdd;

            // 检查物品是否已存在库存中，优先将物品放入已存有该物品的插槽
            if (ContainItem(itemToAdd, out List<InventorySlot> invSlot))
            {
                foreach (var slot in invSlot)
                {
                    if (slot.HasEnoughRoomLeftInStack(amountToAdd, out int stackLeft))
                    {
                        //该插槽装得下
                        slot.AddToStack(amountToAdd);
                        OnInventorySlotChanged?.Invoke(slot);
                        left = 0;
                        return left;
                    }
                    else
                    {
                        //该插槽装不下
                        slot.AddToStack(stackLeft);
                        OnInventorySlotChanged?.Invoke(slot);
                        amountToAdd -= stackLeft;
                    }
                }
            }
            left = amountToAdd; // 更新剩余未放入的物品数


            while (amountToAdd > 0)
            {
                // 获取空插槽进行放入
                if (HasFreeSlot(out InventorySlot freeSlot))
                {
                    if (amountToAdd > itemToAdd.max_stack_size)
                    {
                        // 当剩余要加入物品数大于单个插槽的容量时
                        freeSlot.UpdateInventorySlot(itemToAdd, itemToAdd.max_stack_size);
                        amountToAdd -= itemToAdd.max_stack_size;
                        OnInventorySlotChanged?.Invoke(freeSlot);
                    }
                    else
                    {
                        // 当剩余要加入物品数小于单个插槽的容量时
                        freeSlot.UpdateInventorySlot(itemToAdd, amountToAdd);
                        amountToAdd = 0;
                        OnInventorySlotChanged?.Invoke(freeSlot);
                    }
                }
                else
                {
                    // 没有空插槽放入
                    left = amountToAdd;
                    return left;
                }
            }
            if (amountToAdd != 0) Debug.LogWarning($"移动物品时发生计数错误！剩余未处理物品{itemToAdd.displayName}，数量为{amountToAdd}");
            left = amountToAdd;

            UpdateItemsRecord();

            return left;
        }

        /// <summary>
        /// 查询库存中是否有存储相同物品的插槽，输出存储相同物品插槽的列表
        /// </summary>
        /// <param name="itemToAdd"></param>
        /// <param name="invSlot"></param>
        /// <returns></returns>
        public bool ContainItem(ItemData itemToAdd, out List<InventorySlot> invSlot)
        {
            invSlot = InventorySlots.Where(i => i.ItemDate == itemToAdd).ToList();

            return invSlot != null;
        }

        /// <summary>
        /// 检查插槽列表中是否有空插槽，输出一个空插槽
        /// </summary>
        /// <param name="freeSlot"></param>
        /// <returns></returns>
        public bool HasFreeSlot(out InventorySlot freeSlot)
        {
            freeSlot = InventorySlots.FirstOrDefault(i => i.ItemDate == null);
            return freeSlot != null;
        }

        private void UpdateItemsRecord()
        {
            itemsRecord.Clear();

            foreach (var slot in InventorySlots)
            {
                if (slot.ItemDate == null) continue;

                //该字典仅计算所持有物品的数量，不考虑最大可堆叠数
                if (!itemsRecord.ContainsKey(slot.ItemDate))
                {
                    itemsRecord.Add(slot.ItemDate, slot.StackSize);
                }
                else
                {
                    itemsRecord[slot.ItemDate] += slot.StackSize;
                }
            }
        }

        /// <summary>
        /// 从库存中移除物品(有保护)
        /// </summary>
        /// <param name="data">要移除的物品</param>
        /// <param name="amountToRemove">要移除的数量</param>
        /// <param name="left">库存中该物品数量</param>
        /// <returns>是否移除成功</returns>
        public bool RemoveItemsFromInventory(ItemData data, int amountToRemove, out int left)
        {
            //获取当前库存中该物品的数量
            left = 0;
            if (ContainItem(data, out List<InventorySlot> slots))
            {
                foreach (InventorySlot slot in slots)
                {
                    left += slot.StackSize;
                }
            }

            // 当可移除的物品少于当前库存中物品时，不允许移除
            if (left < amountToRemove) return false;
            if (amountToRemove == 0) return true;

            // 逐个插槽进行物品删除
            foreach (InventorySlot slot in slots)
            {
                int stackSize = slot.StackSize;

                if (stackSize >= amountToRemove)
                {
                    //单插槽中存量足够移除
                    slot.RemoveFromStack(amountToRemove);
                    left -= amountToRemove;
                    UpdateItemsRecord();
                    return true;
                }
                else
                {
                    //单插槽中存量不够移除
                    slot.RemoveFromStack(stackSize);
                    amountToRemove -= stackSize;
                    left -= stackSize;
                }
                OnInventorySlotChanged?.Invoke(slot);
            }

            return false;
        }

        public int GetItemCountFormInventory(ItemData item)
        {

            if (itemsRecord.TryGetValue(item, out int count))
            {
                //Debug.Log($"物品[{item.displayName}]剩余{count}个");
                return count;
            }
            else
            {
                //Debug.Log($"该库存中不存在[{item.displayName}]!");
                return 0;
            }
        }

        /// <summary>
        /// 以字典形式检测多种物品能否放入库存(Shop System Support)
        /// </summary>
        /// <param name="itemsForCheck"></param>
        /// <returns></returns>
        public bool CheckInventoryRemaining(Dictionary<ItemData, int> itemsForCheck)
        {
            //复制玩家库存
            var clonedSystem = new InventoryStorage(this.InventorySize);
            for (int i = 0; i < InventorySize; i++)
            {
                clonedSystem.InventorySlots[i].AssignItem(this.InventorySlots[i].ItemDate, this.InventorySlots[i].StackSize);
            }

            //遍历库存是否能装入物品
            foreach (var kvp in itemsForCheck)
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    if (clonedSystem.AddToInventory(kvp.Key, kvp.Value) > 0) return false;
                }
            }

            return true;
        }
    }
}