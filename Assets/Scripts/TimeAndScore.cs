﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TimeAndScore : MonoBehaviour
{
    public GameObject scoreText;
    public GameObject coinText;
    public GameObject gemText;

    public GameObject finalScoreText;
    public GameObject finalTimeRemainingText;
    public GameObject finalOutcomeText;
    public GameObject finalGemText;
    public GameObject finalCoinText;
    public GameObject finalDeaths;

    public static int score;
    public static int coins;
    public static int gems;
    public static float timeRemaining;
    public static int numberOfDeaths;

    int TotalCoins = 0;
    int TotalGems = 0;

    public static bool GameOver;
    public static bool win = false;

    public GameObject gameOverPanel;
    public GameObject timerSlider;

    float TimeTaken = 0;
    float finalScore = 0;

    bool doneCalculatingScore = false;
    bool doneCalculatingTime = false;

    public static bool UpdateMap = true;

    public AudioClip timeTick;
    public AudioClip scoreTick;

    public GameObject retryButton;
    public GameObject continueButton;

    void Awake()
    {
        doneCalculatingScore = false;
        doneCalculatingTime = false;
        numberOfDeaths = 0;
        score = 0;
        coins = 0;
        gems = 0;
        GameOver = false;
        win = false;
        timeRemaining = 240;
        if (timerSlider != null)
            timerSlider.GetComponent<Slider>().maxValue = timeRemaining;
    }

    // Use this for initialization
    void Start()
    {
        doneCalculatingScore = false;
        doneCalculatingTime = false;
        numberOfDeaths = 0;
        score = 0;
        coins = 0;
        gems = 0;
        GameOver = false;
        win = false;
        timeRemaining = 240;
        if (timerSlider != null)
            timerSlider.GetComponent<Slider>().maxValue = timeRemaining;
    }

    // Update is called once per frame
    void Update()
    {
        if (TotalCoins == 0)
        {
            TotalCoins = GameObject.FindGameObjectsWithTag("Coin").Length;
        }
        if (TotalGems == 0)
        {
            TotalGems = GameObject.FindGameObjectsWithTag("Gem").Length;
        }
        AudioSource audio = GetComponent<AudioSource>();
        if (!GameOver)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (Input.GetKeyDown(KeyCode.L))
                {
                    win = true;
                    GameOver = true;
                    GameObject.Find("ScorePanel").SetActive(false);
                    GameObject.Find("UIParent").SetActive(false);
                    GameObject.Find("PowerBar").SetActive(false);
                    GameObject.Find("TimeRemainingSlider").SetActive(false);
                    GameObject.Find("PauseButtonBack").SetActive(false);
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().speed = 0;
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = false;
                    TimeAndScore.score -= TimeAndScore.numberOfDeaths * 10;
                    TimeAndScore.score += (int)TimeAndScore.timeRemaining;
                    TimeAndScore.score += (int)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().foxPowerLevelUI.GetComponent<Slider>().value * 20;
                    TimeAndScore.score += (int)GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().wolfPowerLevelUI.GetComponent<Slider>().value * 20;
                }
            }
            if (timeRemaining <= 30)
            {
                GetComponent<AudioSource>().pitch = 1.02f;
            }
            if (timeRemaining >= 0)
            {
                timeRemaining -= Time.deltaTime;
                if (timerSlider != null)
                    timerSlider.GetComponent<Slider>().value = timeRemaining;
                if (scoreText != null)
                    scoreText.GetComponent<Text>().text = score.ToString();
                if (coinText != null)
                    coinText.GetComponent<Text>().text = coins.ToString();
                if (gemText != null)
                    gemText.GetComponent<Text>().text = gems.ToString();
            }
            else
            {
                timeRemaining = 0;
                doneCalculatingTime = true;
                GameOver = true;
            }
        }
        else
        {
            if (GameObject.Find("ScorePanel"))
                GameObject.Find("ScorePanel").SetActive(false);
            if (GameObject.Find("UIParent"))
                GameObject.Find("UIParent").SetActive(false);
            if (GameObject.Find("PowerBar"))
                GameObject.Find("PowerBar").SetActive(false);
            if (GameObject.Find("TimeRemainingSlider"))
                GameObject.Find("TimeRemainingSlider").SetActive(false);
            if (GameObject.Find("PauseButtonBack"))
                GameObject.Find("PauseButtonBack").SetActive(false);
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().speed = 0;
            gameOverPanel.SetActive(true);
            if (win)
            {
                finalOutcomeText.GetComponent<Text>().text = "Level Complete!";
                retryButton.SetActive(false);
            }
            else
            {
                finalOutcomeText.GetComponent<Text>().text = "You ran out of time";
                continueButton.SetActive(false);
            }

            finalScoreText.GetComponent<Text>().text = "" + (int)finalScore;
            finalDeaths.GetComponent<Text>().text = "Number of Deaths : " + numberOfDeaths;
            finalGemText.GetComponent<Text>().text = "Gems : " + gems + "/" + TotalGems;
            finalCoinText.GetComponent<Text>().text = "Coins : " + coins + "/" + TotalCoins;
            finalTimeRemainingText.GetComponent<Text>().text = "Time Remaining : " + (int)TimeTaken;
            if (TimeTaken <= timeRemaining && doneCalculatingScore)
            {
                TimeTaken += Time.deltaTime * 100;
                if (!audio.isPlaying)
                {
                    audio.clip = timeTick;
                    audio.Play();
                }
            }
            else if (TimeTaken > timeRemaining && doneCalculatingScore)
            {
                TimeTaken = timeRemaining;
                doneCalculatingTime = true;
            }

            if (finalScore <= score && !doneCalculatingScore)
            {
                finalScore += Time.deltaTime * 250;
                if (!audio.isPlaying)
                {
                    audio.clip = scoreTick;
                    audio.Play();
                }
            }
            else if (finalScore > score && !doneCalculatingScore)
            {
                finalScore = score;
                doneCalculatingScore = true;
            }
        }
        if (doneCalculatingScore && doneCalculatingTime)
        {
            switch (Application.loadedLevelName)
            {
                case "Level_1":
                    {
                        if (finalScore > PlayerPrefs.GetInt("level1HighScore"))
                            PlayerPrefs.SetInt("level1HighScore", (int)finalScore);
                        if (timeRemaining > PlayerPrefs.GetInt("level1FastestTime") || PlayerPrefs.GetInt("level1FastestTime") == 0)
                            PlayerPrefs.SetInt("level1FastestTime", (int)timeRemaining);
                        break;
                    }
                case "Level_2":
                    {
                        if (finalScore > PlayerPrefs.GetInt("level2HighScore"))
                            PlayerPrefs.SetInt("level2HighScore", (int)finalScore);
                        if (timeRemaining > PlayerPrefs.GetInt("level2FastestTime") || PlayerPrefs.GetInt("level2FastestTime") == 0)
                            PlayerPrefs.SetInt("level2FastestTime", (int)timeRemaining);
                        break;
                    }
                case "Level_3":
                    {
                        if (finalScore > PlayerPrefs.GetInt("level3HighScore"))
                            PlayerPrefs.SetInt("level3HighScore", (int)finalScore);
                        if (timeRemaining > PlayerPrefs.GetInt("level3FastestTime") || PlayerPrefs.GetInt("level3FastestTime") == 0)
                            PlayerPrefs.SetInt("level3FastestTime", (int)timeRemaining);
                        break;
                    }
                case "Level_4":
                    {
                        if (finalScore > PlayerPrefs.GetInt("level4HighScore"))
                            PlayerPrefs.SetInt("level4HighScore", (int)finalScore);
                        if (timeRemaining > PlayerPrefs.GetInt("level4FastestTime") || PlayerPrefs.GetInt("level4FastestTime") == 0)
                            PlayerPrefs.SetInt("level4FastestTime", (int)timeRemaining);
                        break;
                    }
                case "Level_5":
                    {
                        if (finalScore > PlayerPrefs.GetInt("level5HighScore"))
                            PlayerPrefs.SetInt("level5HighScore", (int)finalScore);
                        if (timeRemaining > PlayerPrefs.GetInt("level5FastestTime") || PlayerPrefs.GetInt("level5FastestTime") == 0)
                            PlayerPrefs.SetInt("level5FastestTime", (int)timeRemaining);
                        break;
                    }
            }
            finalScoreText.GetComponent<Text>().text = ""+(int)finalScore;
            finalDeaths.GetComponent<Text>().text = "Number of Deaths : " + numberOfDeaths;
            finalGemText.GetComponent<Text>().text = "Gems : " + gems + "/" + TotalGems;
            finalCoinText.GetComponent<Text>().text = "Coins : " + coins + "/" + TotalCoins;
            finalTimeRemainingText.GetComponent<Text>().text = "Time Remaining : " + (int)TimeTaken;
            this.enabled = false;
        }
    }
}
