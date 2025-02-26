using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IPoolableObject : MonoBehaviour
{
    public abstract void OnGet();

    public abstract void OnRecycle();
}
