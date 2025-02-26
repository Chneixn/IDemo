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
        private EnemyController col;
        private Timer timer;

        protected override void OnStart()
        {
            col = agent as EnemyController;
            if (col == null) return;

            if (timer == null)
                timer = TimerManager.CreateTimer();

            timer.StartTiming(targetUpdateTime, repeateTime: 0, onCompleted: () =>
            {
                if (blackboard.TargetTransform != null)
                {
                    col.nav.destination = blackboard.TargetTransform.position;
                }
            });

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (col == null) return State.Failure;

            if (blackboard.TargetTransform != null || col.nav.pathPending || !col.nav.isOnNavMesh || col.nav.remainingDistance > 0.1f)
                return State.Running;

            col.nav.destination = range * Random.insideUnitCircle;
            return State.Success;
        }
    }
}
