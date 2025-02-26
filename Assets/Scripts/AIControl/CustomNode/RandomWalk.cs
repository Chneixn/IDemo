using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using BehaviourTreesSystem;

public class RandomWalk : ActionNode
{
    public float range = 25.0f;
    public float targetUpdateTime = 1.0f;
    private NavMeshAgent nav;
    private Timer timer;

    protected override void OnStart()
    {
        nav = (agent as ZombieAgent).nav;
        if (nav == null) return;

        timer ??= TimerManager.CreateTimer();

        timer.StartTiming(targetUpdateTime, repeateTime: 0, onCompleted: () =>
        {
            if (blackboard.TargetTransform != null)
            {
                nav.destination = blackboard.TargetTransform.position;
            }
        });

    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (nav == null) return State.Failure;

        if (blackboard.TargetTransform != null || nav.pathPending || !nav.isOnNavMesh || nav.remainingDistance > 0.1f)
            return State.Running;

        nav.destination = range * Random.insideUnitCircle;
        return State.Success;
    }
}