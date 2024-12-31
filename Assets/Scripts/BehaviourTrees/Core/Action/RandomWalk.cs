using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourTreesSystem
{
    public class RandomWalk : ActionNode
    {
        public float range = 25.0f;
        public float targetUpdateTime = 1.0f;
        private NavMeshAgent nmAgent;
        private Timer timer;

        protected override void OnStart()
        {
            if (enemyControl.agent != null || !nmAgent) nmAgent = enemyControl.agent;
            else return;

            if (timer == null)
                timer = TimerManager.CreateTimer();

            timer.StartTiming(targetUpdateTime, repeateTime: 0, onCompleted: () =>
            {
                if (blackboard.TargetTransform != null)
                {
                    nmAgent.destination = blackboard.TargetTransform.position;
                }
            });

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (nmAgent == null) return State.Failure;
                
            if (blackboard.TargetTransform != null || nmAgent.pathPending || !nmAgent.isOnNavMesh || nmAgent.remainingDistance > 0.1f)
                return State.Running;

            nmAgent.destination = range * Random.insideUnitCircle;
            return State.Success;
        }
    }
}
