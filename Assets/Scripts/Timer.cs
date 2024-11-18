using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public Image circleImage;
    public float totalTime = 5f;
    [SerializeField] private float timeRemaining;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button scanButton;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private SpawnManager spawnManager;

    void Start()
    {
        timeRemaining = totalTime;
        circleImage.fillAmount = 1f;
        playAgainButton.onClick.AddListener(PlayAgain);
        playAgainButton.gameObject.SetActive(false);
        scanButton.gameObject.SetActive(true);
        highScoreText.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("highscore") == 0) PlayerPrefs.SetInt("highscore", 0);
    }

    void Update()
    {
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            circleImage.fillAmount = timeRemaining / totalTime;
        }
        else
        {
            circleImage.fillAmount = 0f;
            GameEnded();
        }
    }

    void GameEnded()
    {
        playAgainButton.gameObject.SetActive(true);
        scanButton.gameObject.SetActive(false);
        highScoreText.gameObject.SetActive(true);
        if(spawnManager.GetScore() > PlayerPrefs.GetInt("highscore"))
        {
            PlayerPrefs.SetInt("highscore", spawnManager.GetScore());
        }
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("highscore").ToString();

    }

    void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
