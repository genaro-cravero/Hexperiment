using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState;
    public GameState CurrentState => _currentState;

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
        _currentState = GameState.Playing;
    }

    public void WinGame()
    {
        _currentState = GameState.Win;

        UIManager.Instance.ShowWinScreen();

        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        _currentState = GameState.Lose;

        UIManager.Instance.ShowLoseScreen();

        Time.timeScale = 0f;
    }

    public void SetCurrentState(GameState state)
    { 
        _currentState = state; 
    }
}

public enum GameState
{
    Playing,
    Win,
    Lose,
    Pause
}