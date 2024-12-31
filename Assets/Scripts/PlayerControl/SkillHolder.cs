using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillInput
{
    public bool tryUseAbility;
    public bool tryEndAbliity;
}

public class SkillHolder : MonoBehaviour
{
    public BaseAbility enableAbility;

    public void ApplyInput(ref SkillInput inputs)
    {
        if (inputs.tryUseAbility)
        {
            enableAbility.StartAbility();
        }
        else if (inputs.tryEndAbliity)
        {
            enableAbility.StopAbility();
        }
    }
}
