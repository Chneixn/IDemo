using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BehaviourTreeSystem;

public class RandomWalk : ActionNode
{
    [Tooltip("随机范围的半径")]
    public float range = 25.0f;

    protected override void OnStart()
    {
        // 获得的随机坐标为，xz平面上的随机坐标，z轴为0
        var randomXZ = range * Random.insideUnitCircle;
        var randomXYZ = new Vector3(randomXZ.x, 0f, randomXZ.y);
        blackboard.TargetPos = randomXYZ + blackboard.mover.transform.position;
        if (isLog) Debug.Log(blackboard.transform.name + " 获得随机坐标: " + blackboard.TargetPos.ToString());
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (blackboard.mover == null) return State.Failure;
        return State.Success;
    }
}