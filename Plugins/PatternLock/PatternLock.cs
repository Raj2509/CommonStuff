using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternLock : MonoBehaviour {

	public Camera raycastCam;
	public LayerMask layerMask;

	public Transform line;
	public Transform helperTransform;
	public tk2dTextMesh heading;

	private RaycastHit hit;
	private Ray ray;
	private Touch touch;
	private bool mouseHeldDown;
	

	void OnEnable () 
	{
		heading.text = "ENTER PATTERN";
	}


	float xResolutionFactor;
	float yResolutionFactor;
	void Start () 
	{
		if(!PlayerPrefs.HasKey("SavedPatternString")){PlayerPrefs.SetString("SavedPatternString","123654789");}

		if(Screen.width > Screen.height)
		{
			xResolutionFactor = 40.0f*(Screen.width/800.0f);
			yResolutionFactor = 40.0f*(Screen.height/480.0f);
		}
		else
		{
			xResolutionFactor = 40*(Screen.width/480.0f);
			yResolutionFactor = 40*(Screen.height/800.0f);
		}
	}





	void Update () 
	{
		if(Input.GetKey("escape")){Application.Quit();}
		if(Input.touchCount > 0)
		{
			touch = Input.touches[0];
			if (touch.phase == TouchPhase.Began){CastRay(touch.position);}
			if (touch.phase == TouchPhase.Moved){FingerMoved(touch.position);}
			if (touch.phase == TouchPhase.Ended){FingerLifted(touch.position);}
		}
		
		if(Input.mousePresent)
		{
			if(Input.GetMouseButtonDown(0)) {CastRay(Input.mousePosition); mouseHeldDown = true;}
			if(mouseHeldDown)               {FingerMoved(Input.mousePosition);}
			if(Input.GetMouseButtonUp(0))   {FingerLifted(Input.mousePosition); mouseHeldDown = false;}
		}
	}


	public void CastRay(Vector3 fingerPosition) 
	{
		ray = raycastCam.ScreenPointToRay(fingerPosition);
		if(Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask))
		{
			FingerTouchedCollider(fingerPosition,hit.transform);
			Debug.DrawLine(ray.origin, hit.point,Color.red,5);
		}
	}


	Vector3 fingerRecordedPosition;
	bool fingerTouchedCollider;
	List<Transform> lineClones = new List<Transform> ();
	List<Transform> connectedPoints = new List<Transform> ();
	string patternString;
	private void FingerTouchedCollider(Vector3 fingerPosition,Transform collidedTransform) 
	{
		if(connectedPoints.Count > 0)
		{ 
			if(collidedTransform == connectedPoints[connectedPoints.Count - 1]){return;}
		}

		if(connectedPoints.Count > 1)
		{
			if(collidedTransform == connectedPoints[connectedPoints.Count - 2])
			{
				connectedPoints.Remove(connectedPoints[connectedPoints.Count - 1]);
				GameObject lineClone_ = lineClones[lineClones.Count - 1].gameObject;
				lineClones.Remove(lineClones[lineClones.Count - 1]);
				Destroy(lineClone_);
				return;
			}
		}

		if(!connectedPoints.Contains(collidedTransform))
		{ 
			if(lineClones.Count == 0)
			{
				helperTransform.position = collidedTransform.position;
				helperTransformRecordedPosition = helperTransform.localPosition;
				fingerRecordedPosition = fingerPosition;
				fingerTouchedCollider = true;
			}

			Transform lineClone = Instantiate(line) as Transform;
			lineClone.gameObject.SetActive(true);
			lineClone.position = collidedTransform.position;
			lineClone.parent = transform;
			lineClone.LookAt(helperTransform);
			lineClone.localScale = new Vector3(1,1,Vector3.Distance(helperTransform.position,collidedTransform.position));
			lineClones.Add(lineClone);
			connectedPoints.Add (collidedTransform);
			patternString += collidedTransform.name;

			if(lineClones.Count > 1)
			{ 
				lineClones[lineClones.Count - 2].LookAt(connectedPoints[connectedPoints.Count - 1]); 
				lineClones[lineClones.Count - 2].localScale = new Vector3(1,1,Vector3.Distance(connectedPoints[connectedPoints.Count - 2].position,connectedPoints[connectedPoints.Count - 1].position));
			}


		}
	}

	Vector3 helperTransformRecordedPosition;
	private void FingerMoved(Vector3 fingerPosition) 
	{
		if(fingerTouchedCollider)
		{
			float tempX = helperTransformRecordedPosition.x + ((fingerPosition.x - fingerRecordedPosition.x)/xResolutionFactor);
			float tempY = helperTransformRecordedPosition.y + ((fingerPosition.y - fingerRecordedPosition.y)/yResolutionFactor);
			helperTransform.localPosition = new Vector3(tempX,tempY,helperTransform.localPosition.z);

			lineClones[lineClones.Count - 1].LookAt(helperTransform);
			lineClones[lineClones.Count - 1].localScale = new Vector3(1,1,Vector3.Distance(lineClones[lineClones.Count - 1].position,helperTransform.position));

			CastRay(fingerPosition);
		}
	}

	private void FingerLifted(Vector3 fingerPosition) 
	{
		if(fingerTouchedCollider)
		{
			PatternMade ();

			while (lineClones.Count > 0) 
			{
				GameObject lineClone = lineClones[0].gameObject;
				lineClones.Remove(lineClones[0]);
				Destroy(lineClone);
			}

			while (connectedPoints.Count > 0) 
			{
				connectedPoints.Remove(connectedPoints[0]);
			}

			fingerTouchedCollider = false;
			patternString = "";
		}
	}

	void PatternMade () 
	{
		if (connectedPoints.Count == 1) {return;}

		if(heading.text == "ENTER ORIGINAL PATTERN")
		{
			if(patternString == PlayerPrefs.GetString("SavedPatternString"))
			{
				heading.text = "ENTER NEW PATTERN";
			}
			else{NotificationManager.instance.Alert("WRONG PATTERN");}
			return;
		}
		
		if(heading.text == "ENTER NEW PATTERN")
		{
			if(connectedPoints.Count < 3)
			{
				NotificationManager.instance.Alert("PATTERN SHOULD BE ATLEAST 3 DOTS LONG");
			}
			else
			{
				PlayerPrefs.SetString("SavedPatternString",patternString);
				heading.text = "ENTER PATTERN";
			}
			return;
		}
		
		if(heading.text == "ENTER PATTERN")
		{
			if(patternString == PlayerPrefs.GetString("SavedPatternString"))
			{
				gameObject.SetActive(false);
				PlayerPrefs.SetString("PatternLockActive","False");
			}
			else
			{
				NotificationManager.instance.Alert("WRONG PATTERN");
			}
			return;
		}
		
	}

	public void ChangePattern () 
	{
		heading.text = "ENTER ORIGINAL PATTERN";
	}


}
