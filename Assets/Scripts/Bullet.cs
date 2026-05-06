using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    //ToDO change this to a scriptable object 
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 3f;
    private float _damage;
    private LayerMask _targetLayer;
    private IObjectPool<Bullet> _pool;

    public void Init(IObjectPool<Bullet> pool)
    {
        _pool = pool;
        StartCoroutine(ReturnAfterLifetime());
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //ToDo Check for healthManager
        if((_targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            _pool.Release(this);
        }
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(_lifeTime);
        _pool.Release(this);
    }

    public void SetParameters(LayerMask mask, float damage)
    {
        _targetLayer = mask;
        _damage = damage;
    }
}
