using UnityEngine.Pool;
using UnityEngine;
using static CubeSpriteManager;
using System.Collections;

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

    public GameObject CreateCube(CubeType type, int x, int y)
    {
        GameObject cube = CubePoolManager.Instance.GetCube();
        if (isObstacle(type))
        {
            ObstacleController obstacleController = cube.AddComponent<ObstacleController>();
            obstacleController.SetObstacleType(type);
            cube.SetActive(true);
        }
        CubeController controller = cube.GetComponent<CubeController>();
        controller.SetXY(x, y);
        controller.SetCubeType(type);
        return cube;
    }

    public GameObject CreateCube(CubeType type, bool tntHint, int x, int y)
    {
        GameObject cube = CubePoolManager.Instance.GetCube();
        if (isObstacle(type))
        {
            ObstacleController obstacleController = cube.AddComponent<ObstacleController>();
            obstacleController.SetObstacleType(type);
            cube.SetActive(true);
        }
        CubeController controller = cube.GetComponent<CubeController>();
        controller.SetXY(x, y);
        controller.SetCubeType(type, tntHint);
        return cube;
    }

    public void ReturnCube(GameObject cube)
    {
        CubePoolManager.Instance.ReturnCube(cube);
    }

    public void ReturnCube(GameObject cube, float delay)
    {
        StartCoroutine(ReturnCubeDelayed(cube, delay));
    }

    private IEnumerator ReturnCubeDelayed(GameObject cube, float delay)
    {
        yield return new WaitForSeconds(delay);
        CubePoolManager.Instance.ReturnCube(cube);
    }

    public void ReturnCube(GameObject cube, bool isObstacle)
    {
        if (isObstacle)
        {
            Destroy(cube.GetComponent<ObstacleController>());
        }
        CubePoolManager.Instance.ReturnCube(cube);
    }

    private bool isObstacle(CubeType type)
    {
        return type == CubeType.Box || type == CubeType.Stone || type == CubeType.Vase;
    }
}
