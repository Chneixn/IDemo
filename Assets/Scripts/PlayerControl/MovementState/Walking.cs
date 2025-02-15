using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Walking : IMovementState
{
    public override MovementState State => MovementState.Walking;
    public float walkingSpeed;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection == Vector3.zero)
        {
            CC.ChangeMovementState(CC.idle);
        }
        else if (inputs.TryRun) // Walk
        {
            CC.ChangeMovementState(CC.run);
        }
        else if (inputs.TryCrouch && CC.IsStableGround) // Crouch
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
        CC.MaxSpeed = walkingSpeed;
    }

    public override void OnStateExit(MovementState newState)
    {

    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        base.UpdateVelocity(ref currentVelocity, deltaTime);

        if (!CC.IsAnyGround && CC.EnableGravity)
        {
            CC.ChangeMovementState(CC.inAir);
        }
    }
}
