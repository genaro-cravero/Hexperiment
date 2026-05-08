using Health;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [Header("Wave")]
    [SerializeField] private Wave[] _waves;
    [SerializeField] private int _currentWave = 0;

    [Header("Spawning")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _meleeEnemyPrefab;
    [SerializeField] private GameObject _rangedEnemyPrefab;

    [Header("Timing")]
    [SerializeField] private float _timeBetweenWaves = 3f;
    [SerializeField] private float _spawnDelay = 0.5f;

    private int _enemiesAlive;

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

        _enemiesAlive = meleeCount + rangedCount;

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
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        Transform randomSpawn = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        GameObject enemy = Instantiate(enemyPrefab, randomSpawn.position, Quaternion.identity);

        HealthComponent enemyHealth = enemy.GetComponent<HealthComponent>();

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
}