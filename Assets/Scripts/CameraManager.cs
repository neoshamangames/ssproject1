using UnityEngine;
using System.Collections;
using Vectrosity;

public class CameraManager : SingletonMonoBehaviour<CameraManager> {

	#region Attributes
	public Plant plant;
	public Cloud cloud;
	public Transform raindrops;
	public Transform background;
	public float autoScrollBuffer = .5f;
	[Range(0, 50)]public float backgroundMovementFactor = 1f;
	[Range(0, 50)]public float mouseZoomSensitivity = 2f;
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
	}

	void Update () {
		float distanceFromEdge = topEdge - plant.Height;
		if (distanceFromEdge < autoScrollBuffer)
		{
			MoveCamera(new Vector3(0,autoScrollBuffer - distanceFromEdge, 0));
		}

		#if UNITY_EDITOR
		
		if (Input.GetMouseButton(0))
		{
			ProcessTouch(Input.mousePosition);
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0)
		{
			ProcessZoom(-scroll * mouseZoomSensitivity);
		}
		#else
		#if UNITY_ANDROID || UNITY_IPHONE
		int+ numOfTouches = Input.touches.Length;
		if (numOfTouches == 1)
		{
			firstTouch = Input.touches[0];
			if (firstTouch.phase != TouchPhase.Ended && firstTouch.phase != TouchPhase.Canceled)
			{
				ProcessTouch(firstTouch.position);
			}
		}
		else if (numOfTouches > 1)
		{
			float distance = (Vector2.Distance(Input.touches[0].position, Input.touches[1].position));
		}
		#endif
		#endif
		
	}
	#endregion
	
	#region Private
	private float height;
	private float width;
	private Camera vectorCam;
	private Camera mainCam;
	private float topEdge;
	
	private void ProcessTouch(Vector2 coordinates)
	{
		Ray ray = mainCam.ScreenPointToRay(coordinates);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit))
		{
			cloud.Rain(Time.deltaTime);
		}
	}
	
	private void ProcessZoom(float delta)
	{
		vectorCam.orthographicSize += delta;
	}
	
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
