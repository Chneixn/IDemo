using System;
using UnityEngine;
using InventorySystem;
using UnityGameObjectPool;

[RequireComponent(typeof(SphereCollider))]
public class ItemPickUp : IPoolableObject
{
    public ItemData Data;
    public int Count = 1;
    [Tooltip("允许使用碰撞捡取")]
    public bool allowPickUpByTouch;

    public bool MovingModel = false;
    [Tooltip("振幅")]
    public float length = 0.1f;
    [Tooltip("一次往复周期(秒)")]
    public float moveTime = 2f;
    public Transform model;

    float YOffset = 0f;

    [SerializeField] private float touchRadius = 1.0f;

    [SerializeField] private ItemPickUpSaveData itemSaveData;

    void Start()
    {
        if (model != null)
            YOffset = model.transform.position.y;
    }

    void Update()
    {
        if (MovingModel) MoveVisualModel();
    }

    private void MoveVisualModel()
    {
        var t = 4 * length * Time.time / moveTime; // 時間の進行速度を調整
        // 指定された振幅と周期のPingPong
        var value = Mathf.PingPong(t, 2 * length) - length + YOffset;
        model.position = new Vector3(model.position.x, value, model.position.z);
    }

    public override void OnGet()
    {

    }

    public override void OnRecycle()
    {

    }

#if UNITY_EDITOR
    public void OnValidate()
    {
        var trigger = GetComponent<SphereCollider>();
        trigger.radius = touchRadius;
    }
#endif

}

[System.Serializable]
public struct ItemPickUpSaveData
{
    public ItemData data;
    public int count;
    public Vector3 position;
    public Quaternion rotation;

    public ItemPickUpSaveData(ItemData data, int count, Vector3 position, Quaternion rotation)
    {
        this.data = data;
        this.count = count;
        this.position = position;
        this.rotation = rotation;
    }
}