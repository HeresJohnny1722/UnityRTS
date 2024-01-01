using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TextMeshProUGUI[] tutorialTextObjects;
    private int currentIndex = 0;

    private void Start()
    {
        // Ensure all tutorial objects are initially hidden
        foreach (var textObject in tutorialTextObjects)
        {
            textObject.gameObject.SetActive(false);
        }

        // Show the first tutorial text
        ShowCurrentTutorialText();
    }

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
        // Move to the next index and wrap around if necessary
        currentIndex = (currentIndex + 1) % tutorialTextObjects.Length;

        // Show the next tutorial text
        ShowCurrentTutorialText();
    }
}
