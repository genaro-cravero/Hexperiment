using UnityEngine;

namespace Health
{
    public interface IDamageable
    {
        void TakeDamage(float damage, GameObject source);
        void Heal(float amount);
        void IncreaseMaxHealth(float amount);
        bool IsAlive { get; }
    }
}
