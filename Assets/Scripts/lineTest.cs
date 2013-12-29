using UnityEngine;
using System.Collections;
using Vectrosity;

public class lineTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		Debug.Log("FOV 60");
		printInfo();
		
		mainCam.fieldOfView = 30;
		Debug.Log("FOV 30");
		printInfo();
		
		mainCam.fieldOfView = 80;
		Debug.Log("FOV 80");
		printInfo();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void printInfo()
	{
		Vector3 point1 = mainCam.WorldToScreenPoint(new Vector3(15, 0, 15));
		Vector3 point2 = mainCam.WorldToScreenPoint(new Vector3(30, 0, 15));
		Vector3 point3 = mainCam.WorldToScreenPoint(new Vector3(30, 15, 15));
		Vector3 point4 = mainCam.WorldToScreenPoint(new Vector3(40, 15, 15));
		Debug.Log("point1: " + point1);
		Debug.Log("point2: " + point1);
		float width1 = Mathf.Abs(point1.x - point2.x);
		float width2 = Mathf.Abs(point4.x - point3.x);
		float widthPropotion = width1/width2;
		float FOVwidthProp = mainCam.fieldOfView/width1;
		Debug.Log("width1: " + width1);
		Debug.Log("width2: " + width2);
		Debug.Log("widthPropotion: " + widthPropotion);
		Debug.Log("FOVwidthProp: " + FOVwidthProp);
	}
	
	Camera mainCam;
}
