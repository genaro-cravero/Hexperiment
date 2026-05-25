using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _upgradeText;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private AudioClip _selectClip;
    [SerializeField] private AudioClip _hoverClip;

    [Header("Rarity Colors")]
    [SerializeField] private Color _commonColor = Color.white;
    [SerializeField] private Color _uncommonColor = new Color(0.4f, 1f, 0.6f);
    [SerializeField] private Color _rareColor = new Color(0.4f, 0.6f, 1f);
    [SerializeField] private Color _epicColor = new Color(0.8f, 0.4f, 1f);
    [SerializeField] private Color _legendaryColor = new Color(1f, 0.75f, 0.2f);

    private UpgradeData _currentUpgrade;

    public void Setup(UpgradeData upgrade)
    {
        _currentUpgrade = upgrade;

        _upgradeText.text = "";
    }

    private void OnEnable()
    {
        ApplyRarityColor();
    }

    private void ApplyRarityColor()
    {
        if (!_buttonImage) return;
        var color = _commonColor;
        var time = 0.2f;
        switch (_currentUpgrade.rarity)
        {
            case UpgradeRarity.Common:
                color = _commonColor;
                break;
            case UpgradeRarity.Uncommon:
                color = _uncommonColor;
                time *= 1.5f;
                break;
            case UpgradeRarity.Rare:
                color = _rareColor;
                time *= 3;
                break;
            case UpgradeRarity.Epic:
                color = _epicColor;
                time *= 4;
                break;
            case UpgradeRarity.Legendary:
                color = _legendaryColor;
                time *= 5;
                break;
            default:
                color = _commonColor;
                break;
        }

        StartCoroutine(FadeColorIn(color, time));
    }

    private IEnumerator FadeColorIn(Color color, float time)
    {

        var elapsedTime = 0f;
        var initialColor = _buttonImage.color;
        while (elapsedTime < time) 
        {
            elapsedTime += Time.unscaledDeltaTime;
            _buttonImage.color = Color.Lerp(initialColor, color, elapsedTime / time);
            yield return null;
        }

        _buttonImage.color = color;
        yield return null;
        _upgradeText.text = _currentUpgrade.upgradeName;
    }

    public void OnUpgradeSelected() //Called from button event
    {
        UpgradeManager.Instance.ApplyUpgrade(_currentUpgrade);

        SoundFXManager.Instance.PlaySound(_selectClip, Vector3.zero);

        UIManager.Instance.HideUpgradePanel();

        WaveManager.Instance.StartNextWave();
    }

    public void OnHover() //Called from button event
    {
        SoundFXManager.Instance.PlaySound(_hoverClip, Vector3.zero, 0.5f);

    }
}