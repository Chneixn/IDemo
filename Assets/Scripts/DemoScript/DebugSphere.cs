using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSphere : MonoBehaviour
{
    private CharacterControl characterControl;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out characterControl))
        {
            Debug.Log("character here!" + characterControl.transform.position);
        }
    }
}
