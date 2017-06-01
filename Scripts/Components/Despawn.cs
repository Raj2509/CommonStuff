using UnityEngine;
using System.Collections;


public class Despawn : MonoBehaviour 
{
	public float despawnTime;
	public string poolName;

	public Transform selfTransform;

	public UIEvents functionsToRunAfterDespawn;
	public UIEvents functionsToRunBeforeDespawn;

	void Awake()
	{
		if(selfTransform == null){selfTransform = transform;}
	}

	void OnEnable()
	{
		StartSelfDespawn ();
	}

	public void StartSelfDespawn()
	{
		StartCoroutine ("SelfDespawn");
	}

	public void StopSelfDespawn()
	{
		StopCoroutine ("SelfDespawn");
	}

	float recordedTime;
	IEnumerator SelfDespawn()
	{
		recordedTime = Time.realtimeSinceStartup;
		/*if(Time.timeScale == 1){*/yield return new WaitForSeconds (despawnTime);//}
		//if(Time.timeScale != 1){while(Time.realtimeSinceStartup - recordedTime < despawnTime){yield return null;}}
		if(gameObject.activeInHierarchy)
		{
			functionsToRunBeforeDespawn.Invoke (gameObject);
			PoolManager.instance.Despawn(poolName,selfTransform);
			functionsToRunAfterDespawn.Invoke (gameObject);
		}
	}

}
