using System;
using Unity.Collections;
using UnityEngine;

public struct InteractionInput
{
    public bool tryInteraction;
}

public class Interactor : MonoBehaviour
{
    public Camera UsingCam;

    public Action<IInteractable> OnHoverEnter;
    public Action<IInteractable> OnHoverExit;

    [Header("交互设置")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField] private bool isInteracting;
    private IInteractable interactingObject;

    [SerializeField] private bool Log;
    private string lastObject;

    private void Start()
    {
        if (UsingCam == null)
        {
            if (Log) Debug.LogWarning("Auto set UsingCam to PlayerCam.brain.OutputCamera");
            UsingCam = PlayerManager.Instance.PlayerCam.brain.OutputCamera;
        }
    }

    public void ApplyInput(ref InteractionInput inputs)
    {
        if (!enabled) return;
        bool _tryInteraction = inputs.tryInteraction;

        // 射线检测可交互物品
        Ray _aimRay = UsingCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(_aimRay, out RaycastHit _hitInfo, interactionRange, interactionLayer))
        {
            // if (Log) Debug.Log("检测到物体: " + _hitInfo.collider.name);
            // 当检测到可交互物品时
            if (!isInteracting && _hitInfo.collider.TryGetComponent(out IInteractable interactable))
            {
                if (interactingObject != interactable)
                {
                    HoverExited();
                    interactingObject = interactable;
                    interactable.OnHoverEnter(this);
                    if (Log)
                    {
                        lastObject = _hitInfo.collider.name;
                        Debug.Log("物体: " + lastObject + " 获取焦点");
                    }
                }

                if (_tryInteraction)
                {
                    if (Log) Debug.Log("尝试与物体交互: " + _hitInfo.collider.name);
                    StartInteraction(interactable);
                }
            }
        }
        else HoverExited();
    }

    private void HoverExited()
    {
        if (interactingObject != null)
        {
            interactingObject.OnHoverExit(this);
            OnHoverExit?.Invoke(interactingObject);
            if (Log) Debug.Log("物体: " + lastObject + " 失去焦点");
            interactingObject = null;
        }
    }

    private void StartInteraction(IInteractable interactable)
    {
        isInteracting = true;
        interactable.OnInteractionComplete += EndInteraction;
        interactable.Interact(this);
    }

    public void EndInteraction(IInteractable interactable)
    {
        if (Log) Debug.Log("结束与物体的交互");
        isInteracting = false;
        interactable.OnInteractionComplete -= EndInteraction;
    }
}
