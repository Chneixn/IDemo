using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    #region Members
    private LineRenderer trajectoryLine;

    [SerializeField, Tooltip("第一次碰撞发生的位置")]
    private Transform hitMarker;

    [SerializeField, Range(10, 100), Tooltip("预测曲线的分辨率，点数越多，预测曲线越平滑")]
    private int maxPoints = 50;

    [SerializeField, Range(0.01f, 0.5f), Tooltip("用于计算轨迹曲线的时间增量")]
    private float increment = 0.025f;

    [SerializeField, Range(1.05f, 2f), Tooltip("两预测点之间的碰撞检测距离倍率")]
    private float rayOverlap = 1.1f;

    [SerializeField, Tooltip("检测碰撞的层")]
    private LayerMask layerMask;
    #endregion

    private void Start()
    {
        if (trajectoryLine == null)
            trajectoryLine = GetComponent<LineRenderer>();

        SetTrajectoryVisible(true);
    }

    public void PredictTrajectory(ProjectileProperties projectile)
    {
        Vector3 velocity = projectile.direction * (projectile.initialSpeed / projectile.mass);
        Vector3 position = projectile.initialPosition;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(maxPoints, (0, position));

        for (int i = 1; i < maxPoints; i++)
        {
            // 计算下一个点的速度和位置
            velocity = CalculateNewVelocity(velocity, projectile.drag, increment);
            nextPosition = position + velocity * increment;

            // 在两预测点之间的距离增加一定长度，避免精度不够而忽略平面
            overlap = Vector3.Distance(position, nextPosition) * rayOverlap;

            // 当检测到可碰撞的平面，则停止预测，并显示碰撞点
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap, layerMask))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                MoveHitMarker(hit);
                break;
            }

            // 当预测曲线终点没有碰撞，只渲染预测曲线
            hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            UpdateLineRender(maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    /// <summary>
    /// 预测轨迹渲染的点数和点坐标
    /// </summary>
    /// <param name="count">曲线的点数</param>
    /// <param name="pointPos">对应点的位置</param>
    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        trajectoryLine.positionCount = count;
        trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    /// <summary>
    /// 显示碰撞点
    /// </summary>
    /// <param name="hit"></param>
    private void MoveHitMarker(RaycastHit hit)
    {
        hitMarker.gameObject.SetActive(true);

        hitMarker.position = hit.point;
        hitMarker.rotation = Quaternion.identity;
    }

    /// <summary>
    /// 是否显示预测轨迹
    /// </summary>
    /// <param name="visible"></param>
    public void SetTrajectoryVisible(bool visible)
    {
        trajectoryLine.enabled = visible;
        hitMarker.gameObject.SetActive(visible);
    }
}
