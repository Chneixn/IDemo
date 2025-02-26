using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class WaitNode : ActionNode
    {
        public float duration = 1;
        Timer timer;

        protected override void OnStart()
        {
            if (timer == null)
                timer = TimerManager.CreateTimer();
        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            State state = State.Running;
            if (timer.IsEnd)
                timer.StartTiming(duration, onCompleted: () => { state = State.Success; });
            return state;
        }
    }
}

