using System.Collections;
using System.Collections.Generic;
using BehaviourTreesSystem;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStateHolder))]
[RequireComponent(typeof(RagdollControl))]
public class ZombieAgent : Agent
{
    public NavMeshAgent nav;


    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        
    }

}
