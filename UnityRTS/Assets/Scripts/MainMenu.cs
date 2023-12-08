using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private LevelLoader levelLoader;

    public void Play()
    {
        levelLoader.LoadNextLevel();
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("Player Has Quit");
    }
}
