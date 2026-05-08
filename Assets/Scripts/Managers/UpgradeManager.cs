using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    [SerializeField] private PlayerStats playerStats;

    private void Awake()
    {
        Instance = this;
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.FireRate:
                playerStats.IncreaseFireRate(upgrade.value);
                break;

            case UpgradeType.Damage:
                playerStats.IncreaseDamage((int)upgrade.value);
                break;

            case UpgradeType.MaxHealth:
                playerStats.IncreaseMaxHealth((int)upgrade.value);
                break;

            case UpgradeType.MoveSpeed:
                playerStats.IncreaseMoveSpeed(upgrade.value);
                break;
        }
    }
}

public enum UpgradeType
{
    FireRate,
    Damage,
    MaxHealth,
    MoveSpeed
}

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public UpgradeType type;
    public float value;
}
