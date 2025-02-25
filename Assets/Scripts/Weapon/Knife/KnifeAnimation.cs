using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnifeAnimation : MonoBehaviour
{
    public Knife knife;
    public Animator animator;

    private void Awake()
    {
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (knife == null) knife = GetComponent<Knife>();
    }

    private void Start()
    {
        knife.OnEnableWeapon += OnEnableWeapon;
        knife.OnDisableWeapon += OnDisableWeapon;
        knife.OnAttack += OnAttack;

        PlayerManager.Instance.CharacterControl.OnMovementStateChanged += OnMovementStateChanged;
    }

    private void OnMovementStateChanged(MovementState state)
    {
        if (state == MovementState.Walking)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isRunning", false);
        }
        else if (state == MovementState.Run)
        {
            animator.SetBool("isRunning", true);
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
        }
    }

    private void OnDisableWeapon()
    {
        animator.SetTrigger("isDisable");
    }

    private void OnEnableWeapon()
    {

    }

    private void OnAttack(bool lit)
    {
        if (lit)
            animator.SetTrigger("Attack_1");
        else animator.SetTrigger("Attack_2");
    }
}
