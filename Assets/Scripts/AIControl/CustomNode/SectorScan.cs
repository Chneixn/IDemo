using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreeSystem
{
    public class SectorScan : ActionNode
    {
        [Tooltip("是否扫描到目标才返回 Success")]
        public bool scanToContinue = true;
        [Tooltip("扫描半径")]
        public float distance = 10f;
        [Tooltip("扫描角度"), Range(0f, 360f)]
        public float angle = 30f;
        [Tooltip("扫描范围的高度")]
        public float height = 1.0f;
        [Tooltip("扫描频率")]
        public int scanFrequency = 30;
        [Tooltip("扫描层")]
        public LayerMask scanLayers;
        [Tooltip("阻挡层")]
        public LayerMask occlusionLayers;
        private List<GameObject> objects = new();
        public List<GameObject> Objects
        {
            get
            {
                objects.RemoveAll(obj => !obj);
                return objects;
            }
        }

        #region Debug Viusal
        [Header("Debug")]
        public bool enableDebug = false;
        public bool debugDetectRange;
        public bool debugScaner;
        public bool debugTarget;
        public Color debugColor = new(169, 84, 236, 0.5f);
        private SectorScanDebug debugger;
        #endregion

        private readonly Collider[] colliders = new Collider[50];
        private float TimePerScan => 1.0f / scanFrequency;
        private float timer = 0f;

        protected override void OnStart()
        {

        }

        protected override void OnStop()
        {

        }

        protected override State OnUpdate()
        {
            timer -= Time.deltaTime;
            if (timer < 0f)
            {
                timer = TimePerScan;
                Scan();
                if (scanToContinue)
                {
                    if (objects.Count > 0) return State.Success;
                }
                else return State.Success;
            }

            if (enableDebug && debugger == null)
            {
                debugger = blackboard.gameObject.AddComponent<SectorScanDebug>();
                debugger.scan = this;
            }

            return State.Failure;
        }

        private void Scan()
        {
            if (blackboard.transform == null)
            {
                Debug.LogError("Blackboard transform is null in SectorScan.Scan method.");
                return;
            }

            int count = Physics.OverlapSphereNonAlloc(blackboard.transform.position, distance, colliders, scanLayers, QueryTriggerInteraction.Collide);
            
            if (isLog) Debug.Log(blackboard.transform.name + " 物理检测目标数: " + count);
            objects.Clear();
            if (count > 0) blackboard.TargetObj = colliders[0].gameObject;
            for (int i = 0; i < count; i++)
            {
                GameObject obj = colliders[i].gameObject;
                if (IsInSight(obj))
                {
                    objects.Add(obj);
                }
            }
        }

        // FIXME: 当前目标检测有问题
        public bool IsInSight(GameObject obj)
        {
            bool result = true;
            Vector3 origin = blackboard.transform.position;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;
            // 如果物体在扫描高度范围外，则忽略该物体
            if (direction.y < 0 || direction.y > height) result = false;

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, blackboard.transform.forward);
            if (deltaAngle > angle * 0.5f) result = false;

            origin.y += height * 0.5f;
            dest.y = origin.y;
            if (Physics.Linecast(origin, dest, occlusionLayers)) result = false;

            if (isLog) {
                if (result) Debug.Log(blackboard.transform.name + " 目标在视线内: " + obj.name);
                else Debug.Log(blackboard.transform.name + " 目标不在视线内: " + obj.name);
            }
            
            return result;
        }

        /// <summary>
        /// 获取扫描器中指定Layer层的物体(该Layer层需要在扫描层中)
        /// </summary>
        /// <param name="buffer">装载物体的数组</param>
        /// <param name="layerName">获取物体的层级名</param>
        /// <returns></returns>
        public int Filter(GameObject[] buffer, string layerName)
        {
            int layer = LayerMask.NameToLayer(layerName);
            int count = 0;
            foreach (var obj in objects)
            {
                if (obj.layer == layer) buffer[count++] = obj;

                if (buffer.Length == count) break;
            }

            return count;
        }
    }
}

