using UnityEngine;
using System.Collections;

public class SplashScreenManager : MonoBehaviour {


	#region Attributes
	public Texture splash1;
	public Texture splash2;
	public float splashTime;
	#endregion

	#region Unity
	void Awake()
	{
		width = Screen.width;
		height = Screen.height;
		buttonStyle = new GUIStyle();
		Time.timeScale = 1;
	}
	
	void OnGUI ()
	{
		if (GUI.Button(new Rect(0, 0, width, height), "", buttonStyle))
		{
			state++;
			timer = 0;
		}
//		Debug.Log ("timer: " + timer);
		switch (state)
		{
		case 0:
			GUI.DrawTexture(new Rect(0, 0, width, height), splash1, ScaleMode.ScaleToFit);
			if (timer > splashTime)
			{
				state++;
				timer = 0;
			}
			break;
		case 1:
			GUI.DrawTexture(new Rect(0, 0, width, height), splash2, ScaleMode.ScaleToFit);
			if (timer > splashTime)
			{
				state++;
				timer = 0;
			}
			break;
		case 2:
			Application.LoadLevel("game");
//			Debug.Log("load level");
			state++;
			break;
		}
		
		timer += Time.deltaTime;
	}
	#endregion
	
	#region Private
	private GUIStyle buttonStyle;
	private float width, height;
	private float timer;
	private int state;
	#endregion
}
