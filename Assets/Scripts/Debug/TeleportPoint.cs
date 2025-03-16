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

    public bool Interact(Interactor interactor)
    {
        if (isChangeScene)
        {
            TeleportToNewScene();
            
            EndInteraction();
            return true;
        }
        return false;
    }

    public void TeleportToNewScene()
    {
        Debug.Log("开始传送！");
    }
}
