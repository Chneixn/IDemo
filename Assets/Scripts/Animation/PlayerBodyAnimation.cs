using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBodyAnimation : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private CharacterControl character;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void Init(CharacterControl character)
    {
        this.character = character;
    }

    public void UpdateAnimations(IMovementState state)
    {
        string animationName = nameof(state);

        if (HasAnimation(animationName))
            animator.CrossFadeInFixedTime(animationName, 0.2f);
    }

    private bool HasAnimation(string name, int layerIndex = 0)
    {
        return animator.HasState(layerIndex, Animator.StringToHash(name));
    }
}
