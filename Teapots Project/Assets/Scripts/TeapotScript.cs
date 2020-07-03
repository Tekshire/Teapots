#define TRACE_COLLISIONS

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

    float timer = 0;

    Renderer m_Renderer;


    // Start is called before the first frame update
    void Start()
    {
        //Fetch the Renderer component of the GameObject
        m_Renderer = GetComponent<Renderer>();

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



