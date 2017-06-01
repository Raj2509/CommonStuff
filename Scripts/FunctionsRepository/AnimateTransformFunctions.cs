using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AnimateTransformFunctions : MonoBehaviour 
{
	public static AnimateTransformFunctions ins; 
	void Awake() {ins = this;}

	public AnimEasingCurve[] animEasingCurves;

	[Serializable]
	public class AnimEasingCurve
	{
		public string animEasingCurveName;
		public AnimationCurve animEasingCurve;
	}

	[Serializable]
	public class AnimatingTransformCoroutine
	{
		public Transform animatingTransform;
		public Coroutine coroutine;
	}

	public void Translate(Transform transformToAnimate,Vector3 amount,float time,string easing,bool animateIfAlreadyAnimating = false)
	{
		AnimateTransform(transformToAnimate,transformToAnimate.localPosition,transformToAnimate.localPosition + amount,time,"Position",easing,animateIfAlreadyAnimating);
	}


	public List<AnimatingTransformCoroutine> animatingTransformCoroutines = new List<AnimatingTransformCoroutine>();
	public void AnimateTransform(Transform transformToAnimate,Vector3 start,Vector3 destination,float time,string whichTransform,string easing,bool animateIfAlreadyAnimating = false)
	{
		AnimatingTransformCoroutine animatingTransformCoroutine = null;
		for (int i = 0; i < animatingTransformCoroutines.Count; i++) 
		{
			if(animatingTransformCoroutines[i].animatingTransform != null)
			{
				if(animatingTransformCoroutines[i].animatingTransform == transformToAnimate)
				{
					animatingTransformCoroutine = animatingTransformCoroutines[i];
				}
			}
			else{animatingTransformCoroutines.Remove(animatingTransformCoroutines[i]);}
		}

		if(animatingTransformCoroutine != null)
		{
			if(animateIfAlreadyAnimating)
			{
				StopCoroutine(animatingTransformCoroutine.coroutine);
				animatingTransformCoroutines.Remove(animatingTransformCoroutine);

				AddAnimatingTransformToAnimatingTransformCoroutinesList(transformToAnimate,start,destination,time,whichTransform,easing);
			}
		}
		else
		{
			AddAnimatingTransformToAnimatingTransformCoroutinesList(transformToAnimate,start,destination,time,whichTransform,easing);
		}
	}

	void AddAnimatingTransformToAnimatingTransformCoroutinesList(Transform transformToAnimate,Vector3 start,Vector3 destination,float time,string whichTransform,string easing)
	{
		AnimatingTransformCoroutine animatingTransformCoroutine = new AnimatingTransformCoroutine ();
		animatingTransformCoroutine.animatingTransform = transformToAnimate;
		animatingTransformCoroutine.coroutine = StartCoroutine(AnimateTransformCoroutine(transformToAnimate,start,destination,time,whichTransform,easing));
		animatingTransformCoroutines.Add (animatingTransformCoroutine);
	}


	IEnumerator AnimateTransformCoroutine(Transform transformToAnimate,Vector3 start,Vector3 destination,float time,string whichTransform,string easing)
	{
		float animValue;
		float evaluateValue = 0;
		bool position = false;
		bool rotation = false;
		bool scale = false;
		float value_x = 0;
		float value_y = 0;
		float value_z = 0;
		float amount_x = 0;
		float amount_y = 0;
		float amount_z = 0;
		float recordedTime = 0;
		Vector3 tempVector_3 = Vector3.zero;
		bool snapAllAxis = true;
		AnimationCurve activeAnimCurve = null;

		for (int i = 0; i < animEasingCurves.Length; i++) 
		{
			if(easing == animEasingCurves[i].animEasingCurveName)
			{
				activeAnimCurve = animEasingCurves[i].animEasingCurve;
				break;
			}
		}

		amount_x = destination.x - start.x;
		amount_y = destination.y - start.y;
		amount_z = destination.z - start.z;
		
		if(whichTransform == "Position") {position = true; rotation = false; scale = false; tempVector_3 = transformToAnimate.localPosition;}
		if(whichTransform == "Rotation") {position = false; rotation = true; scale = false; tempVector_3 = transformToAnimate.localEulerAngles;}
		if(whichTransform == "Scale")    {position = false; rotation = false; scale = true; tempVector_3 = transformToAnimate.localScale;}
		
		if(snapAllAxis)
		{
			value_x = start.x;
			value_y = start.y;			
			value_z = start.z;
		}
		else
		{
			if(amount_x == 0){value_x = transformToAnimate.localPosition.x;}else{value_x = start.x;}
			if(amount_y == 0){value_y = transformToAnimate.localPosition.y;}else{value_y = start.y;}
			if(amount_z == 0){value_z = transformToAnimate.localPosition.z;}else{value_z = start.z;}
		}
		
		if(position) {transformToAnimate.localPosition =    new Vector3(value_x,value_y,value_z);}
		if(rotation) {transformToAnimate.localEulerAngles = new Vector3(value_x,value_y,value_z);}
		if(scale) 	 {transformToAnimate.localScale =       new Vector3(value_x,value_y,value_z);}
		
		recordedTime = Time.realtimeSinceStartup;
		if(time > 0)
		{
			evaluateValue = 0;
			while(evaluateValue < 1)
			{
				evaluateValue += 1/time * Time.deltaTime;
				if(Time.timeScale != 1){evaluateValue = (Time.realtimeSinceStartup - recordedTime) * 1/time;}
				//evaluateValue = (Time.realtimeSinceStartup - recordedTime) * 1/time;

				if(evaluateValue > 1){evaluateValue = 1;}
				animValue = activeAnimCurve.Evaluate(evaluateValue);

				if(position) {tempVector_3 = transformToAnimate.localPosition;}
				if(rotation) {tempVector_3 = transformToAnimate.localEulerAngles;}
				if(scale)    {tempVector_3 = transformToAnimate.localScale;}
				
				if(amount_x == 0){value_x = tempVector_3.x;}
				if(amount_y == 0){value_y = tempVector_3.y;}
				if(amount_z == 0){value_z = tempVector_3.z;}
				
				if(position) {transformToAnimate.localPosition =    new Vector3(value_x + amount_x*animValue,value_y + amount_y*animValue,value_z + amount_z*animValue);}
				if(rotation) {transformToAnimate.localEulerAngles = new Vector3(value_x + amount_x*animValue,value_y + amount_y*animValue,value_z + amount_z*animValue);}
				if(scale)    {transformToAnimate.localScale =       new Vector3(value_x + amount_x*animValue,value_y + amount_y*animValue,value_z + amount_z*animValue);}
				yield return null;
			}
		}
		else
		{
			if(snapAllAxis)
			{
				value_x = destination.x;
				value_y = destination.y;
				value_z = destination.z;
			}
			else
			{
				if(amount_x == 0){value_x = transformToAnimate.localPosition.x;}else{value_x = destination.x;}
				if(amount_y == 0){value_y = transformToAnimate.localPosition.y;}else{value_y = destination.y;}
				if(amount_z == 0){value_z = transformToAnimate.localPosition.z;}else{value_z = destination.z;}
			}
			
			if(position) {transformToAnimate.localPosition =    new Vector3(value_x,value_y,value_z);}
			if(rotation) {transformToAnimate.localEulerAngles = new Vector3(value_x,value_y,value_z);}
			if(scale) 	 {transformToAnimate.localScale =       new Vector3(value_x,value_y,value_z);}
		}


		for (int i = 0; i < animatingTransformCoroutines.Count; i++) 
		{
			if(animatingTransformCoroutines[i].animatingTransform == transformToAnimate)
			{
				animatingTransformCoroutines.Remove(animatingTransformCoroutines[i]);
			}
		}
	}


}
