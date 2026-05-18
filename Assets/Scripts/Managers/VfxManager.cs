using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class VfxManager : MonoBehaviour
{
    public static VfxManager Instance { get; private set; }

    private readonly Dictionary<ParticleSystem, IObjectPool<ParticleSystem>> _pools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void Play(ParticleSystem vfxPrefab, Vector3 position, Quaternion rotation)
    {
        if (!vfxPrefab) return;

        var pool = GetPool(vfxPrefab);
        var vfx = pool.Get();
        vfx.transform.SetPositionAndRotation(position, rotation);
        vfx.Play();
        StartCoroutine(ReleaseAfter(vfx, pool));
    }

    private IObjectPool<ParticleSystem> GetPool(ParticleSystem prefab)
    {
        if (_pools.TryGetValue(prefab, out var pool))
            return pool;

        pool = new ObjectPool<ParticleSystem>(
            createFunc: () => Instantiate(prefab),
            actionOnGet: vfx => vfx.gameObject.SetActive(true),
            actionOnRelease: vfx =>
            {
                if (vfx != null)
                    vfx.gameObject.SetActive(false);
            },
            actionOnDestroy: vfx => Destroy(vfx.gameObject),
            collectionCheck: false,
            maxSize: 50
        );

        _pools[prefab] = pool;
        return pool;
    }

    private IEnumerator ReleaseAfter(ParticleSystem vfx, IObjectPool<ParticleSystem> pool)
    {
        var main = vfx.main;
        float lifetime = main.duration + main.startLifetime.constantMax;
        yield return new WaitForSeconds(lifetime);
        if (vfx != null)
            pool.Release(vfx);
    }
}
