using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityGameObjectPool
{
    [RequireComponent(typeof(ParticleSystem))]
    public class IPoolableParticleSystem : IPoolableObject
    {
        public ParticleSystem particle;

        public override void OnGet()
        {
            if (particle == null) particle = GetComponent<ParticleSystem>();
        }

        public override void OnRecycle()
        {
            particle.Stop();
        }
    }
}