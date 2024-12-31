using UnityEngine;
using Unity.Collections;
using System;

[Serializable]
public class DiscoverState : ICharacterState
{
    [Header("隐蔽值")]
    [SerializeField] private float discoverPoint = 0f;
    [SerializeField] private float maxDiscoverPoint = 100f;
    [SerializeField][ReadOnly] private bool bediscovered = false;
    public bool Bediscovered => bediscovered;

    public event OnStateChange OnStateUpdate;

    public void OnBeDiscovered(float p)
    {
        if (discoverPoint >= maxDiscoverPoint)
        {
            bediscovered = true;
            discoverPoint = maxDiscoverPoint;
        }
        else discoverPoint += p * 50f * Time.deltaTime;

        OnStateUpdate.Invoke();
    }

    public void LowerDiscoverPoint()
    {
        //防止发现点负数
        if (discoverPoint <= 0f) { discoverPoint = 0f; return; }
        if (!bediscovered)
            discoverPoint -= 10f * Time.deltaTime;
    }
}
