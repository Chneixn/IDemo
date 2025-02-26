using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTreesSystem;

public class MoveToPosition : ActionNode
{
    NavMeshAgent nav;
    public float speedMultiple = 1f;
    public float stoppingDistance = 0.1f;
    public bool updateRotation = true;
    public float acceleration = 40.0f;  //加速度
    [Tooltip("离目标的容错距离(允许修改下次目标地点")]
    public float tolerance = 1.0f;

    protected override void OnStart()
    {
        nav = (agent as ZombieAgent).nav;
        if (nav == null) return;
        nav.stoppingDistance = stoppingDistance;
        nav.speed = blackboard.MoveSpeed * speedMultiple;
        nav.destination = blackboard.MoveToPosition;
        nav.updateRotation = updateRotation;
        nav.acceleration = acceleration;
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (nav == null) return State.Failure;

        if (nav.pathPending)
        {
            return State.Running;
        }
        else if (nav.remainingDistance < tolerance)
        {
            return State.Success;
        }
        else if (nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
        {
            return State.Failure;
        }

        return State.Running;
    }
}