using System.Collections;
using System.Collections.Generic;
using BehaviourTreeSystem;
using UnityEngine;

[RequireComponent(typeof(ZombieMover))]
public class ZombieAgent : BehaviourTreeRunner
{
    public AgentMover agentMover;
    
    protected override void Start()
    {
        base.Start();
        tree.blackboard.gameObject = gameObject;
        tree.blackboard.transform = transform;
        tree.blackboard.mover = agentMover;
    }

}
