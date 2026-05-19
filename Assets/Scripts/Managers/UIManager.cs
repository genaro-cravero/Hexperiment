using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Gameplay UI")]
    private CanvasGroup _waveCanvasGroup;
    [SerializeField] private TMP_Text _waveText;

    [Header("Upgrade UI")]
    [SerializeField] private Transform _buttonsContainer;
    [SerializeField] private UpgradeButtonUI _upgradeButtonPrefab;

    [Header("Panels")]
    [SerializeField] private GameObject _wavePanel;
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    public bool IsWavePanelActive => _wavePanel.activeSelf;

    [Header("Wave Panel Times")]
    private const float WAVE_FADE_TIME = 0.5f;
    private const float WAVE_PANEL_TIME = 2f;
    private const float WAVE_FULLBLACK_TIME = 1.3f;

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

        _waveCanvasGroup = _wavePanel.GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
        _waveText.gameObject.SetActive(false);
    }

    public void ShowWavePanel(int wave, bool firstTime = false)
    {
        _waveText.text = $"{wave}";
        _wavePanel.SetActive(true);
        StartCoroutine(FadeInWavePanel(firstTime));
    }
    public void ShowUpgradePanel(List<UpgradeData> upgrades)
    {
        foreach (Transform child in _buttonsContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (UpgradeData upgrade in upgrades)
        {
            UpgradeButtonUI button =
                Instantiate(_upgradeButtonPrefab, _buttonsContainer);

            button.Setup(upgrade);
        }

        _upgradePanel.SetActive(true);
        Time.timeScale = 0f;
        GameManager.Instance.SetCurrentState(GameState.Pause);
    }

    public void HideUpgradePanel()
    {
        _upgradePanel.SetActive(false);
        Time.timeScale = 1f;
        GameManager.Instance.SetCurrentState(GameState.Playing);
    }

    public void ShowWinScreen()
    {
        _winPanel.SetActive(true);
    }

    public void ShowLoseScreen()
    {
        _losePanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator FadeInWavePanel(bool firstTime = false)
    {
        GameManager.Instance.SetCurrentState(GameState.Waving);

        _wavePanel.SetActive(true);
        if (!firstTime)
        {
            var alpha = 0f;
            while (_waveCanvasGroup.alpha < 1f)
            {
                alpha += Time.deltaTime / WAVE_FADE_TIME;
                _waveCanvasGroup.alpha = Mathf.Clamp01(alpha);
                yield return null;
            }
        }
        _waveCanvasGroup.alpha = 1f;
        WaveManager.Instance.ClearEnemies();
        yield return new WaitForSeconds(WAVE_FULLBLACK_TIME);
        _waveText.gameObject.SetActive(true);
        StartCoroutine(HideWavePanel());
    }

    private IEnumerator HideWavePanel()
    {
        yield return new WaitForSeconds(WAVE_PANEL_TIME);
        var alpha = 1f;
        while (_waveCanvasGroup.alpha > 0f)
        {
            alpha -= Time.deltaTime / WAVE_FADE_TIME;
            _waveCanvasGroup.alpha = Mathf.Clamp01(alpha);
            yield return null;
        }
        _wavePanel.SetActive(false);
        _waveText.gameObject.SetActive(false);
        GameManager.Instance.SetCurrentState(GameState.Playing);
    }
}
