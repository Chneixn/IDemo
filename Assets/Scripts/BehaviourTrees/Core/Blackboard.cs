using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 用于存储需要公开的属性，记录等
/// </summary>
[System.Serializable]
public class Blackboard
{
    public float MoveSpeed = 5.0f;

    public bool HasTarget;
    public GameObject Target;
    public Transform TargetTransform;
    public Vector3 MoveToPosition;
}
