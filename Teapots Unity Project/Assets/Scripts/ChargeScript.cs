#define TRACE_COLLISIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ChargeScript : MonoBehaviour
{
    private const float maxChargeDist = 40f;
    // Shot speed set as part of velocity at Charge instantiation in PlayerControl.
    public float chargeSpeed;
    public GameManager gameManager;


    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

    }

    void Update()
    {
        if ((transform.position.x > maxChargeDist) || (transform.position.x < -maxChargeDist) ||
            (transform.position.z > maxChargeDist) || (transform.position.z < -maxChargeDist) ||
            (transform.position.y > maxChargeDist) || (transform.position.y < -maxChargeDist))
        {
            // We've gone too far, so destroy charge.
            Destroy(gameObject);
        }
        else
        {
            // Regular movement is just continuing velocity vector set up in PlayerController.
            // (Make sure i don't add drag to charge.)
            //transform.Translate(-Vector3.forward * Time.deltaTime * shotSpeed);
        }
    }

    // Here the "other" object is entering our charge's trigger area.
    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Charge OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        // I only want to blow up teapots, so check for that.
        if (other.gameObject.CompareTag("Teapot"))
        {
            // Teapot destruction now happens in teapot script.
            //Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Charge OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Just seeing if we come here at all.
    }
}
