using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    public Action<IInteractable> OnInteractionComplete { get; set; }

    //定义梯子的攀爬范围
    [SerializeField] private Transform ladderBottomPos;
    [SerializeField] private float ladderHeight;
    [SerializeField] private float allowInteractDistance = 1f;

    //当从上或下离开梯子时会移动到什么位置
    public Transform bottomReleasePoint;
    public Transform topReleasePoint;

    public void Interact(Interactor interactor, out bool interactSuccessful)
    {
        if ((GameManager.Instance.CharacterControl.transform.position - transform.position).magnitude <= allowInteractDistance)
        {
            interactSuccessful = GameManager.Instance.PlayerInput.StartClimbLabber(ladderBottomPos, ladderHeight);
            return;
        }

        interactSuccessful = false;
    }

    public void EndInteraction()
    {

    }
}
