using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeScript : MonoBehaviour
{
    private const float maxShotDist = 40f;
    // Shot speed set as part of velocity at Charge instantiation in PlayerControl.
    public float shotSpeed;

    void Start()
    {
        
    }

    void FixedUpdate()
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
            // Regular movement is just continuing velocity vector set up in PlayerController.
            // (Make sure i don't add drag to charge.)
            //transform.Translate(-Vector3.forward * Time.deltaTime * shotSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // I only want to blow up teapots, so check for that.
        if (other.gameObject.tag == "Teapot")
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }

}
