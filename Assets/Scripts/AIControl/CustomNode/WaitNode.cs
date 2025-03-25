using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class WaitNode : ActionNode
    {
        [Tooltip("等待时间")]
        public float duration = 5;
        private float timer = 0f;

        protected override void OnStart()
        {
            timer = duration;
            if (isLog) Debug.Log(blackboard.transform.name + " 开始计时: " + timer);
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            timer -= Time.deltaTime;
            // if (isLog) Debug.Log(blackboard.transform.name + " 计时剩余: " + timer);
            if (timer < 0f)
            {
                if (isLog) Debug.Log(blackboard.transform.name + " 计时结束");
                return State.Success;
            }
            else return State.Running;
        }
    }
}

