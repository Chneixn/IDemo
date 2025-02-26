using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Idle : IMovementState
{
    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection.sqrMagnitude > 0.01f)
        {
            if (inputs.TryRun)
            {
                CC.ChangeMovementState(typeof(Run));
            }
            else
            {
                CC.ChangeMovementState(typeof(Walking));
            }
        }
        else if (inputs.TryCrouch && CC.IsStableGround)
        {
            CC.ChangeMovementState(typeof(Crouching));
        }
        else if (inputs.TryJump && CC.IsStableGround)
        {
            CC.ChangeMovementState(typeof(Jump));
        }
    }

    public override void OnStateEnter()
    {
        CC.MaxSpeed = 0f;
    }

    public override void OnStateExit(IMovementState newState)
    {

    }
}
