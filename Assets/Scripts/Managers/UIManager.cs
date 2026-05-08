using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text _waveText;
    [SerializeField] private TMP_Text _hpText;

    [Header("Panels")]
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _losePanel;

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
    }

    private void Start()
    {
        _winPanel.SetActive(false);
        _losePanel.SetActive(false);
    }

    public void UpdateWaveText(int wave)
    {
        _waveText.text = $"Wave Nº{wave}";
    }

    public void UpdateHP(int currentHP)
    {
        _hpText.text = $"HP: {currentHP}";
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
}
