using System;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    public class ChestInventory : InventoryHolder
    {
        public Action<IInteractable> OnInteractionComplete;

        public static UnityAction<InventorySystem, int> OnChestInteraction;

        [SerializeField] private InventorySaveData chestSaveData;
        private string id;

        protected override void Awake()
        {
            base.Awake();
        }

        public void Interact(Interactor interactor, out bool interactSuccessful)
        {
            //OnDynamicInventoryDisplayRequested?.Invoke(primaryInventorySystem, 0);
            OnChestInteraction?.Invoke(primaryInventorySystem, 0); //此动作在playerInventoryHolder处订阅
            interactSuccessful = true;
        }

        public void EndInteraction()
        {

        }
    }

    public struct InventorySaveData
    {
        private InventorySystem system;
        private Vector3 position;
        private Quaternion rotation;

        public InventorySaveData(InventorySystem system, Vector3 position, Quaternion rotation)
        {
            this.system = system;
            this.position = position;
            this.rotation = rotation;
        }
    }
}
