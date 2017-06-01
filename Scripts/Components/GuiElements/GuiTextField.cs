using UnityEngine;
using System.Collections;

public class GuiTextField : MonoBehaviour {

	public GUISkin GUISkin;
	public Camera cam;

	public string placeHolderTxt;
	public tk2dTiledSprite bg;


	public void SetPlaceHolderTxt (string txt) 
	{
		placeHolderTxt = txt;
	}

	void OnGUI () 
	{
		GUI.skin = GUISkin;
		Vector3 bgScreenPos = cam.WorldToScreenPoint(bg.transform.position);
		placeHolderTxt = GUI.TextField(new Rect(bgScreenPos.x,Screen.height - bgScreenPos.y + 7.5f,bg.dimensions.x,bg.dimensions.y),placeHolderTxt);
	}
}
