using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Bullet : MonoBehaviour
{
    //ToDO change this to a scriptable object 
    [SerializeField] private float _speed = 20f;
    [SerializeField] private float _lifeTime = 3f;

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
        _pool.Release(this);
    }

    private IEnumerator ReturnAfterLifetime()
    {
        yield return new WaitForSeconds(_lifeTime);
        _pool.Release(this);
    }
}
