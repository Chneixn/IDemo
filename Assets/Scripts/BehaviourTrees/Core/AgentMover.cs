using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public enum AgentState
    {
        FindingPath,
        Waiting,
        Moveing
    }

    public enum PathStatus
    {
        /// <summary>
        /// The path terminates at the destination.
        /// </summary>
        PathComplete,

        /// <summary>
        /// The path cannot reach the destination.
        /// </summary>
        PathPartial,

        /// <summary>
        /// The path is not valid.
        /// </summary>
        PathInvalid
    }

    public abstract class AgentMover : MonoBehaviour
    {
        [Tooltip("移动状态")]
        [SerializeField] protected AgentState state = AgentState.Waiting;
        public AgentState State => state;

        public float speed = 3f;
        public Vector3 destination = Vector3.zero;

        [Tooltip("移动速度系数")]
        public float speedMultiple = 1f;
        [Tooltip("离目标位置还有多远的路径距离停下")]
        public float stoppingDistance = 0.1f;
        [Tooltip("是否更新旋转")]
        public bool updateRotation = true;
        [Tooltip("沿路径行驶时的最大转弯速度 (deg/s)")]
        public float angularSpeed = 720f;
        [Tooltip("移动加速度")]
        public float acceleration = 40.0f;
        [Tooltip("离目标的容错距离")]
        public float tolerance = 0.1f;
        [Tooltip("代理是否应自动制动以避免超过目标点(建议在巡逻时关闭)")]
        public bool autoBraking = true;
        [Tooltip("代理是否应自动重新寻路")]
        public bool autoRepath = true;

        [SerializeField] protected PathStatus pathStatus = PathStatus.PathInvalid;
        public PathStatus PathStatus => pathStatus;

        /// <summary>
        /// 移动到指定位置
        /// </summary>
        /// <param name="position"></param>
        public abstract void MoveTo(Vector3 position);
        /// <summary>
        /// 移动到 destination 位置
        /// </summary>
        public abstract void Move();
        public abstract PathStatus GetPathStatus();
    }
}