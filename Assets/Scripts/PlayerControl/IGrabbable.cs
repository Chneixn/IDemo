using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGrabbable : MonoBehaviour
{
    [SerializeField] private float moveMultiplider = 10f;
    private Rigidbody rb;
    private Transform objectGrabPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 物品被抓取时状态
    /// </summary>
    /// <param name="_objectGrabPos">物品追随位置</param>
    public void Grab(Transform _objectGrabPos)
    {
        objectGrabPos = _objectGrabPos;
        rb.useGravity = false;
        rb.freezeRotation = true;
    }

    /// <summary>
    /// 物品被放下时状态
    /// </summary>
    public void Drop()
    {
        objectGrabPos = null;
        rb.useGravity = true;
    }

    private void FixedUpdate()
    {
        if (objectGrabPos != null)
        {
            //使用MovePosition移动刚体，使用Lerp函数顺滑移动
            Vector3 _targetPos = Vector3.Lerp(transform.position, objectGrabPos.position, moveMultiplider * Time.deltaTime);
            rb.MovePosition(_targetPos);
        }
    }
}
