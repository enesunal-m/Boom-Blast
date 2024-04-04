using UnityEngine;
using System.Collections.Generic;
using static CubeSpriteManager;
using System;
using System.Collections;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public static event Action<List<Vector2Int>> OnCubeMatch;

    public GameObject cubePrefab;
    public GameObject gridContainer;

    private CubeController[,] grid;
    private string[,] gridLayout;
    private RectTransform gridRectTransform;
    private List<GameObject> cubes = new List<GameObject>();

    private int width;
    private int height;

    private int cubeSize = 100;
    private int spacing = 0;

    private float containerWidth;
    private float containerHeight;

    private bool isProcessing = false;

    public float timeToMove = 0.5f;

    private List<List<CubeController>> matches = new List<List<CubeController>>();
    private List<List<CubeController>> tntMatches = new List<List<CubeController>>();

    public GameObject destructionParticlePrefab;

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

    public CubeController[,] GetGrid()
    {
        return grid;
    }

    public string[,] GetGridLayout()
    {
        return gridLayout;
    }

    public void CreateGrid(int width, int height, List<string> layout)
    {
        this.width = width;
        this.height = height;
        ResizeGridContainer();
        grid = new CubeController[width, height];
        gridLayout = new string[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                gridLayout[x, y] = layout[y * width + x];
                Vector2 position = GetCubePosition(x, y); // Adjust for bottom-left origin

                string typeStr = layout[y * width + x];
                CubeType cubeType = DetermineCubeType(typeStr);

                GameObject cubeObj = CubeFactory.Instance.CreateCube(cubeType, x, y);

                cubeObj.transform.position = position;
                cubeObj.transform.rotation = Quaternion.identity;

                cubes.Add(cubeObj);

                cubeObj.transform.SetParent(gridContainer.transform, false);
                cubeObj.GetComponent<RectTransform>().anchoredPosition = position;

                CubeController cube = cubeObj.GetComponent<CubeController>();

                grid[x, y] = cube;
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
        return CubeUtils.ConvertStringToCubeType(typeStr);
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

        int randomIndex = UnityEngine.Random.Range(0, possibleTypes.Length);
        return possibleTypes[randomIndex];
    }

    public void OnCubeClicked(CubeController clickedCube)
    {
        // Prevent multiple clicks while processing matches
        if (isProcessing)
            return;
        else
            isProcessing = true;

        if (clickedCube.type == CubeType.TNT)
        {
            HandleTNTExplosion(clickedCube);
        }
        else
        {
            foreach (var match in matches)
            {
                if (match.Contains(clickedCube))
                {
                    if (match.Count >= 5)
                    {
                        gridLayout[clickedCube.GetX(), clickedCube.GetY()] = CubeUtils.ConvertCubeTypeToString(CubeType.TNT);
                        clickedCube.ConvertToTNT();
                        grid[clickedCube.GetX(), clickedCube.GetY()] = clickedCube;
                        // match.Remove(clickedCube);
                    }
                    GameManager.Instance.UseMove();
                    HandleMatch(match);
                    break; // Assuming a cube can be part of only one match at a time
                }
            }
        }

        isProcessing = false;
    }

    private void HandleMatch(List<CubeController> match)
    {
        RemoveCubes(match);
        PrintEmptySpaceCount();
        StartCoroutine(UpdateGridAfterRemoval());
    }

    private void HandleTNTExplosion(CubeController tntCube)
    {
        int radius = 5;
        if (CheckNearByTNT(new Vector2Int(tntCube.GetX(), tntCube.GetY())))
        {
            radius = 7;
        }
        List<Vector2Int> affectedPositions = CubeUtils.CalculateTNTEffectedPositions(new Vector2Int(tntCube.GetX(), tntCube.GetY()), radius, width, height);

        RemoveCube(tntCube);

        RemoveTNTEffectedCubes(affectedPositions);
        StartCoroutine(UpdateGridAfterRemoval());
    }
    private bool CheckNearByTNT(Vector2Int position)
    {
        for (int x = position.x - 1; x <= position.x + 1; x++)
        {
            for (int y = position.y - 1; y <= position.y + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    if (grid[x, y] != null && grid[x, y].type == CubeType.TNT && (x != position.x || y != position.y))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private void FindAllMatches()
    {
        matches.Clear();
        tntMatches.Clear();
        bool[,] visited = new bool[width, height];

        RemoveTNTHints();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y])
                {
                    List<CubeController> match = new List<CubeController>();
                    CheckAdjacentCubes(x, y, grid[x, y].type, ref match, ref visited);

                    if (match.Count >= 2) // Minimum match size
                    {
                        matches.Add(match);
                    }

                    if (match.Count >= 5)
                    {
                        tntMatches.Add(match);
                    }
                }
            }
        }

        ApplyTNTHint(tntMatches.SelectMany(x => x).ToList());
    }

    private void RemoveTNTHints()
    {
        foreach (CubeController cube in grid)
        {
            if (cube != null)
            {
                cube.DeactivateTNTHint();
            }
        }
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
        List<Vector2Int> cubePositions = new List<Vector2Int>();
        foreach (CubeController cube in cubes)
        {
            cubePositions.Add(new Vector2Int(cube.GetX(), cube.GetY()));
            if (gridLayout[cube.GetX(), cube.GetY()] != CubeUtils.ConvertCubeTypeToString(CubeType.TNT)) RemoveCube(cube);
        }
        OnCubeMatch?.Invoke(cubePositions);
    }

    private void RemoveTNTEffectedCubes(List<Vector2Int> cubePositions)
    {
        foreach (Vector2Int cubePosition in cubePositions)
        {
            CubeController cube = grid[cubePosition.x, cubePosition.y];
            if (cube == null)
            {
                gridLayout[cubePosition.x, cubePosition.y] = null;
                continue;
            }
            if (cube.type == CubeType.TNT)
            {
                HandleTNTExplosion(cube);
            }
            else if (cube.type == CubeType.Box || cube.type == CubeType.Stone || cube.type == CubeType.Vase)
            {
                ObstacleController obstacle = cube.GetComponent<ObstacleController>();
                obstacle.PlayDestructionEffect();
                obstacle.GetTNTHit();
            }
            else
            {
                RemoveCube(cube);
            }
        }
    }

    private void RemoveCube(CubeController cube)
    {
        cube.PlayDestructionEffect();
        grid[cube.GetX(), cube.GetY()] = null;
        gridLayout[cube.GetX(), cube.GetY()] = null;

        cube.SetXY(-1, -1);

        CubePoolManager.Instance.ReturnCube(cube.gameObject);
    }

    private IEnumerator UpdateGridAfterRemoval()
    {
        bool[] columnNeedsFill = new bool[width];
        for (int x = 0; x < width; x++)
        {
            int firstEmptySpace = -1; // Track the first empty space found in this column

            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null && firstEmptySpace == -1)
                {
                    firstEmptySpace = y; // Found the first empty space in this column
                    columnNeedsFill[x] = true; // This column will need new cubes at the top
                }
                else if (grid[x, y] != null && firstEmptySpace != -1 && grid[x, y].IsMovable)
                {
                    // Found a movable cube and there's an empty space below it
                    grid[x, firstEmptySpace] = grid[x, y]; // Move the cube down to the first empty space
                    grid[x, firstEmptySpace].SetXY(x, firstEmptySpace); // Update the cube's logical position
                    grid[x, y] = null; // Clear the old position

                    // Update the cube's visual position with animation
                    StartCoroutine(MoveCubeAnimation(grid[x, firstEmptySpace].gameObject, new Vector2Int(x, firstEmptySpace)));

                    columnNeedsFill[x] = true; // This column will need new cubes at the top

                    // Continue checking from the position just below the moved cube
                    y = firstEmptySpace;
                    firstEmptySpace = -1; // Reset firstEmptySpace for the next empty space
                }
            }
        }

        yield return new WaitForSeconds(timeToMove);

        // Fill the top of each column as needed
        for (int x = 0; x < width; x++)
        {
            if (columnNeedsFill[x])
            {
                FillColumnTop(x);
            }
        }

        FindAllMatches();
    }

    private void FillColumnTop(int columnIndex)
    {
        int firstEmptySpace = -1; // Find the first empty space from the bottom
        for (int y = 0; y < height; y++)
        {
            if (grid[columnIndex, y] == null)
            {
                firstEmptySpace = y;
                break; // Stop at the first empty space
            }
        }
        int createCubeCount = 0;
        // If there's an empty space, start filling from there up
        if (firstEmptySpace != -1)
        {
            for (int y = firstEmptySpace; y < height; y++)
            {
                createCubeCount++;
                Vector2 startPosition = GetCubePosition(columnIndex, height + y); // Ensure this is above the grid
                CubeType cubeType = DetermineCubeType("rand");
                GameObject cubeObj = CubeFactory.Instance.CreateCube(cubeType, columnIndex, y);
                cubeObj.transform.SetParent(gridContainer.transform, false);

                // Directly set to start position without affecting the anchored position just yet
                cubeObj.transform.position = startPosition;
                cubeObj.transform.rotation = Quaternion.identity;

                CubeController cube = cubeObj.GetComponent<CubeController>();
                cube.SetXY(columnIndex, y);
                cube.DeactivateTNTHint();

                grid[columnIndex, y] = cube;
                gridLayout[columnIndex, y] = CubeUtils.ConvertCubeTypeToString(cubeType);

                cubeObj.GetComponent<RectTransform>().anchoredPosition = startPosition;
                // Animate the cube dropping into place
                StartCoroutine(MoveCubeAnimation(cubeObj, new Vector2Int(columnIndex, y)));
            }
        }

        Debug.Log("Created " + createCubeCount + " cubes in column " + columnIndex);
    }


    IEnumerator MoveCubeAnimation(GameObject cube, Vector2Int targetPosition)
    {
        float t = 0;
        RectTransform rectTransform = cube.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 modifiedTargetPosition = GetCubePosition(targetPosition.x, targetPosition.y);

        // Define an animation curve with a 'bounce' effect at the end
        AnimationCurve curve = new AnimationCurve(
            new Keyframe(0, 0, 0, 2),
            new Keyframe(0.7f, 0.9f, 1, 1),
            new Keyframe(1, 1, 1, 0)
        );

        cube.transform.SetParent(gridContainer.transform, false);

        while (t < 1)
        {
            t += Time.deltaTime / timeToMove;
            float curveValue = curve.Evaluate(t);
            rectTransform.anchoredPosition = Vector2.LerpUnclamped(startPosition, modifiedTargetPosition, curveValue);
            yield return null;
        }

        // Ensure the cube is exactly at the target position at the end of the animation
        rectTransform.anchoredPosition = modifiedTargetPosition;
    }

    private void PrintEmptySpaceCount()
    {
        int emptySpaces = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == null)
                {
                    emptySpaces++;
                }
            }
        }
        Debug.Log("Empty spaces: " + emptySpaces);
    }

    private void ApplyTNTHint(List<CubeController> tntMatch)
    {
        foreach (CubeController cube in tntMatch)
        {
            cube.ActivateTNTHint();
        }
    }
}
