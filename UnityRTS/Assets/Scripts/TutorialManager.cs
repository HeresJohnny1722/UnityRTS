using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Manages the tutorial popup text and showing the right UI while in the tutorial
/// </summary>
public class TutorialManager : MonoBehaviour
{
    [SerializeField] private WaveSpawner waveSpawner;
    public Animator animtor;
    public TextMeshProUGUI[] tutorialTextObjects;
    private int currentIndex = 0;

    private void Start()
    {
        // Ensure all tutorial objects are initially hidden
        foreach (var textObject in tutorialTextObjects)
        {
            textObject.gameObject.SetActive(false);
        }

        foreach (var button in GameManager.instance.buildingButtons)
        {
            button.SetActive(false);
        }

        GameManager.instance.populationText.transform.parent.gameObject.SetActive(false);
        GameManager.instance.populationText.gameObject.SetActive(false);
        GameManager.instance.goldText.gameObject.SetActive(false);
        GameManager.instance.woodText.gameObject.SetActive(false);
        GameManager.instance.foodText.gameObject.SetActive(false);
        GameManager.instance.towerText.gameObject.SetActive(false);

    // Show the first tutorial text
    ShowCurrentTutorialText();
    }

    /// <summary>
    /// Goes to the next text popup in the tutorial when enter is pressed
    /// </summary>
    private void Update()
    {
        // Check for Enter key press
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // Hide the current text and show the next one
            HideCurrentTutorialText();
            ShowNextTutorialText();
        }
    }

    private void ShowCurrentTutorialText()
    {
        if (currentIndex >= 0 && currentIndex < tutorialTextObjects.Length)
        {
            tutorialTextObjects[currentIndex].gameObject.SetActive(true);
        }
    }

    private void HideCurrentTutorialText()
    {
        if (currentIndex >= 0 && currentIndex < tutorialTextObjects.Length)
        {
            tutorialTextObjects[currentIndex].gameObject.SetActive(false);
        }
    }

    private void ShowNextTutorialText()
    {
        // Move to the next index
        currentIndex++;

        if (currentIndex == 5)
        {
            GameManager.instance.buildingButtons[0].SetActive(true);
            GameManager.instance.populationText.transform.parent.gameObject.SetActive(true);
            GameManager.instance.populationText.gameObject.SetActive(true);

        } else if (currentIndex == 7)
        {
            GameManager.instance.buildingButtons[1].SetActive(true);
            GameManager.instance.goldText.gameObject.SetActive(true);
            
        }
        else if (currentIndex == 8)
        {
            GameManager.instance.buildingButtons[2].SetActive(true);
            GameManager.instance.woodText.gameObject.SetActive(true);
            
        }
        else if (currentIndex == 9)
        {
            GameManager.instance.buildingButtons[3].SetActive(true);
            GameManager.instance.foodText.gameObject.SetActive(true);
        }
        else if (currentIndex == 10)
        {
            GameManager.instance.buildingButtons[4].SetActive(true);
            
        }
        else if (currentIndex == 13)
        {
            GameManager.instance.buildingButtons[5].SetActive(true);
            GameManager.instance.towerText.gameObject.SetActive(true);
        }
        else if (currentIndex == 14)
        {
            
            GameManager.instance.buildingButtons[6].SetActive(true);
            GameManager.instance.buildingButtons[7].SetActive(true);
            GameManager.instance.buildingButtons[8].SetActive(true);
        }

        if (currentIndex < tutorialTextObjects.Length)
        {
            // Show the next tutorial text
            ShowCurrentTutorialText();
        }
        else
        {
            // All tutorial texts have been shown
            TutorialComplete();
        }
    }

    /// <summary>
    /// Starts the enemies waves in 15 seconds after the tutorial is over
    /// </summary>
    private void TutorialComplete()
    {
        Debug.Log("Tutorial is over!");
        animtor.Play("ReturnTutorialPopup");
        StartCoroutine(waveSpawner.WaitAndStartWaves(15f));
    }
}
