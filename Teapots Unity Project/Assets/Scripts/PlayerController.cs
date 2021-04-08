#undef DEBUG_ANGLE_Y
#undef DEBUG_ANGLE_X
#undef TEST_BACK_FLIP
#undef TEST_GYROSCOPE
#define TRACE_COLLISIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// PlayerController is Unity’s name for main character.
// In Tempest, player controls a ship!
public class PlayerController : MonoBehaviour
{
    private const int MAX_GYRO_DELAY = 20;  // ABOUT 1/3 SEC

    private float horizontalRawInput, verticalRawInput;
    public float turnSpeed;     // Radians / sec
    public float raiseSpeed;    // Radians / sec
    public float shipSpeed;
    public float shipSpeedMax;
    public float shipSpeedStep;
    public float levelSpeed;
    public float shotSpeed;
    public float shipRange = 25f;
    ///    public float startMouseX;
    ///    public float deltaMouseX;
    public GameObject chargePrefab;
    public int gyroDelay = MAX_GYRO_DELAY;

    public Rigidbody rb;
    public AudioSource playerAudioSource;
    public AudioClip shotSound;
    public GameManager gameManager; // change to private after we let inspector show we get value.


#if (DEBUG_ANGLE_Y)
    float debugAngleY;       // DEBUG
#endif
#if (DEBUG_ANGLE_X)
    float debugAngleX;
#endif


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        rb = GetComponent<Rigidbody>();
        playerAudioSource = GetComponent<AudioSource>();

        // Let speeds be set in Inspector while developing.
        // May want to programatically init to allow for increased speed as we go up levels.
        turnSpeed = 15.0f;      // Good speed to get ship turning from dead stop.
        raiseSpeed = 5.0f;      // Same speed as rotation was too much.
        shotSpeed = 10.0f;
        shipSpeed = 0.0f;       // Start with no forward motion.
        shipSpeedMax = 4.0f;
        shipSpeedStep = 0.5f;
        levelSpeed = 1.0f;      // Don't level off too fast so we hopefully don't notice it.
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

        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // yaw:
            // Using the RAW axis returns hard coded +1 or -1. (Doesn't seem so RAW to me.)
            horizontalRawInput = Input.GetAxisRaw("Horizontal");
            if (Input.GetKeyDown(KeyCode.D) || horizontalRawInput == 1)
            {
                torque.y += turnSpeed;
                //torque.z += turnSpeed;  // Try Bob's idea in tekshire.com/teapotroll
                bChangeTorqueY = true;
            }
            if (Input.GetKeyDown(KeyCode.A) || horizontalRawInput == -1)
            {
                torque.y -= turnSpeed;
                //torque.z -= turnSpeed;  // Try Bob's idea in tekshire.com/teapotroll
                bChangeTorqueY = true;
            }
        }
        if (bChangeTorqueY)
        {
#if (DEBUG_ANGLE_Y)
            Debug.Log("Change rotate y by torque: " + torque);
#endif
            gyroDelay = MAX_GYRO_DELAY;

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


        // Moved all rotation changes into sequence in case we can coallese all logic
        // into a single bit of control.

        /* 2. Change Pitch */
        // Limit pitch change to -90 through 90 degrees, so we don't flip over upside down.
        // Quaternion Note: When debugging the pitch not getting clamped, i discovered that
        // while force is being applied and Unity physics is doing its work, when i then read
        // the Euler angles... As the X axis angle is nearing 90 degrees, i'm expecting to
        // see the next angle get to 91. However, the conversion from quaternion to angle
        // determines the height of my ship is the same at either 91 OR 89! So the Euler angle
        // conversion walks backward instead of going beyond 90. Right now i have a hack of
        // comparing for only 85 degrees, which i do get beyond and my calculations can clamp
        // at that value! Yay! Almost.. Turns out that as the there are ways of steering that
        // can aggect all 3 Euler angles and my x axis comparison can break and stay broken
        // after that. I will try a few things to see if i can mitigate this.

        bool bChangeTorqueX = false;
        torque = Vector3.zero;

        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
                // pitch:
            verticalRawInput = Input.GetAxisRaw("Vertical");
            if (Input.GetKeyDown(KeyCode.W) || verticalRawInput == 1)
            {
                torque.x -= raiseSpeed;
                bChangeTorqueX = true;
            }
            if (Input.GetKeyDown(KeyCode.S) || verticalRawInput == -1)
            {
                torque.x += raiseSpeed;
                bChangeTorqueX = true;
            }
        }   // isGameActive

        if (bChangeTorqueX)
        {
#if (DEBUG_ANGLE_X || TEST_BACK_FLIP)
            Debug.Log("Change pitch x by torque: " + torque);
#endif
            gyroDelay = MAX_GYRO_DELAY;

            // For rotation, we could just use transform change rotation, but want
            // to use AddTorque so that angular drag can have an effect.
            rb.AddRelativeTorque(torque, ForceMode.Force);
            // OK, we changed the pitch more, but did we go beyound 90 degrees?
            // Mod 360 to make sure we don't compare wrapped angle values.
#if (DEBUG_ANGLE_X || TEST_BACK_FLIP)
            Debug.Log(" Changed pitch x to: " + transform.eulerAngles.x);
#endif
        }

        // After applying force, angles continue to change, so check every loop through.
#if (DEBUG_ANGLE_X || TEST_BACK_FLIP)
        Debug.Log("... Before ANGLES: " + transform.eulerAngles);
#endif
        bool bClampedPitch = false;
        float checkAndleX = transform.eulerAngles.x % 360f;
        // If angle > 180, it means it is on the other side of the circle;
        // reverse the angle for easier comparison.
        if (checkAndleX > 180f)
        {
            checkAndleX -= 360f;    // Display as negative angle
#if (TEST_BACK_FLIP)
            Debug.Log("if (checkAndleX > 180f), checkAndleX: " + checkAndleX);
#endif
        }
        else if (checkAndleX < -180f)
        {
            checkAndleX += 360f;    // Display as positive angle
#if (TEST_BACK_FLIP)
            Debug.Log("if (checkAndleX < -180f), checkAndleX: " + checkAndleX);
#endif
        }
        // Now see if angle exceeds -90 to 90 degrees.
        if (checkAndleX > 85f)
        {
            checkAndleX = 85f;
            bClampedPitch = true;
#if (TEST_BACK_FLIP)
            Debug.Log("if (checkAndleX > 90f), checkAndleX: " + checkAndleX);
#endif
        }
        else if (checkAndleX < -85f)
        {
            checkAndleX = -85f;
            bClampedPitch = true;
#if (TEST_BACK_FLIP)
            Debug.Log("if (checkAndleX < -90f), checkAndleX: " + checkAndleX);
#endif
        }
        if (bClampedPitch)
        {
#if (TEST_BACK_FLIP)
            Debug.Log("##### Clamping to: " + checkAndleX);
#endif
            // Convert new angles (well, x) to quaternion
            float origAngleY = transform.eulerAngles.y % 360f;
            float origAngleZ = transform.eulerAngles.z % 360f;
            //float origAngleY = transform.eulerAngles.y;
            //float origAngleZ = transform.eulerAngles.z;
            transform.eulerAngles = new Vector3(checkAndleX, origAngleY, origAngleZ);
#if (DEBUG_ANGLE_X || TEST_BACK_FLIP)
            Debug.Log("+++ After Clamp: " + transform.eulerAngles);
#endif
        }
#if (DEBUG_ANGLE_X || TEST_BACK_FLIP)
        Debug.Log("*** After ANGLES: " + transform.eulerAngles);
#endif





        // Using AddForce to allow for collisions between objects. However, AddTorque gets vectors wrong when
        // ship turns upside down. So use AddRelativeTorque to keep right arrow always turning right.

        //horizontalRawInput = Input.GetAxisRaw("Horizontal");
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


        /* Close 2. Change Pitch */


        /* 3. Level Ship */
        // As we change rotation (yaw and pitch) the vector changes can also affect roll.
        // That leaves the ship flying at an angle to the horizon, which can be disorienting.
        // Pretend we have a gyroscope that can level off the ship when we are not actively
        // changing the other 2 axis. Bring the roll back to 0 degrees off the z axis.
        if (!bChangeTorqueX && !bChangeTorqueY)
        {
            // Also don't start the gyroscope right away if we have just sent steering commands.
            // Let them play out.
            //public int gyroDelay = MAX_GYRO_DELAY;
            gyroDelay--;
            if (gyroDelay < 0)
            {
#if (TEST_GYROSCOPE)
            // Begin by seeing what vector we are flying at.
            Debug.Log("----- Leveling up from: " + transform.eulerAngles);
#endif
                float checkAndleZ = transform.eulerAngles.z % 360f;
#if (TEST_GYROSCOPE)
            Debug.Log("Starting with Z angle: " + checkAndleZ);
#endif
                if (!Mathf.Approximately(checkAndleZ, 0.0f))
                {
#if (TEST_GYROSCOPE)
                    Debug.Log("Not approximately 0: " + checkAndleZ);
#endif
                    // If not already level, we know we will be making a change.

                    // If angle > 180, it means it is on the other side of the circle;
                    // reverse the angle for easier comparison.
        // ToDo: Use while instead of if
                    if (checkAndleZ > 180f)
                    {
                        checkAndleZ -= 360f;    // Display as negative angle
#if (TEST_GYROSCOPE)
                    Debug.Log("checkAndleZ > 180f: " + checkAndleZ);
#endif
                    }
                    else if (checkAndleZ < -180f)
                    {
                        checkAndleZ += 360f;    // Display as positive angle
#if (TEST_GYROSCOPE)
                    Debug.Log("checkAndleZ < -180f: " + checkAndleZ);
#endif
                    }
                    // Now see if angle exceeds -levelSpeed to levelSpeed degrees.
                    if (checkAndleZ > levelSpeed)
                    {
                        checkAndleZ -= levelSpeed;
#if (TEST_GYROSCOPE)
                    Debug.Log("checkAndleZ > levelSpeed: " + checkAndleZ);
#endif
                    }
                    else if (checkAndleZ < -levelSpeed)
                    {
                        checkAndleZ += levelSpeed;
#if (TEST_GYROSCOPE)
                    Debug.Log("checkAndleZ < -levelSpeed: " + checkAndleZ);
#endif
                    }
                    else
                    {
                        checkAndleZ = 0.0f;
#if (TEST_GYROSCOPE)
                    Debug.Log("else: " + checkAndleZ);
#endif
                    }
#if (TEST_GYROSCOPE)
                Debug.Log("New Z angle: " + checkAndleZ);
#endif
                    // Convert new angles (well, x) to quaternion
                    float origAngleX = transform.eulerAngles.x % 360f;
                    float origAngleY = transform.eulerAngles.y % 360f;
                    //float origAngleX = transform.eulerAngles.x;
                    //float origAngleY = transform.eulerAngles.y;
                    transform.eulerAngles = new Vector3(origAngleX, origAngleY, checkAndleZ);
#if (TEST_GYROSCOPE)
            Debug.Log("transform.eulerAngles: " + transform.eulerAngles);
#endif
                }
        }
        }
        /* Close 3. Level Ship */

                // Use NORMALIZED to keep claw from accelerating too fast
                //// create a normalized vector in the rotation direction
                //var moveDir = Vector3.Cross(rigidbody.angular.velocity, Vector3.up).normalized;
                //rigidbody.AddForce(moveDir * force);


        /* 4. Forward speed */
        // Forward speed
        bool bChangeSpeed = false;
        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // No arrow keys to affect forward speed at this time.
            if (Input.GetKeyDown(KeyCode.Q))
            {
                shipSpeed += shipSpeedStep;
                if (shipSpeed > shipSpeedMax)
                    shipSpeed = shipSpeedMax;
                bChangeSpeed = true;
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                shipSpeed -= shipSpeedStep;
                if (shipSpeed < 0)
                    shipSpeed = 0;
                bChangeSpeed = true;
            }
        }   // isGameActive

        if (bChangeSpeed)
        {
            // rb.AddRelativeForce((Vector3.forward * deltaMouseX).normalized, ForceMode.VelocityChange);
            rb.velocity = transform.forward * shipSpeed;
        }

        //to get the direction as a unit vector do this:
        //Vector3 direction = velocity.normalized;
        //To get the speed do this:
        //float speed = velocity.magnitude;

        /* Close 4. Forword speed */



        // We rotate horizontally and vertical. If we do both at the same time,
        // the added vectors can wind up banking our ship, which i find confusling.
        // This is true even if we do 2 separate calls to AddRelativeTorgue each
        // with only the horizontal torque or the vertical torgue. See if it makes
        // any difference if we put the AddRelativeForce call in between.


        /* 5. Ship Range Check */
        // Make sure ship does not get out of range.
        bool changePos = false;
        float x = transform.position.x;
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
        float y = transform.position.y;
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
        float z = transform.position.z;
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
            // Plan is that we should be beyond the range of other objects
            // we might collide with, so can simply transform location.
            transform.position = new Vector3(x, y, z);
        }
        /* 5. Ship Range Check */

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
        // No input, spawning, or scoring if game not active.
        if (gameManager.isGameActive)
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

                // Get lower score if you just blast away. (Not part of regular Tempest.)
                gameManager.UpdateScore(-1);

                // Eventually check for sound
                playerAudioSource.PlayOneShot(shotSound, 1.0f);
            }
        }   // isGameActive
    }


    private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Player OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Just seeing if we come here at all.
    }


    private void OnTriggerEnter(Collider other)
    {
        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // When we first fire a shot, when the charge is created, it is within the
            // colllision area of our ship. So we trigger this as soon as we shoot.
            // Check tag to avoid this situation.
            if (other.CompareTag("Charge")) return;

        // So far, there are only 2 ojects with triggers: charges and enemy charges.
        // (I suppose later we may have flippers etc that may have triggers.)
        // Player probably does not move fast enough to catch up to any of its own charges,
        // so any trigger enter must be for enemycharge. But maybe check tag just to be sure
        gameManager.UpdateLives(-1);
        }   // isGameActive

    }


}

