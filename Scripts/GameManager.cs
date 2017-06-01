using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
	void Start () 
	{
		FileManager.instance.DirectoriesAndFilesCreatedEvent += DirectoriesAndFilesCreated;
		FileManager.instance.FolderOrFileCreatedEvent += FolderOrFileCreated;
	}

	void OnDisable () 
	{
		FileManager.instance.DirectoriesAndFilesCreatedEvent -= DirectoriesAndFilesCreated;
		FileManager.instance.FolderOrFileCreatedEvent -= FolderOrFileCreated;
	}



	void Update () 
	{
		
	}

	void DirectoriesAndFilesCreated (string currentDir) 
	{
		string baseDirectory = "Assets";
		#if !UNITY_EDITOR
		baseDirectory = Application.persistentDataPath + "/" + baseDirectory;
		#endif

		if(currentDir == baseDirectory)
		{
			FileManager.instance.CreateDirectoriesAndFiles(baseDirectory + "/Database");
			NotificationManager.instance.Conform("ARE YOU SURE YOU WANT TO EXIT THE APP","GameExit");
		}
	}

	void FolderOrFileCreated (RefrencesAndValues refrencesAndValues) 
	{
		string remoteBackup = "Assets/Database/Remote";
		#if !UNITY_EDITOR
		remoteBackup = Application.persistentDataPath + "/" + remoteBackup;
		#endif

		if(refrencesAndValues.GetBool("IsFolder"))
		{
			if(FileManager.instance.activePath.IndexOf(remoteBackup) > -1)
			{
				refrencesAndValues.GetSprite("Icon").SetSprite("Folder");
			}
		}
	}
}
