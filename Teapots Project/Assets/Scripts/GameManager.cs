using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Since the main goal of Teapots is to become Tempest in a Teapot and Tempest
    // has shooting, make Teapots Blaster the default.
    public bool bPlayingTag = false;
    public int iLives;
    public int iScore;      // score for blaster
    public int iTimer;      // score for tag
    public int iTeapots;    // zero based 16 total
    public int iLevel;      // zero based 96 total
    public int iRange;      // iLevel % 16
    public TextMeshProUGUI scoreText;

    // In Teapots, we know we only have 16 objects per level, so we can use an array.
    // When we begin to add in the Tempest objects, we can use the List recommended by
    // Unity in the Code Live Tutorials, Week 6, session 1.
    void Start()
    {
        // Stuff that is the same for each level
        iScore = 0;
        iTimer = 0;
        iTeapots = 16;
        iLevel = 0;
        iRange = 0;
        UpdateScore(0);

        if (bPlayingTag)
        {
            iLives = 1;
        }
        else
        {
            iLives = 3;
        }

    }


    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdateScore(int scoreToAdd)
    {
        iScore += scoreToAdd;
        scoreText.text = iScore.ToString();
    }

}
