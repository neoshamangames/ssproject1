using UnityEngine;
using System.Collections;
using Vectrosity;

public class CameraManager : SingletonMonoBehaviour<CameraManager> {

	#region Attributes
	public Camera cloudCam;
	public Plant plant;
	public Cloud cloud;
	public float scrollEdge = .75f;
	public float panSensitivity = 1f;
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
		mainCam = Camera.main;
		vectorCam = VectorLine.SetCamera(cloudCam);
		VectorLine.SetCamera3D(mainCam);
//		vectorCam.orthographic = false;
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
		if (!prevCoordsSet)
		{
			float scrollDelta = topEdge - plant.TopPosisiton.y;
			if (scrollDelta < 0)
			{
//				MoveCamera(new Vector3(0, -scrollDelta, 0));
			}
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
		}
		else
		{
			prevCoordsSet = false;
			if (numOfTouches > 1)
			{
				float distance = (Vector2.Distance(Input.touches[0].position, Input.touches[1].position));
				
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
	private float plantDistanceFromCam;
	private Vector2 prevCoords;
	private bool prevCoordsSet;
	private bool touchBeganOnCloud;
	
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
		mainCam.fieldOfView += delta;
//		vectorCam.fieldOfView += delta;
		Vector3 newPos = mainCam.WorldToScreenPoint(initialPos);
		Debug.Log("newPos: " + newPos);
		CalculateEdges();
	}
	
	void MoveCamera(Vector3 movement)
	{
//		Debug.Log("Move cam: " + movement);
		mainCam.transform.position += movement;
		CalculateEdges();
	}
	
	void CalculateEdges()
	{
		topEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, scrollEdge, plantDistanceFromCam)).y;
	}
	#endregion
}
