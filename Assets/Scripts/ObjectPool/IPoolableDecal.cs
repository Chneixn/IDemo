using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(DecalProjector))]
public class IPoolableDecal : IPoolableObject
{
    public override void OnGet()
    {
        
    }

    public override void OnRecycle()
    {
        
    }

}
