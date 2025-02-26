using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Jump : IMovementState
{
    public float jumpForce;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {

    }

    public override void OnStateEnter()
    {
        CC.InternalVelocity = CC.Motor.CharacterUp * jumpForce;
    }

    public override void OnStateExit(IMovementState newState)
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
