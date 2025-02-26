using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class MoveToPosition : ActionNode
    {
        EnemyController col;
        public float speedMultiple = 1f;
        public float stoppingDistance = 0.1f;
        public bool updateRotation = true;
        public float acceleration = 40.0f;  //加速度
        [Tooltip("离目标的容错距离(允许修改下次目标地点")]
        public float tolerance = 1.0f;

        protected override void OnStart()
        {
            col = agent as EnemyController;
            if (col == null) return;
            col.nav.stoppingDistance = stoppingDistance;
            col.nav.speed = blackboard.MoveSpeed * speedMultiple;
            col.nav.destination = blackboard.MoveToPosition;
            col.nav.updateRotation = updateRotation;
            col.nav.acceleration = acceleration;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (col == null) return State.Failure;

            if (col.nav.pathPending)
            {
                return State.Running;
            }

            if (col.nav.remainingDistance < tolerance)
            {
                return State.Success;
            }

            if (col.nav.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                return State.Failure;
            }

            return State.Running;
        }
    }
}
