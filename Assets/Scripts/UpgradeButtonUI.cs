using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _upgradeText;

    private UpgradeData _currentUpgrade;

    public void Setup(UpgradeData upgrade)
    {
        _currentUpgrade = upgrade;

        _upgradeText.text = upgrade.upgradeName;
    }

    public void OnUpgradeSelected() //Called from button event
    {
        UpgradeManager.Instance.ApplyUpgrade(_currentUpgrade);

        UIManager.Instance.HideUpgradePanel();

        WaveManager.Instance.StartNextWave();
    }
}