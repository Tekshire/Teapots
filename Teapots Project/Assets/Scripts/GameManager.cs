#define TEST_LOSE_LIFE
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
    public int iHiScore;    // High score for Teapot Blaster
    public int iLoScore;    // Low score for Teapot Tag
    public int iTimer;      // score for Teapot Tag
    public int iTeapots;    // zero based 16 total / level
    public int iLevel;      // tube number % 16 (0-5 regular; 6 -> tubenum > 96)
    public int scorePerTeapot;
    public TextMeshProUGUI teapotsGameText; // Tag or Blaster?
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiScoreText;
    public TextMeshProUGUI loScoreText;
    public TextMeshProUGUI teapotsLeftText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winnerText;
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

        UpdateLives(0);
        UpdateScore(0);
        UpdateTeapotsDisplay();

        // Stuff we should update from app memory
        //iHiScore
        //iLoScore
        //UpdateHiScore(iHiScore);
        //UpdateLoScore(iLoScore);

        iLevel = 0;
        StartNewLevel(iLevel);
    }


    public void StartNewLevel(int level)
    {
        Color levelColor;
        switch (level)
        {
            case 0:
                levelColor = Color.blue;
                break;
            case 1:
                levelColor = Color.red;
                break;
            case 2:
                levelColor = Color.yellow;
                break;
            case 3:
                levelColor = Color.cyan;
                break;
            case 4:
                levelColor = Color.black;
                break;
            default:        // Default color for tubnum > 96
                // Want darker green than Color.green
                levelColor = new Color(0f, 100f / 255f, 0f, 1.0f);
                break;
        }

        // Not all GameObjects from previous levels have been destroyed. (Think rammed
        // teapots that haven't yet left the playing field.) Free them up.
        foreach (GameObject tp in teapots)
        {
            if (tp != null) Destroy(tp); ;
        }

        // Create new teapots for this level
        for (int i = 0; i < teapots.Length; i++)
        {
            Vector3 startVector = startLocations[i] * (1.0f + (level / 5.0f));
            GameObject teapot = Instantiate(teapotPrefab, startVector, Quaternion.identity);
            teapots[i] = teapot;
            TeapotScript script = teapot.GetComponent<TeapotScript>();
            Renderer render = teapot.GetComponent<Renderer>();
            script.m_Renderer = render;
            render.material.color = levelColor;
        }
    }


    public void StartTagGame()
    {
        iTotalLives = iLives = 1;
        scorePerTeapot = 0; // We don't destroy any teapots in tag
        bPlayingTag = true;
        teapotsGameText.SetText("Teapots Tag");
        loScoreElement.SetActive(true);
        hiScoreElement.SetActive(false);
        scorePerTeapot = 0; // We don't destroy any teapots in tag

        StartPlay();
    }


    public void StartBlasterGame()
    {
        iTotalLives = iLives = 3;
        //bPlayingTag = false;                          // Already set by LoadScene
        //teapotsGameText.SetText("Teapots Blaster");   // Already set by LoadScene
        loScoreElement.SetActive(false);
        hiScoreElement.SetActive(true);
        scorePerTeapot = 1000 + (500 * iLevel);

        StartPlay();
    }


    // Teapot Tag and Teapot Blaster use the same game logic with just a few
    // parameters different between the two games. StartTagGame() and StartBlasterGame()
    // will set up the parameters that are different beteween the game versions.
    // StartGame() then do all the work to get the game playing.
    public void StartPlay()
    {
        // ToDo: Some of these have already been done in Start() etc.
        UpdateLives(iLives);
        UpdateScore(iScore);
        UpdateHiScore(iHiScore);
        UpdateLoScore(iLoScore);
        UpdateTeapotsDisplay();

        teapotsTitleText.gameObject.SetActive(false);
        tagStartButton.gameObject.SetActive(false);
        blasterStartButton.gameObject.SetActive(false);

        bGameOver = false;
        isGameActive = true;
    }


    public void GameOver()
    {
        bGameOver = true;
        isGameActive = false;
        if (iLives > 0)
            winnerText.gameObject.SetActive(true);
        else
            gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        // Check to see if this is a new high score,
        if (bPlayingTag)
        {
            if (iTimer < iLoScore)
            {
                iLoScore = iTimer;
                UpdateLoScore(iLoScore);
                // ToDo: Save in app memory so Start() can retrieve it.
            }

        }
        else    // Playing blasster
        {
            if (iScore > iHiScore)
            {
                iHiScore = iScore;
                UpdateHiScore(iHiScore);
                // ToDo: Save in app memory so Start() can retrieve it.
            }
        }
        // Note, Start overwrites the hi and lo scores.
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
        }
        // If teapots down to 0, go to next level.
        if (iTeapots == 0)
        {
            if (iLevel < 5)
            {
                iLevel++;       // Will do up to level 5.
                StartNewLevel(iLevel);
            }
            else
            {
                // Really should say Game Winner!
                GameOver();
            }
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
    public void UpdateHiScore(int newHiScore)
    {
        hiScoreText.text = newHiScore.ToString();
    }


    // Update lo score only happens at end of game, so just display score.
    public void UpdateLoScore(int newLoScore)
    {
        loScoreText.text = newLoScore.ToString();
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
