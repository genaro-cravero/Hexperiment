using UnityEngine;
public class EnemyStateMachine
{
    private IEnemyState currentState;

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}