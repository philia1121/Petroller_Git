using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetrollerStateMachine : MonoBehaviour
{
    PetrollerBaseState _currentState;
    PetrollerStateFactory _states;
    public PetrollerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }

    void Start()
    {
        _states = new PetrollerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
    void Update()
    {
        _currentState.UpdateState();
    }

    void ResetCatStateMachine()
    {
        // reset all parameters


        // reset state
        _currentState = _states.Idle();
        _currentState.EnterState();
    }
}
