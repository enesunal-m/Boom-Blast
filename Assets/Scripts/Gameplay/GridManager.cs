using UnityEngine;
using System.Collections.Generic;
using static CubeSpriteManager;

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

    private int cubeSize = 100;
    private int spacing = 0;

    private float containerWidth;
    private float containerHeight;

    private bool isProcessing = false;

    List<List<CubeController>> matches = new List<List<CubeController>>();

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
                Vector2 position = GetCubePosition(width - x - 1, y); // Adjust for bottom-left origin

                string typeStr = layout[y * width + x];
                CubeType cubeType = DetermineCubeType(typeStr);

                GameObject cubeObj = CubeFactory.Instance.CreateCube(cubeType);

                cubeObj.transform.position = position;
                cubeObj.transform.rotation = Quaternion.identity;

                cubes.Add(cubeObj);

                cubeObj.transform.SetParent(gridContainer.transform, false);
                cubeObj.GetComponent<RectTransform>().anchoredPosition = position;

                CubeController cube = cubeObj.GetComponent<CubeController>();

                grid[x, y] = cube;
                cube.SetXY(x, y);
            }
        }

        FindAllMatches();
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


    private CubeType DetermineCubeType(string typeStr)
    {
        if (typeStr == "rand")
        {
            return GetRandomCubeType();
        }
        switch (typeStr)
        {
            case "r": return CubeType.Red;
            case "g": return CubeType.Green;
            case "b": return CubeType.Blue;
            case "y": return CubeType.Yellow;
            case "t": return CubeType.TNT;
            case "bo": return CubeType.Box;
            case "s": return CubeType.Stone;
            case "v": return CubeType.Vase;
            case "rand": return CubeType.Random;
            default: return CubeType.Default;
        }
    }

    private CubeType GetRandomCubeType()
    {
        CubeType[] possibleTypes = new CubeType[]
        {
        CubeType.Red,
        CubeType.Green,
        CubeType.Blue,
        CubeType.Yellow
        };

        int randomIndex = Random.Range(0, possibleTypes.Length);
        return possibleTypes[randomIndex];
    }

    public void OnCubeClicked(CubeController clickedCube)
    {
        Debug.Log("Cube clicked: " + clickedCube.GetX() + ", " + clickedCube.GetY());
        // Prevent multiple clicks while processing matches
        if (isProcessing)
            return;
        else
            isProcessing = true;
        foreach (var match in matches)
        {
            if (match.Contains(clickedCube))
            {
                // Handle match (e.g., remove cubes)
                HandleMatch(match);
                break; // Assuming a cube can be part of only one match at a time
            }
        }
        isProcessing = false;
    }

    private void HandleMatch(List<CubeController> match)
    {
        RemoveCubes(match);
        UpdateGridAfterRemoval();
        FindAllMatches();
    }

    private void FindAllMatches()
    {
        matches.Clear(); // Clear previous matches
        bool[,] visited = new bool[width, height]; // Track visited cubes

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y])
                {
                    List<CubeController> match = new List<CubeController>();
                    CheckAdjacentCubes(x, y, grid[x, y].type, ref match, ref visited);

                    if (match.Count >= 3) // Minimum match size
                    {
                        matches.Add(match);
                    }
                }
            }
        }

        Debug.Log("Found " + matches.Count + " matches");
    }

    private void CheckAdjacentCubes(int x, int y, CubeType type, ref List<CubeController> match, ref bool[,] visited)
    {
        // Check bounds and whether the cube is already visited
        if (x < 0 || x >= width || y < 0 || y >= height || visited[x, y] || grid[x, y].type != type)
            return;

        visited[x, y] = true;
        match.Add(grid[x, y]);

        // Recursively check adjacent cubes
        CheckAdjacentCubes(x - 1, y, type, ref match, ref visited); // Left
        CheckAdjacentCubes(x + 1, y, type, ref match, ref visited); // Right
        CheckAdjacentCubes(x, y - 1, type, ref match, ref visited); // Down
        CheckAdjacentCubes(x, y + 1, type, ref match, ref visited); // Up
    }

    private void RemoveCubes(List<CubeController> cubes)
    {
        foreach (CubeController cube in cubes)
        {
            grid[cube.GetX(), cube.GetY()] = null;
            // Assuming you have a method in ObjectPooler to handle returning objects to the pool
            CubePoolManager.Instance.ReturnCube(cube.gameObject);
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
