using System;
using UnityEngine;
using UnityEngine.Events;
using InventorySystem;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickUp : MonoBehaviour
{
    public ItemData ItemDate;
    public int itemCount = 1;

    [Tooltip("允许使用碰撞捡取")]
    [SerializeField] private bool allowPickUpByTouch;
    public float PickUpRadius = 1.0f;
    private SphereCollider myCollider;

    [SerializeField] private ItemPickUpSaveData itemSaveData;
    public string id { get; private set; }
    public Action<IInteractable> OnInteractionComplete;

    private void Awake()
    {
        myCollider = GetComponent<SphereCollider>();
        if (allowPickUpByTouch)
        {
            myCollider.isTrigger = true;
            myCollider.radius = PickUpRadius;
        }
        else myCollider.enabled = false;

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
        PlayerInventoryHolder inventory = PlayerManager.Instance.PlayerInventory;
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
    public ItemData ItemData;
    public int ItemCount;
    public Vector3 position;
    public Quaternion rotation;

    public ItemPickUpSaveData(ItemData _data, int _itemCount, Vector3 _position, Quaternion _rotation)
    {
        this.ItemData = _data;
        this.ItemCount = _itemCount;
        this.position = _position;
        this.rotation = _rotation;
    }
}