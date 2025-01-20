using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponAnimation : MonoBehaviour
{
    private Animator animator;

    

    public void Start()
    {
        animator = GetComponent<Animator>();
    }
}
