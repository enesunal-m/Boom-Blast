using System.Collections.Generic;
using static CubeSpriteManager;

[System.Serializable]
public class LevelData
{
    public int level_number;
    public int grid_width;
    public int grid_height;
    public int move_count;
    public List<string> grid;
}

[System.Serializable]
public class Objective
{
    public CubeType obstacleType;
    public int requiredCount;
    public int currentCount;

    public Objective(CubeType obstacleType, int requiredCount)
    {
        this.obstacleType = obstacleType;
        this.requiredCount = requiredCount;
        this.currentCount = 0;
    }

    public void ObstacleCleared()
    {
        currentCount++;
    }

    public bool IsCompleted() => currentCount >= requiredCount;
}
