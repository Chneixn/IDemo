using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    /// <summary>
    /// 只要有一个节点成功, 则返回成功
    /// </summary>
    public class SelectorNode : CompositeNode
    {
        protected int current;
        protected override void OnStart() { current = 0; }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            Node child = children[current];
            switch (child.Update())
            {
                case State.Running:
                    return State.Running;
                case State.Failure:
                    current++;
                    break;
                case State.Success:
                    return State.Success;
            }

            return current == children.Count ? State.Failure : State.Running;
        }
    }
}