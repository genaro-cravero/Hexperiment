using UnityEngine;

[CreateAssetMenu(fileName = "BulletData", menuName = "Scriptable Objects/BulletData")]
public class BulletData : ScriptableObject
{
    [Range(0, 100)] public float speed = 20f;
    [Range(0,10)] public float lifeTime = 3f;
}
