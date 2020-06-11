#undef DEBUG_ANGLE_Y
#define DEBUG_ANGLE_X

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerController is Unity’s name for main character.
// In Tempest, player controls a ship!
public class PlayerController : MonoBehaviour
{
    private float horizontalRawInput, verticalRawInput;
    public float turnSpeed;     // Radians / sec
    public float shipSpeed;
    public float shotSpeed;
///    public float startMouseX;
///    public float deltaMouseX;
    public GameObject chargePrefab;
    public Rigidbody rb;

    public float shipRange = 25f;

#if (DEBUG_ANGLE_Y)
    float debugAngleY;       // DEBUG
#endif
#if (DEBUG_ANGLE_X)
    float debugAngleX;
#endif


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Let speeds be set in Inspector while developing.
        // May want to programatically init to allow for increased speed as we go up levels.
        turnSpeed = 15.0f;      // Good speed to get ship turning from dead stop.
        shotSpeed = 10.0f;
        ///    startMouseX = Input.mousePosition.x;
#if (DEBUG_ANGLE_Y)
        Debug.Log("Max Angular Velocity: " + rb.maxAngularVelocity);
        debugAngleY = transform.eulerAngles.y;
        Debug.Log(" Starting rotate y: " + debugAngleY);
#endif
#if (DEBUG_ANGLE_X)
        debugAngleX = transform.eulerAngles.x;
        Debug.Log(" Starting pitch X: " + debugAngleX);
#endif
    }


    void FixedUpdate()     // Don't need Time.deltaTime when using FixedUpdate.
    {
        // If we change rotation on both horizontal and vertical axis, the result can be a 45 degree angle bank.
        // If we want the ship to stay level in scene, Try the rotations separately.

        /* 1. Change horizontal rotation Y; Allow 360 degree */
        // Allow 360 degree settings
        bool bChangeTorqueY = false;
        Vector3 torque = Vector3.zero;      // Torque to be added.

        // yaw:
        horizontalRawInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.D) || horizontalRawInput == 1)
        {
            torque.y += turnSpeed;
            bChangeTorqueY = true;
        }
        if (Input.GetKey(KeyCode.A) || horizontalRawInput == -1)
        {
            torque.y -= turnSpeed;
            bChangeTorqueY = true;
        }
        if (bChangeTorqueY)
        {
#if (DEBUG_ANGLE_Y)
            Debug.Log("Change rotate y by torque: " + torque);
#endif
            // For rotation, we could just use transform change rotation, but want
            // to use AddTorque so that angular drag can have an effect.
            rb.AddRelativeTorque(torque, ForceMode.Force);
        }
#if (DEBUG_ANGLE_Y)
        float newDebugAngleY = transform.eulerAngles.y;
        if (newDebugAngleY != debugAngleY)
        {
            Debug.Log(" new rotate y: " + newDebugAngleY);
            debugAngleY = newDebugAngleY;
        }
        // Also just use this as a convenient time to see what angular velocity is.
        Debug.Log(" Angular Velocity: " + rb.angularVelocity + "; Angular Speed: " + rb.angularVelocity.magnitude +
            "; Vector: " + rb.angularVelocity.normalized);
#endif
        /* Close 1. Change horizontal rotation Y */


        // Use NORMALIZED to keep claw from accelerating too fast
        //// create a normalized vector in the rotation direction
        //var moveDir = Vector3.Cross(rigidbody.angular.velocity, Vector3.up).normalized;
        //rigidbody.AddForce(moveDir * force);


        /* 2. Forward speed
        // Forward speed
            //   rb.AddForce((Vector3.forward * deltaMouseX).normalized, ForceMode.VelocityChange);
        rb.AddForce(Vector3.up * shipSpeed, ForceMode.Force);
        Debug.Log("Forward vector: " + Vector3.up + "; speed: " + shipSpeed + "; velocity: " + rb.velocity);

        torque = Vector3.zero;
        verticalRawInput = Input.GetAxisRaw("Vertical");
        // pitch:
       if (Input.GetKey(KeyCode.W) || verticalRawInput == 1)
           torque.x -= turnSpeed;
       if (Input.GetKey(KeyCode.S) || verticalRawInput == -1)
           torque.x += turnSpeed;

        rb.AddRelativeTorque(torque);


        // We rotate horizontally and vertical. If we do both at the same time,
        // the added vectors can wind up banking our ship, which i find confusling.
        // This is true even if we do 2 separate calls to AddRelativeTorgue each
        // with only the horizontal torque or the vertical torgue. See if it makes
        // any difference if we put the AddRelativeForce call in between.

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
        
        rb.AddRelativeForce(0f, 0f, shipSpeed);

        Close 2. Forword speed */





        /* 3. Change Pitch */
        // Limit pitch change to -90 through 90 degrees, so we don't flip over upside down.

        bool bChangeTorqueX = false;
        torque = Vector3.zero;

        // pitch:
        verticalRawInput = Input.GetAxisRaw("Vertical");
        if (Input.GetKey(KeyCode.W) || verticalRawInput == 1)
        {
            torque.x -= turnSpeed;
            bChangeTorqueX = true;
        }
        if (Input.GetKey(KeyCode.S) || verticalRawInput == -1)
        {
            torque.x += turnSpeed;
            bChangeTorqueX = true;
        }
        if (bChangeTorqueX)
        {
#if (DEBUG_ANGLE_X)
            Debug.Log("Change pitch x by torque: " + torque);
#endif
            // For rotation, we could just use transform change rotation, but want
            // to use AddTorque so that angular drag can have an effect.
            rb.AddRelativeTorque(torque, ForceMode.Force);
            // OK, we changed the pitch more, but did we go beyound 90 degrees?
            // Mod 360 to make sure we don't compare wrapped angle values.
#if (DEBUG_ANGLE_X)
            Debug.Log(" Changed pitch x to: " + transform.eulerAngles.x);
#endif
         }

        // After applying force, angles continue to change, so check every loop through.
#if (DEBUG_ANGLE_X)
        Debug.Log("... Before ANGLES: " + transform.eulerAngles);
#endif
        bool bClampedPitch = false;
        float checkAndleX = transform.eulerAngles.x % 360f;
        // If angle > 180, it means it is on the other side of the circle;
        // reverse the angle for easier comparison.
        if (checkAndleX > 180f)
        {
            checkAndleX -= 360f;    // Display as negative angle
        }
        else if (checkAndleX < -180f)
        {
            checkAndleX += 360f;    // Display as positive angle
        }
        // Now see if angle exceeds -90 to 90 degrees.
        if (checkAndleX > 90)
        {
            checkAndleX = 90;
            bClampedPitch = true;
        }
        else if (checkAndleX < -90)
        {
            checkAndleX = -90;
            bClampedPitch = true;
        }
        if (bClampedPitch)
        {
            // Convert new angles (well, x) to quaternion
            float origAngleY = transform.eulerAngles.y % 360f;
            float origAngleZ = transform.eulerAngles.z % 360f;
            transform.eulerAngles = new Vector3(checkAndleX, origAngleY, origAngleZ);
        }
#if (DEBUG_ANGLE_X)
        Debug.Log("*** After ANGLES: " + transform.eulerAngles);
#endif





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


        torque = Vector3.zero;
        horizontalRawInput = Input.GetAxisRaw("Horizontal");

        // yaw:
        //        if (Input.GetKey(KeyCode.D) || horizontalRawInput == 1)
        //            torque.z -= turnSpeed;
        //        if (Input.GetKey(KeyCode.A) || horizontalRawInput == -1)
        //            torque.z += turnSpeed;
        //
        //        rb.AddRelativeTorque(torque);
        /* Close 3. Change Pitch */


        /* 4. Ship Range Check 
        // Make sure ship does not get out of range.
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;
        bool changePos = false;
        if (x > shipRange)
        {
            x = shipRange;
            changePos = true;
        }
        else if (x < -shipRange)
        {
            x = -shipRange;
            changePos = true;
        }
        if (y > shipRange)
        {
            y = shipRange;
            changePos = true;
        }
        else if (y < -shipRange)
        {
            y = -shipRange;
            changePos = true;
        }
        if (z > shipRange)
        {
            z = shipRange;
            changePos = true;
        }
        else if (z < -shipRange)
        {
            z = -shipRange;
            changePos = true;
        }
        if (changePos)
        {
            transform.position = new Vector3(x, y, z);
        }
        4. Ship Range Check */



        /*
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
        */
    }


    // I noticed that the vector that the charges seem to leave the claw on may be
    // canted to one side or the other. I'm speculating that i have applied a force
    // in the FixedUpdate call but when i grab the claw rotation vector it seems like
    // the force has not yet been applied. So let the Update or FiexedUpdate finish
    // and then get the claw vector to use for the charge. (All the documentation talks
    // about wanting to insure the camera is pointing correctly after movement. I'm
    // assuming the same logic may apply to the shot vector.)
    void LateUpdate()
    {
        // Check for charge fired after ship move, so charge move will match ship move.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // launch projectile
            ///GameObject sgo = Instantiate(chargePrefab, transform.position, chargePrefab.transform.rotation);
            GameObject sgo = Instantiate(chargePrefab, transform.position, transform.rotation);
            // Make sure the shot is going the way the ship is pointing.
            Rigidbody srb = sgo.GetComponent<Rigidbody>();
            srb.velocity = transform.forward * shotSpeed;
        }
    }
}

