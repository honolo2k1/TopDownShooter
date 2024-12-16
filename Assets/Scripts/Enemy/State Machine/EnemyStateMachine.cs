public class EnemyStateMachine
{
    public EnemyState currentState;

    public void Init(EnemyState startState)
    {
        this.currentState = startState;
        startState.Enter();
    }
    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

}
