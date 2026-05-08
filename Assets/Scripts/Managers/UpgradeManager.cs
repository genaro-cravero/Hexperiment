using Health;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Player;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [SerializeField] private List<UpgradeData> _allUpgrades;
    private PlayerStats _playerStats;
    private IDamageable _playerHealth;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        _playerStats = FindAnyObjectByType<PlayerStats>();
        _playerHealth = _playerStats.GetComponent<IDamageable>();
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        switch (upgrade.type)
        {
            case UpgradeType.FireRate:
                _playerStats.IncreaseFireRate(upgrade.value);
                break;

            case UpgradeType.Damage:
                _playerStats.IncreaseDamage(upgrade.value);
                break;

            case UpgradeType.MaxHealth:
                _playerStats.IncreaseMaxHealth((int)upgrade.value);
                break;

            case UpgradeType.MoveSpeed:
                _playerStats.IncreaseMoveSpeed(upgrade.value);
                break;
            case UpgradeType.Heal:
                _playerHealth.Heal(upgrade.value);
                break;
        }
    }

    public List<UpgradeData> GetRandomUpgrades(int amount)
    {
        return _allUpgrades
            .GroupBy(x => x.type)
            .Select(group => GetWeightedRandom(group.ToList()))
            .OrderBy(x => Random.value)
            .Take(amount)
            .ToList();
    }

    private UpgradeData GetWeightedRandom(List<UpgradeData> upgrades)
    {
        float totalWeight = upgrades.Sum(u => Mathf.Max(0f, GetRarityWeight(u.rarity)));
        if (totalWeight <= 0f)
            return upgrades[Random.Range(0, upgrades.Count)];

        float roll = Random.value * totalWeight;
        float cumulative = 0f;
        foreach (var upgrade in upgrades)
        {
            cumulative += Mathf.Max(0f, GetRarityWeight(upgrade.rarity));
            if (roll <= cumulative)
                return upgrade;
        }

        return upgrades[upgrades.Count - 1];
    }

    private float GetRarityWeight(UpgradeRarity rarity)
    {
        switch (rarity)
        {
            case UpgradeRarity.Common:
                return 1f;
            case UpgradeRarity.Uncommon:
                return 0.6f;
            case UpgradeRarity.Rare:
                return 0.3f;
            case UpgradeRarity.Epic:
                return 0.15f;
            case UpgradeRarity.Legendary:
                return 0.05f;
            default:
                return 1f;
        }
    }

}

public enum UpgradeType
{
    FireRate,
    Damage,
    MaxHealth,
    Heal,
    MoveSpeed
}

[System.Serializable]
public class UpgradeData
{
    public string upgradeName;
    public UpgradeType type;
    public float value;
    public UpgradeRarity rarity = UpgradeRarity.Common;
}

public enum UpgradeRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}
