namespace Enemy
{
    public interface IEnemyAttack
    {
        void Initialize(EnemyContext context);
        void Attack();
        bool IsAttacking { get; }
    }
}
