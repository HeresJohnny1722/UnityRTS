using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewLoadSave : MonoBehaviour
{
    public GameObject loadGameButton;
    public Transform cameraTransform;
    public GameObject savingLoadingScreen;

    public void OnNewGameClicked()
    {
        DataPersistenceManager.instance.NewGame();

        cameraTransform.position = new Vector3(1.7f, 0, 3.9f);
    }

    public void OnLoadGameClicked()
    {
        DataPersistenceManager.instance.LoadGame();

        cameraTransform.position = new Vector3(1.7f, 0, 3.9f);
    }

    public void OnSaveGameClicked()
    {
        DataPersistenceManager.instance.SaveGame();
    }

    public void Start()
    {
        savingLoadingScreen.gameObject.SetActive(true);
        //Time.timeScale = 0f;

        //There is no save file made already
        if (DataPersistenceManager.instance.gameData == null)
        {
            loadGameButton.SetActive(false);
        }

    }
}
