using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAnimation : MonoBehaviour
{
    protected Animator animator;

    public void OnEnable()
    {
        animator = GetComponent<Animator>();
    }
}
