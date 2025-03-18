using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Running : IMovementState
{
    public float runSpeed;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection == Vector3.zero)
        {
            inputs.TryRun = false;
            CC.ChangeMovementState(typeof(Idle));
        }
        else if (!inputs.TryRun) // Walk
        {
            CC.ChangeMovementState(typeof(Walking));
        }
        else if (inputs.TryCrouch && CC.IsStableGround) // Crouch
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
        CC.MaxSpeed = 10f;
    }

    public override void OnStateExit(IMovementState newState)
    {
        
    }
}
