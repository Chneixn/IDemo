using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using DRockInputBridge;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace InventorySystem
{
    public class InventoryUIController : UnityInputReceiver
    {
        [FormerlySerializedAs("ChestPanel")]
        public DynamicInventoryDisplay inventoryPanel;
        public DynamicInventoryDisplay playerBackpackPanel;

        private void Awake()
        {
            inventoryPanel.gameObject.SetActive(false);
            playerBackpackPanel.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            //InventoryHolder.OnDynamicInventoryDisplayRequested += DisplayInventory;
            PlayerInventoryHolder.OnPlayerInventoryDisplayRequested += DisplayPlayerInventory;
            PlayerInventoryHolder.WithChestInteraction += DisplayInventory;
        }

        private void OnDisable()
        {
            //InventoryHolder.OnDynamicInventoryDisplayRequested -= DisplayInventory;
            PlayerInventoryHolder.OnPlayerInventoryDisplayRequested -= DisplayPlayerInventory;
            PlayerInventoryHolder.WithChestInteraction -= DisplayInventory;
        }

        public override void SetActionMap(SourceInput sourceInput)
        {
            sourceInput.InventoryInput.Enable();
        }

        public override void OnUpdate()
        {
            // 按下escape关闭库存面板
            if (inventoryPanel.gameObject.activeInHierarchy && userInput.GetKeyDown(""))
                inventoryPanel.gameObject.SetActive(false);

            // 按下escape关闭玩家背包面板
            if (playerBackpackPanel.gameObject.activeInHierarchy && Keyboard.current.escapeKey.wasPressedThisFrame)
                playerBackpackPanel.gameObject.SetActive(false);
        }

        private void DisplayInventory(InventorySystem invToDisplay, int offest)
        {
            inventoryPanel.gameObject.SetActive(true);
            inventoryPanel.RefreshDynamicInventory(invToDisplay, offest);
        }

        private void DisplayPlayerInventory(InventorySystem invToDisplay, int offest)
        {
            playerBackpackPanel.gameObject.SetActive(true);
            playerBackpackPanel.RefreshDynamicInventory(invToDisplay, offest);
        }

        public override void OnEnter()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnPause()
        {

        }

        public override void OnResume()
        {

        }
    }
}