using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateme : MonoBehaviour {

    public Transform target;
    public int x;
    public int y;

	void Update ()
    {
        transform.localRotation = Quaternion.Euler(x,y, target.eulerAngles.y);
	}
}
