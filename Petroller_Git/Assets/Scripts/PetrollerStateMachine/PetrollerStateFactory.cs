using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum PetrollerStates
{
    idle,
    happy,
    surprised,
    angry,
    sleep,
    uncomfortable,
    spit,
    passOut,
    inactive
}
public class PetrollerStateFactory
{
    PetrollerStateMachine _context;
    Dictionary<PetrollerStates, PetrollerBaseState> _states = new Dictionary<PetrollerStates, PetrollerBaseState>();
    public PetrollerStateFactory(PetrollerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PetrollerStates.idle] = new PetrollerIdleState(_context, this);
        _states[PetrollerStates.happy] = new PetrollerHappyState(_context, this);
        _states[PetrollerStates.surprised] = new PetrollerSurprisedState(_context, this);
        _states[PetrollerStates.angry] = new PetrollerAngryState(_context, this);
        _states[PetrollerStates.sleep] = new PetrollerSleepState(_context, this);
        _states[PetrollerStates.uncomfortable] = new PetrollerUncomfortableState(_context, this);
        _states[PetrollerStates.spit] = new PetrollerSpitState(_context, this);
        _states[PetrollerStates.passOut] = new PetrollerPassOutState(_context, this);
        _states[PetrollerStates.inactive] = new PetrollerInactiveState(_context, this);
    }
    public PetrollerBaseState Idle() { return _states[PetrollerStates.idle]; }
    public PetrollerBaseState Happy() { return _states[PetrollerStates.happy]; }
    public PetrollerBaseState Surprised() { return _states[PetrollerStates.surprised]; }
    public PetrollerBaseState Angry() { return _states[PetrollerStates.angry]; }
    public PetrollerBaseState Sleep() { return _states[PetrollerStates.sleep]; }
    public PetrollerBaseState Umcomfortable() { return _states[PetrollerStates.uncomfortable]; }
    public PetrollerBaseState Spit() { return _states[PetrollerStates.spit]; }
    public PetrollerBaseState PassOut() { return _states[PetrollerStates.passOut]; }
    public PetrollerBaseState Inactive() { return _states[PetrollerStates.inactive]; }
}
