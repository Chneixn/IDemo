using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;
using UnityEngine.AI;

public class ZombieMover : AgentMover
{
    [SerializeField] private NavMeshAgent agent;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) agent = gameObject.AddComponent<NavMeshAgent>();
    }

    public override void MoveTo(Vector3 position)
    {
        destination = position;
        StartCoroutine(AgentMove());
    }

    public override void Move()
    {
        StartCoroutine(AgentMove());
    }

    private IEnumerator AgentMove()
    {
        if (state == AgentState.Moveing || state == AgentState.FindingPath) yield return null;

        agent.speed = speed * speedMultiple;
        agent.stoppingDistance = stoppingDistance;
        agent.updateRotation = updateRotation;
        agent.angularSpeed = angularSpeed;
        agent.acceleration = acceleration;
        agent.autoBraking = autoBraking;
        agent.autoRepath = autoRepath;
        agent.isStopped = false;

        if (agent.SetDestination(destination))
        {
            state = AgentState.FindingPath;
            yield return new WaitUntil(() => agent.hasPath);
            state = AgentState.Moveing;
            pathStatus = GetPathStatus();

            yield return new WaitUntil(() => agent.remainingDistance < tolerance);
            agent.isStopped = true;
        }
        state = AgentState.Waiting;
    }

    public override PathStatus GetPathStatus()
    {
        return agent.pathStatus switch
        {
            NavMeshPathStatus.PathComplete => PathStatus.PathComplete,
            NavMeshPathStatus.PathPartial => PathStatus.PathPartial,
            NavMeshPathStatus.PathInvalid => PathStatus.PathInvalid,
            _ => PathStatus.PathInvalid,
        };
    }
}
