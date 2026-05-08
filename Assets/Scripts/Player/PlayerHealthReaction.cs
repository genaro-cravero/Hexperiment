using Enemy;
using Health;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Player
{
    public class PlayerHealthReaction : MonoBehaviour
    {

        private HealthComponent _health;

        private void Awake()
        {
            _health = GetComponent<HealthComponent>();
        }

        private void Start()
        {
            _health.OnDamage += HandleDamage;
            _health.OnDie += HandleDie;
        }

        private void OnDisable()
        {
            _health.OnDamage -= HandleDamage;
            _health.OnDie -= HandleDie;
        }

        private void HandleDamage(float damage, GameObject source)
        {
            //VFX and SFX for player damage
        }

        private void HandleDie()
        {
            GameManager.Instance.LoseGame();
        }
    }
}
