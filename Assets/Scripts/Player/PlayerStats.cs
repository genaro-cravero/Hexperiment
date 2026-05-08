using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [HideInInspector] public float moveSpeed;
    [HideInInspector] public float fireRate;
    [HideInInspector] public int damage;
    [HideInInspector] public int maxHealth;

    public void IncreaseMoveSpeed(float perc)
    {
        var multiplier = 1 + (perc / 100f);
        moveSpeed *= multiplier;
    }

    public void IncreaseFireRate(float perc)
    {
        var multiplier = 1 + (perc / 100f);
        fireRate *= multiplier;
    }

    public void IncreaseDamage(int amount)
    {
        damage += amount;
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
    }
}