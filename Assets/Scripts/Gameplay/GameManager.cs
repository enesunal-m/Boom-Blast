using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private LevelData levelData;
    private int currentMoveCount;

    public UIManager uiManager;
    public LevelLoader levelLoader;

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
    }

    public void StartGame()
    {
        int level = LevelManager.Instance.GetCurrentLevel();
        Debug.Log("Level: " + level);
        levelData = levelLoader.LoadLevel(level);
        currentMoveCount = levelData.move_count;

        uiManager.SetMoveCountText(levelData.move_count);

        GridManager.Instance.CreateGrid(levelData.grid_width, levelData.grid_height, levelData.grid);
    }

    public void UseMove()
    {
        currentMoveCount--;
        uiManager.SetMoveCountText(currentMoveCount);
        if (currentMoveCount <= 0)
        {
            // TODO: GameOver();
        }
    }
}
