using Unity.Collections;
using UnityEngine;

public struct InteractionInput
{
    public bool tryInteraction;
}

public class Interactor : MonoBehaviour
{
    public Camera UsingCam;

    [Header("交互设置")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField] private bool isInteracting;

    private void Start()
    {
        if (UsingCam == null)
        {
            Debug.LogWarning("Cam missing!" + name);
            enabled = false;
        }
    }

    public void ApplyInput(ref InteractionInput inputs)
    {
        if (enabled != true) return;
        bool _tryInteraction = inputs.tryInteraction;

        // 射线检测可交互物品
        Ray _aimRay = UsingCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(_aimRay, out RaycastHit _hitInfo, interactionRange, interactionLayer))
        {
            // 高亮描边
            if (_hitInfo.collider.TryGetComponent(out Outline outline))
            {
                if (outline.enabled != true) outline.enabled = true;
            }

            if (_tryInteraction && !isInteracting)
            {
                if (_hitInfo.collider.TryGetComponent(out IInteractable interactable))
                {
                    StartInteraction(interactable);
                }
            }
        }
    }

    private void StartInteraction(IInteractable interactable)
    {
        isInteracting = interactable.Interact(this);
        if (isInteracting)
        {
            interactable.OnInteractionComplete += EndInteraction;

        }
    }

    public void EndInteraction(IInteractable interactable)
    {
        isInteracting = false;
        interactable.OnInteractionComplete -= EndInteraction;
    }
}
