#define TRACE_COLLISIONS

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TeapotScript : MonoBehaviour
{
    private float timer = 0;
    private GameManager gameManager;
    public Renderer m_Renderer;
    public AudioSource teapotAudio;    // Made public so i can play with pitch modifier in inspector
    public GameObject explosionPrefab;


    static float maxVel = 2.0f;
    public AudioClip[] crashSoundArray = new AudioClip[6];
    public AudioClip explosionSound;
    public int pointValue;
    public bool isTagged = false;
    public Vector3 axis;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();

        //Fetch the Renderer component of the GameObject to change color
        m_Renderer = GetComponent<Renderer>();
        teapotAudio = GetComponent<AudioSource>();

        pointValue = 1000;  // Depends upon game level
    }


    // Teapot movement uses force of gravity pulling down, while centripital
    // force tends to keep teapot moving forward.
    // Force of gravity:
    //      F = G (m1 * m2) / r^2
    // Programming:
    /*
    void Update() {
      foreach (GameObject s in spheres) {
        Vector3 difference = this.transform.position - s.transform.position;
        float dist = difference.magnitude;
        Vector3 gravityDirection = difference.normalized;
        float gravity = 9.81f * (this.transform.localScale.x * s.transform.localScale.x
            * 80) / (dist * dist);
        Vector3 gravityVector = (gravityDirection * gravity);
        s.transform.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration); }}
    */
    void FixedUpdate()    // Don't need Time.deltaTime when using FixedUpdate.
    {
        if (gameManager.isGameActive)
        {
            // The center of our globular cluster is (0,0,0).
            //           Vector3 difference = -this.transform.position;
            //           float dist = difference.magnitude;
            //           Vector3 gravityDirection = difference.normalized;
            //           float gravity = (9.81f * this.transform.localScale.x) / (dist * dist);
            //           Vector3 gravityVector = (gravityDirection * gravity);
            //           this.transform.GetComponent<Rigidbody>().AddForce(transform.forward, ForceMode.Acceleration);
            //           this.transform.GetComponent<Rigidbody>().AddForce(gravityVector, ForceMode.Acceleration);
            ////this.transform.RotateAround(Vector3.zero, axis, 5.0f);
            this.transform.RotateAround(Vector3.zero, Vector3.up, .05f);
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
        // Right now only object with trigger is charge, so no need to check at this time.
        // Only blow up teapot if it hits a charge.
        // if (other.gameObject.tag == "Charge")
        // {
        // First do animation because light moves faster than sound.
        // Call explosion animation.
        Destroy(gameObject);
        GameObject explosionObject =
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);

        // Explosion sound
        AudioSource explosionAudio = explosionObject.GetComponent<AudioSource>();
        explosionAudio.PlayOneShot(explosionSound);

        gameManager.UpdateScore(pointValue);

        // Plan to have explosion animation overwhelm teapot, so don't destroy until a bit later.
        // (Use co-routine to do destruction later.)
        //       Destroy(gameObject);
        //   StartCoroutine(DelayDeath());
        // }    // tag == "Charge"
    }



IEnumerator DelayDeath()
{
    yield return new WaitForSeconds(1.0f);
    Destroy(gameObject);
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
        // fasted collision equals loudest sound.
        float collisionSpeed = collision.relativeVelocity.magnitude;
        if (collisionSpeed > maxVel)
            maxVel = collisionSpeed;
        teapotAudio.PlayOneShot(crashSoundArray[crashSoundIndex], collisionSpeed/maxVel);

        // ToDo: collision sound should also be dependent upon if we are playing game or not.
        if (gameManager.isGameActive)  // No input, spawning, or scoring if game not active.
        {
            // Change teapot color only if hit by Player
            // AND ONLY IF WE'RE PLAYING TAG. (Collision noise is OK though.)
            if (collision.gameObject.CompareTag("Player") && gameManager.bPlayingTag)
            // And only if playing tag.
            {
                m_Renderer.material.color = Color.white;
                isTagged = true;
            }
        }   // isGameActive
    }


}

