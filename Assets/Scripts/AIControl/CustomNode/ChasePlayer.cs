using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

public class ChasePlayer : ActionNode
{
    protected override void OnStart()
    {

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (blackboard.HasTarget) return State.Failure;
        blackboard.TargetPos = blackboard.TargetObj.transform.position;
        blackboard.transform.LookAt(blackboard.TargetObj.transform);
        return State.Success;
    }
}
