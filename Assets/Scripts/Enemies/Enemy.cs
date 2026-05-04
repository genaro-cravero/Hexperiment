using UnityEngine;

public class Enemy : MonoBehaviour
{
    private EnemyStateMachine stateMachine;

    void Awake()
    {
        stateMachine = new EnemyStateMachine();
    }

    void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(IEnemyState newState)
    {
        stateMachine.ChangeState(newState);
    }
}