using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float fireRate = 1f;
    public int damage = 10;
    public int maxHealth = 100;

    public void IncreaseMoveSpeed(float amount)
    {
        moveSpeed += amount;
    }

    public void IncreaseFireRate(float amount)
    {
        fireRate += amount;
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