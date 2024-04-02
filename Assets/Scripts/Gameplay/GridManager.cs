using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }
    public GameObject cubePrefab;
    public GameObject gridContainer;
    private CubeController[,] grid;
    private RectTransform gridRectTransform;
    private List<GameObject> cubes = new List<GameObject>();
    private int width;
    private int height;

    private int cubeWidth = 1;
    private int cubeHeight = 1;
    private int cubeSize = 100;
    private int spacing = 0;

    private float containerWidth;
    private float containerHeight;

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

        gridRectTransform = gridContainer.GetComponent<RectTransform>();
    }

    public void CreateGrid(int width, int height, List<string> layout)
    {
        this.width = width;
        this.height = height;
        ResizeGridContainer();
        grid = new CubeController[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 position = GetCubePosition(x, height - 1 - y); // Adjust for bottom-left origin
                GameObject cubeObj = ObjectPooler.Instance.SpawnFromPool("Cube", position, Quaternion.identity);
                cubes.Add(cubeObj);
                cubeObj.transform.SetParent(gridContainer.transform, false);
                cubeObj.GetComponent<RectTransform>().anchoredPosition = position;

                CubeController cube = cubeObj.GetComponent<CubeController>();
                string typeStr = layout[y * width + x]; // Adjust if necessary
                cube.type = DetermineCubeType(typeStr);

                grid[x, y] = cube;
            }
        }
    }

    private void ResizeGridContainer()
    {
        containerWidth = width * cubeSize + width * spacing;
        containerHeight = height * cubeSize + height * spacing;
        gridRectTransform.sizeDelta = new Vector2(containerWidth + 55, containerHeight + 55);
    }


    private Vector2 GetCubePosition(int x, int y)
    {
        float posX = x * (cubeSize + spacing) - containerWidth / 2 + cubeSize / 2;
        float posY = y * (cubeSize + spacing) - containerHeight / 2 + cubeSize / 2;
        return new Vector2(posX, posY);
    }


    private CubeController.CubeType DetermineCubeType(string typeStr)
    {
        if (typeStr == "rand")
        {
            return GetRandomCubeType();
        }
        switch (typeStr)
        {
            case "r": return CubeController.CubeType.Red;
            case "g": return CubeController.CubeType.Green;
            case "b": return CubeController.CubeType.Blue;
            case "y": return CubeController.CubeType.Yellow;
            case "t": return CubeController.CubeType.TNT;
            case "bo": return CubeController.CubeType.Box;
            case "s": return CubeController.CubeType.Stone;
            case "v": return CubeController.CubeType.Vase;
            case "rand": return CubeController.CubeType.Random;
            default: return CubeController.CubeType.Default;
        }
    }

    private CubeController.CubeType GetRandomCubeType()
    {
        CubeController.CubeType[] possibleTypes = new CubeController.CubeType[]
        {
        CubeController.CubeType.Red,
        CubeController.CubeType.Green,
        CubeController.CubeType.Blue,
        CubeController.CubeType.Yellow
        };

        int randomIndex = Random.Range(0, possibleTypes.Length);
        return possibleTypes[randomIndex];
    }
    public List<CubeController> FindMatchesAt(Vector2 position, CubeController.CubeType type)
    {
        List<CubeController> matches = new List<CubeController>();
        // Check adjacent cubes in all four directions (up, down, left, right)
        // and add them to the matches list if they're of the same type
        // Make sure to account for bounds of the grid

        // Example for right-side check
        Vector2 right = new Vector2(position.x + 1, position.y);
        if (IsCubeOfType(right, type))
        {
            matches.Add(grid[(int)right.x, (int)right.y]);
        }

        // Repeat for other directions...

        return matches;
    }

    private bool IsCubeOfType(Vector2 position, CubeController.CubeType type)
    {
        // Check if the position is within the grid bounds and the cube at this position is of the specified type
        if (position.x >= 0 && position.x < grid.GetLength(0) &&
            position.y >= 0 && position.y < grid.GetLength(1))
        {
            return grid[(int)position.x, (int)position.y].type == type;
        }
        return false;
    }

    public void RemoveCubes(List<CubeController> cubes)
    {
        foreach (CubeController cube in cubes)
        {
            // Deactivate cube and return it to the pool
            cube.gameObject.SetActive(false);
            // Assuming you have a method in ObjectPooler to handle returning objects to the pool
            ObjectPooler.Instance.ReturnToPool(cube.type.ToString(), cube.gameObject);
        }
        // After removing cubes, you may need to make other cubes fall down and fill the gaps
        UpdateGridAfterRemoval();
    }

    private void UpdateGridAfterRemoval()
    {
        // Implement logic to let cubes fall down and fill the gaps left by removed cubes
        // You may also need to spawn new cubes at the top of the grid to replace those that were removed
    }
}
