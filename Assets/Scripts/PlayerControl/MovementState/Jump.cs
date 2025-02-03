using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Jump : IMovementState
{
    public override MovementState State => MovementState.Jump;
    public float jumpForce;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {

    }

    public override void OnStateEnter(MovementState lastState)
    {
        CC.InternalVelocity = CC.Motor.CharacterUp * jumpForce;
    }

    public override void OnStateExit(MovementState newState)
    {

    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        CC.Motor.ForceUnground();
        base.UpdateVelocity(ref currentVelocity, deltaTime);
    }

    public override void PostGroundingUpdate(float deltaTime)
    {
        base.PostGroundingUpdate(deltaTime);
    }
}
