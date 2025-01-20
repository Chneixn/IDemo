using System;

public interface IInteractable
{
    public Action<IInteractable> OnInteractionComplete { get; set; }

    public void Interact(Interactor interactor, out bool interactSuccessful);

    public void EndInteraction();
}
