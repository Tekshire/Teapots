#define TRACE_COLLISIONS
#define TEST_SOUND

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
    private Renderer m_Renderer;
    public AudioSource teapotAudio;    // Made public so i can play with pitch modifier in inspector

    static float maxVel = 2.0f;
     public AudioClip[] crashSoundArray = new AudioClip[6];


    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Renderer component of the GameObject
        m_Renderer = GetComponent<Renderer>();
        teapotAudio = GetComponent<AudioSource>();

        yOffset = transform.position.y;
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
            Debug.Log("Trying to play crashSoundArray.");
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
    // Just enabled to see if we come here even when trigger is on charge.
    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
    }


    private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Generate sound if teapot collides with Player or other teapot

        // Change teapot color only if hit by Player
        if (collision.gameObject.CompareTag("Player"))
        {
            m_Renderer.material.color = Color.white;
        }
    }


}



