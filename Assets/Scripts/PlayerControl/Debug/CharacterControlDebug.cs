using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharacterControlDebug : MonoBehaviour
{
    [SerializeField]
    private Transform moveArrow;
    [SerializeField]
    private CharacterControl cc;

    private void Update()
    {
        //if (moveArrow != null && cc != null)
        //    moveArrow.transform.forward =
        //        _moveDirection == Vector3.zero ? motor.CharacterForward : _moveDirection;
    }
}
