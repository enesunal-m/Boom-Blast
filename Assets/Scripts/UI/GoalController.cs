using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;

public class GoalController : MonoBehaviour
{
    [SerializeField]
    private GameObject goalTickImage;

    [SerializeField]
    private Image goalImage;

    [SerializeField]
    private GameObject goalText;

    public void CompleteGoal()
    {
        goalTickImage.SetActive(true);
        goalText.SetActive(false);
    }

}