using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 投掷体的属性
/// </summary>
public struct ProjectileProperties
{
    public Vector3 direction;
    public Vector3 initialPosition; // 初始位置
    public float initialSpeed;  // 初始速度
    public float mass;  // 质量
    public float drag;  // 阻力
}

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    TrajectoryPredictor trajectoryPredictor;

    [SerializeField, Tooltip("投掷物体")]
    private Rigidbody objectToThrow;

    [SerializeField, Range(0.0f, 50.0f), Tooltip("投掷体的初速")]
    private float force;

    [SerializeField, Tooltip("投掷起始位置")]
    private Transform StartPosition;

    void OnEnable()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        if (StartPosition == null)
            StartPosition = transform;

    }

    private void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    private ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new();
        Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

        properties.direction = StartPosition.forward;
        properties.initialPosition = StartPosition.position;
        properties.initialSpeed = force;
        properties.mass = r.mass;
        properties.drag = r.drag;

        return properties;
    }

    void ThrowObject()
    {
        Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);
    }
}
