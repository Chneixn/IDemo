using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Run : IMovementState
{
    public override MovementState State => MovementState.Run;
    public float runSpeed;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection == Vector3.zero)
        {
            inputs.tryRun = false;
            CC.ChangeMovementState(CC.idle);
        }
        else if (!inputs.tryRun) // Walk
        {
            CC.ChangeMovementState(CC.walking);
        }
        else if (inputs.tryCrouch && CC.IsStableGround) // Crouch
        {
            CC.ChangeMovementState(CC.crouching);
        }
        else if (inputs.tryJump && CC.IsStableGround)
        {
            CC.ChangeMovementState(CC.jump);
        }
    }

    public override void OnStateEnter(MovementState lastState)
    {
        CC.MaxSpeed = 10f;
    }

    public override void OnStateExit(MovementState newState)
    {
        
    }
}
