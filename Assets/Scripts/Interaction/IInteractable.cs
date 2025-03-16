using System;

public interface IInteractable
{
    public Action<IInteractable> OnInteractionComplete { get; set; }

    /// <summary>
    /// 交互
    /// </summary>
    /// <param name="interactor"></param>
    /// <returns>交互是否成功</returns>
    public bool Interact(Interactor interactor);

    public void EndInteraction();
}
