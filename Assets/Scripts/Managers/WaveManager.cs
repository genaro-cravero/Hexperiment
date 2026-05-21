using Health;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    [SerializeField] private GameObject _cinemachineCamera;

    [Header("Wave")]
    [SerializeField] private Wave[] _waves;
    [SerializeField] private int _currentWave = 0;

    [Header("Spawning")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _meleeEnemyPrefab;
    [SerializeField] private GameObject _rangedEnemyPrefab;
    [SerializeField] private GameObject _bossPrefab;
    private List<GameObject> _spawnedEnemies = new List<GameObject>();

    [Header("Timing")]
    [SerializeField] private float _timeBetweenWaves = 3f;
    [SerializeField] private float _spawnDelay = 0.5f;

    private int _enemiesAlive;
    //public float WaveProgress => _enemiesAlive > 0 ? 1f - (float)_enemiesAlive / GetTotalEnemiesInCurrentWave() : 1f;
    public float TotalWavesProgress => (float)_currentWave / (_waves.Length - 1);

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

    public void StartWaves()
    {
        StartCoroutine(StartGame());
    }

    private IEnumerator StartGame()
    {
        Time.timeScale = 1f;
        _cinemachineCamera.SetActive(true);
        var mainCam = Camera.main;
        var brain = mainCam.GetComponent<CinemachineBrain>();
        if (brain != null)
        {
            yield return null;
            yield return new WaitUntil(() => !brain.IsBlending);
        }
        yield return new WaitForSecondsRealtime(0.5f);
        GameManager.Instance.SetCurrentState(GameState.Playing);
        UIManager.Instance.ShowWavePanel(_currentWave + 1, true);
        StartCoroutine(StartWave());

    }

    private IEnumerator StartWave()
    {
        if(_currentWave > 0)
        {
            yield return new WaitForSeconds(_timeBetweenWaves);
            UIManager.Instance.ShowWavePanel(_currentWave + 1);
        }

        yield return null;
        yield return new WaitUntil(() => !UIManager.Instance.IsWavePanelActive);

        yield return new WaitForSeconds(_timeBetweenWaves);
        
        var currentWave = _waves[_currentWave];
        int meleeCount = currentWave.meleeCount;
        int rangedCount = currentWave.rangedCount;
        bool addBoss = currentWave.addBoss;

        _enemiesAlive = meleeCount + rangedCount;
        _enemiesAlive += addBoss ? 1 : 0;

        for (int i = 0; i < meleeCount; i++)
        {
            SpawnEnemy(_meleeEnemyPrefab);
            yield return new WaitForSeconds(_spawnDelay);
        }

        for (int i = 0; i < rangedCount; i++)
        {
            SpawnEnemy(_rangedEnemyPrefab);
            yield return new WaitForSeconds(_spawnDelay);
        }

        if(addBoss)
        {
            SpawnEnemy(_bossPrefab);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform randomSpawn = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, randomSpawn.position, Quaternion.identity);

        HealthComponent enemyHealth = enemy.GetComponent<HealthComponent>();
        _spawnedEnemies.Add(enemy);
        enemyHealth.OnDie += HandleEnemyDeath;
    }

    private void HandleEnemyDeath()
    {
        _enemiesAlive--;

        if (_enemiesAlive <= 0)
        {
            _currentWave++;
            if (_currentWave >= _waves.Length)
            {
                GameManager.Instance.WinGame();
                return;
            }
            List<UpgradeData> randomUpgrades = UpgradeManager.Instance.GetRandomUpgrades(3);
            UIManager.Instance.ShowUpgradePanel(randomUpgrades);
        }
    }

    public void ClearEnemies()
    {
        foreach (var enemy in _spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _spawnedEnemies.Clear();
    }

    public void StartNextWave()
    {
        StartCoroutine(StartWave());
    }
}

[System.Serializable]
public struct Wave
{
    public int meleeCount;
    public int rangedCount;
    public bool addBoss;
}