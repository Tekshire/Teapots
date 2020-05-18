using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PlayerController is Unity’s name for main character.
// In Tempest, player controls a ship!
public class PlayerController : MonoBehaviour
{
    public float horizontalRawInput, verticalRawInput;
    public float turnSpeed;
    public float shipSpeed;
    public float shotSpeed;
    public float startMouseX;
    public float deltaMouseX;
    public GameObject chargePrefab;
    public Rigidbody rb;

    private float xRange = 17f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        turnSpeed = 2.0f;
        shotSpeed = 20.0f;
        startMouseX = Input.mousePosition.x;
    }

//    void Update()
     void FixedUpdate()     // Don't need Time.deltaTime when using FixedUpdate.
    {
        // Player Move:
        // Old Note: When the claw was laid out on graph paper, X and Y were flat dimensions.
        // However, when put into Unity, the Y vector was heading up, Vector3.up. Since then,
        // the Player prefab is based on an empty game object. For that, Vector3.forward is
        // motion in the forward direction and the claw must be placed inside the empty parent
        // rotated so it appears to be moving forward.
        // New Note: Trying to do flying with ship model as game object.

        // If we change rotation on both horizontal and vertical axis, the result can be a 45 degree angle bank.
        // If we want the ship to stay level in scene, Try the rotations separately.

        Vector3 torque = Vector3.zero;

        horizontalRawInput = Input.GetAxisRaw("Horizontal");
        verticalRawInput = Input.GetAxisRaw("Vertical");

        // yaw:
        if (Input.GetKey(KeyCode.D) || horizontalRawInput == 1)
            torque.z -= turnSpeed;
        if (Input.GetKey(KeyCode.A) || horizontalRawInput == -1)
            torque.z += turnSpeed;
        // pitch:
        if (Input.GetKey(KeyCode.W) || verticalRawInput == 1)
            torque.x -= turnSpeed;
        if (Input.GetKey(KeyCode.S) || verticalRawInput == -1)
            torque.x += turnSpeed;

        rb.AddRelativeTorque(torque);


        // Test output
       // Vector3 currentEulerAngles = rb.transform.rotation;
      //  Quaternion currentRotation;
      //  float x;
      //  float y;
      //  float z;
      //  Debug.Log(myRotation.eulerAngles);



        // Using AddForce to allow for collisions between objects. However, AddTorque gets vectors wrong when
        // ship turns upside down. So use AddRelativeTorque to keep right arrow always turning right.

        //horizontalRawInput = Input.GetAxisRaw("Horizontal");
        // Using the RAW axis returns hard coded +1 or -1. (Doesn't seem so RAW to me.)
        // verticalRawInput = Input.GetAxisRaw("Vertical");
        //Debug.Log("HIn = " + horizontalInput + " Vin = " + verticalInput + " HRaw = " + horizontalRawInput + " VRaw = " + verticalRawInput);



        // transform.rotation.y is not a euler angle representation. That is the 'y' component of a quaternion.
        //
        // Example of calcilating angles separately then converting to quaternion before assigning to transform.
        //rotationX += -Input.GetAxis("Vertical") * rotateSpeed * Time.deltaTime;
        //rotationX = Mathf.Clamp(rotationX, minRotationX, maxRotationX);
        //rotationY += Input.GetAxis("Horizontal") * Time.deltaTime * rotateSpeed; // added speed
        //transform.rotation = Quaternion.Euler(rotationX, rotationY, 0);


        // Use clamp to keep from going too far up
        //     transform.rotation.x = Mathf.Clamp(transform.rotation.x, -85, 85);
        //transform.rotation = new Vector3 (Mathf.Clamp(transform.rotation.x, -85, 85), transform.rotation.y, transform.rotation.z);
        //        transform.rotation.eulerAngles.y = Mathf.Clamp(transform.eulerAngles.y, -90, 90);


        // torque = Vector3.zero;
        // horizontalRawInput = Input.GetAxisRaw("Horizontal");

        // yaw:
        //        if (Input.GetKey(KeyCode.D) || horizontalRawInput == 1)
        //            torque.z -= turnSpeed;
        //        if (Input.GetKey(KeyCode.A) || horizontalRawInput == -1)
        //            torque.z += turnSpeed;
        //
        //        rb.AddRelativeTorque(torque);

        // Forward speed
        //        float inX = Input.mousePosition.x;
        //        deltaMouseX = inX - startMouseX;
        //        if (deltaMouseX < 0.0f)
        //        {
        //            // Make this our new start point so we don't have to roll a long ways up to get delta positive again.
        //            deltaMouseX = 0;
        //            startMouseX = inX;
        //        }
        //        shipSpeed = deltaMouseX / 20f;    // Do we need to scale this for better control?
        //        shipSpeed = 1f;    // Hack just to see if constant speed works better
        //
        //        rb.AddRelativeForce(0f, shipSpeed, 0f);


        /*
//        horizontalInput = Input.GetAxis("Horizontal");
//	    transform.Rotate(Vector3.up, horizontalInput * turnSpeed * Time.deltaTime, 0);

        //transform.Rotate(Vector3.left, verticalInput * turnSpeed * Time.deltaTime, 0);
        rb.AddRelativeTorque(Vector3.left * turnSpeed * verticalInput);

        //     shipSpeed += Input.GetAxis("Mouse Y");
        //      transform.Translate(Vector3.forward * Time.deltaTime * shipSpeed);
        //        rb.AddRelativeForce(Vector3.forward * shipSpeed);

        // Limit player movement
//
//        if (transform.position.x < -xRange)
//        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }
        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }





        Vector3 torque = Vector3.zero;

        // pitch:
        if (Input.GetKey(KeyCode.W) == true)
            torque.x -= 10.0f;
        if (Input.GetKey(KeyCode.S) == true)
            torque.x += 10.0f;
        // yaw:
        if (Input.GetKey(KeyCode.A) == true)
            torque.y -= 10.0f;
        if (Input.GetKey(KeyCode.D) == true)
            torque.y += 10.0f;
        // roll:
        if (Input.GetKey(KeyCode.Q) == true)
            torque.z -= 10.0f;
        if (Input.GetKey(KeyCode.E) == true)
            torque.z += 10.0f;

        float forward = 0.0f;

        if (Input.GetKey(KeyCode.Space) == true)
            forward = 10.0f;

        // other lateral forces, not going to keep typing heh

        rigidbody.AddRelativeTorque(torque);
        rigidbody.AddRelativeForce(0f, 0f, forward);

        */

        // Check for charge fired after ship move, so charge move will match ship move.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // launch projectile
            Instantiate(chargePrefab, transform.position, chargePrefab.transform.rotation);
        }

    }
}

