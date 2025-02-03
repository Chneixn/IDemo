using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Freeze : IMovementState
{
    public override MovementState State => MovementState.Freeze;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        
    }

    public override void OnStateEnter(MovementState lastState)
    {
        CC.EnableGravity = false;
        CC.MaxSpeed = 0f;
    }
    public override void OnStateExit(MovementState newState)
    {
        CC.EnableGravity = true;
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        // 停止速度，彻底冻结
        currentVelocity = CC.Motor.CharacterForward * 0f;
    }

    public override void PostGroundingUpdate(float deltaTime)
    {
        
    }
}
