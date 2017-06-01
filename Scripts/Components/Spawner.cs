using UnityEngine;
using System.Collections;


public class Spawner : MonoBehaviour 
{
	public string poolName;
	public string objectName;
	public bool snapPosition;
	public bool snapRotation;


	public float[] initialDelay = new float[2]{0,0};
	public float[] interval = new float[2]{0,0};
	public UIEvents functionsToRunAfterSpawn;

	void OnEnable () 
	{
		StartCoroutine ("SpawnObjects");
	}

	public Transform objectInstance;
	float recordedTime;
	IEnumerator SpawnObjects () 
	{
		if(initialDelay.Length > 0){yield return new WaitForSeconds(Random.Range(initialDelay[0],initialDelay[1]));}
		while (true) 
		{
			recordedTime = Time.realtimeSinceStartup;

			objectInstance = PoolManager.instance.Spawn(poolName,objectName);
			if(snapPosition){objectInstance.position = transform.position;}
			if(snapRotation){objectInstance.rotation = transform.rotation;}

			functionsToRunAfterSpawn.Invoke(gameObject);

			float delay = Random.Range(interval[0],interval[1]);
			if(Time.timeScale == 1){yield return new WaitForSeconds(delay);}
			if(Time.timeScale != 1)
			{
				while(Time.realtimeSinceStartup - recordedTime < delay)
				{
					yield return null;
				}
			}
			yield return null;
		}	
	}
}
