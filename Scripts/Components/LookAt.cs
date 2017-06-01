using UnityEngine;
using System.Collections;

public class LookAt : MonoBehaviour 
{
	public Transform lookTowards;
	public Transform lookingObject;
	public bool lockXZ;

	void Update () 
	{
		lookingObject.LookAt(lookTowards);
		if(!lockXZ){lookingObject.localEulerAngles = new Vector3(0,lookingObject.localEulerAngles.y,0);}
	}
}
