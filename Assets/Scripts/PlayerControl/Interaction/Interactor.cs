using Unity.Collections;
using UnityEngine;

public struct InteractionInput
{
    public bool tryInteraction;
}

public class Interactor : MonoBehaviour
{
    private Camera cam;

    [Header("交互设置")]
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;

    [SerializeField] private bool isInteracting;

    public void ApplyInput(ref InteractionInput inputs)
    {
        bool _tryInteraction = inputs.tryInteraction;

        if (cam == null)
        {
            cam = GetComponentInParent<Camera>();
        }

        // 射线检测可交互物品
        Ray _aimRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(_aimRay, out RaycastHit _hitInfo, interactionRange, interactionLayer))
        {
            // 高亮描边
            if (_hitInfo.collider.TryGetComponent(out Outline outline))
            {
                outline.enabled = true;
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
        interactable.Interact(this, out bool interactSuccessful);
        isInteracting = interactSuccessful;
        if (isInteracting)
        {
            interactable.OnInteractionComplete += EndInteraction;

        }
    }

    public void EndInteraction(IInteractable interactable)
    {
        isInteracting = false;
    }
}
