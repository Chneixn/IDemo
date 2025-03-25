using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

public class AIState : SelectorNode
{
    protected override State OnUpdate()
    {
        Node child = children[current];
        child.Update();
        current++;
        if (current >= children.Count) current = 0;

        return State.Running;
    }
}
