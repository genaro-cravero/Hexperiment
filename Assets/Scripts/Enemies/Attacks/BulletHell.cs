using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BulletHell : MonoBehaviour
{
    [SerializeField] private BulletHellStage[] _stage;
    [SerializeField] private LayerMask _shootLayer;
    [SerializeField] private CharacterData _enemyData;

    private int _currentStage;

    private bool _isShooting;
    private float _bulletDamage;
    private float _timeBetweenBullets;
    private float _lastAttackTime;
    private bool _isPaused;
    private float _pauseEndTime;
    private float _nextPauseTime;

    private List<IObjectPool<Bullet>> _bulletPoolList = new List<IObjectPool<Bullet>>();

    private void Awake()
    {
        _bulletDamage = _enemyData.attackDamage;
        _timeBetweenBullets = _enemyData.attackCoolDown;
        _nextPauseTime = Time.time + _stage[_currentStage].pauseEveryNSeconds;

        for (int i = 0; i < _stage.Length; i++)
        {
            int stageIndex = i;
            var bulletPool = new ObjectPool<Bullet>(
                createFunc: () => Instantiate(_stage[stageIndex].bulletPrefab),
                actionOnGet: bullet =>
                {
                    bullet.SetParameters(_shootLayer, _bulletDamage, false);
                    bullet.gameObject.SetActive(true);
                },
                actionOnRelease: bullet => bullet.gameObject.SetActive(false),
                actionOnDestroy: bullet => Destroy(bullet.gameObject),
                collectionCheck: false,
                maxSize: 50
            );
            _bulletPoolList.Add(bulletPool);
        }

    }

    private void Update()
    {
        transform.Rotate(0f, _stage[_currentStage].rotationSpeed * Time.deltaTime, 0f);
        UpdatePauseState();
        if (_isPaused) return;
        if (_isShooting) return;
        if (Time.time < _lastAttackTime + _timeBetweenBullets) return;

        Shoot();
    }

    public void Shoot()
    {
        _isShooting = true;
        switch (_stage[_currentStage].bulletPattern)
        {
            case BulletPattern.DoubleSpiral:
                ShootSpiral(2, 180f);
                break;
            case BulletPattern.RingBurst:
                ShootRing();
                break;
            default:
                ShootSpiral(1, 0f);
                break;
        }

        _lastAttackTime = Time.time;
        _isShooting = false;
    }

    private void ShootSpiral(int count, float angleOffset)
    {
        for (int i = 0; i < count; i++)
        {
            float offset = angleOffset * i;
            Quaternion rotation = transform.rotation * Quaternion.Euler(0f, offset, 0f);
            var bullet = _bulletPoolList[_currentStage].Get();
            bullet.transform.SetPositionAndRotation(transform.position, rotation);
            bullet.Init(_bulletPoolList[_currentStage]);
        }
    }

    private void ShootRing()
    {
        var bulletCount = _stage[_currentStage].ringBulletCount;

        float step = 360f / bulletCount;
        for (int i = 0; i < bulletCount; i++)
        {
            Quaternion rotation = transform.rotation * Quaternion.Euler(0f, step * i, 0f);
            var bullet = _bulletPoolList[_currentStage].Get();
            bullet.transform.SetPositionAndRotation(transform.position, rotation);
            bullet.Init(_bulletPoolList[_currentStage]);
        }
    }

    public void ChangeStage(int stage)
    {
        _currentStage = stage;
        _isPaused = false;
        _nextPauseTime = Time.time + _stage[_currentStage].pauseEveryNSeconds;
    }

    private void UpdatePauseState()
    {
        if (_isPaused)
        {
            if (Time.time >= _pauseEndTime)
                _isPaused = false;
            return;
        }

        float pauseEvery = _stage[_currentStage].pauseEveryNSeconds;
        if (pauseEvery <= 0f)
            return;

        if (Time.time >= _nextPauseTime)
        {
            _isPaused = true;
            _pauseEndTime = Time.time + _stage[_currentStage].timePaused;
            _nextPauseTime = Time.time + pauseEvery;
        }
    }

}
[System.Serializable]
public struct BulletHellStage
{
    public Bullet bulletPrefab;
    public float rotationSpeed;
    public BulletPattern bulletPattern;
    [Min(1)] public int ringBulletCount;
    [Min(5)] public float pauseEveryNSeconds;
    public float timePaused;

}

public enum BulletPattern
{
    Spiral,
    DoubleSpiral,
    RingBurst
}