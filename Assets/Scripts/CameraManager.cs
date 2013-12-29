using UnityEngine;
using System.Collections;
using Vectrosity;

public class CameraManager : SingletonMonoBehaviour<CameraManager> {

	#region Attributes
	public Camera cloudCam;
	public Plant plant;
	public Cloud cloud;
	public float scrollEdgePercent = .75f;
	public float panSensitivity = 1f;
	public float maxXScroll = 0f;
	public float yScrollBuffer = 2f;
	[Range(0, 50)]public float backgroundMovementFactor = 1f;
	[Range(0, 50)]public float mouseZoomSensitivity = 2f;
	[Range(0, 50)]public float pinchZoomSensitivity = 2f;
	[Range(0, 50)]public float minFOV = 40f;
	[Range(0, 50)]public float maxFOV = 120f; 
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
		mainCam = Camera.main;
		VectorLine.SetCamera3D(mainCam);
		vectorCam = VectorLine.SetCamera(cloudCam);
		vectorCam.orthographic = true;
		vectorCam.transform.position = cloudCam.transform.position;
//		vectorCam.transform.localScale = mainCam.transform.localScale;
//		vectorCam.orthographicSize = mainCam.orthographicSize;
//		vectorCam.fieldOfView = mainCam.fieldOfView;
		vectorCam.nearClipPlane = 1;
		vectorCam.farClipPlane = 1000;
		height = 10;
		width = height * Screen.width / Screen.height;
		plantDistanceFromCam = plant.transform.position.z - transform.position.z;
		Debug.Log("plantDistanceFromCam: " + plantDistanceFromCam);
	}
	
	void Start()
	{
		CalculateEdges();
	}

	void Update () {
		//float distanceFromEdge = topEdge - plant.Height;
//		float plantYPos = mainCam.WorldToViewportPoint(plant.TopPosisiton).y;
		float plantY = plant.TopPosisiton.y;
		bool overScrollEdge = plantY > scrollEdge;
		if (!prevCoordsSet)
		{
			if (!prevOverScrollEdge && overScrollEdge)
			{
				float scrollDelta = scrollEdge - plant.TopPosisiton.y;
				MoveCamera(new Vector3(0, -scrollDelta, 0));
			}
		}
		else
		{
			prevOverScrollEdge = overScrollEdge;
		}

		#if UNITY_EDITOR
		Vector2 mousePos = Input.mousePosition;
		if (Input.GetMouseButtonDown(0))
			ProcessTouchBegan(mousePos);
			
		if (Input.GetMouseButton(0))
		{
			ProcessTouch(mousePos);
		}
		else
		{
			prevCoordsSet = false;
		}
		float scroll = Input.GetAxis("Mouse ScrollWheel");
		if (scroll != 0)
		{
			ProcessZoom(mousePos, -scroll * mouseZoomSensitivity);
		}
		#else
		#if UNITY_ANDROID || UNITY_IPHONE
		int numOfTouches = Input.touches.Length;
		if (numOfTouches == 1)
		{
			Touch firstTouch = Input.touches[0];
			Vector2 firstTouchPos = firstTouch.position;
			if (firstTouch.phase == TouchPhase.Began)
				ProcessTouchBegan(firstTouchPos);
			
			if (firstTouch.phase != TouchPhase.Ended && firstTouch.phase != TouchPhase.Canceled)
			{
				ProcessTouch(firstTouchPos);
			}
			prevDistanceSet = false;
		}
		else
		{
			prevCoordsSet = false;
			if (numOfTouches > 1)
			{
				Vector2 firstTouchPos = Input.touches[0].position;
				Vector2 secondTouchPos = Input.touches[1].position;
				Vector2 center = (firstTouchPos + secondTouchPos)/2;
				float distance = (Vector2.Distance(Input.touches[0].position, Input.touches[1].position));
				
				if (prevDistanceSet)
				{
					float deltaDistance = prevDistance - distance;
					if (distance != 0)
						ProcessZoom(center, deltaDistance * pinchZoomSensitivity);
				}
				else
				{
					prevDistanceSet = true;
				}
				prevDistance = distance;
			}
			else
			{
				prevDistanceSet = false;
			}
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
	private float scrollEdge;
	private float plantDistanceFromCam;
	private Vector2 prevCoords;
	private bool prevCoordsSet;
	private bool touchBeganOnCloud;
	private bool prevOverScrollEdge;
	private float prevDistance;
	private bool prevDistanceSet;
	
	private void ProcessTouchBegan(Vector2 coordinates)
	{
		Ray ray = cloudCam.ScreenPointToRay(coordinates);
		touchBeganOnCloud = Physics.Raycast(ray);
	}
	
	private void ProcessTouch(Vector2 coordinates)
	{
		Ray ray = cloudCam.ScreenPointToRay(coordinates);
		RaycastHit hit;
		if (touchBeganOnCloud)
		{
			if (Physics.Raycast(ray, out hit))
			{
				cloud.Rain(Time.deltaTime);
			}
		} else {
			if (prevCoordsSet)
			{
				Vector2 deltaCoords = prevCoords - coordinates;
				Vector3 initialPos = mainCam.ScreenToWorldPoint(new Vector3(prevCoords.x, prevCoords.y, plantDistanceFromCam));
				Debug.Log("initialPos: " + initialPos);
				Vector3 newPos = mainCam.ScreenToWorldPoint(new Vector3(coordinates.x, coordinates.y, plantDistanceFromCam)); 
				Debug.Log("newPos: " + newPos);
				MoveCamera(initialPos - newPos);
			}
			prevCoords = coordinates;
			prevCoordsSet = true;
		}
	}
	
	private void ProcessZoom(Vector2 coordinates, float delta)
	{
		Debug.Log("coordinates: " + coordinates);
		Vector3 initialPos = mainCam.ScreenToWorldPoint(new Vector3(coordinates.x, coordinates.y, plantDistanceFromCam));
		Debug.Log("initialPos: " + initialPos);
		if (mainCam.fieldOfView + delta > maxFOV)
			delta = maxFOV - mainCam.fieldOfView;
		if (mainCam.fieldOfView + delta < minFOV)
			delta = minFOV - mainCam.fieldOfView;
		mainCam.fieldOfView += delta;
		Vector3 newCoords = mainCam.WorldToScreenPoint(initialPos);
		mainCam.fieldOfView -= delta;
		Vector3 newPosOriginalFOV = mainCam.ScreenToWorldPoint(new Vector3(newCoords.x, newCoords.y, plantDistanceFromCam));
		Debug.Log("newCoords: " + newCoords);
		Vector3 coordsDelta = (Vector2)newCoords - coordinates;
		Vector3 posDelta = initialPos - newPosOriginalFOV;
		MoveCamera(newPosOriginalFOV - initialPos);
		mainCam.fieldOfView += delta;
		Debug.Log("coordsDelta: " + coordsDelta);
		Debug.Log("posDelta: " + posDelta);
		CalculateEdges();
	}
	
	void MoveCamera(Vector3 movement)
	{
//		Debug.Log("Move cam: " + movement);
		mainCam.transform.position += movement;
		Vector3 pos = mainCam.transform.position;
		if (pos.x > maxXScroll)
			pos.x = maxXScroll;
		if (pos.x < -maxXScroll)
			pos.x = -maxXScroll;
		if (pos.y < 0)
			pos.y = 0;
		float maxYScroll = plant.TopPosisiton.y + yScrollBuffer;
		if (pos.y > maxYScroll)
			pos.y = maxYScroll;
		mainCam.transform.position = pos;
		CalculateEdges();
	}
	
	void CalculateEdges()
	{
		scrollEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, scrollEdgePercent, plantDistanceFromCam)).y;
		topEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, 1, plantDistanceFromCam)).y;
	}
	#endregion
}
