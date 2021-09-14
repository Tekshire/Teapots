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
#if (TRACE_COLLISIONS)
    public int shotNum; // Only care which shot hits player when debugging.
#endif


    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
#if (TRACE_COLLISIONS)
    shotNum = gameManager.lastShotNum++; // Only care which shot hits player when debugging.
#endif

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
    //Note: Both GameObjects must contain a Collider component.One must have Collider.isTrigger enabled,
    // and contain a Rigidbody. If both GameObjects have Collider.isTrigger enabled, no collision happens.
    // The same applies when both GameObjects do not have a Rigidbody component.
    // (So can't have shot hit charge abecause both have triggers?)

    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Shot.OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        // Enemy shot only wants to blow up player.
        if (other.gameObject.CompareTag("Player"))
        {
            // TEST
            gameManager.hitNum++;

            // Player destruction now happens in player script.
            //Destroy(other.gameObject);
            //Destroy(gameObject);
            // At this point we are only collecting collision info:

#if (TRACE_COLLISIONS)
            Debug.Log("Shot OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
            Debug.Log(" HitNum: " + gameManager.hitNum);
            Debug.Log(" Can we tell what part of player we hit? " + other.gameObject);
#endif

        }
        if (other.gameObject.CompareTag("Teapot"))
        {
        }
        if (other.gameObject.CompareTag("Charge"))
        {
        }
        if (other.gameObject.CompareTag("EnemyShot"))
        {
        }
        else
            Debug.Log("Shot OnTriggerEnter with no match: " + other.gameObject);


    }


    private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Shot OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Just seeing if we come here at all.
    }
}
