using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Initial Setup")]
    [Range(1, 100)] public float initialHealth = 100f;
    [Range(1, 20)] public float moveSpeed;
    [Range(0, 10)] public float rotationSpeed;
    [Range(-10, 0)] public float gravity = -9.81f;

    [Header("Shooting Setup")]
    [Range(0, 10)] public float bulletDamage;
    [Range(0, 5)] public float initialFireRate;
}
