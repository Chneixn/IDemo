using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobbler : MonoBehaviour
{

    public Transform centerTran;

    Rigidbody rb;
    public float hardness;//摇动幅度(力)

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        rb.AddForce(-(transform.position - centerTran.position) * hardness, ForceMode.VelocityChange);
    }

}