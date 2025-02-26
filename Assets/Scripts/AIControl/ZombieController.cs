using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : IPoolableObject
{
    public CharacterStateHolder stateHolder;
    public RagdollControl ragdoll;
    private List<HitBox> hitBoxes;
    private Rigidbody rb;
    private Animator animator;
    void Start()
    {
        stateHolder = GetComponent<CharacterStateHolder>();
        ragdoll = GetComponent<RagdollControl>();
        rb = GetComponent<Rigidbody>();
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

#if UNITY_EDITOR
    [ContextMenu("AddHitBox")]
    public void AddHitBox()
    {
        if (stateHolder == null) Debug.Log("StateHolder is null");
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
#endif

    public override void OnGet()
    {
        Debug.Log("Get Clone! " + this);
    }

    public override void OnRecycle()
    {
        Debug.Log("Recycle " + this);
    }
}
