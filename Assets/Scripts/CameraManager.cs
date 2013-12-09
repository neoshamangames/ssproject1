using UnityEngine;
using System.Collections;
using Vectrosity;

public class CameraManager : SingletonMonoBehaviour<CameraManager> {

	#region Attributes
	public Plant plant;
	public Transform raindrops;
	public Transform background;
	public float autoScrollBuffer = .5f;
	[Range(0, 50)]public float backgroundMovementFactor = 1f;
	#endregion

	#region Properties
	public float Height
	{
		get {return height; }
	}
	
	public float Width
	{
		get {return width; }
	}
	#endregion

	#region Unity
	void Awake() {
		vectorCam = VectorLine.SetCamera ();
		mainCam = Camera.main;
		vectorCam.orthographic = mainCam.orthographic;
		vectorCam.transform.position = mainCam.transform.position;
		vectorCam.transform.localScale = mainCam.transform.localScale;
		vectorCam.orthographicSize = mainCam.orthographicSize;
		vectorCam.nearClipPlane = 0;
		vectorCam.farClipPlane = 5000;
		height = vectorCam.orthographicSize * 2;
		width = height * Screen.width / Screen.height;
		CalculateEdges();
		Debug.Log("topEdge: " + topEdge);
	}

	void Update () {
		float distanceFromEdge = topEdge - plant.Height;
		if (distanceFromEdge < autoScrollBuffer)
		{
			MoveCamera(new Vector3(0,autoScrollBuffer - distanceFromEdge, 0));
		}
	}
	#endregion
	
	#region Private
	private float height;
	private float width;
	private Camera vectorCam;
	private Camera mainCam;
	private float topEdge;
	
	void MoveCamera(Vector3 movement)
	{
		vectorCam.transform.position += movement;
		raindrops.position += movement;
		background.position -= movement * backgroundMovementFactor;
		CalculateEdges();
	}
	
	void CalculateEdges()
	{
		topEdge = vectorCam.ViewportToWorldPoint(new Vector3(0,1,-vectorCam.transform.position.z)).y;
	}
	#endregion
}
