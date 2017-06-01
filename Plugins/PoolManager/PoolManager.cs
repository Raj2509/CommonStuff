using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour 
{
	public static PoolManager instance;


	[HideInInspector] public ObjectsPool[] AllPools;

	void Awake () 
	{
		instance = this;
		AllPools = GameObject.FindObjectsOfType <ObjectsPool>();
	}


	public Transform Spawn(string poolName,string poolObjectName)
	{
		for (int i = 0; i < AllPools.Length; i++) 
		{
			if(poolName == AllPools[i].poolName)
			{
				Transform poolObject = AllPools[i].GetPoolObjectInstance(poolObjectName);
				poolObject.gameObject.SetActive(true);
				return poolObject;
			}
		}

		print ("The Pool With The Name " + poolName + " Not Found");
		return null;
	}

	public void Despawn(string poolName,Transform poolObjectTransform)
	{
		poolObjectTransform.gameObject.SetActive (false);
		for (int i = 0; i < AllPools.Length; i++) 
		{
			if(poolName == AllPools[i].poolName)
			{
				AllPools[i].DespawnPoolObjectInstance(poolObjectTransform);
				return;
			}
		}

		print ("The Pool With The Name " + poolName + " Not Found");
	}

}
