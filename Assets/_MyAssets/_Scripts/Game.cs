using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
    [SerializeField] GameObject panel;

    public static Game Instance { get; private set; } // Static object of the class.
    public SoundManager SOMA;


    private void Awake() // Ensure there is only one instance.
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Will persist between scenes.
            Initialize();
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances.
        }
    }

    private void Initialize()
    {
        SOMA = new SoundManager();
        SOMA.Initialize(gameObject);
        SOMA.AddSound("Jump", Resources.Load<AudioClip>("jump"), SoundManager.SoundType.SOUND_SFX);
        SOMA.AddSound("Roll", Resources.Load<AudioClip>("roll"), SoundManager.SoundType.SOUND_SFX);
        SOMA.AddSound("StillDre", Resources.Load<AudioClip>("StillDre"), SoundManager.SoundType.SOUND_MUSIC);
        SOMA.AddSound("I_Ran", Resources.Load<AudioClip>("I_Ran"), SoundManager.SoundType.SOUND_MUSIC);
        SOMA.PlayMusic("I_Ran");
    }
    public void OptionOpen()
    {
        panel.transform.position = new Vector3(500, 400, 0);
        Time.timeScale = 0;
    }
    public void OptionClose()
    {
        panel.transform.position = new Vector3(-900, 0, 0);
        Time.timeScale = 1;
    }

}
