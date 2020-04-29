using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeScript : MonoBehaviour
{
    public const float maxRadius = 25f;
    public const float maxRadiusSq = maxRadius * maxRadius;
    public float shotRadiusSq;
    public float shotSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Use Quadratic Equation to calculate radius of shot in globe.
        // radius = square root(x^2 + y^2 + z^2).
        // (Don't have to take square root if we compare to redius squared
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        if (x * x + y * y + z * z > maxRadiusSq)
        {
            // We've gone to far, so destroy charge.
            Destroy(gameObject);
        }
        else
        {
            // Normal location, so keep it moving until it hits a trigger.
            transform.Translate(Vector3.up * Time.deltaTime * shotSpeed);
        }
    }
}
