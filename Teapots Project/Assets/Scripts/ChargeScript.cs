using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeScript : MonoBehaviour
{
    private const float maxRadius = 50f;
    private const float maxRadiusSq = maxRadius * maxRadius;
    private float shotRadiusSq;
    // Shot speed set as part of velocity at Charge instantiation in PlayerControl.
    //public float shotSpeed = 2.0f;

    void Start()
    {
        
    }

    void FixedUpdate()
    {
        // Use Quadratic Equation to calculate radius of shot in globe.
        // radius = square root(x^2 + y^2 + z^2).
        // (Don't have to take square root if we compare to redius squared
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (x * x + y * y + z * z > maxRadiusSq)
        {
            // We've gone too far, so destroy charge.
            Destroy(gameObject);
        }
        else
        {
            // Regular movement is just continuing velocity vector set up in PlayerController.
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
