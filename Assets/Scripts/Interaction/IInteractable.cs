using System;

public interface IInteractable
{
    public Action<IInteractable> OnInteractionComplete { get; set; }

    public void OnHoverEnter(Interactor interactor);
    public void OnHoverExit(Interactor interactor);
    public bool Interact(Interactor interactor);
    public void EndInteraction();
}
