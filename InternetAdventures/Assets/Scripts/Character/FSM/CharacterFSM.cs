using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFSM : MonoBehaviour
{
    private Dictionary<Type, CharacterState> _characterStates = new Dictionary<Type, CharacterState>();
    private CharacterState _currentState;
    public CharacterFSMInformation CharacterFsmInformation { get; private set; }
    
    private void Awake()
    {
        CharacterFsmInformation = GetComponent<CharacterFSMInformation>();
        GetAllStates();
        //Always sets starting state to idle.
        ChangeState<Idle>();
    }

    //Gets in all states that are currently on the character GameObject.
    private void GetAllStates()
    {
        foreach (var component in GetComponents<CharacterState>())
        {
            component.Initialize(this);
            if (component is Idle) _currentState = component;
            _characterStates.Add(component.GetType(), component);
        }
    }

    //Public access point for all states.
    public void ChangeState<T>() where T : CharacterState
    {
        ChangeState(typeof(T));
    }

    //Private implementation to change a state.
    private void ChangeState(Type pState)
    {
        if (_currentState != null && _currentState.GetType() == pState) return;
        if(_currentState != null) _currentState.ExitState();
        if(pState != null && _characterStates.ContainsKey(pState))
        {
            _currentState = _characterStates[pState];
            _currentState.EnterState();
            Debug.Log($"State changed to: {_currentState.GetType()}");
        }
    }
}
