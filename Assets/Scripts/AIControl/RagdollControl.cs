using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class RagdollControl : MonoBehaviour
{
    public Transform forceHip;
    public Rigidbody[] rigidbodies;

    [ContextMenu("SetRigidbodies")]
    public void SetRigidbodies()
    {
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        Debug.Log("Rigibody set! " + rigidbodies.Length, this.gameObject);
    }

    [ContextMenu("BindForceHip")]
    public void BindForceHip()
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>(true))
        {
            if (child.name == "Hips")
            {
                forceHip = child;
                break;
            }
        }

        if (forceHip == null) Debug.Log("Hips transform not found!", this.gameObject);
    }

    [ContextMenu("DeactiveRagdoll")]
    public void DeactiveRagdoll()
    {
        if (rigidbodies.Length == 0)
        {
            Debug.Log("Rigibodies not set!", this.gameObject);
            return;
        }

        foreach (var rigidBody in rigidbodies)
        {
            rigidBody.useGravity = false;
            rigidBody.isKinematic = true;
        }
#if UNITY_EDITOR
        Debug.Log("Deactive Ragdoll! ", this.gameObject);
#endif
    }

    [ContextMenu("ActivateRagdoll")]
    public void ActivateRagdoll()
    {
        if (rigidbodies.Length == 0)
        {
            Debug.Log("Rigibodies not set!", this.gameObject);
            return;
        }

        foreach (var rigidBody in rigidbodies)
        {
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;
        }
#if UNITY_EDITOR
        Debug.Log("Activate Ragdoll! ", this.gameObject);
#endif
    }

    public void ApplyForce(Vector3 force)
    {
        ActivateRagdoll();
        if (forceHip != null)
            forceHip.GetComponent<Rigidbody>().AddForce(force, ForceMode.VelocityChange);
    }
}
