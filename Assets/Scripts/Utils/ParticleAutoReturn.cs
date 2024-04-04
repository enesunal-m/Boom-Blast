using UnityEngine;
using UnityEngine.Pool;

public class ParticleAutoReturn : MonoBehaviour
{
    private IObjectPool<GameObject> pool;
    private ParticleSystem ps;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.Callback;
    }

    public void SetPool(IObjectPool<GameObject> pool)
    {
        this.pool = pool;
    }

    void OnParticleSystemStopped()
    {
        if (pool != null)
        {
            gameObject.SetActive(false);
            pool.Release(gameObject);
        }
    }
}
