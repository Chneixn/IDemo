using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Idle : IMovementState
{
    public override MovementState State => MovementState.Idle;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection.sqrMagnitude > 0.01f)
        {
            if (inputs.TryRun)
            {
                CC.ChangeMovementState(CC.run);
            }
            else
            {
                CC.ChangeMovementState(CC.walking);
            }
        }
        else if (inputs.TryCrouch && CC.IsStableGround)
        {
            CC.ChangeMovementState(CC.crouching);
        }
        else if (inputs.TryJump && CC.IsStableGround)
        {
            CC.ChangeMovementState(CC.jump);
        }
    }

    public override void OnStateEnter(MovementState lastState)
    {
        CC.MaxSpeed = 0f;
    }

    public override void OnStateExit(MovementState newState)
    {

    }
}
