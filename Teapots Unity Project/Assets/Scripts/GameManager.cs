﻿#define TEST_LOSE_LIFE
#define TEST_LOSE_TEAPOT
#define TRACE_COLLISIONS
#define COLLISION_TEST_JIG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ToDo: Add check for new life at UpdateScore().

public class GameManager : MonoBehaviour
{
    public bool bGameOver = false;
    // No input, spawning, or scoring if game no longer active.
    // But movement may still happen to get objects off screen.
    public bool isGameActive;
    public int iLives;      // Current lives; 0 => new game
    public int iTotalLives; // To calculate granting new life. (Includes lives you've lost)
    public int iScore;      // score for Teapot Blaster
    public int iHiScore;    // High score for Teapot Blaster
    public int iTeapots;    // zero based 16 total / level
    public int iLevel;      // tube number % 16 (0-5 regular; 6 -> tubenum > 96)
    public int tubeNum = 0; // In Tempest every time we finish a tube, increase the tubeNum as we go forward.
                            // Since Teapot can destroy a teapot outside of Tempest mode, we increase tubeNum
                            // as if destroyed tube was last tube we sailed through in Tempest.
#if (TRACE_COLLISIONS)
    public int lastShotNum = 0; // Only care which shot hits player when debugging.
#endif
#if (COLLISION_TEST_JIG)
    public GameObject homeTarget;
    public GameObject homeCollider;
    public GameObject testTarget;
    public GameObject testCollider;
    public GameObject playerPrefab;
    public GameObject chargePrefab;
    public GameObject shotPrefab;
#endif
    public float teapotShotTimer;
    public const int iMaxShotTimer = 100;  // Max tube num = 96, * 60 frames / sec (Admitidly optimistic)
    public bool bFireEnabled = false;
//    public int scorePerTeapot;          // How are these 2 different?
    public int teapotPointValue;        // How are these 2 different?
    public int nextTeapotShot;  // Spread chance to shoot around to other teapots.
    public Vector3 teapotRotateVector;
    public float teapotRotateSpeed;     // Will be multiplied by Time.deltaTime
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI hiScoreText;
    public TextMeshProUGUI teapotsLeftText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI teapotsTitleText;

    public Button startButton;
    public Button restartButton;
    public GameObject scoreElement;
    public GameObject hiScoreElement;

    public GameObject player;       // For other objects to calculate distance

    public GameObject teapotPrefab;
    ///    public GameObject radarBlipPrefab;

    // In Teapots, we know we only have 16 objects per level, so we can use an array.
    // When we begin to add in the Tempest objects, we can use the List recommended by
    // Unity in the Code Live Tutorials, Week 6, session 1.
    public GameObject[] teapots;

    // TEST VARIABLES
    public int hitNum;  // How many shots hit player per level


    void Start()
    {
        iScore = 0;
        iTeapots = 16;
        bGameOver = false;
        isGameActive = false;       // Not until Start button pressed
        gameOverText.gameObject.SetActive(false);
        restartButton.gameObject.SetActive(false);
        teapotsTitleText.gameObject.SetActive(true);
        hiScoreElement.SetActive(true);
        startButton.gameObject.SetActive(true);
        player = GameObject.FindWithTag("Player");

#if (COLLISION_TEST_JIG)
        // Believe player initialized by scene before GameManager.Start().
        //// Great plan, but not true. mainCamera == null!
        Debug.Log("GameManager.Start.player = " + player);

        iTotalLives = iLives = 50;  // A lot more lives for debugging shots before death.

        // Hard to just create a transform, so instantiate an entire object (inactive)
        // just to appropriate its transform.
        homeTarget = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //homeTarget.transform.position = new Vector3(-3, 2, 2);
        homeTarget.transform.position = new Vector3(-3, .5f, 2);
        homeTarget.SetActive(false);

        // Just a differant way to create an object
        homeCollider = new GameObject("homeCollider");
        //homeCollider.transform.position = new Vector3(3, 2, 2);
        homeCollider.transform.position = new Vector3(3, .5f, 2);
        homeCollider.SetActive(false);
#else
        iTotalLives = iLives = 3;
#endif

        // Stuff we should update from app memory
        //iHiScore

        // Set the display
        UpdateLives(0);
        UpdateScore(0);
        UpdateTeapotsDisplay();
        //UpdateHiScore(iHiScore);

        nextTeapotShot = 1;
        iLevel = 0;
        StartNewLevel(iLevel);
    }


    public void StartNewLevel(int level)
    {
        teapotShotTimer = 0.0f;
        teapotPointValue = 1000 + (200 * iLevel);
        Color levelColor;   // will be assigned to each teapot because color can change during game play.

        // Originally each teapot had its own axis to spin around, but that allowed
        // teapots to bump in to each other. By having only one axis that all teapots
        // orbit around, we avoid most collisions. First incarnation used Vector3.up
        // as the axis, but that meant the top and bottom teapots lined up directly
        // with the axis and only spun instead of orbitting. So now use an axis that is
        // between existing starting spots, so no teapot is directly on the axis so all
        // now orbit, however small the orbit radius may be.
        // There are 4 points in the positive quadrent. Use the angles that
        // are between each set of point:
        //new Vector3(10f, 10f, 10f), // 0
        //new Vector3(0f, 14.1f, 0f), // 9
        //new Vector3(14.1f, 0f, 0f), // 10
        //new Vector3(5f, 0f, 13f), // 14
        //----------------------------
        // angle = (x0 + x1) / 2
        //(10f, 10f, 10f)[0] + (0f, 14.1f, 0f)[9] = (10f, 24.1f, 10f)
        //(10f, 10f, 10f)[0] + (14.1f, 0f, 0f)[10] = (24.1f, 10f, 10f)
        //(10f, 10f, 10f)[0] + (5f, 0f, 13f)[14] = (15f, 10f, 23f)
        //(0f, 14.1f, 0f)[9] + (14.1f, 0f, 0f)[10] = (14.1f, 14.1f, 0f)
        //(0f, 14.1f, 0f)[9] + (5f, 0f, 13f)[14] = (5f, 14.1f, 13f)
        //(14.1f, 0f, 0f)[10] + (5f, 0f, 13f)[14] = (19.1f, 0f, 13f)

        switch (level)
        {
            case 0:
                levelColor = Color.blue;
                //(10f, 10f, 10f)[0] + (0f, 14.1f, 0f)[9] = (10f, 24.1f, 10f)
                teapotRotateVector = new Vector3(5.0f, 12.05f, 5.0f);
                teapotRotateSpeed = .05f;
                break;
            case 1:
                levelColor = Color.red;
                //(10f, 10f, 10f)[0] + (14.1f, 0f, 0f)[10] = (24.1f, 10f, 10f)
                teapotRotateVector = new Vector3(12.05f, 5.0f, 5.0f);
                teapotRotateSpeed = -.07f;
                break;
            case 2:
                levelColor = Color.yellow;
                //(10f, 10f, 10f)[0] + (5f, 0f, 13f)[14] = (15f, 10f, 23f)
                teapotRotateVector = new Vector3(7.5f, 5.0f, 11.5f);
                teapotRotateSpeed = .09f;
                break;
            case 3:
                levelColor = Color.cyan;
                //(0f, 14.1f, 0f)[9] + (14.1f, 0f, 0f)[10] = (14.1f, 14.1f, 0f)
                teapotRotateVector = new Vector3(7.05f, 7.05f, 0.0f);
                teapotRotateSpeed = -.11f;
                break;
            case 4:
                levelColor = Color.black;
                //(0f, 14.1f, 0f)[9] + (5f, 0f, 13f)[14] = (5f, 14.1f, 13f)
                teapotRotateVector = new Vector3(2.5f, 7.05f, 6.5f);
                teapotRotateSpeed = .13f;
                break;
            default:        // Default color for tubnum > 96
                // Want darker green than Color.green
                levelColor = new Color(0f, 100f / 255f, 0f, 1.0f);
                //(14.1f, 0f, 0f)[10] + (5f, 0f, 13f)[14] = (19.1f, 0f, 13f)
                teapotRotateVector = new Vector3(9.55f, 0.0f, 6.5f);
                teapotRotateSpeed = -.15f;
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
            script.m_Renderer = render;     // Need to store render for dynamic color change.
            render.material.color = levelColor;
            // No longer use individual teapot rotate axis, because by assigning only 1 rotate
            // axis per level, all teapots will rotate around same one and there will be no collisions.
            //Vector3 axisVector = new Vector3(startVector.x, startVector.y, startVector.z);
            //script.axis = axisVector;
            script.index = i;
        }

        // TEST:
        hitNum = -1;     // For this level no enemy shots have hit player.
    }


    public void StartPlay()
    {
        // ToDo: Some of these have already been done in Start() etc.
        SetLives(iLives);
        UpdateScore(iScore);
        SetHiScore(iHiScore);
        UpdateTeapotsDisplay();

        teapotsTitleText.gameObject.SetActive(false);
        startButton.gameObject.SetActive(false);

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
        if (iScore > iHiScore)
        {
            iHiScore = iScore;
            SetHiScore(iHiScore);
            // ToDo: Save in app memory so Start() can retrieve it.
        }
        // Note, Start overwrites the hi score.
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

#if (COLLISION_TEST_JIG)
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            // Press 0 -> Delete target
            if (testTarget != null)
            {
                Destroy(testTarget);
                testTarget = null;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            // Press 1 -> Delete collider
            if (testCollider != null)
            {
                Destroy(testCollider);
                testCollider = null;
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            // Press 2 -> Delete target; Instantiate teapot
            if (testTarget != null)
            {
                Destroy(testTarget);
            }
            testTarget = Instantiate(teapotPrefab, homeTarget.transform.position, Quaternion.identity);
            TeapotScript tpScript = testTarget.GetComponent<TeapotScript>();
            tpScript.bTestTarget = true;
            Rigidbody teapotRB = testTarget.GetComponent<Rigidbody>();  // Not Needed
            teapotRB.velocity = new Vector3(0, 0, 0);  // Not Needed
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            // Press 3 -> Delete collider; Instantiate teapot; move left
            if (testCollider != null)
            {
                Destroy(testCollider);
            }
            testCollider = Instantiate(teapotPrefab, homeCollider.transform.position, Quaternion.identity);
            TeapotScript tpScript = testCollider.GetComponent<TeapotScript>();
            tpScript.bTestCollider = true;
            Rigidbody teapotRB = testCollider.GetComponent<Rigidbody>();
            teapotRB.velocity = new Vector3(-10, 0, 0);   // (-200, 0, 0)
            //transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
            //teapotRB.AddForce(transform.forward, ForceMode.Acceleration);
            //child.position += Vector3.up * 10.0f;
        }


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            // Press 4 -> Delete target; Instantiate charge
            if (testTarget != null)
            {
                Destroy(testTarget);
            }
            testTarget = Instantiate(chargePrefab, homeTarget.transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            // Press 5 -> Delete collider; Instantiate charge
            if (testCollider != null)
            {
                Destroy(testCollider);
            }
            testCollider = Instantiate(chargePrefab, homeCollider.transform.position, Quaternion.identity);

            //ChargeScript cScript = testCollider.GetComponent<ChargeScript>();
            //cScript.bTestVelocity = true;
            Rigidbody chargeRB = testCollider.GetComponent<Rigidbody>();
            chargeRB.velocity = new Vector3(-10, 0, 0);
            //transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
            //teapotRB.AddForce(transform.forward, ForceMode.Acceleration);
            //child.position += Vector3.up * 10.0f;
        }


        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            // Press 6 -> Delete target; Instantiate enemy shot
            if (testTarget != null)
            {
                Destroy(testTarget);
            }
            testTarget = Instantiate(shotPrefab, homeTarget.transform.position, Quaternion.Euler(90,90,90));
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            // Press 7 -> Delete collider; Instantiate enemy shot
            if (testCollider != null)
            {
                Destroy(testCollider);
            }
            testCollider = Instantiate(shotPrefab, homeCollider.transform.position, Quaternion.Euler(90, 90, 90));

            //ShotScript shotScript = testCollider.GetComponent<ShotScript>();
            //shotScript.bTestVelocity = true;
            Rigidbody shotRB = testCollider.GetComponent<Rigidbody>();
            shotRB.velocity = new Vector3(-10, 0, 0);
            //transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
            //teapotRB.AddForce(transform.forward, ForceMode.Acceleration);
            //child.position += Vector3.up * 10.0f;
        }


        // Don't instantiate new player ship because we then switch view based on NEW camera.
        //if (Input.GetKeyDown(KeyCode.Alpha8)) {}
        //if (Input.GetKeyDown(KeyCode.Alpha9)) { }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            // Press 8 -> Delete target; Instantiate player (NO DUE TO EXTRA CAMERA)
            if (testTarget != null)
            {
                Destroy(testTarget);
            }
            testTarget = Instantiate(playerPrefab, homeTarget.transform.position, Quaternion.identity);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            // Press 9 -> Delete collider; Instantiate player (NO DUE TO EXTRA CAMERA)
            if (testCollider != null)
            {
                Destroy(testCollider);
            }
            testCollider = Instantiate(playerPrefab, homeCollider.transform.position, Quaternion.identity);
        }

#endif  // COLLISION_TEST_JIG


        // Normal game play

        bFireEnabled = false;
        // In Tempest every time we finish a tube, increase the tubeNum as we go forward.
        // Since Teapot can destroy a teapot outside of Tempest mode, we increase tubeNum
        // as if destroyed tube was last tube we sailed through in Tempest.
        tubeNum = iLevel * 16 + 16 - iTeapots;

        if (isGameActive)
        {
            // If bFireEnabled we will have teapots[nextTeapotShot] do the firing.
            // But what if that entry in the teapots array is null? We will never fire
            // and, even worse, never look for a valid teapot that could shoot. Fix that here.
            if (teapots[nextTeapotShot] == null)
            {
                // Look at next possible valid teapot
                int nextPossibleTeapot = nextTeapotShot + 1;
                if (nextPossibleTeapot >= 16) nextPossibleTeapot = 0;
                nextTeapotShot = nextPossibleTeapot;
            }
            if (teapotShotTimer < iMaxShotTimer)
            {
                // The higher we go in the game, the faster the shots will come.
                teapotShotTimer += (tubeNum + 1) * Time.deltaTime; // tubeNum can be 0.
            }
            else
            {
                bFireEnabled = true;
            }
        }   // isGameActive
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
                // ToDo: Really should say Game Winner!
                GameOver();
            }
        }
    }


    public void SetScore(int newScore)
    {
        iScore += newScore;
        scoreText.text = iScore.ToString();
    }


    public void UpdateScore(int scoreToAdd)
    {
        iScore += scoreToAdd;
        scoreText.text = iScore.ToString();
    }


    // Update hi score only happens at end of game, so just display score.
    public void SetHiScore(int newHiScore)
    {
        hiScoreText.text = newHiScore.ToString();
    }


    public void SetLives(int newLives)
    {
        iTotalLives = iLives = newLives;
        livesText.text = iLives.ToString();
        // Really don't expect to set lives to 0, but maybe as test?
        if (iLives <= 0)
        {
            GameOver();
        }
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


    // Rearrange vector table to randomize where shots come from.
    Vector3[] startLocations = {
        new Vector3(10f, 10f, 10f),       // original index = 0
        new Vector3(5f, 0f, 13f),         // original index = 14
        new Vector3(-10f, -10f, 10f),     // original index = 6
        new Vector3(-5f, 0f, -13f),       // original index = 11
        new Vector3(-10f, -10f, -10f),    // original index = 7
        new Vector3(0f, 14.1f, 0f),       // original index = 9
        new Vector3(-10f, 10f, 10f),      // original index = 4
        new Vector3(14.1f, 0f, 0f),       // original index = 10
        new Vector3(10f, -10f, -10f),     // original index = 3
        new Vector3(-5f, 0f, 13f),        // original index = 8
        new Vector3(10f, -10f, 10f),      // original index = 2
        new Vector3(0f, -14.1f, 0f),      // original index = 12
        new Vector3(-10f, 10f, -10f),     // original index = 5
        new Vector3(-14.1f, 0f, 0f),      // original index = 13
        new Vector3(10f, 10f, -10f),      // original index = 1
        new Vector3(5f, 0f, -13f)         // original index = 15
    };
}
