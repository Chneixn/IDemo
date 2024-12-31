using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class MoveToPosition : ActionNode
    {
        public float speedMultiple = 1f;
        public float stoppingDistance = 0.1f;
        public bool updateRotation = true;
        public float acceleration = 40.0f;  //加速度
        [Tooltip("离目标的容错距离(允许修改下次目标地点")]
        public float tolerance = 1.0f;

        protected override void OnStart()
        {
            enemyControl.agent.stoppingDistance = stoppingDistance;
            enemyControl.agent.speed = blackboard.MoveSpeed * speedMultiple;
            enemyControl.agent.destination = blackboard.MoveToPosition;
            enemyControl.agent.updateRotation = updateRotation;
            enemyControl.agent.acceleration = acceleration;
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            if (enemyControl.agent.pathPending)
            {
                return State.Running;
            }

            if (enemyControl.agent.remainingDistance < tolerance)
            {
                return State.Success;
            }

            if (enemyControl.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                return State.Failure;
            }

            return State.Running;
        }
    }
}
