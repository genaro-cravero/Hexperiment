using UnityEngine;

namespace Health
{
    public interface IDamageable
    {
        void TakeDamage(float damage, GameObject source);
        bool IsAlive { get; }
    }
}
