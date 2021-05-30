#define TRACE_COLLISIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is for the enemy shots, not the player 'charges'.
public class ShotScript : MonoBehaviour
{
    private const float maxShotDist = 40f;
    public float shotSpeed;
    public GameManager gameManager;


    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }

    void Update()
    {
        if ((transform.position.x > maxShotDist) || (transform.position.x < -maxShotDist) ||
            (transform.position.z > maxShotDist) || (transform.position.z < -maxShotDist) ||
            (transform.position.y > maxShotDist) || (transform.position.y < -maxShotDist))
        {
            // We've gone too far, so destroy charge.
            Destroy(gameObject);
        }
        else
        {
            // Regular movement is just continuing velocity vector set up in TeapotScript.
            transform.Rotate(0, 5, 0);      // Add spin around axis of motion.
        }

        // After rotate, are we pointing at player? If so, shoot, maybe.
        // (First just wire in a test case. For test, we check keydown, which should be in Update()
        // function, so we don't miss keydown event. but this is only a test so not important if we
        // miss one.)
///        if (Input.GetKeyDown(KeyCode.Period)) UpdateLives(-1);

    }

    // Here the "other" object is entering our shot's trigger area.
    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Shot OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        // Enemy shot only wants to blow up player.
        if (other.gameObject.CompareTag("Player"))
        {
            // Player destruction now happens in player script.
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

}
