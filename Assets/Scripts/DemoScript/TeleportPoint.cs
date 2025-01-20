using System;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;
    public bool isChangeScene;
    public GameSceneSO sceneToGo;
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
        if (sceneToGo != null)
            loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
