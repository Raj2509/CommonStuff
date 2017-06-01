using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectsPool : MonoBehaviour 
{
	public string poolName;

	[Space]

	[InspectorButtonAttribute("UpdatePoolObjectsLabel")]
	public bool updatePoolObjectsLabel;

	public poolObject[] poolObjects;



	[Serializable]
	public class poolObject
	{
		public string label;
		public Transform poolObjectTransform;
		public int noOfInstances;
		[HideInInspector] public List<Transform> poolObjectInstances = new List<Transform> ();
	}


	void Awake () 
	{
		MakePoolObjectInstances ();
	}


	public Transform GetPoolObjectInstance (string poolObjectName) 
	{
		for (int i = 0; i < poolObjects.Length; i++) 
		{
			if(poolObjectName == poolObjects[i].poolObjectTransform.name)
			{
				for (int j = 0; j < poolObjects[i].poolObjectInstances.Count; j++) 
				{
					if(!poolObjects[i].poolObjectInstances[j].gameObject.activeSelf)
					{
						return poolObjects[i].poolObjectInstances[j];
					}
				}

				return MakePoolObjectInstance(poolObjects[i].poolObjectTransform,poolObjects[i].poolObjectInstances);
			}
		}

		print ("The poolObject With The Name " + poolObjectName + " Not Found In The Pool " + poolName);
		return null;
	}

	public void DespawnPoolObjectInstance (Transform poolObjectTransform) 
	{
		for (int i = 0; i < poolObjects.Length; i++) 
		{
			for (int j = 0; j < poolObjects[i].poolObjectInstances.Count; j++) 
			{
				if(poolObjectTransform == poolObjects[i].poolObjectInstances[j])
				{
					poolObjectTransform.gameObject.SetActive(false);
					poolObjectTransform.parent = transform;
					return;
				}
			}
		}
		
		print ("The poolObject With The Name " + poolObjectTransform.name + " Not Found In The Pool " + poolName);
	}


	void MakePoolObjectInstances () 
	{
		for (int i = 0; i < poolObjects.Length; i++) 
		{
			if(poolObjects[i].noOfInstances == 0){poolObjects[i].noOfInstances = 1;}
			for (int j = 0; j < poolObjects[i].noOfInstances; j++) 
			{
				MakePoolObjectInstance(poolObjects[i].poolObjectTransform,poolObjects[i].poolObjectInstances);
			}
		}
	}

	Transform MakePoolObjectInstance (Transform poolObjectTransform,List<Transform> poolObjectInstancesList) 
	{
		Transform instance = Instantiate (poolObjectTransform,transform);
		instance.gameObject.SetActive (false);
		poolObjectInstancesList.Add (instance);
		return instance;
	}

	void UpdatePoolObjectsLabel () 
	{
		for (int i = 0; i < poolObjects.Length; i++) 
		{
			if(poolObjects[i].poolObjectTransform != null)
			{
				poolObjects[i].label = poolObjects[i].poolObjectTransform.name;
			}
		}
	}
}
