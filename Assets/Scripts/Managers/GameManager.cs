using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState;
    public GameState CurrentState => _currentState;
    public bool IsGameplayActive => _currentState is GameState.Playing || _currentState is GameState.Waving;
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
        Cursor.lockState = CursorLockMode.Confined;
        SetCurrentState(GameState.Playing);
    }

    public void WinGame()
    {
        if (_currentState is GameState.Win) return;
        SetCurrentState(GameState.Win);

        StartCoroutine(WinCoroutine());
    }

    private IEnumerator WinCoroutine()
    {
        yield return new WaitForSeconds(1f);
        UIManager.Instance.ShowWinScreen();
        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        SetCurrentState(GameState.Lose);

        UIManager.Instance.ShowLoseScreen();

        Time.timeScale = 0f;
    }

    public void SetCurrentState(GameState state)
    { 
        _currentState = state;
        Cursor.visible = !IsGameplayActive;
    }
}

public enum GameState
{
    Playing,
    Win,
    Lose,
    Pause,
    Waving
}