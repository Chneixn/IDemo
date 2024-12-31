using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class AITargetingSystem : MonoBehaviour
{
    public bool withBehaviourTree;

    [Header("遗忘时间")]
    public float forgoetTime = 3.0f;
    [Header("目标权重")]
    public float distanceWeight = 1.0f;
    public float angleWeight = 1.0f;
    public float ageWeight = 1.0f;
    [Header("目标层级")]
    public LayerMask targetLayer;
    private AISensoryMemory memory = new(10);
    private AIMemory foucsMemory;
    private AISensor sensor;

    public bool HasTarget { get { return foucsMemory != null; } }
    public GameObject Target { get { return foucsMemory.gameObject; } }
    public Transform TargetPosition { get { return foucsMemory.gameObject.transform; } }
    public bool TargetInSight { get { return foucsMemory.Age < 0.5f; } }
    public float TargetDistance { get { return foucsMemory.distance; } }

    private void Start()
    {
        sensor = GetComponent<AISensor>();
    }

    private void Update()
    {
        memory.UpdateSenses(sensor, targetLayer);
        memory.ForgetMemory(forgoetTime);

        EvaluateScores();
    }

    private void EvaluateScores()
    {
        foucsMemory = null;

        foreach (var memory in memory.memories)
        {
            memory.score = CalculateScore(memory);
            if (foucsMemory == null || memory.score > foucsMemory.score)
                foucsMemory = memory;
        }
    }

    private float CalculateScore(AIMemory memory)
    {
        //应用权重
        float distanceScore = Normalize(memory.distance, sensor.distance) * distanceWeight;
        float angleScore = Normalize(memory.angle, sensor.angle) * angleWeight;
        float ageScore = Normalize(memory.Age, forgoetTime) * ageWeight;
        return distanceScore + angleScore + ageScore;
    }

    private float Normalize(float value, float maxValue)
    {
        float current = 1.0f - (value / maxValue);
        return current;
    }

    private void OnDrawGizmos()
    {
        float maxScore = float.MinValue;
        foreach (var memory in memory.memories)
        {
            maxScore = MathF.Max(maxScore, memory.score);
        }

        foreach (var memory in memory.memories)
        {
            Color color = Color.red;
            if (memory == foucsMemory) color = Color.green;
            color.a = memory.score / maxScore;
            Gizmos.color = color;
            Gizmos.DrawSphere(memory.position, 0.2f);
        }
    }
}

