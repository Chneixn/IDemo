using System;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public bool isChangeScene;
    public Vector3 positionToGo;

    public Action<IInteractable> OnInteractionComplete { get; set; }

    public void EndInteraction()
    {
        OnInteractionComplete?.Invoke(this);
    }

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if (isChangeScene)
        {
            TeleportToNewScene();
            interactSuccessful = true;
            EndInteraction();
            return;
        }
        interactSuccessful = false;
    }

    public void TeleportToNewScene()
    {
        Debug.Log("开始传送！");
    }
}
