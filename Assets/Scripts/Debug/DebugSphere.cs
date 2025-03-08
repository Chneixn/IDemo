using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSphere : MonoBehaviour
{
    public new SphereCollider collider;
    public LayerMask interactableLayer;

    public bool UseUpdate = false;
    public float radius = 0.3f;
    public Vector3 Offest = Vector3.zero;
    public int Cache = 32;
    [SerializeField] private Collider[] colliders;

    void Start()
    {
        // collider = GetComponent<SphereCollider>();
        colliders = new Collider[Cache];
    }


    private void OnTriggerEnter(Collider other)
    {
        if (UseUpdate) return;
        if (other.TryGetComponent(out ItemPickUp item))
        {
            Debug.Log("item here!" + item.name);
        }
    }

    void Update()
    {
        if (UseUpdate)
            DetectCollider();
    }

    public void DetectCollider()
    {
        var length = Physics.OverlapSphereNonAlloc(transform.position + Offest, radius, colliders, interactableLayer);

        for (int i = 0; i < length; i++)
        {
            // Debug.Log("检测到碰撞体: " + colliders[i].name);
            if (colliders[i].TryGetComponent(out ItemPickUp item))
            {
                Debug.Log("检测到物品: " + item.name);
                // if (item.allowPickUpByTouch)
                // {
                //     int left = holderStorge.AddToInventory(item.Data, item.Count);
                //     if (left > 0)
                //     {
                //         // 物品没有全部进入库存
                //         item.Count = left;
                //     }
                //     else
                //     {
                //         // 所有物品均被拾取
                //         GameObjectPoolManager.RecycleItem(item);
                //         Debug.Log("尝试回收物品: " + item.name);
                //     }
                // }
            }

        }

    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position + Offest, radius);
    }
}
