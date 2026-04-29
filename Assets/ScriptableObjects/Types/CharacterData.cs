using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Initial Setup")]
    [Range(1, 100)] public float initialHealth = 100f;
    [Range(1, 20)] public float moveSpeed = 10;
    [Range(0, 10)] public float rotationSpeed;
    [Range(-50, 0)] public float gravity = -20f;
    [Range(0, 50)] public float gravityAcceleration = 10f;

    [Header("Shooting Setup")]
    [Range(0, 10)] public float bulletDamage;
    [Range(0, 5)] public float initialFireRate;
}
