using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponAnimation : MonoBehaviour
{
    public BaseGun Gun;
    protected Animator animator;

    public virtual void Start()
    {
        if (Gun == null)
        {
            gameObject.TryGetComponent(out Gun);
        }

        if (animator == null)
        {
            gameObject.GetComponent<Animator>();
        }
    }
}
