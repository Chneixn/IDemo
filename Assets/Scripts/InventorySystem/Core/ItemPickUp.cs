using System;
using UnityEngine;
using UnityEngine.Events;

namespace InventorySystem
{
    [RequireComponent(typeof(SphereCollider))]
    [RequireComponent(typeof(UniqueID))]
    public class ItemPickUp : MonoBehaviour, IInteractable
    {
        public InventoryItemData ItemDate;
        public int itemCount = 1;

        [Tooltip("允许使用碰撞捡取")]
        [SerializeField] private bool allowPickUpByTouch;
        public float PickUpRadius = 1.0f;
        private SphereCollider myCollider;

        [SerializeField] private ItemPickUpSaveData itemSaveData;
        public string id { get; private set; }
        public Action<IInteractable> OnInteractionComplete { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void Awake()
        {
            id = GetComponent<UniqueID>().ID;
            SaveLoad.OnLoadGame += LoadGame;

            myCollider = GetComponent<SphereCollider>();
            if (allowPickUpByTouch)
            {
                myCollider.isTrigger = true;
                myCollider.radius = PickUpRadius;
            }
            else myCollider.enabled = false;

        }

        private void Start()
        {
            // 初始化物品数据
            itemSaveData = new ItemPickUpSaveData(ItemDate, itemCount, transform.position, transform.rotation);
            // 在存档数据中写入物品数据
            SaveGameManager.data.activeItems.Add(id, itemSaveData);
        }

        private void LoadGame(SaveDate data)
        {
            if (data.collectedItems.Contains(id)) Destroy(this.gameObject);
        }

        private void OnDestroy()
        {
            //在存档标记该物品已被全部拾取
            SaveGameManager.data.collectedItems.Add(id);
            // 在存档数据移除activeItems标记
            if (SaveGameManager.data.activeItems.ContainsKey(id))
                SaveGameManager.data.activeItems.Remove(id);
            SaveLoad.OnLoadGame -= LoadGame;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (allowPickUpByTouch)
            {
                //获取玩家库存
                var inventory = other.transform.GetComponent<PlayerInventoryHolder>();
                if (!inventory) return;

                //加入玩家库存
                int itemLeft = inventory.AddToInventory(ItemDate, itemCount);
                if (itemLeft > 0)
                {
                    //物品没有全部进入库存
                    itemCount = itemLeft;
                    return;
                }
                Destroy(gameObject);
            }
        }

        public void Interact(Interactor interactor, out bool interactSuccessful)
        {
            //获取玩家库存
            PlayerInventoryHolder inventory = GameManager.Instance.PlayerInventory;
            if (!inventory)
            {
                interactSuccessful = false;
                return;
            }

            //加入玩家库存
            int itemLeft = inventory.AddToInventory(ItemDate, itemCount);
            if (itemLeft > 0)
            {
                //物品没有全部进入库存
                itemCount = itemLeft;
                interactSuccessful = true;
                return;
            }
            interactSuccessful = false;
            Destroy(gameObject);
        }

        public void EndInteraction()
        {

        }
    }

    [System.Serializable]
    public struct ItemPickUpSaveData
    {
        public InventoryItemData ItemData;
        public int ItemCount;
        public Vector3 position;
        public Quaternion rotation;

        public ItemPickUpSaveData(InventoryItemData _data, int _itemCount, Vector3 _position, Quaternion _rotation)
        {
            this.ItemData = _data;
            this.ItemCount = _itemCount;
            this.position = _position;
            this.rotation = _rotation;
        }
    }
}