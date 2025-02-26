using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Walking : IMovementState
{
    public float walkingSpeed;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        if (CC.MoveDirection == Vector3.zero)
        {
            CC.ChangeMovementState(typeof(Idle));
        }
        else if (inputs.TryRun) // Walk
        {
            CC.ChangeMovementState(typeof(Run));
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
        CC.MaxSpeed = walkingSpeed;
    }

    public override void OnStateExit(IMovementState newState)
    {

    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        base.UpdateVelocity(ref currentVelocity, deltaTime);

        if (!CC.IsAnyGround && CC.Gravity.Enable)
        {
            CC.ChangeMovementState(typeof(InAir));
        }
    }
}
