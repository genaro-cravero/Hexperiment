using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gameplay UI")]
    [SerializeField] private GameObject _wavePanel;
    private CanvasGroup _waveCanvasGroup;
    [SerializeField] private TMP_Text _waveText;

    [Header("Panels")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

    public bool IsWavePanelActive => _wavePanel.activeSelf;

    [Header ("Wave Panel Times")]
    private const float WAVE_FADE_TIME = 0.5f;
    private const float WAVE_PANEL_TIME = 2f;

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

    public void UpdateWaveText(int wave)
    {
        _waveText.text = $"{wave}";
        _wavePanel.SetActive(true);
        StartCoroutine(ShowWavePanel());
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

    private IEnumerator ShowWavePanel()
    {
        _wavePanel.SetActive(true);
        var alpha = 0f;
        while (_waveCanvasGroup.alpha < 1f)
        {
            alpha += Time.deltaTime / WAVE_FADE_TIME;
            _waveCanvasGroup.alpha = Mathf.Clamp01(alpha);
            yield return null;
        }
        yield return new WaitForSeconds(WAVE_FADE_TIME);
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
    }
}
