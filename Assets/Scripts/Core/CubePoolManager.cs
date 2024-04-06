using UnityEngine;
using UnityEngine.Pool;

public class CubePoolManager : MonoBehaviour
{
    public static CubePoolManager Instance;

    [SerializeField]
    private GameObject cubePrefab;
    [SerializeField]
    private int defaultCapacity = 20;
    [SerializeField]
    private int maxSize = 1000;
    [SerializeField]
    private bool collectionCheck = false;

    private ObjectPool<GameObject> cubePool;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Initialize the cube pool
            cubePool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    return Instantiate(cubePrefab);
                },
                actionOnGet: (obj) =>
                {
                    obj.SetActive(true);
                },
                actionOnRelease: (obj) =>
                {
                    obj.SetActive(false);
                    obj.transform.SetParent(transform);
                },
                actionOnDestroy: (obj) =>
                {
                    Destroy(obj);
                },
                collectionCheck: collectionCheck,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
        }
    }

    public GameObject GetCube()
    {
        return cubePool.Get();
    }

    public void ReturnCube(GameObject cube)
    {
        cube.SetActive(false);
        cube.transform.SetParent(transform);
        cubePool.Release(cube);
    }
}
