using UnityEngine;

namespace InventorySystem
{
    [System.Serializable]
    public class InventorySlot : ItemSlot
    {
        /// <summary>
        /// 创建一个插槽
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amount"></param>
        public InventorySlot(ItemData source, int amount)
        {
            itemDate = source;
            itemID = itemDate.ID;
            stackSize = amount;
        }

        /// <summary>
        /// 创建一个空白插槽
        /// </summary>
        public InventorySlot()
        {
            ClearSlot();
        }

        /// <summary>
        /// 更换插槽数据(无保护)
        /// </summary>
        /// <param name="newDate"></param>
        /// <param name="amount"></param>
        public void UpdateSlot(ItemData newDate, int amount)
        {
            itemDate = newDate;
            itemID = newDate.ID;
            stackSize = amount;
        }

        /// <summary>
        /// 访问当前插槽内物品并与传入的插槽物品是否相同
        /// 相同：增加当前插槽物品数量
        /// 不同：将当前插槽换为传入插槽
        /// </summary>
        /// <param name="invSlot">传入的插槽</param>
        public void AssignItem(InventorySlot invSlot)
        {
            if (itemDate == invSlot.itemDate) AddToStack(invSlot.stackSize);
            else
            {
                itemDate = invSlot.itemDate;
                itemID = itemDate.ID;
                stackSize = 0;
                AddToStack(invSlot.stackSize);
            }
        }

        /// <summary>
        /// 判断当前插槽的剩余空间是否足够添加传入的数量，输出剩余空间的大小
        /// </summary>
        /// <param name="amoutnToAdd"></param>
        /// <param name="amoutnRemaining"></param>
        /// <returns></returns>
        public bool HasEnoughRoomLeftInStack(int amoutnToAdd, out int amoutnRemaining)
        {
            amoutnRemaining = itemDate.max_stack_size - stackSize;

            return HasEnoughRoomLeftInStack(amoutnToAdd);
        }

        /// <summary>
        /// 判断当前插槽的剩余空间是否足够添加传入的数量
        /// </summary>
        /// <param name="amoutnToAdd"></param>
        /// <returns></returns>
        public bool HasEnoughRoomLeftInStack(int amoutnToAdd)
        {
            if (stackSize + amoutnToAdd <= itemDate.max_stack_size) return true;
            else return false;
        }

        /// <summary>
        /// 拆分插槽内物品数量（拆分一半）
        /// </summary>
        /// <param name="splitStack"></param>
        /// <returns></returns>
        public bool SplitStack(out InventorySlot splitStack)
        {
            if (StackSize <= 1) //插槽内物品数量不能拆分
            {
                splitStack = null;
                return false;
            }

            int halfStack = Mathf.RoundToInt(stackSize / 2);
            RemoveFromStack(halfStack);

            splitStack = new InventorySlot(itemDate, halfStack);
            return true;
        }
    }
}
