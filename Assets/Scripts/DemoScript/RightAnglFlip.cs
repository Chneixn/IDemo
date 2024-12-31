using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightAnglFlip : MonoBehaviour
{
    [SerializeField, Tooltip("物体的forward方向")]
    private Transform target;

    private void Start()
    {
        if (target == null)
        {
            target = transform;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col != null)
        {
            if (col.TryGetComponent(out CharacterControl cc))
            {
                cc.Gravity = -target.forward * cc.Gravity.magnitude;
                //Debug.Log($"Set character gravity to {cc.Gravity}!");
            }
        }
    }

    //private void FlipDirection(Vector3 newUpDirection, Transform tr)
    //{
    //    float angleBetweenUpDirections = Vector3.Angle(newUpDirection, tr.up);
    //    float angleThreshold = 0.001f;

    //    if (angleBetweenUpDirections < angleThreshold) { return; }

    //    Quaternion retationDifference = Quaternion.FromToRotation(tr.up, newUpDirection);
    //    tr.rotation = retationDifference * tr.rotation;
    //}
}
