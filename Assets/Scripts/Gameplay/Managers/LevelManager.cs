using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private int currentLevel;
    [SerializeField]
    private bool overrideSavedLevel = false;
    public int maxLevel = 10;

    private bool isFinished = false;

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

    private UIManager GetUIManager()
    {
        if (uiManager != null)
        {
            return uiManager;
        }
        return FindObjectOfType<UIManager>();
    }

    public void LevelCompleted()
    {
        GetUIManager().ShowWinPopup();
        NextLevel();
    }

    public void LevelCompleted(float delay)
    {
        StartCoroutine(LevelCompletedDelayed(delay));
    }

    private IEnumerator LevelCompletedDelayed(float delay)
    {
        GetUIManager().ShowWinPopup();
        yield return new WaitForSeconds(delay);
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

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    private void LoadCurrentLevel()
    {
        if (overrideSavedLevel)
        {
            PlayerPrefs.SetInt("CurrentLevel", currentLevel);
            PlayerPrefs.Save();
        }
        else
        {
            currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
            if (currentLevel > maxLevel)
            {
                currentLevel = 1;
                PlayerPrefs.SetInt("CurrentLevel", 1);
                PlayerPrefs.Save();
            }
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();

        if (currentLevel <= maxLevel)
        {
            SceneManager.LoadScene("MainScene");
            UpdateLevelUI();
        }
        else
        {
            isFinished = true;
            currentLevel = 1;
            SceneManager.LoadScene("MainScene");
            PlayerPrefs.SetInt("CurrentLevel", 1);
            PlayerPrefs.Save();
            UpdateLevelUI();
        }
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void UpdateLevelUI()
    {
        if (isFinished)
        {
            GetUIManager().SetLevelText("Finished!");
            ResetProgress();
        }
        else
        {
            GetUIManager().SetLevelText(currentLevel);
        }
    }

    public void ResetProgress()
    {
        currentLevel = 1;
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }

    private void SetupUIManager()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager != null)
        {
            UpdateLevelUI();
        }
    }

    void OnDestroy()
    {
        UIManager.OnUIReady -= SetupUIManager; // Unsubscribe from the event
    }
}
