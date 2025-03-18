using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterState
{
    public event Action OnStateUpdate;
}

public class CharacterStateHolder : MonoBehaviour
{
    public Health healthState = new();
    public EnergyPower powerState = new();
    public DiscoverState discoverState = new();
    public EXPState expState = new();

    private void OnEnable()
    {
        // UI Update Bind
        healthState.OnStateUpdate += UpdateCharacterState;
        powerState.OnStateUpdate += UpdateCharacterState;
        discoverState.OnStateUpdate += UpdateCharacterState;
        expState.OnStateUpdate += UpdateCharacterState;
    }

    private void OnDisable()
    {
        // UI Update Unbind
        healthState.OnStateUpdate -= UpdateCharacterState;
        powerState.OnStateUpdate -= UpdateCharacterState;
        discoverState.OnStateUpdate -= UpdateCharacterState;
        expState.OnStateUpdate -= UpdateCharacterState;
    }

    private void UpdateCharacterState()
    {

    }
}
