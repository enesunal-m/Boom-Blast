using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static CubeSpriteManager;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static event Action OnUIReady;

    [SerializeField]
    private TMP_Text levelText;
    [SerializeField]
    private TMP_Text moveText;
    [SerializeField]
    private GameObject winPopup;
    [SerializeField]
    private GameObject losePopup;

    [SerializeField]
    private GameObject objectivePrefab;
    [SerializeField]
    private Transform objectivesContainer;
    [SerializeField]
    private Transform objectivesTopRow;
    [SerializeField]
    private Sprite completedTickSprite;

    private Dictionary<CubeType, GameObject> objectiveUIElements = new Dictionary<CubeType, GameObject>(); // To track instantiated objective UI elements

    void Awake()
    {
        OnUIReady?.Invoke();
    }

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            HideWinPopup();
            HideLosePopup();
        }
    }

    public void SetMoveCountText(int moves)
    {
        if (moveText == null)
            return;
        moveText.text = $"{moves}";
    }

    public void SetLevelText(int level)
    {
        if (levelText == null)
            return;
        levelText.text = $"Level {level}";
    }

    public void SetLevelText(string text)
    {
        levelText.text = text;
    }

    public void ShowWinPopup()
    {
        winPopup.SetActive(true);
    }

    public void HideWinPopup()
    {
        if (winPopup.activeSelf)
        {
            winPopup.SetActive(false);
        }
    }

    public void ShowLosePopup()
    {
        losePopup.SetActive(true);
    }

    public void HideLosePopup()
    {
        if (losePopup.activeSelf)
        {
            losePopup.SetActive(false);
        }
    }

    public void RestartLevel()
    {
        LevelManager.Instance.RestartLevel();
    }

    public void ReturnToMenu()
    {
        LevelManager.Instance.ReturnToMenu();
    }

    public void SetObjectives(List<Objective> objectives)
    {
        int i = 0;
        foreach (var objective in objectives)
        {
            Transform parent = i < 2 ? objectivesTopRow : objectivesContainer;
            GameObject objectiveItem = Instantiate(objectivePrefab, parent);
            objectiveItem.GetComponentInChildren<Image>().sprite = CubeSpriteManager.Instance.GetSprite(objective.obstacleType);
            objectiveUIElements.Add(objective.obstacleType, objectiveItem);

            TMP_Text countText = objectiveItem.GetComponentInChildren<TMP_Text>();
            countText.text = $"{objective.requiredCount}";
            i++;
        }
    }

    public void UpdateObjective(Objective objective)
    {
        GameObject objectiveUI = objectiveUIElements[objective.obstacleType];

        if (objective.IsCompleted())
        {
            objectiveUI.GetComponent<GoalController>().CompleteGoal();
        }
        else
        {
            TMP_Text countText = objectiveUI.GetComponentInChildren<TMP_Text>();
            countText.text = $"{objective.requiredCount - objective.currentCount}";
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("LevelScene");
    }
}
