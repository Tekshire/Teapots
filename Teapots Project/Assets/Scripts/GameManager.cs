﻿#define TEST_LOSE_LIFE
#define TEST_LOSE_TEAPOT

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    // Since the main goal of Teapots is to become Tempest in a Teapot and Tempest
    // has shooting, make Teapots Blaster the default.
    public bool bPlayingTag = false;
    public bool bGameOver = false;
    // No input, spawning, or scoring if game no longer active.
    public bool isGameActive;
    public int iLives;      // Current lives; 0 => new game
    public int iTotalLives; // To calculate granting new life. (Includes lives you've lost)
    public int iScore;      // score for Teapot Blaster
    public int iTimer;      // score for Teapot Tag
    public int iTeapots;    // zero based 16 total / level
    public int iLevel;      // zero based 96 total
    public int iRange;      // iLevel % 16
    public int scorePerTeapot;
    public TextMeshProUGUI teapotsGameText; // Tag or Blaster?
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiScoreText;
    public TextMeshProUGUI loScoreText;
    public TextMeshProUGUI teapotsLeftText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI teapotsTitleText;

    public Button tagStartButton;
    public Button blasterStartButton;
    public Button restartButton;
    public GameObject scoreElement;
    public GameObject loScoreElement;
    public GameObject hiScoreElement;

    public GameObject teapotPrefab;

    // In Teapots, we know we only have 16 objects per level, so we can use an array.
    // When we begin to add in the Tempest objects, we can use the List recommended by
    // Unity in the Code Live Tutorials, Week 6, session 1.
    public GameObject[] teapots = new GameObject[16];


    void Start()
    {
        bGameOver = false;
        isGameActive = false;       // Not until Start button pressed
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        teapotsTitleText.gameObject.SetActive(true);
        tagStartButton.gameObject.SetActive(true);
        blasterStartButton.gameObject.SetActive(true);

        // Stuff that is the same for each level
        iScore = 0;
        iTimer = 0;
        iTeapots = 16;
        iLevel = 0;
        iRange = 0;

        if (bPlayingTag)
        {
            iLives = 1;
            scorePerTeapot = 0; // We don't destroy any teapots in tag
            bPlayingTag = true;
        }
        else
        {
            iLives = 3;
            scorePerTeapot = 1000 + (500 * iRange);
            bPlayingTag = false;
        }
        iTotalLives = iLives;
        UpdateLives(0);
        UpdateScore(0);
        UpdateHiScore(10101);
        UpdateLoScore(10101);
        UpdateTeapotsDisplay();

        StartNewLevel(iLevel);
    }


    public void StartNewLevel(int level)
    {
        // Create new teapots for this level
        for (int i = 0; i < teapots.Length; i++)
        {
            Vector3 startVector = startLocations[i];
            teapots[i] = Instantiate(teapotPrefab, startVector, Quaternion.identity);
        }
    }


    public void StartTagGame()
    {
        Debug.Log("StartTagGame.bPlayingTag = " + bPlayingTag);
        iLives = 1;
        scorePerTeapot = 0; // We don't destroy any teapots in tag
        bPlayingTag = true;
        teapotsGameText.SetText("Teapots Tag");
        loScoreElement.SetActive(true);
        hiScoreElement.SetActive(false);
        StartGame();
    }


    public void StartBlasterGame()
    {
        Debug.Log("StartBlasterGame.bPlayingTag = " + bPlayingTag);
        iLives = 3;
        scorePerTeapot = 1000 + (500 * iRange);
        bPlayingTag = false;
        //teapotsGameText.SetText("Teapots Blaster");   // Already set by LoadScene
        loScoreElement.SetActive(false);
        hiScoreElement.SetActive(true);
        StartGame();
    }


    // Teapot Tag and Teapot Blaster use the same game logic with just a few
    // parameters different between the two games. StartTagGame() and StartBlasterGame()
    // will set up the parameters that are different beteween the game versions.
    // StartGame() then do all the work to get the game playing.
    public void StartGame()
    {
        teapotsTitleText.gameObject.SetActive(false);
        tagStartButton.gameObject.SetActive(false);
        blasterStartButton.gameObject.SetActive(false);

        iTotalLives = iLives;
        UpdateLives(0);
        UpdateScore(0);
        UpdateHiScore(10101);
        UpdateLoScore(10101);
        UpdateTeapotsDisplay();

        bGameOver = false;
        isGameActive = true;
    }


    public void GameOver()
    {
        bGameOver = true;
        isGameActive = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        // Normally we would check to see if this is a new high score,
        // but for testing just go ahead and update it.
        UpdateHiScore(iScore);
        // Nope, Start overwrites the hi score.
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        // Don't need to set gameOverText or restartButton back to inActive
        // because that is the normal state that reloading the scene will restore.

        //// Just hit Restart button, so no need to immediately also hit Start Button.
        //// Call StartGame to dismiss Start button
        //StartGame();
        // LoadScene may not execute until next frame, at which time, it will
        // overwrite the variables we just called StartGame to set.
    }


    // Update is called once per frame
    // Other objects call FixedUpdate() for physics,
    // but Update() works more reliably for GetKeyDown().
    void Update()
    {
#if (TEST_LOSE_LIFE)
        // No way to die yet, but we want to test game over scenarios.
        if (Input.GetKeyDown(KeyCode.Period)) UpdateLives(-1);
#endif

#if (TEST_LOSE_TEAPOT)
        // Get rid of those pesky final teapots that are so hard to find
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            foreach (GameObject tp in teapots)
            {
                if (tp == null) continue;
                Destroy(tp);
                break;      // Only kill first one we find
            }

        }
#endif
    }


    // After all of the gameplay has happened, see how many teapots are left.
    void LateUpdate()
    {
        int remainingTeapots = 0;

        foreach (GameObject tp in teapots)
        {
            if (tp == null) continue;

            // Also check for non-white (!tagged) teapots.
            TeapotScript tpScript = tp.GetComponent<TeapotScript>();
            if (tpScript == null) continue;
            if (tpScript.isTagged) continue;

            remainingTeapots++;
        }   

        if (remainingTeapots != iTeapots)
        {
        iTeapots = remainingTeapots;
        UpdateTeapotsDisplay();

        // If teapots down to 0, go to next level.
        }
    }


    public void UpdateScore(int scoreToAdd)
    {
        if (bPlayingTag)
        {
            scoreText.text = iTimer.ToString();
        }
        else
        {
            iScore += scoreToAdd;
            scoreText.text = iScore.ToString();
        }
    }


    // Update hi score only happens at end of game, so just display score.
    public void UpdateHiScore(int scoreToAdd)
    {
        hiScoreText.text = scoreToAdd.ToString();
    }


    // Update lo score only happens at end of game, so just display score.
    public void UpdateLoScore(int scoreToAdd)
    {
        loScoreText.text = scoreToAdd.ToString();
    }


    public void UpdateLives(int livesToAdd)
    {
        iLives += livesToAdd;
        livesText.text = iLives.ToString();
        // If we added a life, keep track of that for future life additions
        if (livesToAdd > 0)
            iTotalLives += livesToAdd;
        // If lives reaches 0, game is over.
        if (iLives <= 0)
        {
            GameOver();
        }
    }


    public void UpdateTeapotsDisplay()
    {
        // Number of teapots calculated each frame; just display count.
        teapotsLeftText.text = iTeapots.ToString();
        // When teapots gets to 0, go to next level.
    }


    Vector3[] startLocations = {
        new Vector3(10f, 10f, 10f),
        new Vector3(10f, 10f, -10f),
        new Vector3(10f, -10f, 10f),
        new Vector3(10f, -10f, -10f),
        new Vector3(-10f, 10f, 10f),
        new Vector3(-10f, 10f, -10f),
        new Vector3(-10f, -10f, 10f),
        new Vector3(-10f, -10f, -10f),
        new Vector3(-5f, 0f, 13f),
        new Vector3(0f, 14.1f, 0f),
        new Vector3(14.1f, 0f, 0f),
        new Vector3(-5f, 0f, -13f),
        new Vector3(0f, -14.1f, 0f),
        new Vector3(-14.1f, 0f, 0f),
        new Vector3(5f, 0f, 13f),
        new Vector3(5f, 0f, -13f)
    };
}
