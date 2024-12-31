using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMemory
{
    public float Age { get { return Time.time - lastSeen; } }
    public GameObject gameObject;
    public Vector3 position;
    public Vector3 direction;
    public float distance;
    public float angle;
    public float lastSeen;
    public float score;
}

public class AISensoryMemory
{
    public List<AIMemory> memories = new();
    GameObject[] characters;

    public AISensoryMemory(int maxPlayers)
    {
        characters = new GameObject[maxPlayers];
    }

    public void UpdateSenses(AISensor sensor, LayerMask targetLayer)
    {
        int targets = sensor.Filter(characters, targetLayer);
        for (int i = 0; i < targets; i++)
        {
            GameObject target = characters[i];
            RefreshMemory(sensor.gameObject, target);
        }
    }

    public void RefreshMemory(GameObject agent, GameObject target)
    {
        AIMemory memory = FetchMemory(target);
        memory.gameObject = target;
        memory.position = target.transform.position;
        memory.direction = target.transform.position - agent.transform.position;
        memory.distance = memory.direction.magnitude;
        memory.angle = Vector3.Angle(agent.transform.forward, memory.direction);
        memory.lastSeen = Time.time;
    }

    public AIMemory FetchMemory(GameObject gameObject)
    {
        AIMemory memory = memories.Find(x => x.gameObject == gameObject);
        if (memory == null)
        {
            memory = new();
            memories.Add(memory);
        }

        return memory;
    }

    public void ForgetMemory(float olderThen)
    {
        memories.RemoveAll(m => m.Age > olderThen);
        memories.RemoveAll(m => !m.gameObject);
        memories.RemoveAll(m =>
        {
            if (m.gameObject.TryGetComponent(out CharacterStateHolder stateHolder))
                return stateHolder.healthState.IsDead;
            return false;
        });
    }
}
