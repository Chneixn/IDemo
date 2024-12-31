using System;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [RequireComponent(typeof(UniqueID))]
    public class ChestInventory : InventoryHolder, IInteractable
    {
        public Action<IInteractable> OnInteractionComplete { get; set; }

        public static UnityAction<InventorySystem, int> OnChestInteraction;

        [SerializeField] private InventorySaveData chestSaveData;
        private string id;

        protected override void Awake()
        {
            base.Awake();

            //绑定当前id
            id = GetComponent<UniqueID>().ID;
        }

        private void Start()
        {
            // 初始化箱子数据
            chestSaveData = new InventorySaveData(primaryInventorySystem, transform.position, transform.rotation);
            //// 在存档数据中写入箱子数据
            //SaveGameManager.data.chestDictionary.Add(id, chestSaveData);
        }

        /// <summary>
        /// 加载存档数据
        /// </summary>
        /// <param name = "data" ></ param >
        //private void LoadInventory(SaveDate data)
        //{
        //    // 检查存档数据是否存在，存在则导入数据
        //    if (data.chestDictionary.TryGetValue(GetComponent<UniqueID>().ID, out InventorySaveData chestData))
        //    {
        //        this.primaryInventorySystem = chestData.InvSystem;
        //        this.transform.position = chestData.Position;
        //        this.transform.rotation = chestData.Rotation;
        //    }
        //}

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
