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

        

        if (HasAnimation(0, animationName))
            animator.CrossFadeInFixedTime(animationName, 0.2f);
    }

    private bool HasAnimation(int layerIndex, string name)
    {
        return animator.HasState(layerIndex, Animator.StringToHash(name));
    }
}
