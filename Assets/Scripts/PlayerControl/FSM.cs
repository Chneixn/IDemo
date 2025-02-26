using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T> where T : IMovementState
{
    public Dictionary<System.Type, T> StateMap { get; protected set; }
    protected CharacterControl CC;
    protected T curState;
    public T CurState => curState;

    public FSM(T startState, CharacterControl CC)
    {
        StateMap = new();
        StateMap.Add(startState.GetType(), startState as T);

        curState = StateMap[startState.GetType()];
        this.CC = CC;
        curState.CC = CC;
        curState.OnStateEnter();
    }

    public FSM(CharacterControl CC) => this.CC = CC;

    public void SwitchOn(T state)
    {
        curState = StateMap[state.GetType()];
        curState.OnStateEnter();
    }

    public void AddState(T state)
    {
        StateMap.Add(state.GetType(), state);
        state.CC = CC;
    }

    public void ChangeState(System.Type newState)
    {
        curState.OnStateExit(StateMap[newState]);
        curState = StateMap[newState];
        curState.OnStateEnter();
    }
}
