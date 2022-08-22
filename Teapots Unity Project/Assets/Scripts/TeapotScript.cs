#define TRACE_COLLISIONS
#define COLLISION_TEST_JIG
#define MOVE_TEAPOTS
#undef TEST_FIRE_SHOTS  // If using alpha keys to test collisions, don't also fire shots

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The original plan for teapot motion was to allow them to collide with each other and
// the player's ship and then use physics to bounce away. In order to use physics, we needed
// to use FixedUpdate() instead of Update(). However, to get the teapots flying in orbits,
// we used transform.RotateAround() which does not use physics and thus cancelled out our
// bouncing. So we keep teapot motion in the Update() function which has the added advantage
// of moving each frame for smoother motion. The player colliding with teapots had 2 other
// purposes. 1. We had a non-shooting version of Teapots that was "Tag". For that we wanted
// the bouncing away behavior, but we have simplified our game design by removing that option,
// so we no longer need it for that reason. 2. For Tempest in a Teapot, the original plan had
// been to collide with the spout in order to dive down inside the teapot to play Tempest.
// However that was too challenging, so latest plans just require colliding anywhere on the
// teapot and then slurping automatically to the spout. So i now no longer need teapot motion
// to be in FixedUpdate().

public class TeapotScript : MonoBehaviour
{
    private GameManager gameManager;
    public Renderer m_Renderer;
    public AudioSource teapotAudio;    // Made public so i can play with pitch modifier in inspector
    public GameObject explosionPrefab;
    public GameObject shotPrefab;
    public ParticleSystem steamParticles;
    public float shotSpeed = 5.0f;  // For now, 1/2 chargeSpeed (set in PlayerController.Start().)
    public float aimSpeed = 0.01f;

    static float maxVel = 2.0f;
    public AudioClip[] crashSoundArray = new AudioClip[6];
    public AudioClip explosionSound;    // purchased "Explosion Glass Blasstwave Fx".
    public AudioClip steamSound;        // PressurizedSteam from Particle Pack 5 package.
    public bool isTagged = false;
    public int index = -1;      // Valid indexes are 0 - 15.

    //public Vector3 axis;
    public static Vector3 spoutOffset = new Vector3(-1.45f, 1.2f, 0.0f);
    // Values above are overridden by values entered into inspector.
    public static Quaternion spoutToFace = Quaternion.Euler(90, 0, 0);

#if (COLLISION_TEST_JIG)
    public float speed = 0.00001f;
    public Rigidbody rb;
    public Vector3 colliderMovement;

    public bool bTestTarget = false;
    public bool bTestCollider = false;
#endif

    // Start is called before the first frame update
    void Start()
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot Start: ");
#endif
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

#if (COLLISION_TEST_JIG)
        rb = this.GetComponent<Rigidbody>();
        colliderMovement = new Vector3(500.0f, 0.0f, 0.0f);
#endif

        //Fetch the Renderer component of the GameObject to change color
        m_Renderer = GetComponent<Renderer>();
        teapotAudio = GetComponent<AudioSource>();

        // Each teapot has a child particle system to provide steam for shot launch.
        steamParticles = GetComponentInChildren<ParticleSystem>();
        //Debug.Log("Start FindObjectsOfType<ParticleSystem>: " + steamParticles + "\n");
    }


    // Want to apply physics to teapots, so move him in FixedUpdate.
    void FixedUpdate()
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot FixedUpdate.index: " + index);
#endif

#if (COLLISION_TEST_JIG)
        if (bTestCollider)
        {
#if (TRACE_COLLISIONS)
            Debug.Log("Teapot.FixedUpdate bTestCollider @ (" +
                colliderMovement.x + ", " + colliderMovement.y +
                ", " + colliderMovement.z + ")");
#endif
            // Add the motion from collider to target. ie. left
            rb.AddForce(colliderMovement, ForceMode.Impulse);


          //ForceMode.Force: Interprets the input as force (measured in Newtons), and changes the velocity by the value of force* DT / mass.The effect depends on the simulation step length and the mass of the body.
          //ForceMode.Acceleration: Interprets the parameter as acceleration (measured in meters per second squared), and changes the velocity by the value of force* DT. The effect depends on the simulation step length but doesn't depend on the mass of the body.
          //ForceMode.Impulse: Interprets the parameter as an impulse(measured in Newtons per second), and changes the velocity by the value of force / mass.The effect depends on the mass of the body but doesn't depend on the simulation step length.
          //ForceMode.VelocityChange: Interprets the parameter as a direct velocity change(measured in meters per second), and changes the velocity by the value of force.The effect doesn't depend on the mass of the body or the simulation step length.

        }
        else if (bTestTarget)
        {
#if (TRACE_COLLISIONS)
            Debug.Log("Teapot.FixedUpdate bTestTarget");
#endif
            // Target does not move. It just stays here to get hit.
        }
        else
        {   // final else
            // Normal teapot movement
#if (TRACE_COLLISIONS)
            Debug.Log("Teapot Normal Movement");
#endif
#endif  // COLLISION_TEST_JIG

            if (gameManager.isGameActive)
            {
#if (MOVE_TEAPOTS)
#if (TRACE_COLLISIONS)
                Debug.Log("Teapot Actually Moves");
#endif
                // REGULAR TEAPOT MOVEMENT
                // The center of our globular cluster is (0,0,0).
                transform.RotateAround(Vector3.zero, gameManager.teapotRotateVector, gameManager.teapotRotateSpeed);
                /*
                 * RotateAround() is deprecated; suggestion is to use Rotate().
                 * If i use the same axis vector for all teapots, they all orbit parallel to each other,
                 * so there should be no collisions. OK for first pass. Not sure if basis for RotateAround
                 * is Translate or AddForce. If the former, collisions will not respond to physics, so may
                 * need a way to escape from RotateAround if collied with (using "isTagged" variable).
                 * NOTE: IF WE DO MOVE TO USING FORCE, IT WILL HAVE TO BE DONE IN FIXEDUPDATE() INSTEAD 
                 * OF UPDATE().
                 */
                // this.transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
#endif  // MOVE_TEAPOTS
            }   // gameManager.isGameActive

#if (COLLISION_TEST_JIG)
        }   // final else
#endif  // COLLISION_TEST_JIG
    }


    // Using rb.AddForce which adds speed and slows
    //    void MoveCharacter(Vector3 direction)
    //    {
    //        rb.AddForce(direction * speed);
    //    }

    //    // Using rb.velocity overrides all other forces including gravity.
    //    void MoveCharacter(Vector3 direction)
    //    {
    //        rb.velocity = direction * speed;
    //    }


#if false
    void Update()
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot Update: ");
#endif
#if (COLLISION_TEST_JIG)
        // Move test objects velocity even if all other teapot motion stopped
        ///        if (bTestVelocity)
        ///        {
        ///            colliderMovement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ///
        ///            Rigidbody rb = GetComponent<Rigidbody>();
        ///            Vector3 vel = rb.velocity;
        ///
        ///            // Collider is only object that sets bTestVelocity.
        ///        }
#endif

        if (gameManager.isGameActive)
        {
            //#if (MOVE_TEAPOTS && !COLLISION_TEST_JIG)
#if (MOVE_TEAPOTS)
            // REGULAR TEAPOT MOVEMENT
            // The center of our globular cluster is (0,0,0).
            transform.RotateAround(Vector3.zero, gameManager.teapotRotateVector, gameManager.teapotRotateSpeed * Time.deltaTime);
            /*
             * RotateAround() is deprecated; suggestion is to use Rotate().
             * If i use the same axis vector for all teapots, they all orbit parallel to each other,
             * so there should be no collisions. OK for first pass. Not sure if basis for RotateAround
             * is Translate or AddForce. If the former, collisions will not respond to physics, so may
             * need a way to escape from RotateAround if collied with (using "isTagged" variable).
             * NOTE: IF WE DO MOVE TO USING FORCE, IT WILL HAVE TO BE DONE IN FIXEDUPDATE() INSTEAD 
             * OF UPDATE().
             */
            // this.transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
#endif  // MOVE_TEAPOTS

            // Right now, in all cases, want spout facing player ship
            Vector3 direction = gameManager.player.transform.position - transform.position;
            // This is the quaternion that points the teapot's NORMAL face toward the player
            Quaternion rotation = Quaternion.LookRotation(direction);
            // Now adjust the direction to point the spout toward the player
            Quaternion aimDirection = rotation * spoutToFace;    // Use multiply to add 2 quaternions
            transform.rotation = Quaternion.Lerp(transform.rotation, aimDirection, aimSpeed * Time.deltaTime);
        }   // isGameActive
    }   // Update()

#endif  // false    Update


    // When firing enemy shots from Update(), shots did not fire from tip of teapot spout.
    // So move shot spawning here so teapot has already moved when we check for spout location.
    void LateUpdate()
    {
        // No input, spawning, or scoring if game not active.
        if (gameManager.isGameActive)
        {

            // Check to see if we fire shot at player
            bool bFireShot = false;
#if TEST_FIRE_SHOTS
            switch (index)
            {
                case 0:
                case 10:
                    if (Input.GetKeyDown(KeyCode.Alpha0)) bFireShot = true;
                    break;
                case 1:
                case 11:
                    if (Input.GetKeyDown(KeyCode.Alpha1)) bFireShot = true;
                    break;
                case 2:
                case 12:
                    if (Input.GetKeyDown(KeyCode.Alpha2)) bFireShot = true;
                    break;
                case 3:
                case 13:
                    if (Input.GetKeyDown(KeyCode.Alpha3)) bFireShot = true;
                    break;
                case 4:
                case 14:
                    if (Input.GetKeyDown(KeyCode.Alpha4)) bFireShot = true;
                    break;
                case 5:
                case 15:
                    if (Input.GetKeyDown(KeyCode.Alpha5)) bFireShot = true;
                    break;
                case 6:
                    if (Input.GetKeyDown(KeyCode.Alpha6)) bFireShot = true;
                    break;
                case 7:
                    if (Input.GetKeyDown(KeyCode.Alpha7)) bFireShot = true;
                    break;
                case 8:
                    if (Input.GetKeyDown(KeyCode.Alpha8)) bFireShot = true;
                    break;
                case 9:
                    if (Input.GetKeyDown(KeyCode.Alpha9)) bFireShot = true;
                    break;
            }
#endif
            // Normal check to see if we do fire a shot.
            if (gameManager.bFireEnabled)
            {
                //int nextTeapotToFire = gameManager.lastTeapotShot = index;
                int nextTeapotToFire = gameManager.nextTeapotShot;

                // if we are the next teapot to fire, fire
                if (nextTeapotToFire == index)
                    bFireShot = true;
            }

            if (bFireShot)
            {
                // Start enemy shot with a puff of steam.
                if (steamParticles != null)
                    steamParticles.Play();

                //// Make sure the steam is going the way the ship spout is pointing.
                //Rigidbody steamRB = steamObj.GetComponent<Rigidbody>();
                //steamRB.velocity = transform.up * shotSpeed;

                // Fire shot
                GameObject shotObj = Instantiate(shotPrefab,
                    transform.position + (transform.rotation * spoutOffset),
                    transform.rotation);
                // Note: Matrix multiplication must be Quarternion * vector, not reverse.

                // Make sure the shot is going the way the ship is pointing.
                Rigidbody shotRB = shotObj.GetComponent<Rigidbody>();
                shotRB.velocity = transform.up * shotSpeed;

#if (TRACE_COLLISIONS)
                ShotScript sScript = shotObj.GetComponent<ShotScript>();

                int newShotNum = gameManager.lastShotNum + 1;
                sScript.shotNum = newShotNum;
                gameManager.lastShotNum = newShotNum;
#endif

                // shot sound from teapot is actually sound of steam coming from pot.

                // Finally, if we shot, reset the shot counter.
                gameManager.bFireEnabled = false;
                gameManager.teapotShotTimer = 0;
                int nextPossibleTeapotToShoot = index + 1;
                if (nextPossibleTeapotToShoot >= 16) nextPossibleTeapotToShoot = 0;
                gameManager.nextTeapotShot = nextPossibleTeapotToShoot;


            }

        }   // isGameActive
    }   // LateUpdate()


    // No longer have trigger on teapot in order to enable bouncing off ship.
    // But still get here when hit by charge. Charge OnTriggerEnter used to
    // destroy both charge and hit teapot. Now moved teapot destruction
    // here to add animation and sound.
    private void OnTriggerEnter(Collider other)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnTriggerEnter: " + gameObject + " triggered by " + other.gameObject);
#endif
        // Only blow up teapot if it hit by player charge.
        if (other.gameObject.CompareTag("Charge"))
        {
            // First do animation because light moves faster than sound.
            // Call explosion animation.
            GameObject explosionObject =
                Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            // Explosion sound
            // (Remember teapotAudio is destroyed when we Destroy(gameObject) below,
            // so need new AudioSource that will last until sound is done.)
            AudioSource explosionAudio = explosionObject.GetComponent<AudioSource>();
            Vector3 toPlayer = gameManager.player.transform.position - transform.position;
            float distance = toPlayer.magnitude;
            // Want sound volume to fall off faster than pure distance so use r^2
            distance = distance * distance / 25.0f;
            if (distance < 0) distance = -distance;
            if (distance < 1.0f) distance = 1.0f;
            explosionAudio.PlayOneShot(explosionSound, 1.0f / distance);

            Destroy(gameObject);    // Someday replace with exploding teapot segments.

            // Plan to have explosion animation overwhelm teapot, so don't destroy until a bit later.
            // (Use co-routine to do destruction later.)
            //       Destroy(gameObject);
            //   StartCoroutine(DelayDeath());

            gameManager.UpdateScore(gameManager.teapotPointValue);
        }   // Charge
    }


    IEnumerator DelayDeath()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);    // Maybe use to clean up explosionObject residue.
    }


    private void OnCollisionEnter(Collision collision)
    {
#if (TRACE_COLLISIONS)
        Debug.Log("Teapot OnCollisionEnter: " + gameObject + " collided with " + collision.gameObject);
#endif
        // Generate sound if teapot collides with Player or other teapot
        int crashSoundIndex = Random.Range(0, crashSoundArray.Length);
        Debug.Log("Trying to play crashSoundArray[" + crashSoundIndex + "] from AudioSource: " +
            teapotAudio);
        //        Debug.Log("Audio Clip should be: " + crashSoundArray[crashSoundIndex]);
        // Affect volume by how fast the ship is striking the teapot.
        // Sound only attenuates, so value between 0.0 and 1.0. Use maxVel to make sure
        // fastest collision equals loudest sound.
        float collisionSpeed = collision.relativeVelocity.magnitude;
        if (collisionSpeed > maxVel)
            maxVel = collisionSpeed;
        teapotAudio.PlayOneShot(crashSoundArray[crashSoundIndex], collisionSpeed / maxVel);

        // ToDo: collision sound should also be dependent upon if we are playing game or not.
        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // Change teapot color only if hit by Player
            // AND ONLY IF WE'RE PLAYING TAG. (Collision noise is OK though.)
            if (collision.gameObject.CompareTag("Player"))
            {
                m_Renderer.material.color = Color.white;
                isTagged = true;
            }
        }   // isGameActive
    }




}




#if false
                Vector3 shotPos = transform.position + spoutOffset;
                Debug.Log("Shooting from teapot[" + index + "]\n" +
                    "transform.localPosition: x = " + transform.localPosition.x + ", y = " +
                    transform.localPosition.y + ", z = " + transform.localPosition.z + "\n" +
                    "transform.position: x = " + transform.position.x + ", y = " +
                    transform.position.y + ", z = " + transform.position.z + "\n" +
                    "spoutOffset: x = " + spoutOffset.x + ", y = " +
                    spoutOffset.y + ", z = " + spoutOffset.z + "\n" +
                    "shotPos: x = " + shotPos.x + ", y = " +
                    shotPos.y + ", z = " + shotPos.z + "\n" +
                    "teapot spout: x = " + shotObj.transform.position.x + ", y = " +
                    shotObj.transform.position.y + ", z = " + shotObj.transform.position.z + "\n" +
                    "transform.rotation: " + transform.rotation + "\n"
                    );
#endif
