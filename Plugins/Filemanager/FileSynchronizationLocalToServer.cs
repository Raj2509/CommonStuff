 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class FileSynchronizationLocalToServer : MonoBehaviour 
{

	public void SynchronizeFiles (string dirToSyncPath,string dirToCompareFromPath,string filesExtensionsToSynchronize,string phpScriptUrl,string onlineDirToSyncPath) 
	{
		if(Application.internetReachability == NetworkReachability.NotReachable){NotificationManager.instance.Alert("No Internet Connection"); return;}
		#if !UNITY_EDITOR
		dirToSyncPath = Application.persistentDataPath + "/" + dirToSyncPath;
		dirToCompareFromPath = Application.persistentDataPath + "/" + dirToCompareFromPath;
		#endif
		if(!Directory.Exists(dirToSyncPath)){NotificationManager.instance.Alert("The Directory Path You Are Trying To Sync Does Not Exists."); return;}
		if(!Directory.Exists(dirToCompareFromPath)){NotificationManager.instance.Alert("The Directory Path You Are Trying To Compare From Does Not Exists."); return;}
		StartCoroutine (SynchronizeFilesCoroutine(dirToSyncPath,dirToCompareFromPath,filesExtensionsToSynchronize,phpScriptUrl,onlineDirToSyncPath));
	}

	string[] filesExtensionsToSynchronizeArray;
	IEnumerator SynchronizeFilesCoroutine (string dirToSyncPath,string dirToCompareFromPath,string filesExtensionsToSynchronize,string phpScriptUrl,string onlineDirToSyncPath) 
	{
		filesExtensionsToSynchronizeArray = filesExtensionsToSynchronize.Split (new string[1]{","},System.StringSplitOptions.RemoveEmptyEntries);

		GetNewAndDeletedFilesAndFolders (dirToSyncPath,dirToCompareFromPath);

		WWWForm form = new WWWForm();
		form.AddField("password", "RajMehta");
		form.AddField("dirToSyncPath", onlineDirToSyncPath);
		form.AddField("allNewFolders", allNewFolders);
		form.AddField("allDeletedFolders", allDeletedFolders);
		form.AddField("allNewFiles", allNewFiles);
		form.AddField("allDeletedFiles", allDeletedFiles);
		form.AddField("allEditedFiles", allEditedFiles);

		WWW w = new WWW(phpScriptUrl,form);
		yield return w;
		//print (w.text);
		if (w.error != null)
		{
			Debug.Log("Error .. " +w.error);
			yield break;
		}
		else
		{
			string[] allNewFoldersArray = allNewFolders.Split(new string[1]{"!"},System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < allNewFoldersArray.Length; i++) 
			{
				Directory.CreateDirectory(dirToCompareFromPath + "/" + allNewFoldersArray[i]);
			}

			string[] allDeletedFoldersArray = allDeletedFolders.Split(new string[1]{"!"},System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < allDeletedFoldersArray.Length; i++) 
			{
				Directory.Delete(dirToCompareFromPath + "/" + allDeletedFoldersArray[i],true);
			}

			string[] allNewFilesArray = allNewFiles.Split(new string[1]{"!"},System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < allNewFilesArray.Length; i++) 
			{
				string[] fileInfo = allNewFilesArray[i].Split(new string[1]{"@"},System.StringSplitOptions.RemoveEmptyEntries);
				File.WriteAllText(dirToCompareFromPath + "/" + fileInfo[0],fileInfo[1]);
			}

			string[] allDeletedFilesArray = allDeletedFiles.Split(new string[1]{"!"},System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < allDeletedFilesArray.Length; i++) 
			{
				File.Delete(dirToCompareFromPath + "/" + allDeletedFilesArray[i]);
			}

			string[] allEditedFilesArray = allEditedFiles.Split(new string[1]{"!"},System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < allEditedFilesArray.Length; i++) 
			{
				string[] fileInfo = allEditedFilesArray[i].Split(new string[1]{"@"},System.StringSplitOptions.RemoveEmptyEntries);
				File.WriteAllText(dirToCompareFromPath + "/" + fileInfo[0],fileInfo[1]);
			}
		}
	}


	void GetNewAndDeletedFilesAndFolders (string dirToSyncPath,string dirToCompareFromPath) 
	{
		GetNewFolders (dirToSyncPath,dirToCompareFromPath);
		GetDeletedFolders (dirToSyncPath,dirToCompareFromPath);
		GetNewFiles (dirToSyncPath,dirToCompareFromPath);
		GetDeletedFiles (dirToSyncPath,dirToCompareFromPath);
		GetEditedFiles (dirToSyncPath,dirToCompareFromPath);

		while (allNewFolders.IndexOf(dirToSyncPath) > -1) {allNewFolders = allNewFolders.Replace(dirToSyncPath + "/","");}
		while (allDeletedFolders.IndexOf(dirToSyncPath) > -1) {allDeletedFolders = allDeletedFolders.Replace(dirToSyncPath + "/","");}
		while (allNewFiles.IndexOf(dirToSyncPath) > -1) {allNewFiles = allNewFiles.Replace(dirToSyncPath + "/","");}
		while (allDeletedFiles.IndexOf(dirToSyncPath) > -1) {allDeletedFiles = allDeletedFiles.Replace(dirToSyncPath + "/","");}
		while (allEditedFiles.IndexOf(dirToSyncPath) > -1) {allEditedFiles = allEditedFiles.Replace(dirToSyncPath + "/","");}

		print (allNewFolders);
		//print (allDeletedFolders);
		//print (allNewFiles);
		//print (allDeletedFiles);
		//print (allEditedFiles);
	}



	[HideInInspector] public string allNewFolders;
	void GetNewFolders (string dirToSyncPath,string dirToCompareFromPath) 
	{
		string[] allDirectories = Directory.GetDirectories (dirToSyncPath);

		for (int i = 0; i < allDirectories.Length; i++) 
		{
			if(!Directory.Exists(dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i])))
			{
				allNewFolders += dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]) + "!";
			}
			GetNewFolders(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]),dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]));
		}
	}

	[HideInInspector] public string allDeletedFolders;
	void GetDeletedFolders (string dirToSyncPath,string dirToCompareFromPath) 
	{
		string[] allDirectories = Directory.GetDirectories (dirToCompareFromPath);
		
		for (int i = 0; i < allDirectories.Length; i++) 
		{
			if(!Directory.Exists(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i])))
			{
				allDeletedFolders += dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]) + "!";
			}
			else
			{
				GetDeletedFolders(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]),dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]));
			}
		}
	}

	[HideInInspector] public string allNewFiles;
	void GetNewFiles (string dirToSyncPath,string dirToCompareFromPath) 
	{
		string[] allFiles = Directory.GetFiles (dirToSyncPath);
		for (int i = 0; i < allFiles.Length; i++) 
		{
			if(!IsFileExtensionValid(Path.GetExtension(allFiles[i]))){continue;}

			if(!File.Exists(dirToCompareFromPath + "/" + Path.GetFileName(allFiles[i])))
			{
				allNewFiles += dirToSyncPath + "/" + Path.GetFileName(allFiles[i]) + "@" + File.ReadAllText(dirToSyncPath + "/" + Path.GetFileName(allFiles[i])) + "@!";
			}
		}
		string[] allDirectories = Directory.GetDirectories (dirToSyncPath);
		for (int i = 0; i < allDirectories.Length; i++) 
		{
			GetNewFiles(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]),dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]));
		}
	}

	[HideInInspector] public string allDeletedFiles;
	void GetDeletedFiles (string dirToSyncPath,string dirToCompareFromPath) 
	{
		string[] allFiles = Directory.GetFiles (dirToCompareFromPath);
		for (int i = 0; i < allFiles.Length; i++) 
		{
			if(!IsFileExtensionValid(Path.GetExtension(allFiles[i]))){continue;}
			
			if(!File.Exists(dirToSyncPath + "/" + Path.GetFileName(allFiles[i])))
			{
				allDeletedFiles += dirToSyncPath + "/" + Path.GetFileName(allFiles[i]) + "!";
			}
		}
		string[] allDirectories = Directory.GetDirectories (dirToCompareFromPath);
		for (int i = 0; i < allDirectories.Length; i++) 
		{
			if(!Directory.Exists(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]))){continue;}
			GetDeletedFiles(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]),dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]));
		}
	}

	[HideInInspector] public string allEditedFiles;
	void GetEditedFiles (string dirToSyncPath,string dirToCompareFromPath) 
	{
		string[] allFiles = Directory.GetFiles (dirToSyncPath);
		for (int i = 0; i < allFiles.Length; i++) 
		{
			if(!IsFileExtensionValid(Path.GetExtension(allFiles[i]))){continue;}
			
			if(File.Exists(dirToCompareFromPath + "/" + Path.GetFileName(allFiles[i])))
			{
				if(File.ReadAllText(dirToSyncPath + "/" + Path.GetFileName(allFiles[i])) != File.ReadAllText(dirToCompareFromPath + "/" + Path.GetFileName(allFiles[i])))
				{
					allEditedFiles = dirToSyncPath + "/" + Path.GetFileName(allFiles[i]) + "@" + File.ReadAllText(dirToSyncPath + "/" + Path.GetFileName(allFiles[i])) + "@!";
				}
			}
		}
		string[] allDirectories = Directory.GetDirectories (dirToSyncPath);
		for (int i = 0; i < allDirectories.Length; i++) 
		{
			if(!Directory.Exists(dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]))){continue;}
			GetEditedFiles(dirToSyncPath + "/" + Path.GetFileName(allDirectories[i]),dirToCompareFromPath + "/" + Path.GetFileName(allDirectories[i]));
		}
	}


	bool IsFileExtensionValid (string fileExtension) 
	{
		for (int j = 0; j < filesExtensionsToSynchronizeArray.Length; j++) 
		{
			if(fileExtension == filesExtensionsToSynchronizeArray[j]){return true;}
		}
		return false;
	}
}
