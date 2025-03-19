using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour, IInteractable
{
    public Action<IInteractable> OnInteractionComplete { get; set; }

    public string InteractionText => throw new NotImplementedException();

    //定义梯子的攀爬范围
    [SerializeField] private Transform ladderBottomPos;
    [SerializeField] private float ladderHeight;
    [SerializeField] private float allowInteractDistance = 1f;

    //当从上或下离开梯子时会移动到什么位置
    public Transform bottomReleasePoint;
    public Transform topReleasePoint;

    public bool Interact(Interactor interactor)
    {
        //if ((GameManager.Instance.CharacterControl.transform.position - transform.position).magnitude <= allowInteractDistance)
        //{

        //    return false;
        //}

        return false;
    }

    public void EndInteraction()
    {

    }

    public void OnHoverEnter(Interactor interactor)
    {

    }

    public void OnHoverExit(Interactor interactor)
    {

    }
}
