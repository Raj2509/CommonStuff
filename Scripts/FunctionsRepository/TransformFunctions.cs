using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformFunctions : MonoBehaviour {

	public static TransformFunctions instance; 
	void Awake() {instance = this;}

	public void SetParent(Transform subject,Transform target,bool matchPosition = true,bool matchRotation = true)
	{
		subject.parent = target;
		if(target != null)
		{
			if(matchPosition){subject.position = target.position;}
			if(matchRotation){subject.eulerAngles = target.eulerAngles;}
		}
	}

	public void LookAt(Transform subject,Transform target){subject.LookAt(target);}

	public void Translate_(Transform subject,Vector3 amount){subject.Translate (amount.x,amount.y,amount.z);}

	public void RotateZ(Transform subject,float amount){subject.Rotate (0,0,amount);}
	
	public void SetLocalPosYIfDeceedLimit(Transform subject,float setTo,float limit){if(subject.localPosition.y < limit){subject.localPosition = new Vector3(subject.localPosition.x,setTo,subject.localPosition.z);}}
	public void SetLocalPosYIfExceedLimit(Transform subject,float setTo,float limit){if(subject.localPosition.y > limit){subject.localPosition = new Vector3(subject.localPosition.x,setTo,subject.localPosition.z);}}

	public void SetLocalPosXIfDeceedLimit(Transform subject,float setTo,float limit){if(subject.localPosition.x < limit){subject.localPosition = new Vector3(setTo,subject.localPosition.y,subject.localPosition.z);}}
	public void SetLocalPosition(Transform subject,Vector3 localPos){subject.localPosition = localPos;}
	public void SetLocalScale(Transform subject,Vector3 localScale){subject.localScale = localScale;}
	
	
	
	public void ActivateChilds(Transform parent){for (int i = 0; i < parent.childCount; i++) {parent.GetChild(i).gameObject.SetActive(true);}}
	public void ActivateChildIndex(Transform parent,int index){parent.GetChild(index).gameObject.SetActive(true);}
	
	public void DeactivateChilds(Transform parent){for (int i = 0; i < parent.childCount; i++) {parent.GetChild(i).gameObject.SetActive(false);}}
	public void DeactivateChildsActivateChild(Transform parent,GameObject childToActivate)
	{
		for (int i = 0; i < parent.childCount; i++) {parent.GetChild(i).gameObject.SetActive(false);}
		childToActivate.SetActive (true);
	}

	public void ActivateChildsWithDelay(Transform parent,float delay)
	{
		StartCoroutine (ActivateChildsWithDelayCoroutine(parent,delay));
	}

	IEnumerator ActivateChildsWithDelayCoroutine(Transform parent,float delay)
	{
		for (int i = 0; i < parent.childCount; i++) 
		{
			parent.GetChild(i).gameObject.SetActive(true);
			yield return new WaitForSeconds(delay);
		}
	}

	public void SetChildsLocalPositionBasedOnActive(Transform parent,Vector3 distanceBetweenChilds)
	{
		int activeChildsFound = 0;
		for (int i = 0; i < parent.childCount; i++) 
		{
			if(parent.GetChild(i).gameObject.activeInHierarchy)
			{
				parent.GetChild(i).localPosition = new Vector3(distanceBetweenChilds.x*activeChildsFound,distanceBetweenChilds.y*activeChildsFound,distanceBetweenChilds.z*activeChildsFound);
				activeChildsFound += 1;
			}
		}
	}


	public void MatchPosition(Transform subject,Transform target)
	{
		subject.position = target.position;
	}

	public void SetRandomLocalEulerAngle(Transform subject,Vector3 rangeOne,Vector3 rangeTwo)
	{
		subject.localEulerAngles = new Vector3 (Random.Range(rangeOne.x,rangeTwo.x),Random.Range(rangeOne.y,rangeTwo.y),Random.Range(rangeOne.z,rangeTwo.z));
	}
}
