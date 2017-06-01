using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileSynchronizationServerToLocal : MonoBehaviour 
{
	[HideInInspector] public List<DirectoryAndItsTxtFiles> directoriesAndThereTxtFiles = new List<DirectoryAndItsTxtFiles>();

	[Serializable]
	public class DirectoryAndItsTxtFiles
	{
		public string dirtectoryName;
		public string dirtectoryParentName;
		public List<TxtFile> childTxtFiles = new List<TxtFile>();
	}

	[Serializable]
	public class TxtFile
	{
		public string fileName;
		public string fileContent;
	}

	string[] localRootFoldersArray;
	public void FetchDirectoriesAndThereTxtFiles (string phpScriptUrl,string remoteDirToFetch,string localRootFolders) 
	{
		localRootFoldersArray = localRootFolders.Split (new string[1]{","},StringSplitOptions.RemoveEmptyEntries);
		#if !UNITY_EDITOR
		for (int i = 0; i < localRootFoldersArray.Length; i++) 
		{
			localRootFoldersArray[i] = Application.persistentDataPath + "/" + localRootFoldersArray[i];
		}
		#endif

		StartCoroutine (FetchDirectoriesAndThereTxtFilesCoroutine(phpScriptUrl,remoteDirToFetch));
	}

	IEnumerator FetchDirectoriesAndThereTxtFilesCoroutine (string phpScriptUrl,string remoteDirToFetch) 
	{
		if(Application.internetReachability == NetworkReachability.NotReachable){NotificationManager.instance.Alert("No Internet Connection"); yield break;}

		phpScriptUrl = phpScriptUrl + "?password=RajMehta&dirToFetch=" + remoteDirToFetch;
		phpScriptUrl = phpScriptUrl.Replace (" ","%20");
		//print (phpScriptUrl);
		WWW w = new WWW(phpScriptUrl);
		yield return w;
		if (w.error != null)
		{
			Debug.Log("Error .. " +w.error);
			yield break;
		}
		else
		{//print (w.text);
			string[] directoriesInfo = w.text.Split(new string[1]{" @@@@"},StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < directoriesInfo.Length; i++) 
			{
				if(directoriesInfo[i].IndexOf(" ~~~~") < 0){continue;}

				string[] directoryInfo = directoriesInfo[i].Split(new string[1]{" ~~~~"},StringSplitOptions.RemoveEmptyEntries);
				DirectoryAndItsTxtFiles directoryAndItsTxtFiles = new DirectoryAndItsTxtFiles();

				if(directoryInfo[0] == "_notes"){continue;}

				directoryInfo[1] = directoryInfo[1].Replace("Root/","");
				directoryAndItsTxtFiles.dirtectoryName = directoryInfo[0];
				directoryAndItsTxtFiles.dirtectoryParentName = directoryInfo[1];
				if(directoryInfo[2] != "NoTxtFilesInThisFolder")
				{
					string[] txtFilesInfo = directoryInfo[2].Split(new string[1]{" $$$$"},StringSplitOptions.RemoveEmptyEntries);
					for (int j = 0; j < txtFilesInfo.Length; j++) 
					{
						string[] txtFileInfo = txtFilesInfo[j].Split(new string[1]{" !!!!"},StringSplitOptions.RemoveEmptyEntries);
						TxtFile txtFile = new TxtFile();
						txtFileInfo[1] = txtFileInfo[1].Replace("Blank","");
						txtFile.fileName = txtFileInfo[0];
						txtFile.fileContent = txtFileInfo[1];
						directoryAndItsTxtFiles.childTxtFiles.Add(txtFile);
					}
				}

				directoriesAndThereTxtFiles.Add(directoryAndItsTxtFiles);
			}

			for (int i = 0; i < localRootFoldersArray.Length; i++) 
			{
				if(Directory.Exists(localRootFoldersArray[i])){Directory.Delete(localRootFoldersArray[i],true);}
			}

			for (int i = 0; i < directoriesAndThereTxtFiles.Count; i++) 
			{
				for (int j = 0; j < localRootFoldersArray.Length; j++) 
				{
					string directoryPath = localRootFoldersArray[j] + "/" + directoriesAndThereTxtFiles[i].dirtectoryParentName + "/" + directoriesAndThereTxtFiles[i].dirtectoryName;
					Directory.CreateDirectory(directoryPath);
					for (int k = 0; k < directoriesAndThereTxtFiles[i].childTxtFiles.Count; k++) 
					{
						File.WriteAllText(directoryPath + "/" + directoriesAndThereTxtFiles[i].childTxtFiles[k].fileName,directoriesAndThereTxtFiles[i].childTxtFiles[k].fileContent);
					}
				}
			}

		}

	}
}
