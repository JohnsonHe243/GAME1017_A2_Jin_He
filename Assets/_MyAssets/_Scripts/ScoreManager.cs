using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TMP_Text timertext;
    [SerializeField] TMP_Text highScoreText;
    private float startTime; // We're going to use Time.time;
    public float elapsedTime;

    private Coroutine timerCoroutine;

    private void Start()
    {
        startTime = Time.time; // Gives us a number of seconds.
        if (SceneManager.GetActiveScene().buildIndex == 1)
            StartCoroutine("UpdateTimer");
    }

    private void Update()
    {
        CheckHighScore();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            UpdateHighScoreText();
        }
    }

    private IEnumerator UpdateTimer()
    {
        while (true)
        {
            elapsedTime = Time.time - startTime;
            timertext.text = "Time: " + elapsedTime.ToString("F3") + "s";
            yield return null;
        }
    }
    void CheckHighScore()
    {
        if (elapsedTime > PlayerPrefs.GetFloat("HighScore", 0))
        {
            PlayerPrefs.SetString("HighScore", elapsedTime.ToString("F3"));
        }
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = $"HighScore: {PlayerPrefs.GetFloat("HighScore", 0)} + s";
    }

    public void StopTimer()
    {
        StopCoroutine(timerCoroutine);
    }
}
