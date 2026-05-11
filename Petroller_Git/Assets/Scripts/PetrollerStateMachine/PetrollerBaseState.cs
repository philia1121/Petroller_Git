public abstract class PetrollerBaseState
{
    protected PetrollerStateMachine _ctx;
    protected PetrollerStateFactory _factory;
    protected PetrollerBaseState _currentState;
    public PetrollerBaseState(PetrollerStateMachine currentContext, PetrollerStateFactory stateFactory)
    {
        _ctx = currentContext;
        _factory = stateFactory;
    }
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    protected void SwitchState(PetrollerBaseState newState)
    {
        ExitState();
        newState.EnterState();
        _ctx.CurrentState = newState;
    }
}
