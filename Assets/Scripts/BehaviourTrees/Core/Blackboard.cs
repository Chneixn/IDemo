using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    /// <summary>
    /// 用于存储需要公开的属性，记录等
    /// </summary>
    [System.Serializable]
    public class Blackboard
    {
        [Tooltip("Runner mover")]
        public AgentMover mover;
        [Tooltip("Runner gameObject")]
        public GameObject gameObject;
        [Tooltip("Runner transform")]
        public Transform transform;
        [Tooltip("Runner found target?")]
        public bool HasTarget => TargetObj != null;
        [Tooltip("Target which runner fond")]
        public GameObject TargetObj;
        [Tooltip("Runner want to move to")]
        public Vector3 TargetPos;
    }
}