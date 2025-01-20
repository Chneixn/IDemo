using KinematicCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementState : ICharacterController
{
    MovementState State { get; }
    void OnStateExit(MovementState newState);
    void OnStateEnter(MovementState lastState);
}
