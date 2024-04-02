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
                    // Function to create a new cube
                    return Instantiate(cubePrefab);
                },
                actionOnGet: (obj) =>
                {
                    // Function called when a cube is taken from the pool; e.g., make the cube active
                    obj.SetActive(true);
                },
                actionOnRelease: (obj) =>
                {
                    // Function called when a cube is returned to the pool; e.g., make the cube inactive
                    obj.SetActive(false);
                },
                actionOnDestroy: (obj) =>
                {
                    // Function called when a cube is destroyed
                    Destroy(obj);
                },
                collectionCheck: collectionCheck, // Set to true if you want the pool to check for double returns
                defaultCapacity: defaultCapacity, // Initial size of the pool
                maxSize: maxSize // Maximum size of the pool
            );
        }
    }

    // Method to get a cube from the pool
    public GameObject GetCube()
    {
        return cubePool.Get();
    }

    // Method to return a cube to the pool
    public void ReturnCube(GameObject cube)
    {
        cubePool.Release(cube);
    }
}
