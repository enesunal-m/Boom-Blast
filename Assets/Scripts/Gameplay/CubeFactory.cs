using UnityEngine.Pool;
using UnityEngine;
using static CubeSpriteManager;

public class CubeFactory : MonoBehaviour
{
    public static CubeFactory Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public GameObject CreateCube(CubeType type)
    {
        GameObject cube = CubePoolManager.Instance.GetCube();
        CubeController controller = cube.GetComponent<CubeController>();
        controller.SetCubeType(type); // Method to set the cube's type and visuals
        return cube;
    }

    public GameObject CreateCube(CubeType type, bool tntHint)
    {
        GameObject cube = CubePoolManager.Instance.GetCube();
        CubeController controller = cube.GetComponent<CubeController>();
        controller.SetCubeType(type, tntHint); // Method to set the cube's type and visuals with TNT hint
        return cube;
    }

    public void ReturnCube(GameObject cube)
    {
        CubePoolManager.Instance.ReturnCube(cube);
    }
}
