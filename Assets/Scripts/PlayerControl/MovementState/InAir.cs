using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InAir : IMovementState
{
    public override MovementState State => MovementState.InAir;
    public float maxAirSpeed;
    public float airMovementSharpness;
    public Timer airTimer;
    public Vector3 groundLeavePosition = new(0, 0, 0);
    public float fall_time;
    public float fall_high;
    public InAir()
    {
        airTimer = TimerManager.CreateTimer();
    }

    public override void HandleStateChange(ref PlayerCharacterInput inputs)
    {

    }

    public override void OnStateEnter(MovementState lastState)
    {
        if (!airTimer.IsActive)
        {
            airTimer.StartCounting();
            groundLeavePosition = CC.transform.position;
        }
    }

    public override void OnStateExit(MovementState newState)
    {
        if (airTimer.IsActive)
        {
            fall_time = airTimer.StopTimer();
            fall_high = Mathf.Abs(CC.transform.position.y - groundLeavePosition.y);
        }
    }

    public override void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
    {
        //空中移动
        if (CC.MoveDirection.sqrMagnitude > 0.01f)
        {
            var targetMovementVelocity = CC.MoveDirection * maxAirSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity,
            1 - Mathf.Exp(-airMovementSharpness * deltaTime)
        );
        }
    }

    public override void PostGroundingUpdate(float deltaTime)
    {
        if (CC.IsStableGround && !CC.Motor.LastGroundingStatus.IsStableOnGround)
        {
            OnLanded();
        }
    }

    private void OnLanded()
    {
        CC.ChangeMovementState(CC.idle);
    }
}
