using UnityEngine;
using System.Collections;

public class RayCasterVR : MonoBehaviour {

	public static RayCasterVR instance; 
	void Awake() {instance = this;}

	[InspectorButton("Invokewww")]
	public bool ttt;

	public float rayCastingSpeed;
	public LayerMask rayCastLayerMask;
	public Transform cursor;
	public tk2dSpriteAnimator animatedLoadingBar;

	public void StartRays()
	{
		cursor.gameObject.SetActive (true);
		StartCoroutine ("CastRays");
	}

	public void StopRays()
	{
		cursor.gameObject.SetActive (false);
		StopCoroutine ("CastRays");
	}

	void Start() {StartRays();}

	IEnumerator CastRays()
	{
		while (true) 
		{
			CastRay ();
			yield return new WaitForSeconds(rayCastingSpeed);
		}
	}

	private Ray ray;
	public static RaycastHit hitInfo;
	private RayRecieverVR RayRecieverVR;
	public static RayRecieverVR lastHitRayRecieverVR;
	public void CastRay()
	{
		ray = new Ray(cursor.position,cursor.forward);
		if(Physics.Raycast(ray,out hitInfo,50000,rayCastLayerMask))
		{
			//if(GM.instance.VR.showRayImpactPoint){GM.instance.VR.rayImpactPointCube.position = hitInfo.point;}
			if(RayRecieverVR == null)
			{
				RayRecieverVR = hitInfo.collider.transform.GetComponent<RayRecieverVR>();
				if(RayRecieverVR == null){RayRecieverVR = hitInfo.collider.transform.parent.GetComponent<RayRecieverVR>();}
				if(RayRecieverVR == null){RayRecieverVR = hitInfo.collider.transform.parent.parent.GetComponent<RayRecieverVR>();}
				if(RayRecieverVR != null){StartCoroutine("ExecuteFunctionAfterFocusCompletes");}
			}
			else
			{
				RayRecieverVR RayRecieverVR_ = hitInfo.collider.transform.GetComponent<RayRecieverVR>();
				if(RayRecieverVR_ == null){RayRecieverVR_ = hitInfo.collider.transform.parent.GetComponent<RayRecieverVR>();}
				if(RayRecieverVR_ == null){RayRecieverVR_ = hitInfo.collider.transform.parent.parent.GetComponent<RayRecieverVR>();}
				if(RayRecieverVR != RayRecieverVR_)
				{
					StopCoroutine("ExecuteFunctionAfterFocusCompletes");
					animatedLoadingBar.gameObject.SetActive(false);
					RayRecieverVR = null;
				}
			}
			Debug.DrawLine(ray.origin,hitInfo.point,Color.green,5);
		}
		else
		{
			if(RayRecieverVR != null)
			{
				StopCoroutine("ExecuteFunctionAfterFocusCompletes");
				animatedLoadingBar.gameObject.SetActive(false);
				RayRecieverVR = null;
			}
		}
	}

	public IEnumerator ExecuteFunctionAfterFocusCompletes()
	{
		lastHitRayRecieverVR = RayRecieverVR;
		if(RayRecieverVR.focusDuration > 0)
		{
			animatedLoadingBar.gameObject.SetActive(true);
			animatedLoadingBar.Stop ();
			animatedLoadingBar.Play ();
			animatedLoadingBar.ClipFps = animatedLoadingBar.ClipFps/RayRecieverVR.focusDuration;
			yield return new WaitForSeconds(RayRecieverVR.focusDuration);
			animatedLoadingBar.gameObject.SetActive(false);
		}

		RayRecieverVR.OnFocusComplete.Invoke(gameObject);
		RayRecieverVR = null;
	}
}
