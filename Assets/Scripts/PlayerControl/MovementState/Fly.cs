using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Fly : IMovementState
{
    public bool allowFly;
    public float FlySpeed;

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {
        // 移动方向与玩家视角方向相同
        CC.MoveDirection = inputs.LookDirection + CC.MoveDirectionInput;
        if (!inputs.TryFly)
        {
            CC.ChangeMovementState(typeof(InAir));
        }
    }

    public override void OnStateEnter()
    {
        CC.MaxSpeed = FlySpeed;
        CC.Gravity.Enable = false;
        CC.Motor.SetCapsuleCollisionsActivation(false);
        CC.Motor.SetMovementCollisionsSolvingActivation(false);
        CC.Motor.SetGroundSolvingActivation(false);
    }
    public override void OnStateExit(IMovementState newState)
    {
        CC.Gravity.Enable = true;
        CC.Motor.SetCapsuleCollisionsActivation(true);
        CC.Motor.SetMovementCollisionsSolvingActivation(true);
        CC.Motor.SetGroundSolvingActivation(true);
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        var targetMovementVelocity = CC.MoveDirection * FlySpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
            1 - Mathf.Exp(-CC.StableMovementSharpness * deltaTime)
        );
    }
}
