using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletData _bulletData;
    [SerializeField] private bool _collideWithInnerWalls;

    [SerializeField, Range(0,360)] private float _rotateSpeed = 0;
    private GameObject _visual;

    [Header("VFX")]
    [SerializeField] private ParticleSystem _hitVfxPrefab;

    private float _speed = 20f;
    private float _lifeTime = 3f;

    private float _damage;
    private LayerMask _targetLayer;
    private IObjectPool<Bullet> _pool;
    private bool _push = false;
    private Vector3 _lastPosition;

    public void Init(IObjectPool<Bullet> pool)
    {
        _pool = pool;
        _speed = _bulletData.speed;
        _lifeTime = _bulletData.lifeTime;
        _visual = transform.GetChild(0).gameObject;
        _lastPosition = transform.position;

        StartCoroutine(ReturnAfterLifetime());
    }

    private void Update()
    {
        Vector3 start = _lastPosition;
        Vector3 end = transform.position + transform.forward * _speed * Time.deltaTime;
        if (Physics.Linecast(start, end, out var hit))
        {
            HandleHit(hit.collider, hit.point);
            _lastPosition = end;
            return;
        }

        transform.position = end;
        _lastPosition = end;
        if(_rotateSpeed > 0)
        {
            _visual.transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other, other.ClosestPoint(transform.position));
    }

    private void HandleHit(Collider other, Vector3 hitPoint)
    {
        if (other.CompareTag("OuterWall") || (_collideWithInnerWalls && other.CompareTag("InnerWall")))
        {
            SpawnHitVfx(hitPoint);
            _pool.Release(this);
            return;
        }
        if ((_targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent(out Health.IDamageable damageable))
            {
                damageable.TakeDamage(_damage, gameObject, _push);
            }
            SpawnHitVfx(hitPoint);
            _pool.Release(this);
        }
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(_lifeTime);
        _pool.Release(this);
    }

    public void SetParameters(LayerMask mask, float damage, bool push)
    {
        _targetLayer = mask;
        _damage = damage;
        _push = push;
    }

    private void SpawnHitVfx(Vector3 position)
    {
        VfxManager.Instance.Play(_hitVfxPrefab, position, Quaternion.identity);
    }
}
