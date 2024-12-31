using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStateHolder))]
[RequireComponent(typeof(RagdollControl))]
public class EnemyController : MonoBehaviour, IPoolObjectItem
{
    public NavMeshAgent agent;
    public CharacterStateHolder stateHolder;
    private List<HitBox> hitBoxes;
    public RagdollControl ragdoll;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        stateHolder = GetComponent<CharacterStateHolder>();
        ragdoll = GetComponent<RagdollControl>();
        animator = GetComponentInChildren<Animator>();

        stateHolder.healthState.OnCharacterDead += OnDead;
    }

    private void OnDead(Vector3 direction)
    {
        if (animator != null)
        {
            animator.enabled = false;
        }

        if (ragdoll != null)
        {
            direction.y = 1f;   //起飞
            ragdoll.ApplyForce(direction);
        }
    }

    [ContextMenu("AddHitBox")]
    public void AddHitBox()
    {
        Rigidbody[] rigids = transform.GetComponentsInChildren<Rigidbody>();
        hitBoxes = new();
        foreach (var go in rigids)
        {
            if (!go.gameObject.TryGetComponent(out HitBox hitBox))
            {
                hitBox = go.gameObject.AddComponent<HitBox>();
            }
            hitBox.stateHolder = this.stateHolder;
            hitBoxes.Add(hitBox);
        }
        Debug.Log("Set " + hitBoxes.Count + " HitBoxes");
    }

    [ContextMenu("RemoveAllHitBox")]
    public void RemoveAllHitBox()
    {
        Rigidbody[] rigids = transform.GetComponentsInChildren<Rigidbody>();
        foreach (var o in rigids)
        {
            if (o.gameObject.TryGetComponent(out HitBox _))
            {
                Destroy(o.gameObject.GetComponent<HitBox>());
            }
        }
    }

    public void OnGetHandle()
    {
        Debug.Log("Get Clone! " + this);
    }

    public void OnRecycleHandle()
    {
        Debug.Log("Recycle " + this);
    }
}
