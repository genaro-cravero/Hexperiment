using UnityEngine;

namespace Health
{
    public interface IDamageable
    {
        void TakeDamage(float damage, GameObject source, bool push);
        void Heal(float amount);
        void IncreaseMaxHealth(float amount);
        bool IsAlive { get; }
    }
}
