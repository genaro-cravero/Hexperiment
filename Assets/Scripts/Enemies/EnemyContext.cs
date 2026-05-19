using UnityEngine;

namespace Enemy
{
    public class EnemyContext
    {
        public CharacterData enemyData;
        public EnemyCombat enemyCombat;
        public Transform player;
        public EnemyMovement movement;
        public IEnemyState attackState;
        public IEnemyState chaseState;
        public ICharacterAnimator cAnimator;
    }
}
