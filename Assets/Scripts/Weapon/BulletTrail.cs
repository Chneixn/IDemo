using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrailRenderer))]
public class BulletTrail : MonoBehaviour
{
    public TrailRenderer trail;
    public void OnEnable()
    {
        trail = GetComponent<TrailRenderer>();
    }

    public void OnDisable()
    {
        trail.Clear();
    }

}
