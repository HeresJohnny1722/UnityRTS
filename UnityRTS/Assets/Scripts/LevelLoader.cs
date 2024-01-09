using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Manages scene transitions and has a nice fade in animation
/// </summary>
public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelSelectCanvas;

    /// <summary>
    /// Loads the next level in the build index
    /// </summary>
    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }


    /// <summary>
    /// Loads the level by build index
    /// </summary>
    /// <param name="index"></param>
    public void LoadLevelByIndex(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    /// <summary>
    /// Reloads the scene by loading the active scene
    /// </summary>
    public void ReloadScene()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    /// <summary>
    /// Loads the level with a index, also calls the animation fade trigger
    /// </summary>
    /// <param name="levelIndex"></param>
    /// <returns></returns>
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);

        if (SceneManager.GetActiveScene().buildIndex == 3 || SceneManager.GetActiveScene().buildIndex == 4)
        {
            yield return new WaitForSeconds(transitionTime);

            Time.timeScale = 0f;
        }
    }
}
