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

    public void UpdateAnimations(MovementState nextState, MovementState lastState)
    {
        string animationName = nextState.ToString();

        switch (nextState)
        {
            case MovementState.Idle:
                {
                    if (lastState == MovementState.InAir)
                    {
                        animationName = "Jump-Land";
                    }
                }
                break;
            case MovementState.Walking:
                break;
            case MovementState.Running:
                break;
            case MovementState.Jumping:
                {
                    animationName = "Jump-Start";
                }
                break;
            case MovementState.Crouching:
                break;
            case MovementState.Sliding:
                break;
            case MovementState.Swimming:
                break;
            case MovementState.InAir:
                {
                    if (lastState == MovementState.Jumping)
                    {
                        animationName = "Jump-MidAir";
                    }
                    else
                    {
                        animationName = "Fall";
                    }
                }
                break;
            case MovementState.Fly:
                {
                    animationName = "Jump-MidAir";
                }
                break;
            case MovementState.ClimbingLadder:
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
