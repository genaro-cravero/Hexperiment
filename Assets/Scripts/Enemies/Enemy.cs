using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStateMachine _stateMachine;

    void Awake()
    {
        _stateMachine = new EnemyStateMachine();
    }

    void Update()
    {
        _stateMachine.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        _stateMachine.ChangeState(newState);
    }
}