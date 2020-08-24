#define TRACE_COLLISIONS
#undef TEST_SOUND

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeapotScript : MonoBehaviour
{
    // Most of this is set up when testing teapots motion when we first started.
    // This has not been maintained!
    public float xSpread = 10;
    public float zSpread = 10;
    public float yOffset;
    //public Transform centerPoint;

    public float rotSpeed = 2;
    public bool rotateClockwise;
    public bool doRotate = false;

    private float timer = 0;
    public GameManager gameManager; // change to private after we let inspector show we get value.
    public Renderer m_Renderer;     // change to private
    public AudioSource teapotAudio;    // Made public so i can play with pitch modifier in inspector
    public GameObject explosionPrefab;


    static float maxVel = 2.0f;
    public AudioClip[] crashSoundArray = new AudioClip[6];
    public AudioClip explosionSound;
    public int pointValue;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Fetch the Renderer component of the GameObject to change color
        m_Renderer = GetComponent<Renderer>();
        teapotAudio = GetComponent<AudioSource>();

        yOffset = transform.position.y;

        pointValue = 1000;  // Depends upon game level
    }


    void FixedUpdate()    // Don't need Time.deltaTime when using FixedUpdate.
    {
        // Realign teapots so they all have different forward vectors.

        // Test calculated rotation to see what happens when we have colllisions
        if (doRotate)
        {
            //timer += Time.deltaTime * rotSpeed;
            timer += rotSpeed;
            Rotate();
        }

#if (TEST_SOUND)
        // Testing sounds by playing them based on key press
        bool bPlaySound = false;
        int crashSoundIndex = 0;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            crashSoundIndex = 0;
            bPlaySound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            crashSoundIndex = 1;
            bPlaySound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            crashSoundIndex = 2;
            bPlaySound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            crashSoundIndex = 3;
            bPlaySound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            crashSoundIndex = 4;
            bPlaySound = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            crashSoundIndex = 5;
            bPlaySound = true;
        }

        if (bPlaySound)
        {
            teapotAudio.PlayOneShot(crashSoundArray[crashSoundIndex], 1.0f);
        }
#endif
    }


    // Really liked this rotate script to drive teapots in circle.
    // Not sure how flexible this would be if collisions force teapots out of circle.
    void Rotate()
    {
        if (rotateClockwise)
        {
            float x = -Mathf.Cos(timer) * xSpread;
            float z = Mathf.Sin(timer) * zSpread;
            Vector3 pos = new Vector3(x, yOffset, z);
            //transform.position = pos + centerPoint.position;
            transform.position = pos;
        }
        else
        {
            float x = Mathf.Cos(timer) * xSpread;
            float z = Mathf.Sin(timer) * zSpread;
            Vector3 pos = new Vector3(x, yOffset, z);
            //transform.position = pos + centerPoint.position;
            transform.position = pos;
        }
    }


    // No longer have trigger on teapot in order to enable bouncing off ship.
    // But still get here when hit by charge. Charge OnTriggerEnter used to
    // destroy both charge and hit teapot. Now moved teapot destruction
    // here to add animation and sound.
    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        // Right now only object with trigger is charge, so no need to check at this time.
        // Only blow up teapot if it hits a charge.
        // if (other.gameObject.tag == "Charge")
        // {
        // First do animation because light moves faster than sound.
        // Call explosion animation.
        Destroy(gameObject);
        GameObject explosionObject =
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Explosion sound
        AudioSource explosionAudio = explosionObject.GetComponent<AudioSource>();
        explosionAudio.PlayOneShot(explosionSound);

        gameManager.UpdateScore(pointValue);

        // Plan to have explosion animation overwhelm teapot, so don't destroy until a bit later.
        // (Use co-routine to do destruction later.)
        //       Destroy(gameObject);
        //   StartCoroutine(DelayDeath());
        // }    // tag == "Charge"
    }



IEnumerator DelayDeath()
{
    yield return new WaitForSeconds(1.0f);
    Destroy(gameObject);
}


private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Generate sound if teapot collides with Player or other teapot
        int crashSoundIndex = Random.Range(0, crashSoundArray.Length);
        Debug.Log("Trying to play crashSoundArray[" + crashSoundIndex + "] from AudioSource: " +
            teapotAudio);
        //        Debug.Log("Audio Clip should be: " + crashSoundArray[crashSoundIndex]);
        // Affect volume by how fast the ship is striking the teapot.
        // Sound only attenuates, so value between 0.0 and 1.0. Use maxVel to make sure
        // fasted collision equals loudest sound.
        float collisionSpeed = collision.relativeVelocity.magnitude;
        if (collisionSpeed > maxVel)
            maxVel = collisionSpeed;
        teapotAudio.PlayOneShot(crashSoundArray[crashSoundIndex], collisionSpeed/maxVel);

        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // Change teapot color only if hit by Player
            if (collision.gameObject.CompareTag("Player"))
            // And only if playing tag.
            {
                m_Renderer.material.color = Color.white;
            }
        }   // isGameActive
    }


    // Called by game manager to count tagged teapots.
    public bool isWhite()
    {
        return m_Renderer.material.color == Color.white;
    }

}

