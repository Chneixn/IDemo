using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviourTreeSystem;

public class MoveToPosition : ActionNode
{
    [Tooltip("移动速度系数")]
    public float speedMultiple = 1f;
    [Tooltip("离目标位置还有多远的路径距离停下")]
    public float stoppingDistance = 0.1f;
    [Tooltip("是否更新旋转")]
    public bool updateRotation = true;
    [Tooltip("沿路径行驶时的最大转弯速度 (deg/s)")]
    public float angularSpeed = 360f;
    [Tooltip("移动加速度")]
    public float acceleration = 40.0f;
    [Tooltip("离目标的容错距离")]
    public float tolerance = 0.1f;
    [Tooltip("代理是否应自动制动以避免超过目标点(建议在巡逻时关闭)")]
    public bool autoBraking = true;
    [Tooltip("代理是否应自动重新寻路")]
    public bool autoRepath = true;

    protected override void OnStart()
    {
        var mover = blackboard.mover;
        if (mover == null) return;
        mover.speedMultiple = speedMultiple;
        mover.stoppingDistance = stoppingDistance;
        mover.updateRotation = updateRotation;
        mover.angularSpeed = angularSpeed;
        mover.acceleration = acceleration;
        mover.tolerance = tolerance;
        mover.autoBraking = autoBraking;
        mover.autoRepath = autoRepath;
        mover.MoveTo(blackboard.TargetPos);
        if (isLog) Debug.Log(blackboard.transform.name + " 开始移动, 目的: " + blackboard.TargetPos.ToString());
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        if (blackboard.mover.State == AgentState.Waiting)
        {
            return State.Success;
        }
        else return State.Running;
    }
}