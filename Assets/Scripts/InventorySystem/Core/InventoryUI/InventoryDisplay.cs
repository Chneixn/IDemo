using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace InventorySystem
{
    public abstract class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] MouseItemDate mouseInventoryItem;

        protected InventorySystem inventorySystem;
        protected Dictionary<InventorySlot_UI, InventorySlot> slotDictionary; //将插槽与插槽UI绑定，key是UI脚本，value是插槽脚本

        public InventorySystem InventorySystem => inventorySystem;
        public Dictionary<InventorySlot_UI, InventorySlot> SlotDictionary => slotDictionary;

        protected virtual void Start()
        {

        }

        public abstract void AssignSlot(InventorySystem invToDisplay, int offest); //预留需要实现的函数

        /// <summary>
        /// 更新对插槽对应的UI
        /// </summary>
        /// <param name="updateSlot"></param>
        protected virtual void UpdateSlot(InventorySlot updateSlot)
        {
            foreach (var slot in SlotDictionary)
            {
                //key是UI脚本，value是插槽脚本
                if (slot.Value == updateSlot)
                {
                    slot.Key.UpdateUISlot(updateSlot);
                }
            }
        }

        /// <summary>
        /// 实现插槽交互功能
        /// </summary>
        /// <param name="clickedUISlot"></param>
        public void SlotClicked(InventorySlot_UI clickedUISlot)
        {
            bool isShiftPressed = Keyboard.current.leftShiftKey.isPressed;
            //点击有物品的插槽时 + 鼠标上没有物品 -> 拿起物品
            if (clickedUISlot.AssignedInventorySlot.ItemDate != null && mouseInventoryItem.AssignedInventorySlot.ItemDate == null)
            {
                //按住shift时，拿起一半物品
                if (isShiftPressed && clickedUISlot.AssignedInventorySlot.SplitStack(out InventorySlot halfStackSlot))
                {
                    mouseInventoryItem.UpdateMouseSlot(halfStackSlot);
                    clickedUISlot.UpdateUISlot();
                    return;
                }
                else
                {
                    mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);
                    clickedUISlot.ClearSlot();
                    return;
                }
            }

            //点击空插槽时 + 鼠标上有物品 -> 将物品放入插槽
            if (clickedUISlot.AssignedInventorySlot.ItemDate == null && mouseInventoryItem.AssignedInventorySlot.ItemDate != null)
            {
                clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                clickedUISlot.UpdateUISlot();

                mouseInventoryItem.ClearSlot();
                return;
            }

            //鼠标插槽和点击插槽都有物品时
            if (clickedUISlot.AssignedInventorySlot.ItemDate != null && mouseInventoryItem.AssignedInventorySlot.ItemDate != null)
            {
                bool isSameItem = clickedUISlot.AssignedInventorySlot.ItemDate == mouseInventoryItem.AssignedInventorySlot.ItemDate;

                //物品相同时
                //插槽物品数 + 鼠标物品数 <= 物品堆叠数 -> 增加插槽物品数，清除鼠标插槽
                if (isSameItem &&
                    clickedUISlot.AssignedInventorySlot.HasEnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize))
                {
                    clickedUISlot.AssignedInventorySlot.AssignItem(mouseInventoryItem.AssignedInventorySlot);
                    clickedUISlot.UpdateUISlot();

                    mouseInventoryItem.ClearSlot();
                    return;
                }
                //插槽物品数 + 鼠标物品数 < 物品堆叠数
                else if (isSameItem &&
                    !clickedUISlot.AssignedInventorySlot.HasEnoughRoomLeftInStack(mouseInventoryItem.AssignedInventorySlot.StackSize, out int leftInStack))
                {
                    //插槽物品数 = MaxStackSize -> 切换物品
                    if (leftInStack < 1) { SwapSlots(clickedUISlot); return; }
                    //插槽物品数 < MaxStackSize -> 拆分鼠标插槽物品
                    else
                    {
                        int remainingOnMouse = mouseInventoryItem.AssignedInventorySlot.StackSize - leftInStack;
                        clickedUISlot.AssignedInventorySlot.AddToStack(leftInStack);
                        clickedUISlot.UpdateUISlot();

                        var tempItem = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemDate, remainingOnMouse);
                        mouseInventoryItem.ClearSlot();
                        mouseInventoryItem.UpdateMouseSlot(tempItem);
                        return;
                    }
                }
                //物品不同时 -> 切换物品
                else if (!isSameItem)
                {
                    SwapSlots(clickedUISlot);
                    return;
                }
            }
        }

        /// <summary>
        /// 切换UI插槽和鼠标插槽
        /// </summary>
        /// <param name="clickedUISlot"></param>
        private void SwapSlots(InventorySlot_UI clickedUISlot)
        {
            //缓存鼠标插槽
            var clonedSlot = new InventorySlot(mouseInventoryItem.AssignedInventorySlot.ItemDate, mouseInventoryItem.AssignedInventorySlot.StackSize);
            mouseInventoryItem.ClearSlot();

            //将鼠标插槽换为被点击的插槽
            mouseInventoryItem.UpdateMouseSlot(clickedUISlot.AssignedInventorySlot);

            //清除被点击的插槽
            clickedUISlot.ClearSlot();
            //将被点击的插槽换为缓存的鼠标插槽
            clickedUISlot.AssignedInventorySlot.AssignItem(clonedSlot);
            clickedUISlot.UpdateUISlot();
        }
    }
}