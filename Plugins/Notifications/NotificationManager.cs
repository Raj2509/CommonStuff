using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationManager : MonoBehaviour 
{
	public static NotificationManager instance; 
	void Awake() {instance = this;}


	public Transform notificationsParent;
	public RefrencesAndValues simpleAlertPrefab;
	public RefrencesAndValues alertPrefab;
	public RefrencesAndValues confirmPrefab;
	public RefrencesAndValues promptPrefab;
	public RefrencesAndValues textAreaPrefab;


	public void SimpleAlert (string notice) 
	{
		StartCoroutine (ShowNotificationCoroutine(notice,Instantiate(simpleAlertPrefab) as RefrencesAndValues));
	}

	public void Alert (string notice) 
	{
		StartCoroutine (ShowNotificationCoroutine(notice,Instantiate(alertPrefab) as RefrencesAndValues));
	}

	public void Conform (string notice,string uiEventsLabel) 
	{
		RefrencesAndValues notification = Instantiate(confirmPrefab) as RefrencesAndValues;
		notification.GetGuiRayReceiver ("OkBtn").onFingerUp = notification.GetUiEvents (uiEventsLabel);
		StartCoroutine (ShowNotificationCoroutine(notice,notification));
	}

	public void Prompt (string notice,string placeHolderTxt,string uiEventsLabel) 
	{
		RefrencesAndValues notification = Instantiate(promptPrefab) as RefrencesAndValues;
		notification.GetComponent<GuiTextField> ().SetPlaceHolderTxt (placeHolderTxt);
		notification.GetGuiRayReceiver ("OkBtn").onFingerUp = notification.GetUiEvents (uiEventsLabel);
		StartCoroutine (ShowNotificationCoroutine(notice,notification));
	}

	IEnumerator ShowNotificationCoroutine (string notice,RefrencesAndValues noticePrefab) 
	{
		noticePrefab.GetTransform ("SelfTransform").parent = notificationsParent;
		noticePrefab.GetTransform ("SelfTransform").localPosition = new Vector3(0,0,-(notificationsParent.childCount - 1));

		noticePrefab.GetTextMesh ("NoticeTextMesh").text = notice;
		Transform noticeTxtTransform = noticePrefab.GetTransform ("NoticeTxtTransform");
		noticeTxtTransform.localPosition = new Vector3(noticeTxtTransform.localPosition.x,-noticePrefab.GetFloat ("NoticeTxtTopPadding"),noticeTxtTransform.localPosition.z);

		yield return new WaitForEndOfFrame ();

		float noticeTextMeshHeight = noticePrefab.GetTextMesh ("NoticeTextMesh").GetComponent<MeshRenderer>().bounds.size.y;
		tk2dTiledSprite bg = noticePrefab.GetTiledSprite ("Bg");
		float bgHeight = noticeTextMeshHeight + noticePrefab.GetFloat ("NoticeTxtTopPadding") + noticePrefab.GetFloat ("NoticeTxtBottomPadding");
		bg.dimensions = new Vector2 (bg.dimensions.x,bgHeight*40);

		Transform btns = noticePrefab.GetTransform ("Btns");
		if(btns != null){btns.localPosition = new Vector3 (btns.localPosition.x,-(noticeTextMeshHeight + noticePrefab.GetFloat ("NoticeTxtTopPadding") + noticePrefab.GetFloat ("BtnsTopPadding")),btns.localPosition.z);}

		Transform notificationHelper = noticePrefab.GetTransform ("NotificationHelper");
		if(notificationHelper != null){notificationHelper.localPosition = new Vector3 (0,bgHeight/2,0);}

	}

}
