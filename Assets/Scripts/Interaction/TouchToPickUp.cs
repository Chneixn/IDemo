using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InventorySystem;
using UnityGameObjectPool;

public class TouchToPickUp : MonoBehaviour
{
    [SerializeField] private InventoryStorage holderStorge;
    [SerializeField] private SphereCollider detectCollider;
    [SerializeField] private LayerMask interactableLayer;

    public bool enableDetect = true;
    // public float radius = 0.3f;
    public int Cache = 32;
    [SerializeField] private Collider[] colliders;

    void Start()
    {
        holderStorge = PlayerManager.Instance.PlayerInventory.Storage;
        colliders = new Collider[Cache];
    }

    void Update()
    {
        if (enableDetect) DetectCollider();
    }

    public void DetectCollider()
    {
        var length = Physics.OverlapSphereNonAlloc(detectCollider.transform.position, detectCollider.radius, colliders, interactableLayer);

        for (int i = 0; i < length; i++)
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
        Gizmos.color = Color.blue;
        if (detectCollider != null)
            Gizmos.DrawWireSphere(detectCollider.transform.position, detectCollider.radius);
    }
#endif
}
