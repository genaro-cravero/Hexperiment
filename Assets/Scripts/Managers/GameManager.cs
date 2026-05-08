using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameState _currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        } else
        {
            Instance = this;
        }
    }

    public void WinGame()
    {
        _currentState = GameState.Win;

        //UIManager.Instance.ShowWinScreen();

        Time.timeScale = 0f;
    }

    public void LoseGame()
    {
        _currentState = GameState.Lose;

        //ToDo UIManager
        //UIManager.Instance.ShowLoseScreen();

        Time.timeScale = 0f;
    }
}

public enum GameState
{
    Playing,
    Win,
    Lose,
    Pause
}