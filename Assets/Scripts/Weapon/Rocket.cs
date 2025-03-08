using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : ExplosionBullet
{
    [Header("TrackSet追踪设置")]
    [Tooltip("角速度")]
    public float Palstance;
    public Transform Target;

    [Tooltip("线性加速度")]
    public float Acceleration;
    public float MaxVelocity;

    protected override void Update()
    {
        base.Update();
        // 追踪逻辑
        if (Target != null)
        {
            // 获取目标方向
            Vector3 _direction = (Target.position - transform.position).normalized;
            // 当前方向与目标方向的角度差
            float _angle = Vector3.Angle(transform.forward, _direction);
            float _needTime = _angle / Palstance;
            if (_needTime < 0.02f)
                transform.forward = _direction;
            else
                transform.forward = Vector3.Slerp(transform.forward, _direction, Time.deltaTime / _needTime).normalized;
            SetRigiBodyVelocity(velocity);
        }
    }
}
