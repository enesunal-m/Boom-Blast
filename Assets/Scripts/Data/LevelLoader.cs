using UnityEngine;
using System.IO;

public class LevelLoader : MonoBehaviour
{
    public LevelData LoadLevel(int levelNumber)
    {
        string levelNumberText = levelNumber.ToString().PadLeft(2, '0');
        string path = Path.Combine(Application.streamingAssetsPath, $"level_{levelNumberText}.json");
        if (File.Exists(path))
        {
            string jsonContents = File.ReadAllText(path);
            return JsonUtility.FromJson<LevelData>(jsonContents);
        }
        else
        {
            Debug.LogError("Level file not found.");
            return null;
        }
    }
}
