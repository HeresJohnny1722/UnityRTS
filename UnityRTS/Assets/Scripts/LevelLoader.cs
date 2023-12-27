using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    [SerializeField] private GameObject mainMenuCanvas;
    [SerializeField] private GameObject levelSelectCanvas;

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            LoadNextLevel();
        }*/
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadLevelByIndex(int index)
    {
        Debug.Log("Load Level " + index);
        StartCoroutine(LoadLevel(index));
    }

    public void ReloadScene()
    {
        //Debug.Log("Load Level " + index);
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    public void MainToLevelSelect()
    {
        StartCoroutine(LoadLevelSelect());

        

    }

    IEnumerator LoadLevelSelect()
    {
        transition.SetTrigger("Menu");

        yield return new WaitForSeconds(transitionTime);

        Debug.Log("Loading to Level Select");
        mainMenuCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }
}
