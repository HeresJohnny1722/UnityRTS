using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Organizes sound effects and creates a method that is easy to call sound effects
/// </summary>
public class SoundFeedback : MonoBehaviour
{
    [SerializeField]
    private AudioClip clickSound, placeSound, removeSound, wrongPlacementSound, mainMenuSoundtrack, levelSoundtrack, gunSound, hitSound;

    [SerializeField]
    private AudioSource audioSource;

    private static SoundFeedback _instance;
    public static SoundFeedback Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            audioSource.PlayOneShot(mainMenuSoundtrack);
        } else
        {
            audioSource.PlayOneShot(levelSoundtrack);
        }
    }

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Click:
                audioSource.PlayOneShot(clickSound);
                break;
            case SoundType.Place:
                audioSource.PlayOneShot(placeSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.wrongPlacement:
                audioSource.PlayOneShot(wrongPlacementSound);
                break;
            case SoundType.Gun:
                audioSource.PlayOneShot(gunSound);
                break;
            case SoundType.Hit:
                audioSource.PlayOneShot(hitSound);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    Click,
    Place,
    Remove,
    wrongPlacement,
    Gun,
    Hit,
}