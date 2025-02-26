using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class IPoolableParticleSystem : IPoolableObject
{
    public ParticleSystem system;

    public override void OnGet()
    {
        if (system == null) system = GetComponent<ParticleSystem>();
    }

    public override void OnRecycle()
    {
        system.Stop();
    }
}
