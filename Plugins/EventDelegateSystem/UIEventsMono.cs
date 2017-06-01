using System;
using UnityEngine;
using System.Collections;
using UIEventDelegate;


[Serializable]
public class UIEvents
{
	public UIEvent[] events; 
	
	[Serializable]
	public class UIEvent
	{
		public string label;
		public bool disable;
		public EventDelegate ev; 
		public float delay;
		public bool delayByFrame;
		public float[] randomDelay;
		public bool invokeOnlyIfActive = true;	
	}
	
	
	//54645645364536breakdfghdfg
	
	public void Invoke(GameObject reqActiveGameObject)
	{
		for (int i = 0; i < events.Length; i++) 
		{
			if (events[i].ev != null && !events[i].disable) 
			{
				if (events[i].delay == 0)
				{
					events[i].ev.Execute ();
				}
				else 
				{    
					//Creates an instance of UnityEventWithAttributes(monobehaviour) if its static instance is null/never instantiated
					//otherwise starts the coroutine at previously created instance
					//also passes the gameobject that is required to be active before invoking the event
					if (UIEventsMono.instance == null) 
					{
						UIEventsMono.instance = new GameObject ("UIEventsMonoInstance", new Type[]{ typeof(UIEventsMono) }).GetComponent<UIEventsMono> ();
						UIEventsMono.instance.StartCoroutine (UIEventsMono.instance.InvokeUIEventWithDelay (events[i].ev,events[i].delay,events[i].randomDelay,events[i].delayByFrame,reqActiveGameObject,events[i].invokeOnlyIfActive));
					} 
					else UIEventsMono.instance.StartCoroutine (UIEventsMono.instance.InvokeUIEventWithDelay (events[i].ev,events[i].delay,events[i].randomDelay,events[i].delayByFrame,reqActiveGameObject,events[i].invokeOnlyIfActive));
				}
			}
		}
	} 
}



public class UIEventsMono : MonoBehaviour
{
	public static UIEventsMono instance;
	
	void OnEnable(){
		instance = this;
	}
	void OnDisable(){
		instance = null;
	}
	
	public IEnumerator invokeUIEventWithDelay(EventDelegate _ev, float _delay, GameObject _reqAlive = null){
		yield return new WaitForSeconds (_delay);
		if (_reqAlive!=null) if (!_reqAlive.activeInHierarchy) yield break;
		_ev.Execute();
	}
	
	public IEnumerator InvokeUIEventWithDelay(EventDelegate eventDelegate, float delay, float[] randomDelay, bool delayByFrame ,GameObject reqGO, bool invokeOnlyIfActive)
	{
		if (randomDelay.Length == 0) 
		{
			if(delay > 0)
			{
				if(Time.timeScale == 1)
				{
					yield return new WaitForSeconds (delay * Time.timeScale);
				}
				else
				{
					float recordedTime = Time.realtimeSinceStartup;
					while (Time.realtimeSinceStartup - recordedTime < delay) {yield return null;}
				}
			}
		}
		else
		{
			if(Time.timeScale == 1)
			{
				yield return new WaitForSeconds (UnityEngine.Random.Range(randomDelay[0],randomDelay[1]) * Time.timeScale);
			}
			else
			{
				float recordedTime = Time.realtimeSinceStartup;
				while (Time.realtimeSinceStartup - recordedTime < UnityEngine.Random.Range(randomDelay[0],randomDelay[1])) {yield return null;}
			}
		}

		if (delayByFrame) {yield return new WaitForEndOfFrame();}
		if (invokeOnlyIfActive) 
		{
			if (!reqGO.activeInHierarchy) yield break;
		}
		eventDelegate.Execute();
	}
}