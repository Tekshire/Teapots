#define TRACE_COLLISIONS
#define COLLISION_TEST_JIG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is for the enemy shots, not the player 'charges'.
public class ShotScript : MonoBehaviour
{
    public GameManager gameManager;
    private const float maxShotDist = 40f;
    public float shotSpeed;
#if (TRACE_COLLISIONS)
    public int shotNum = 0; // Only care which shot hits player when debugging.
#endif


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


#if false
    // Here the "other" object is entering our shot's trigger area.
    //Note: Both GameObjects must contain a Collider component.One must have Collider.isTrigger enabled,
    // and contain a Rigidbody. If both GameObjects have Collider.isTrigger enabled, no collision happens.
    // The same applies when both GameObjects do not have a Rigidbody component.
    // (So can't have shot hit charge because both have triggers?)

    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Shot.OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // If shot hits player or player hits shot, both cases will call both shot.OnTriggerEnter()
            // and player.OnTriggerEnter(). Only implement player.OnTriggerEnter because Player will
            // have the most logic to implement.
            if (other.gameObject.CompareTag("Player"))
            {
            }
            // Any other GameObjects hit Shot trigger?
        }
    }
#endif  // false

}






