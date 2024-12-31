using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "MyEvent/UI/CharacterStateEventSO")]
public class CharacterStateEventSO : ScriptableObject
{
    public UnityAction<CharacterStateHolder> OnEventRaised;

    public void RasieEvent(CharacterStateHolder stateHolder)
    {
        OnEventRaised?.Invoke(stateHolder);
    }
}
