using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpringCalculater
{
    public bool active = false;

    [Header("Taget")]
    [Tooltip("弹簧另一点的位置")]
    public Vector3 TargetPosition = Vector3.zero;
    public Vector3 TargetVelocity = Vector3.zero;
    [Header("Self")]
    public Vector3 SelfPosition = Vector3.zero;
    public Vector3 SelfVelocity = Vector3.zero;
    [Tooltip("自身重量")]
    public float Mass = 1f;
    [Tooltip("弹簧长度")]
    public float Length = 5f;
    [Tooltip("弹簧最大长度")]
    public float maxDistance = Mathf.Infinity;
    [Tooltip("弹簧最短长度")]
    public float minDistance = 0f;
    [Tooltip("弹簧的振动频率")]
    public float UAF = 8f;
    [Tooltip("弹簧的阻尼比 Damping Ratio (DR)")]
    public float DR = 0.8f;
    [Tooltip("重力")]
    public Vector3 gravity = new(0f, -9.81f, 0f);
    [Header("Result")]
    [SerializeField, Tooltip("当前自身应有的速度")] private Vector3 value = Vector3.zero;
    public Vector3 Value => value;

    public void UpdateValue(float dt)
    {
        if (active)
        {
            // 计算弹簧拉伸的长度
            float distance = Vector3.Distance(SelfPosition, TargetPosition) - Length;

            // 当前点相对另一点的方向(归一化)
            Vector3 direction = (TargetPosition - SelfPosition).normalized;

            Vector3 tmp_v;
            // // 下一帧立刻到达最小/大距离
            // if (distance < minDistance)
            // {
            //     tmp_v = -1 * distance * direction;
            // }
            // else if (distance > maxDistance)
            // {
            //     tmp_v = distance * direction;
            // }
            // else
            {
                // 计算当前受到的弹簧力(矢量)
                Vector3 f_spring = UAF * UAF * distance * direction;
                // Vector3 f_spring = UAF * UAF * (TargetPosition - SelfPosition);

                // 计算当前收到的阻尼力(矢量)
                Vector3 f_damp = 2.0f * UAF * DR * (TargetVelocity - SelfVelocity);

                // 添加重力计算后的速度(dt使用一阶泰勒级数预测)
                tmp_v = (f_spring + f_damp + gravity * Mass) * dt / (1.0f + dt * 2.0f * DR * UAF) + SelfVelocity;
            }
            value = tmp_v;
        }
    }

    public void Reset()
    {
        TargetPosition = Vector3.zero;
        SelfPosition = Vector3.zero;
        Mass = 1f;
        Length = 1f;
        UAF = 20f;
        DR = 1.8f;
        SelfVelocity = Vector3.zero;
        value = Vector3.zero;
    }
}
