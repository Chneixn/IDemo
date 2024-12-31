using Unity.Collections;
using UnityEngine;

public struct InteractionInput
{
    public bool tryInteraction;
}

public class Interactor
{
    [Header("绑定")]
    private CameraController playerCam;
    private Camera cam;
    [SerializeField] private Transform objectGrabPos;

    [Header("交互设置")]
    [SerializeField][Min(1)] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayer;
    
    [SerializeField] private bool isInteracting;
    [SerializeField] private IInteractable InteractingItem;
    [SerializeField] private bool isGrabbing;

    public void Init(CameraController playerCam)
    {
        this.playerCam = playerCam;
        cam = playerCam.transform.GetComponent<Camera>();
    }

    public void ApplyInput(ref InteractionInput inputs)
    {
        bool _tryInteraction = inputs.tryInteraction;

        //实例化射线与射线信息
        Ray _aimRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(_aimRay, out RaycastHit _hitInfo, interactionRange, interactionLayer))
        {
            //描边功能
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

                //功能1：握持某物品，被握持物品可与世界物品发生碰撞
                //当未存入grabbedObject时，尝试获取
                if (_hitInfo.collider.TryGetComponent(out IGrabbable grabbedObject))
                {
                    if (!isGrabbing)
                    {
                        isGrabbing = true;
                        grabbedObject.Grab(objectGrabPos);
                    }
                    //按下交互键传入物品需要跟随的位置
                    else
                    {
                        //已存入grabbedObject，则是在尝试丢掉物品
                        grabbedObject.Drop();
                        isGrabbing = false;
                    }
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
            InteractingItem = interactable;
        }
    }

    public void EndInteraction()
    {
        isInteracting = false;
    }
}
