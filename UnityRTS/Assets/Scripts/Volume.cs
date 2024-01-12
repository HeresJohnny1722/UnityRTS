using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Saves and Loads the volume of the game
/// </summary>
public class Volume : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider = null;

    [SerializeField] private Text volumeTextUI = null;

    private void Start()
    {
        LoadValues();
    }

    public void VolumeSlider(float volume)
    {
//        volumeTextUI.text = volume.ToString("0.0");
    }

    public void SaveVolumeButton()
    {
        float volumevalue = volumeSlider.value;
        SQLdatabase.Instance.SaveVolume(volumevalue);
        //PlayerPrefs.SetFloat("VolumeValue", volumevalue);
        LoadValues();
    }

    void LoadValues()
    {
        //float volumeValue = PlayerPrefs.GetFloat("VolumeValue");

        SQLdatabase.Instance.LoadVolume();
        volumeSlider.value = AudioListener.volume;
    }
}
