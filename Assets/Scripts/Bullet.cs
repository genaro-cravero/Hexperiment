using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    //ToDO change this to a scriptable object 
    [SerializeField] private BulletData _bulletData;
    [SerializeField] private bool _collideWithInnerWalls;
    private float _speed = 20f;
    private float _lifeTime = 3f;

    private float _damage;
    private LayerMask _targetLayer;
    private IObjectPool<Bullet> _pool;
    private bool _push = false;

    public void Init(IObjectPool<Bullet> pool)
    {
        _pool = pool;
        _speed = _bulletData.speed;
        _lifeTime = _bulletData.lifeTime;

        StartCoroutine(ReturnAfterLifetime());
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("OuterWall") || (_collideWithInnerWalls && other.CompareTag("InnerWall")))
        {
            _pool.Release(this);
            return;
        }
        if((_targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent(out Health.IDamageable damageable))
            {
                damageable.TakeDamage(_damage, gameObject, _push);
            }
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
}
