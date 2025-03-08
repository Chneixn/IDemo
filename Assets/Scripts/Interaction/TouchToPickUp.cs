using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;

[RequireComponent(typeof(SphereCollider))]
public class TouchToPickUp : MonoBehaviour
{
    [SerializeField] private InventoryStorage holderStorge;
    [SerializeField] private LayerMask interactableLayer;

    private new SphereCollider collider;

    void Start()
    {
        holderStorge = PlayerManager.Instance.PlayerInventory.PrimaryStorage;
        collider = GetComponent<SphereCollider>();
    }

    void Update()
    {
        DetectCollider();
    }

    public void DetectCollider()
    {
        // FIXME: 无法检测到碰撞体
        var colliders = Physics.OverlapSphere(transform.position + collider.center, collider.radius, interactableLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            // Debug.Log("检测到碰撞体: " + colliders[i].name);
            if (colliders[i].TryGetComponent(out ItemPickUp item))
            {
                Debug.Log("检测到物品: " + item.name);
                if (item.allowPickUpByTouch)
                {
                    int left = holderStorge.AddToInventory(item.Data, item.Count);
                    if (left > 0)
                    {
                        // 物品没有全部进入库存
                        item.Count = left;
                    }
                    else
                    {
                        // 所有物品均被拾取
                        GameObjectPoolManager.RecycleItem(item);
                        Debug.Log("尝试回收物品: " + item.name);
                    }
                }
            }

        }

    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        var collider = GetComponent<SphereCollider>();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + collider.center, collider.radius);
    }
#endif
}
