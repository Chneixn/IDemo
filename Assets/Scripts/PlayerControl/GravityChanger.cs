using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityChanger : MonoBehaviour
{
    [SerializeField, Tooltip("角色旋转后的上方向(使用forward)")]
    private Transform targetUp;
    public float GravityValue = 0f;

    private void Start()
    {
        if (targetUp == null)
        {
            targetUp = transform;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col != null)
        {
            if (col.TryGetComponent(out CharacterControl cc))
            {
                if (GravityValue != 0f)
                    cc.Gravity.Value = -targetUp.forward * GravityValue;
                else
                    cc.Gravity.Value = -targetUp.forward * cc.Gravity.Value.magnitude;
                //Debug.Log($"Set character gravity to {cc.Gravity}!");
            }
        }
    }
}
