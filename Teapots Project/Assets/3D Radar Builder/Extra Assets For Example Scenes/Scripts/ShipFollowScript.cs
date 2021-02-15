using UnityEngine;
using System.Collections;

public class ShipFollowScript : MonoBehaviour {
	// The target we are following
	public Transform target;
	public float distance = 3.0f;
	public float height = 10.0f;
	public float rotationDamping = 0.6f;
    public float damping = 5.0f;
	public bool smoothRotation = true;
	public float AdedHeight = 2;
	private Vector3 wantedPosition;
	private Quaternion wantedRotation;
	private GameObject playerShip;


	
	void Update ()
	{

		if(! playerShip)
		{
			try{
			playerShip = GameObject.FindWithTag ("PlayerShip");
			}
			catch
			{

			}

		}
		try{
				target = playerShip.transform ;    
		}
		catch
		{
			return;
				}




		if (target){
			
			
			/*if (Input.GetButtonUp ("Viewpoint")) {
                 if(distance == 2) {
					distance = Mathf.Lerp(2,6,Time.smoothDeltaTime* rigidbody.*1000);
				}
				else
				{
					distance = Mathf.Lerp(6,2,Time.smoothDeltaTime* rigidbody.mass*1000);
				}
			}*/
		
		}

		}
	
	void FixedUpdate ()
	{

		if (Time.timeScale == 0.3f ) {
			damping = 20.0f;	
			distance = 1;
		} 
		if(Time.timeScale == 1 && damping == 20) {
			damping = 6.0f;
			distance = 2;
		}

		
		if(target!=null){
			 wantedPosition = target.TransformPoint(0 , height  , -distance);
			transform.position = Vector3.Slerp (transform.position, wantedPosition, Time.deltaTime * damping/2);

			if (smoothRotation){
			    wantedRotation = Quaternion.LookRotation(target.position - transform.position, target.up);
				transform.rotation = Quaternion.Slerp (transform.rotation, wantedRotation, Time.deltaTime * rotationDamping);
			}
			else transform.LookAt (target, target.up);
		}
		

	}



	

}
