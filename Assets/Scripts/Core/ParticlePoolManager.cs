using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlePoolManager : MonoBehaviour
{
    public static ParticlePoolManager Instance;

    [System.Serializable]
    public struct ParticlePrefabEntry
    {
        public ParticleType type;
        public GameObject prefab;
    }

    public List<ParticlePrefabEntry> particlePrefabs;

    private Dictionary<ParticleType, ObjectPool<GameObject>> particlePools;

    [SerializeField] private int preWarmCount = 20;
    [SerializeField] private int maxSize = 50;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        particlePools = new Dictionary<ParticleType, ObjectPool<GameObject>>();

        foreach (var entry in particlePrefabs)
        {
            var pool = new ObjectPool<GameObject>(
                () => CreateParticle(entry.prefab), // CreateFunc
                obj => { obj.SetActive(true); obj.GetComponent<ParticleSystem>().Play(); }, // OnGet
                obj => { obj.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear); obj.SetActive(false); obj.transform.SetParent(transform); }, // OnRelease
                obj => Destroy(obj), // OnDestroy
                false, // CollectionCheck
                preWarmCount, // DefaultCapacity
                maxSize // MaxSize
            );

            particlePools.Add(entry.type, pool);

            // Pre-warm the pool
            PreWarmPool(pool, preWarmCount);
        }
    }

    private GameObject CreateParticle(GameObject particlePrefab)
    {
        GameObject particle = Instantiate(particlePrefab);
        particle.transform.position = transform.position;
        return particle;
    }

    private void OnDestroy()
    {
        foreach (var pool in particlePools.Values)
        {
            pool.Dispose();
        }
    }

    public GameObject GetParticle(ParticleType type)
    {
        if (particlePools.TryGetValue(type, out var pool))
        {
            return pool.Get();
        }
        else
        {
            Debug.LogWarning($"No particle pool found for type {type}");
            return null;
        }
    }

    private void PreWarmPool(ObjectPool<GameObject> pool, int count)
    {
        var tempList = new List<GameObject>();
        for (int i = 0; i < count; i++)
        {
            tempList.Add(pool.Get());
        }
        foreach (var obj in tempList)
        {
            obj.SetActive(false);
            pool.Release(obj);
        }
    }
}
