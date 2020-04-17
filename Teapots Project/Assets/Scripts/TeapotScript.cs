using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Second pass:
 * Just using square bounderies for ease of calculation.
 * Would prefer to have globular field to contain teapots, but need to calculate
 * normal for where teapot hits boundry to calculate reflected movemet vector.
 *
 * To Do? Bob's idea is to have boundy "shimmer to indicate edge of motion. Or could
 * add spark effect to show when we hit the boundry.
 */

public class TeapotScript : MonoBehaviour
{
    public float teapotBoundryRadius;

    public Vector3 startVector; // Do we need this?
    public Vector3 teapotVelocity;
    private Rigidbody rb;


    // Start is called before the first frame update
    void Start()
    {
        // To Do?: Realign teapots so they all have different forward vectors.
        rb = GetComponent<Rigidbody>();
        // Use initial location to create movement out vector for now
        //teapotVelocity.x = transform.position.x / 10.0f;
        //teapotVelocity.y = transform.position.y / 10.0f;
        //teapotVelocity.z = transform.position.z / 10.0f;
        teapotVelocity.x = -transform.position.x;
        teapotVelocity.y = -transform.position.y;
        teapotVelocity.z = -transform.position.z;
        // Actual move of teapot
        rb.AddForce(teapotVelocity * 1.0f, ForceMode.VelocityChange);
    }

    // Update is called once per frame,
    // but want FixedUpdate to fit to physics regardless of game system this is being played on.
    void FixedUpdate()
    {
        // If we're going too far out, change velocity to bring them back.
        // Using square boundry for now.
        //if (Mathf.Abs(transform.position.x) > teapotBoundryRadius)
        //    teapotVelocity.x = -teapotVelocity.x;
        //if (Mathf.Abs(transform.position.y) > teapotBoundryRadius)
        //    teapotVelocity.y = -teapotVelocity.y;
        //if (Mathf.Abs(transform.position.z) > teapotBoundryRadius)
        //    teapotVelocity.z = -teapotVelocity.z;
        if (teapotVelocity.x < 0)
        {
            if (transform.position.x < -teapotBoundryRadius)
                teapotVelocity.x = -teapotVelocity.x;
        }
        else
        {
            if (transform.position.x > teapotBoundryRadius)
                teapotVelocity.x = -teapotVelocity.x;
        }

        if (teapotVelocity.y < 0)
        {
            if (transform.position.y < -teapotBoundryRadius)
                teapotVelocity.y = -teapotVelocity.y;
        }
        else
        {
            if (transform.position.y > teapotBoundryRadius)
                teapotVelocity.y = -teapotVelocity.y;
        }

        if (teapotVelocity.z < 0)
        {
            if (transform.position.z < -teapotBoundryRadius)
                teapotVelocity.z = -teapotVelocity.z;
        }
        else
        {
            if (transform.position.z > teapotBoundryRadius)
                teapotVelocity.z = -teapotVelocity.z;
        }

        //if (Mathf.Abs(transform.position.x) > teapotBoundryRadius)
        //    teapotVelocity.x = -teapotVelocity.x;
        //if (Mathf.Abs(transform.position.y) > teapotBoundryRadius)
        //    teapotVelocity.y = -teapotVelocity.y;
        //if (Mathf.Abs(transform.position.z) > teapotBoundryRadius)
        //    teapotVelocity.z = -teapotVelocity.z;

        // Actual move of teapot
        //rb.AddForce(teapotVelocity * 3, ForceMode.Force);

    }

    private void OnTriggerEnter(Collider other)
    {
        // doRotate = false;
    }
}


/* First Pass:
 * Tried circular path, but forcing position placement allowed objects to be
 * placed within each other and not use ridgidbody to bounce off each other
 * Still liked circle calculation, which should be stored in some snippets file,
 * but is just commented out here for now.
 
public class TeapotScript : MonoBehaviour
{

public float xSpread = 10;
public float zSpread = 10;
public float yOffset;
//public Transform centerPoint;

public float rotSpeed = 2;
public bool rotateClockwise;
public bool doRotate = false;

float timer = 0;

// Start is called before the first frame update
void Start()
{
    yOffset = transform.position.y;
}

// Update is called once per frame,
void Update()
{
    // Realign teapots so they all have different forward vectors.

    // Test calculated rotation to see what happens when we have colllisions
    if (doRotate)
    {
        timer += Time.deltaTime * rotSpeed;
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

private void OnTriggerEnter(Collider other)
{
    doRotate = false;
}
}
*/