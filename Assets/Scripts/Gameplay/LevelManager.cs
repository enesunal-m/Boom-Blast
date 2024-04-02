using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int currentLevel;
    public int maxLevel = 10;

    public UIManager uiManager;

    public static LevelManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UIManager.OnUIReady += SetupUIManager;
    }

    void Start()
    {
        LoadCurrentLevel();
        UpdateLevelUI();
    }

    public void LevelCompleted()
    {
        uiManager.ShowWinPopup();
        NextLevel();
    }

    public int GetCurrentLevel()
    {
        if (currentLevel < 1)
        {
            currentLevel = 1;
        }
        return currentLevel;
    }

    private void LoadCurrentLevel()
    {
        // Get the saved level number from PlayerPrefs, default to 1 if not set
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelScene");
    }

    public void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

        if (currentLevel <= maxLevel)
        {
            SceneManager.LoadScene("LevelScene");
            UpdateLevelUI();
        }
        else
        {
            // What happens when all levels are completed.
            currentLevel = maxLevel; // Reset to 1 or to the first unfinished level
            PlayerPrefs.SetInt("CurrentLevel", currentLevel); // Save the reset level
            PlayerPrefs.Save(); // Save PlayerPrefs changes to disk
            uiManager.SetLevelText("Finished");
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateLevelUI()
    {
        uiManager.SetLevelText(currentLevel);
    }

    public void ResetProgress()
    {
        currentLevel = 1;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
        UpdateLevelUI();
    }

    private void SetupUIManager()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            uiManager.SetLevelText(currentLevel);
        }
    }

    void OnDestroy()
    {
        UIManager.OnUIReady -= SetupUIManager; // Unsubscribe from the event
    }
}
