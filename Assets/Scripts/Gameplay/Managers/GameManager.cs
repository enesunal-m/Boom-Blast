using System.Collections.Generic;
using UnityEngine;
using static CubeSpriteManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private LevelData levelData;
    private int currentMoveCount;

    public UIManager uiManager;
    public LevelLoader levelLoader;

    private List<Objective> objectives = new List<Objective>();

    [Header("Shake Effects")]
    [SerializeField]
    private CameraShake cameraShake;
    [SerializeField]
    private UIShake uiShake;
    [SerializeField]
    private float shakeDuration = 0.5f;
    [SerializeField]
    private float shakeMagnitude = 0.5f;

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

    private void Start()
    {
        StartGame();

        InitializeObjectives();
    }

    public void StartGame()
    {
        int level = LevelManager.Instance.GetCurrentLevel();
        levelData = levelLoader.LoadLevel(level);
        currentMoveCount = levelData.move_count;

        uiManager.SetMoveCountText(levelData.move_count);

        GridManager.Instance.CreateGrid(levelData.grid_width, levelData.grid_height, levelData.grid);
    }

    public void UseMove()
    {
        currentMoveCount--;
        if (currentMoveCount < 0)
        {
            currentMoveCount = 0;
        }
        uiManager.SetMoveCountText(currentMoveCount);
        if (!ObjectivesExist())
        {
            CompleteLevel();
        }
        else if (currentMoveCount <= 0)
        {
            uiManager.ShowLosePopup();
        }
    }

    public bool ObjectivesExist()
    {
        bool objectiveExist = false;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (objectives[i].requiredCount > 0)
            {
                objectiveExist = true;
                break;
            }
        }

        return objectiveExist;
    }


    private void InitializeObjectives()
    {
        objectives.Clear();
        Objective stoneObjective = new Objective(CubeType.Stone, 0);
        Objective vaseObjective = new Objective(CubeType.Vase, 0);
        Objective boxObjective = new Objective(CubeType.Box, 0);

        foreach (var cubeType in levelData.grid)
        {
            switch (CubeUtils.ConvertStringToCubeType(cubeType))
            {
                case CubeType.Stone:
                    stoneObjective.requiredCount++;
                    break;
                case CubeType.Vase:
                    vaseObjective.requiredCount++;
                    break;
                case CubeType.Box:
                    boxObjective.requiredCount++;
                    break;
            }
        }

        if (stoneObjective.requiredCount > 0)
        {
            objectives.Add(stoneObjective);
        }

        if (vaseObjective.requiredCount > 0)
        {
            objectives.Add(vaseObjective);
        }

        if (boxObjective.requiredCount > 0)
        {
            objectives.Add(boxObjective);
        }

        uiManager.SetObjectives(objectives);
    }

    public void ObstacleCleared(CubeType obstacleType)
    {
        foreach (Objective objective in objectives)
        {
            if (objective.obstacleType == obstacleType)
            {
                objective.ObstacleCleared();

                uiManager.UpdateObjective(objective);

                CheckObjectivesCompletion();
                break;
            }
        }
    }

    private void CheckObjectivesCompletion()
    {
        foreach (Objective objective in objectives)
        {
            if (!objective.IsCompleted())
            {
                return;
            }
        }

        CompleteLevel();
    }

    private void CompleteLevel()
    {
        LevelManager.Instance.LevelCompleted(3f);
    }

    public void ShakeScreen()
    {
        StartCoroutine(uiShake.Shake(shakeDuration, shakeMagnitude));
    }
}
