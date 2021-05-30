#define TRACE_COLLISIONS
#define MOVE_TEAPOTS
#define TEST_FIRE_SHOTS

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
// teapot and then slurppig automatically to the spout. So i now no longer need teapot motion
// to be in FixedUpdate().

public class TeapotScript : MonoBehaviour
{
    private GameManager gameManager;
    public Renderer m_Renderer;
    public AudioSource teapotAudio;    // Made public so i can play with pitch modifier in inspector
    public GameObject explosionPrefab;
    public GameObject shotPrefab;
    public float shotSpeed = 5.0f;  // For now, 1/2 chargeSpeed (set in PayerController.Start().)

    static float maxVel = 2.0f;
    public AudioClip[] crashSoundArray = new AudioClip[6];
    public AudioClip explosionSound;
    public int pointValue;
    public bool isTagged = false;
    public int index = -1;      // Valid indexes are 0 - 15.

    //public Vector3 axis;
    public static Vector3 spoutOffset = new Vector3(-1.45f, 1.2f, 0.0f);
    // Values above are overridden by values entered into inspector.


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Fetch the Renderer component of the GameObject to change color
        m_Renderer = GetComponent<Renderer>();
        teapotAudio = GetComponent<AudioSource>();

        pointValue = 1000;  // Depends upon game level
    }


    void Update()
    {
        if (gameManager.isGameActive)
        {
#if MOVE_TEAPOTS
            // The center of our globular cluster is (0,0,0).
            // this.transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
            ////this.transform.RotateAround(Vector3.zero, axis, 5.0f);
            transform.RotateAround(Vector3.zero, gameManager.teapotRotateVector, gameManager.teapotRotateSpeed * Time.deltaTime);
            /*
             * RotateAround() is depricated; suggestion is to use Rotate().
             * If i use the same axis vector for all teapots, they all orbit parallel to each other,
             * so there should be no collisions. OK for first pass. Not sure if basis for RotateAround
             * is Translate or AddForce. If the former, collisions will not respond to physics, so may
             * need a way to escape from RotateAround if collied with (using "isTagged" variable).
             * NOTE: IF WE DO MOVE TO USING FORCE, IT WILL HAVE TO BE DONE IN FIXEDUPDATE() INSTEAD 
             * OF UPDATE().
             */
#endif
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
            if (bFireShot)
            {
                // Fire shot
                ///GameObject sgo = Instantiate(chargePrefab, transform.position, chargePrefab.transform.rotation);
                GameObject shotObj = Instantiate(shotPrefab, transform.position + spoutOffset, transform.rotation);
                // Make sure the shot is going the way the ship is pointing.
                Rigidbody shotRB = shotObj.GetComponent<Rigidbody>();
                shotRB.velocity = transform.up * shotSpeed;
            }

        }
    }


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

            gameManager.UpdateScore(pointValue);
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

