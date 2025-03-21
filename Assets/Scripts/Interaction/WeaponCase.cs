using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCase : MonoBehaviour, IInteractable
{
    public bool isOpen = false;
    public GameObject WeaponVisual;
    public IWeapon weaponPrefab;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public string InteractionText
    {
        get
        {
            if (isOpen) return "关闭武器箱";
            else return "打开武器箱";
        }
    }
    
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTime = 0.15f;
    [SerializeField] private bool isAnimating = false;

    public void EndInteraction()
    {
        OnInteractionComplete?.Invoke(this);
    }

    public bool Interact(Interactor interactor)
    {
        if (isAnimating) return false;
        if (!isOpen)
        {
            isOpen = true;
            if (animator == null) animator = GetComponent<Animator>();
            animator.SetBool("Open", true);
            StartCoroutine(Count());
        }
        else if (WeaponVisual.activeSelf)
        {
            var holder = PlayerManager.Instance.WeaponHolder;
            holder.AddWeapon(weaponPrefab);
            WeaponVisual.SetActive(false);
        }
        else
        {
            isOpen = false;
            animator.SetBool("Close", true);
            StartCoroutine(Count());
        }
        EndInteraction();
        return false;
    }

    private IEnumerator Count()
    {
        yield return new WaitForSeconds(animationTime);
        isAnimating = false;
        animator.SetBool("Close", false);
        animator.SetBool("Open", false);
    }

    public void OnHoverEnter(Interactor interactor)
    {
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = true;
        }
    }

    public void OnHoverExit(Interactor interactor)
    {
        if (TryGetComponent(out Outline outline))
        {
            outline.enabled = false;
        }
    }
}
