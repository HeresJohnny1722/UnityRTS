using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public GameObject levelSelect;
    public GameObject mainMenu;

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

    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(levelIndex);
    }

    public void PlayLoadLevelSelect()
    {
        StartCoroutine(LoadLevelSelect());
    }

    IEnumerator LoadLevelSelect()
    {
        transition.SetTrigger("Menu");

        yield return new WaitForSeconds(transitionTime);

        levelSelect.SetActive(true);
        mainMenu.SetActive(false);

        //SceneManager.LoadScene(levelIndex);
    }
}
