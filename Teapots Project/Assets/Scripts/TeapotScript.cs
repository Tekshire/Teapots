using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    void FixedUpdate()
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

   // No longer have trigger on teapot in order to enable bouncing off ship.
   //private void OnTriggerEnter(Collider other)
   // {
   //     doRotate = false;
   // }
}
