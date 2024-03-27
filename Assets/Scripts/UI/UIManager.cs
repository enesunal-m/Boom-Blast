using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static event Action OnUIReady;

    public TMP_Text levelText;
    public GameObject winPopup;
    public GameObject losePopup;

    void Awake()
    {
        OnUIReady?.Invoke();
    }

    void Start()
    {
        HideWinPopup();
        HideLosePopup();
    }
    public void SetLevelText(int level)
    {
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
}
