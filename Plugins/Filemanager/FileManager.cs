using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FileManager : MonoBehaviour 
{
	public tk2dTextMesh tt;
	public static FileManager instance; 
	void Awake() {instance = this; tt.text = Application.persistentDataPath;}


	public string basePath;
	public Transform filesAndFoldersParent;
	public GameObject optionsMenu;
	public Transform addressBar;

	public RefrencesAndValues folderPrefab;
	public RefrencesAndValues filePrefab;
	public RefrencesAndValues addressBarBtnPrefab;

	public string[] filesToShowWithExtensions;

	public class FileOrFolder
	{
		public bool Folder;
		public bool File;
		public Transform transform;
		public Transform Path;
	}

	public delegate void DirectoriesAndFilesCreatedDelegate(string currentDir);
	public event DirectoriesAndFilesCreatedDelegate DirectoriesAndFilesCreatedEvent = delegate {};

	public delegate void FolderOrFileCreatedDelegate(RefrencesAndValues refrencesAndValues);
	public event FolderOrFileCreatedDelegate FolderOrFileCreatedEvent = delegate {};


	public void FingerTouchedFileOrFolder (RefrencesAndValues fileOrFolder) 
	{
		if(IsAnyFileOrFolderIsSelected()){return;}
		StartCoroutine ("CheckForLongPress",fileOrFolder);
	}

	IEnumerator CheckForLongPress(RefrencesAndValues fileOrFolder)
	{
		yield return new WaitForSeconds (.5f);
		fileOrFolder.SetBool ("Selected",true);
		fileOrFolder.SetBool ("JustSelected",true);
		fileOrFolder.GetSprite ("Bg").color = fileOrFolder.GetColor ("SelectedBgColor");
		if(optionsMenu != null){optionsMenu.SetActive(true);}
	}


	public void FingerLiftedFromFileOrFolder (RefrencesAndValues fileOrFolder) 
	{
		StopCoroutine ("CheckForLongPress");
		if(fileOrFolder.GetBool ("JustSelected")){fileOrFolder.SetBool ("JustSelected",false); return;}

		if(IsAnyFileOrFolderIsSelected ())
		{
			if(fileOrFolder.GetBool("Selected"))
			{
				fileOrFolder.SetBool ("Selected",false);
				fileOrFolder.GetSprite ("Bg").color = fileOrFolder.GetColor ("DefaultBgColor");
				if(!IsAnyFileOrFolderIsSelected()){if(optionsMenu != null){optionsMenu.SetActive(false);}}
			}
			else
			{
				fileOrFolder.SetBool ("Selected",true);
				fileOrFolder.GetSprite ("Bg").color = fileOrFolder.GetColor ("SelectedBgColor");
			}
		}
		else
		{
			string shortcuts = "Assets/Database/Shortcuts";
			string path = string.Empty;
			#if !UNITY_EDITOR
			shortcuts = Application.persistentDataPath + "/" + shortcuts;
			path = Application.persistentDataPath + "/";
			#endif
			if(fileOrFolder.GetBool("IsFolder"))
			{
				if(activePath.IndexOf(shortcuts) > -1)
				{CreateDirectoriesAndFiles(path + fileOrFolder.GetString ("Name"));}
				else
				{CreateDirectoriesAndFiles(activePath + "/" + fileOrFolder.GetString ("Name"));}
			}
			if(fileOrFolder.GetBool("IsFile")){fileOrFolder.InvokeUiEvents("OpenTextFile");}
		}
	}

	bool IsAnyFileOrFolderIsSelected () 
	{
		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			if(createdDirectoriesAndFiles[i].GetBool("Selected")){return true;}
		}
		return false;
	}

	int NoOfFilesOrFoldersSelected () 
	{
		int noOfFilesOrFoldersSelected = 0;
		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			if(createdDirectoriesAndFiles[i].GetBool("Selected")){noOfFilesOrFoldersSelected += 1;}
		}
		return noOfFilesOrFoldersSelected;
	}

	List<RefrencesAndValues> createdDirectoriesAndFiles = new List<RefrencesAndValues>();
	[HideInInspector] public string activePath;
	public void CreateDirectoriesAndFiles (string path) 
	{
		DestroyAllFilesAndFolders ();
		activePath = path;
		string[] directories = Directory.GetDirectories (activePath);

		for (int i = 0; i < directories.Length; i++) 
		{
			InstantiateFolderOrFile(folderPrefab,Path.GetFileName(directories[i]).Replace("!@#$","/"),-i * folderPrefab.GetFloat ("Height"),true,false);
		}

		string[] files = Directory.GetFiles (activePath);
		int noOfFilesCreated = 0;
		for (int i = 0; i < files.Length; i++) 
		{
			bool extensionFound = false;
			for (int j = 0; j < filesToShowWithExtensions.Length; j++) 
			{
				if(Path.GetExtension(files[i]) == filesToShowWithExtensions[j]){extensionFound = true; break;}
			}
			if(!extensionFound){continue;}
			 
			InstantiateFolderOrFile(filePrefab,Path.GetFileName(files[i]).Replace("!@#$","/"),-noOfFilesCreated * filePrefab.GetFloat ("Height") - (directories.Length * folderPrefab.GetFloat ("Height")),false,true);
			noOfFilesCreated += 1;
		}

		CreateAddressBar ("Assets");
		DirectoriesAndFilesCreatedEvent.Invoke (activePath);
	}


	void CreateAddressBar (string mask) 
	{
		while (addressBar.childCount > 0) {DestroyImmediate(addressBar.GetChild(0).gameObject);}
		#if !UNITY_EDITOR
		mask = Application.persistentDataPath + "/" + mask;
		#endif
		string activePathCopy = activePath;
		activePathCopy = activePathCopy.Replace (mask + "/","");

		string[] pathArray = activePathCopy.Split (new char[1]{'/'},System.StringSplitOptions.RemoveEmptyEntries);

		string combinedPath = string.Empty;
		for (int i = 0; i < pathArray.Length; i++) 
		{
			combinedPath += "/" + pathArray[i];
			RefrencesAndValues addressBarBtn = Instantiate(addressBarBtnPrefab) as RefrencesAndValues;
			addressBarBtn.GetTextMesh("BtnText").text = " " + pathArray[i] + " /";
			addressBarBtn.SetString("Address",mask + combinedPath);
			addressBarBtn.transform.parent = addressBar;
		}
		StartCoroutine ("PositionAddressBarBtns");
	}

	IEnumerator PositionAddressBarBtns () 
	{
		yield return new WaitForEndOfFrame ();
		float combinedWidth = 0;
		for (int i = 0; i < addressBar.childCount; i++) 
		{
			RefrencesAndValues addressBarBtn = addressBar.GetChild(i).GetComponent<RefrencesAndValues>();
			BoxCollider addressBarBtnCollider = addressBarBtn.GetTransform("Collider").GetComponent<BoxCollider>();
			float addressBarBtnTxtWidth = addressBarBtn.GetTextMesh("BtnText").GetComponent<MeshRenderer>().bounds.size.x;
			addressBarBtnCollider.center = new Vector3(addressBarBtnTxtWidth/2,0,0);
			addressBarBtnCollider.size = new Vector3(addressBarBtnTxtWidth,addressBarBtnCollider.size.y,addressBarBtnCollider.size.z);
			addressBarBtn.transform.localPosition = new Vector3(combinedWidth,0,0);
			combinedWidth += addressBarBtnTxtWidth;
		}

		if(combinedWidth > 12)
		{
			DestroyImmediate(addressBar.GetChild(0).gameObject);
			StartCoroutine ("PositionAddressBarBtns");
		}
	}

	public void AddressBarBtn (RefrencesAndValues refrencesAndValues) 
	{
		CreateDirectoriesAndFiles (refrencesAndValues.GetString("Address"));
	}

	void InstantiateFolderOrFile (RefrencesAndValues folderOrFilePrefab,string folderOrFileLabel,float yLocalposition,bool isFolder,bool isFile) 
	{
		RefrencesAndValues folderOrFile = Instantiate(folderOrFilePrefab) as RefrencesAndValues;
		folderOrFile.GetTransform("SelfTransform").parent = filesAndFoldersParent;
		folderOrFile.GetTransform("SelfTransform").localPosition = new Vector3(0,yLocalposition,0);
		folderOrFile.GetTextMesh ("Label").text = folderOrFileLabel;
		folderOrFile.SetString ("Name",folderOrFileLabel); 
		folderOrFile.SetBool ("IsFolder",isFolder); 
		folderOrFile.SetBool ("IsFile",isFile); 
		folderOrFile.SetBool ("Selected",false); 
		folderOrFile.SetBool ("JustSelected",false); 
		createdDirectoriesAndFiles.Add (folderOrFile);
		FolderOrFileCreatedEvent.Invoke (folderOrFile);
	}

	void DestroyAllFilesAndFolders () 
	{
		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			Destroy(createdDirectoriesAndFiles[i].GetTransform("SelfTransform").gameObject);
		}
		createdDirectoriesAndFiles.Clear ();
	}

	public void BackBtn () 
	{
		if(activePath == string.Empty){NotificationManager.instance.Alert("Heiarchy Dosent Exits . Plz Download the Files From The Server First."); return;}
		CreateDirectoriesAndFiles (activePath.Replace("/" + Path.GetFileNameWithoutExtension(activePath),""));
	}

	public void DeleteSelectedFilesAndFolders () 
	{
		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			if(createdDirectoriesAndFiles[i].GetBool("Selected"))
			{
				Directory.Delete(activePath + "/" + createdDirectoriesAndFiles[i].GetString("Name"),true);
			}
		}
		CreateDirectoriesAndFiles (activePath);
	}

	[HideInInspector] public string selectedFileOrFolderName;
	public void GenerateSelectedFileOrFolderName () 
	{
		if(NoOfFilesOrFoldersSelected() > 1){return;}
		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			if(createdDirectoriesAndFiles[i].GetBool("Selected"))
			{
				selectedFileOrFolderName = createdDirectoriesAndFiles[i].GetTextMesh("Label").text;
			}
		}
	}


	[HideInInspector] public string txtFileTxt;
	public void GetTxtFileTxt (string txtFileName) 
	{
		string shortcuts = "Assets/Database/Shortcuts";
		string path = string.Empty;
		#if !UNITY_EDITOR
		shortcuts = Application.persistentDataPath + "/" + shortcuts;
		path = Application.persistentDataPath + "/";
		#endif

		if(activePath.IndexOf(shortcuts) > -1){txtFileTxt = File.ReadAllText (path + txtFileName);}
		else{txtFileTxt = File.ReadAllText (activePath + "/" + txtFileName);}
	}

	public void SaveTxtFile (string txtFileName,string content) 
	{
		File.WriteAllText (activePath + "/" + txtFileName,content);
	}


	public void AddNewFolder (string folderName,FunctionsContainer functionsContainer) 
	{
		if(Directory.Exists(activePath + "/" + folderName)){NotificationManager.instance.SimpleAlert("Folder Name Already Exits."); return;}
		Directory.CreateDirectory (activePath + "/" + folderName);
		functionsContainer.InvokeFunctions ();
		CreateDirectoriesAndFiles (activePath);
	}

	public void AddNewFile (string fileName,FunctionsContainer functionsContainer) 
	{
		if(File.Exists(activePath + "/" + fileName)){NotificationManager.instance.SimpleAlert("File Name Already Exits."); return;}
		File.WriteAllText (activePath + "/" + fileName + ".txt","");
		functionsContainer.InvokeFunctions ();
		CreateDirectoriesAndFiles (activePath);
	}

	public void AddNewFolderBtn () 
	{
		if(activePath == string.Empty){NotificationManager.instance.Alert("Heiarchy Dosent Exits . Plz Download the Files From The Server First."); return;}
		for (int i = 0; true; i++) 
		{
			if(!Directory.Exists(activePath + "/NewFolder_" + i)){NotificationManager.instance.Prompt("ADD NEW FOLDER","NewFolder_" + i,"AddNewFolder"); return;}
		}
	}

	public void AddNewFileBtn () 
	{
		if(activePath == string.Empty){NotificationManager.instance.Alert("Heiarchy Dosent Exits . Plz Download the Files From The Server First."); return;}
		for (int i = 0; true; i++) 
		{
			if(!File.Exists(activePath + "/NewFile_" + i + ".txt")){NotificationManager.instance.Prompt("ADD NEW FILE","NewFile_" + i + ".txt","AddNewFile"); return;}
		}
	}

	public void RenameFolderOrFile (string folderOrFileName,FunctionsContainer functionsContainer) 
	{
		bool isFile = false;
		if(folderOrFileName.IndexOf(".txt") > -1){isFile = true;}
		if(isFile)
		{
			if(File.Exists(activePath + "/" + folderOrFileName)){NotificationManager.instance.SimpleAlert("Name Already Exists"); return;}
			File.WriteAllText (activePath + "/" + folderOrFileName,File.ReadAllText(activePath + "/" + selectedFileOrFolderName));
			File.Delete(activePath + "/" + selectedFileOrFolderName);
			functionsContainer.InvokeFunctions();
			CreateDirectoriesAndFiles (activePath);

		}
		else
		{
			if(Directory.Exists(activePath + "/" + folderOrFileName)){NotificationManager.instance.SimpleAlert("NAME ALREADY EXITS PLEASE CHOOSE A DIFFERENT NAME"); return;}
			Directory.Move(activePath + "/" + selectedFileOrFolderName,activePath + "/" + folderOrFileName);
			functionsContainer.InvokeFunctions();
			CreateDirectoriesAndFiles (activePath);
		}
	}

	public void CreateShortcut () 
	{
		string shortcuts = "Assets/Database/Shortcuts";
		#if !UNITY_EDITOR
		shortcuts = Application.persistentDataPath + "/" + shortcuts;
		#endif
		if(!Directory.Exists(shortcuts)){Directory.CreateDirectory(shortcuts);}


		for (int i = 0; i < createdDirectoriesAndFiles.Count; i++) 
		{
			if(createdDirectoriesAndFiles[i].GetBool("Selected"))
			{
				if(createdDirectoriesAndFiles[i].GetBool("IsFolder")){Directory.CreateDirectory(shortcuts + "/" + activePath.Replace(Application.persistentDataPath + "/","").Replace("/","!@#$") + "!@#$" + createdDirectoriesAndFiles[i].GetString("Name"));}
				if(createdDirectoriesAndFiles[i].GetBool("IsFile")){File.WriteAllText (shortcuts + "/" + activePath.Replace(Application.persistentDataPath + "/","").Replace("/","!@#$") + "!@#$" + createdDirectoriesAndFiles[i].GetString("Name"),"");}
			}
		}
	}

	string recordedDirectoryBeforeGoingToShortcuts = string.Empty;
	public void ShowShortcuts () 
	{
		string shortcuts = "Assets/Database/Shortcuts";
		#if !UNITY_EDITOR
		shortcuts = Application.persistentDataPath + "/" + shortcuts;
		#endif
		if(!Directory.Exists(shortcuts)){Directory.CreateDirectory(shortcuts);}
		if(string.IsNullOrEmpty(recordedDirectoryBeforeGoingToShortcuts))
		{
			if(activePath.IndexOf(shortcuts) > -1 )
			{
				CreateDirectoriesAndFiles(basePath);
			}
			else
			{
				recordedDirectoryBeforeGoingToShortcuts = activePath;
				if(string.IsNullOrEmpty(recordedDirectoryBeforeGoingToShortcuts))
				{
					recordedDirectoryBeforeGoingToShortcuts = "Assets/Database";
					#if !UNITY_EDITOR
					recordedDirectoryBeforeGoingToShortcuts = Application.persistentDataPath + "/" + recordedDirectoryBeforeGoingToShortcuts;
					#endif
					if(!Directory.Exists(recordedDirectoryBeforeGoingToShortcuts)){Directory.CreateDirectory(recordedDirectoryBeforeGoingToShortcuts);}
				}
				CreateDirectoriesAndFiles(shortcuts);
			}
		}
		else
		{
			CreateDirectoriesAndFiles(recordedDirectoryBeforeGoingToShortcuts);
			recordedDirectoryBeforeGoingToShortcuts = string.Empty;
		}
	}

	void CreateShorcutsFolderIfNotCreated () 
	{
		string shortcuts = "Assets/Database/Shortcuts";
		#if !UNITY_EDITOR
		shortcuts = Application.persistentDataPath + "/" + shortcuts;
		#endif
		if(!Directory.Exists(shortcuts)){Directory.CreateDirectory(shortcuts);}
	}
}






















