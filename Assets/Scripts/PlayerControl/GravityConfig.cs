using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GravityConfig
{
    [Header("Gravity重力")]
    public bool Enable = true;
    public bool walkOnAnyGround = false;
    public bool UsePhysics = false;
    public Vector3 Default = new(0, -15f, 0);
    [SerializeField] private Vector3 gravity = Vector3.zero;
    public Vector3 Velue
    {
        get
        {
            if (Enable)
            {
                if (gravity == Vector3.zero) gravity = Default;
                return UsePhysics == false ? gravity : Physics.gravity;
            }
            else return Vector3.zero;
        }
        set
        {
#if UNITY_EDITOR
            if (value == Vector3.zero) Debug.LogWarning("Gravity should be disable, do not set it to zero.");
#endif
            gravity = value;
        }
    }
}
