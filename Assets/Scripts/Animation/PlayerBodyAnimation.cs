using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyAnimation : MonoBehaviour
{
    [SerializeField] private GameObject characterModel;

    [SerializeField] private Animator animator;
    private CharacterControl character;

    private void Awake()
    {
        animator = characterModel.GetComponent<Animator>();
    }

    public void Init(CharacterControl character)
    {
        this.character = character;
    }

    public void UpdateAnimations(CharacterMovementState nextState, CharacterMovementState lastState)
    {
        string animationName = nextState.ToString();

        switch (nextState)
        {
            case CharacterMovementState.Idle:
                {
                    if (lastState == CharacterMovementState.InAir)
                    {
                        animationName = "Jump-Land";
                    }
                }
                break;
            case CharacterMovementState.Walking:
                break;
            case CharacterMovementState.Running:
                break;
            case CharacterMovementState.Jumping:
                {
                    animationName = "Jump-Start";
                }
                break;
            case CharacterMovementState.Crouching:
                break;
            case CharacterMovementState.Sliding:
                break;
            case CharacterMovementState.Swimming:
                break;
            case CharacterMovementState.InAir:
                {
                    if (lastState == CharacterMovementState.Jumping)
                    {
                        animationName = "Jump-MidAir";
                    }
                    else
                    {
                        animationName = "Fall";
                    }
                }
                break;
            case CharacterMovementState.Fly:
                {
                    animationName = "Jump-MidAir";
                }
                break;
            case CharacterMovementState.ClimbingLadder:
                break;
        }

        if (HasAnimation(0, animationName))
            animator.CrossFadeInFixedTime(animationName, 0.2f);
    }

    private bool HasAnimation(int layerIndex, string name)
    {
        return animator.HasState(layerIndex, Animator.StringToHash(name));
    }
}
