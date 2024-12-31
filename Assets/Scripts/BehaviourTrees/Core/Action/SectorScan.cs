using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTreesSystem
{
    public class SectorScan : ActionNode
    {
        public float distance = 10f;
        [Range(0f, 180f)]
        public float angle = 30f;
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

        [Header("Debug")]
        public bool debugDetectRange;
        public bool debugScaner;
        public bool debugTarget;
        public Color debugColor = new(169, 84, 236, 0.5f);
        private SectorScanDebug debug;

        private Collider[] colliders = new Collider[50];
        private int count;
        private float timePerScan;
        private Timer timer;

        protected override void OnStart()
        {
            if (debug == null)
            {
                if (enemyControl.gameObject.TryGetComponent(out SectorScanDebug scanDebug))
                {
                    debug = scanDebug;
                }
                else debug = enemyControl.gameObject.AddComponent<SectorScanDebug>();

                debug.scan = this;
            }

            if (timer == null)
                timer = TimerManager.CreateTimer();

            timePerScan = 1.0f / scanFrequency;
            timer.StartTiming(timePerScan, repeateTime: 0, onCompleted: () =>
            {
                Scan();
            });
        }

        protected override void OnStop()
        {
            TimerManager.RemoveTimer(timer);
        }

        protected override State OnUpdate()
        {
            return State.Success;
        }

        private void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(enemyControl.transform.position, distance, colliders, scanLayers, QueryTriggerInteraction.Collide);

            objects.Clear();
            for (int i = 0; i < count; ++i)
            {
                GameObject obj = colliders[i].gameObject;
                if (IsInSight(obj))
                {
                    objects.Add(obj);
                }
            }
        }

        public bool IsInSight(GameObject obj)
        {
            Vector3 origin = enemyControl.transform.position;
            Vector3 dest = obj.transform.position;
            Vector3 direction = dest - origin;
            if (direction.y < 0 || direction.y > height) return false;

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, enemyControl.transform.forward);
            if (deltaAngle > angle) return false;

            origin.y += height / 2;
            dest.y = origin.y;
            if (Physics.Linecast(origin, dest, occlusionLayers)) return false;

            return true;
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

