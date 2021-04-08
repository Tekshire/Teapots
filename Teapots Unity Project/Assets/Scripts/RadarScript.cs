using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarScript : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject radarBlipPrefab;      // Filled in by drag and drop
    public GameObject[] radarBlips;         // Only 16 teapots / level, so radar only needs 16 blips.
    public Transform radarTransform;
    public GameObject player;
    public Transform playerTransform;

    // Off GameManager script
    //public GameObject[] teapots;

    public float radarCenterX;
    public float radarCenterY;
    public float radarCenterZ;
    public float radarScale;
    public float radarRadius;   // Max distance radar icons can be from center of radar,
                                // (Also allow room for all of icon to be within radar sphere.)
    // ToDo: After debugging radar move variables to function local (or elimanate if not needed)
    public float tpLocX;
    public float tpLocY;
    public float tpLocZ;
    public float playerX;
    public float playerY;
    public float playerZ;
    public float diffX;
    public float diffY;
    public float diffZ;
    public Vector3 normV;
    public float normX;
    public float normY;
    public float normZ;
    public float blipScale;     // Use to test what display looks like for various scale settings.
    public float blipScaleMin;
    public float blipScaleMax;


    // Start is called before the first frame update
    void Start()
    {
        radarRadius = 27.0f;
        // Could change scale to allow for different max radius per game level.
        radarScale = 35.0f;
        // Determined experimentally, blipScale should be .01 or .02 to .04 or .05.
        blipScaleMin = .01f;
        blipScaleMax = .05f;
        blipScale = .03f;


        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        // This script is attached to Radar, so this.Transform is Radar's transform
        radarTransform = this.transform;
        radarCenterX = radarTransform.position.x;
        radarCenterY = radarTransform.position.y;
        radarCenterZ = radarTransform.position.z;

        // Start New Level instantiates new teapots at each level.
        // For radar blips, we don't destroy and recreate, we only deactivate;
        // so only need to create at one time the whole game:
        radarBlips = new GameObject[16];

        for (int i = 0; i < radarBlips.Length; i++)
        {
            // This one assigns blips to world space.
            //GameObject blip = Instantiate(radarBlipPrefab, blipStartPos, Quaternion.identity);
            //GameObject blip = Instantiate(radarBlipPrefab, radarTransform);
            GameObject blip = Instantiate(radarBlipPrefab, radarTransform);
            radarBlips[i] = blip;
            radarBlips[i].SetActive(false);
        }
    }


    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */


    // After all of the gameplay has happened, move radar icons to show positions.
    void LateUpdate()
    {
        // ToDo: if radarTransform doesn't change, leave version in Start(). If not, leave this here.
        radarTransform = this.transform;
        radarCenterX = radarTransform.position.x;
        radarCenterY = radarTransform.position.y;
        radarCenterZ = radarTransform.position.z;

        //playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        // player initialized in inspector.
        playerTransform = player.GetComponent<Transform>();
        playerX = playerTransform.position.x;
        playerY = playerTransform.position.y;
        playerZ = playerTransform.position.z;


        for (int i = 0; i < radarBlips.Length; i++)
            {
                GameObject thisTeapot = gameManager.teapots[i];

            if (thisTeapot == null)
            {
                radarBlips[i].SetActive(false);
            }
            else
            {
                // All initial calculations based on real world scale, natural for playeer and teapots.
                // Later will scale down to fit within radar sphere.

                Vector3 teapotPos = thisTeapot.transform.position;
                tpLocX = teapotPos.x;
                tpLocY = teapotPos.y;
                tpLocZ = teapotPos.z;

                // What is the difference between teapot and player?
                Vector3 diffPos = teapotPos - playerTransform.position;
                float diffX0 = diffPos.x;
                float diffY0 = diffPos.y;
                float diffZ0 = diffPos.z;

                float blipMagnitude = diffPos.magnitude;
                normV = diffPos.normalized;

                // Just debugging that diffPos not changed by normalizing
                diffX = diffPos.x;
                diffY = diffPos.y;
                diffZ = diffPos.z;

                if (System.Math.Abs(blipMagnitude) > radarRadius)
                {
                    // Must scale back to what will fit within radar sphere
                    normV = diffPos.normalized;
                    normX = normV.x;        // For debugging only
                    normY = normV.y;        // For debugging only
                    normZ = normV.z;        // For debugging only

                    diffPos = normV * radarRadius;   

                    // Just debugging that diffPos not changed by normalizing
                    diffX = diffPos.x;
                    diffY = diffPos.y;
                    diffZ = diffPos.z;
                    normX = normV.x;
                    normY = normV.y;
                    normZ = normV.z;

                    // icon would be drawn outside of radar sphere, so scale back to fit
                    diffX = diffPos.x * radarRadius / blipMagnitude;
                    diffY = diffPos.y * radarRadius / blipMagnitude;
                    diffZ = diffPos.z * radarRadius / blipMagnitude;
                } else {
                    normX = 0;        // For debugging only
                    normY = 0;        // For debugging only
                    normZ = 0;        // For debugging only

                }
                // Compare manual setting of blip icon vs Normalized.
                // Would expect them to be the same size:

                //                radarRadius
                //                Vector3 direction = (Player.pos - transform.position).normalized:
                //float distance = (Player.pos - transform.position).magnitude;

                // Test, reduce x,y by factor to make sure all icons fit on screen.
                //Vector3 blipPos = new Vector3((tpLocX / radarScale) + radarCenterX,
                //                                (tpLocY / radarScale) + radarCenterY,
                //                                (tpLocZ / radarScale) + radarCenterZ);
                Vector3 blipPos = new Vector3((diffX / radarScale) + radarCenterX,
                                                (diffY / radarScale) + radarCenterY,
                                                (diffZ / radarScale) + radarCenterZ);
                ////   Vector3 blipPos = new Vector3((thisTeapot.transform.position.x / 4.0f) + playerTransform.position.x,
                ////                     CENTER_Y + playerTransform.position.y,
                ////                     (thisTeapot.transform.position.z / 4.0f) + playerTransform.position.z);
                radarBlips[i].transform.position = blipPos;

                // Scale blip icons to indicate distance from player.
                // Calculate after location in radar sphere has been determined
                // Determined experimentally, blipScale should be .01 or .02 to .04 or .05.

                //// First pass just load inspector version.
                //radarBlips[i].transform.localScale = new Vector3(blipScale, blipScale, blipScale);
                // Second pass is to calculate scale based on magnitude away from player.
                //   No concern for being outside of radar sphere.
                blipScale = (blipMagnitude > radarRadius) ?
                    blipScaleMin :
                    blipScaleMax - ((blipScaleMax - blipScaleMin) * (blipMagnitude / radarRadius));
                radarBlips[i].transform.localScale = new Vector3(blipScale, blipScale, blipScale);


                radarBlips[i].SetActive(true);

                // Rats! Each Debug.Log statement winds up a discreet output in the Unity console, so i can only copy
                // 1 line at a time to put into a full editor in order to more easily compare parts from different times.
                // Minimize hassle by combining seveeral lines of statements into single massive debug lines.
                Debug.Log(i + ": visable = " +radarBlips[i].activeSelf + "\n" +
                    "   Player Pos: x = " + playerX + "; y = " + playerY + "; z = " + playerZ + "\n" +
                    "   Teapot Pos: x = " + tpLocX + "; y = " + tpLocY + "; z = " + tpLocZ + "\n" +
                    "   DiffZero:  x = " + diffX0 + "; y = " + diffY0 + "; z = " + diffZ0 + "\n" +
                    "   Differenc: x = " + diffX + "; y = " + diffY + "; z = " + diffZ + "\n" +
                    "   Normalized: x = " + normX + "; y = " + normY + "; z = " + normZ + "\n" +
                    "   Radar Center: x = " + radarCenterX + "; y = " + radarCenterY + "; z = " + radarCenterZ + "\n" +
                    "   Radar Icon Pos: x = " + radarBlips[i].transform.position.x + "; y = " +
                        radarBlips[i].transform.position.y + "; z = " + radarBlips[i].transform.position.z + "\n" +
                    "   Magnitude = " + blipMagnitude + "; Scale = " + normY + "\n" +

                    "\n");

    }

            playerX = playerTransform.position.x;

        }

        Debug.Log("=============================");
    }
}
