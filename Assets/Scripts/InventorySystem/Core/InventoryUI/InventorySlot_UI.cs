using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace InventorySystem
{
    public class InventorySlot_UI : MonoBehaviour
    {
        [SerializeField] private Image itemSprite;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private InventorySlot assignedInventorySlot;

        private Button button;

        public InventorySlot AssignedInventorySlot => assignedInventorySlot;
        public InventoryDisplay ParentDisplay { get; private set; }

        private void Awake()
        {
            ClearSlot();

            itemSprite.preserveAspect = true;

            button = GetComponent<Button>();
            button?.onClick.AddListener(OnUISlotClick);

            ParentDisplay = transform.parent.GetComponent<InventoryDisplay>();
        }

        public void Init(InventorySlot slot)
        {
            assignedInventorySlot = slot;
            UpdateUISlot(slot);
        }

        /// <summary>
        /// 将插槽数据同步到UI
        /// </summary>
        /// <param name="slot"></param>
        public void UpdateUISlot(InventorySlot slot)
        {
            if (slot.ItemDate != null)
            {
                itemSprite.sprite = slot.ItemDate.icon;
                itemSprite.color = Color.white;

                if (slot.StackSize > 1) { itemCount.text = slot.StackSize.ToString(); }
                else { itemCount.text = string.Empty; }
            }
            else
            {
                ClearSlot();
            }
        }

        /// <summary>
        /// 将已有的插槽数据同步到UI
        /// </summary>
        public void UpdateUISlot()
        {
            if (assignedInventorySlot != null) { UpdateUISlot(assignedInventorySlot); }
        }

        public void ClearSlot()
        {
            assignedInventorySlot?.ClearSlot();
            itemSprite.sprite = null;
            itemSprite.color = Color.clear;
            itemCount.text = string.Empty;
        }

        public void OnUISlotClick()
        {
            ParentDisplay?.SlotClicked(this);
        }

        public void ToggleHighlight()
        {

        }
    }

}