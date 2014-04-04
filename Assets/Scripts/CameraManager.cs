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
	public float xScrollBuffer = .1f;
	public float topScrollBuffer = .1f;
	public float bottomScrollBuffer = .1f;
	[Range(0, 50)]public float backgroundMovementFactor = 1f;
	[Range(0, 50)]public float mouseZoomSensitivity = 2f;
	[Range(0, 50)]public float pinchZoomSensitivity = 2f;
	[Range(0, 50)]public float minFOV = 40f;
	[Range(0, 50)]public float maxFOV = 120f; 
	#endregion

	#region Properties	
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
		width = Screen.width;
		plantDistanceFromCam = plant.transform.position.z - transform.position.z;
		Debug.Log("plantDistanceFromCam: " + plantDistanceFromCam);
	}
	
	void Start()
	{
		CalculateEdges();
	}

	void Update () {
//		float plantYPos = mainCam.WorldToViewportPoint(plant.TopPosisiton).y;
		float plantY = plant.TopPosisiton.y;
		bool inScrollRange = plantY > scrollEdge && plantY < topEdge;
//		if(plantY < max)
//			Debug.Log ("ERROR");
		if (!prevCoordsSet)
		{
			if (inScrollRange)
			{
				float scrollDelta = prevPlantY - plantY;
				MoveCamera(new Vector3(0, -scrollDelta, 0));
//				SetCameraY(plantY);
//				max = plantY;
			}
		}
		prevPlantY = plantY;

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
	
	#region Actions
	public void ZoomOut()
	{
	
	}
	#endregion
	
	#region Private
	private const int CLOUD_LAYER = 9;
	private LayerMask notCloudLayer = ~(1 << CLOUD_LAYER);
	private float height;
	private float width;
	private Camera vectorCam;
	private Camera mainCam;
	private float scrollEdge, topEdge;
	private float plantDistanceFromCam;
	private Vector2 prevCoords;
	private bool prevCoordsSet;
	private bool touchBeganOnCloud;
	private float prevDistance;
	private bool prevDistanceSet;
	private float prevPlantY;
	
//	private float max =0;//temp
	
	private void ProcessTouchBegan(Vector2 coordinates)
	{
		Ray cloudRay = cloudCam.ScreenPointToRay(coordinates);
		Ray mainRay = mainCam.ScreenPointToRay(coordinates);
		RaycastHit hit;
		
		touchBeganOnCloud = (Physics.Raycast(cloudRay));
			
		if (Physics.Raycast(mainRay, out hit, Mathf.Infinity, notCloudLayer))
		{
			Collider collider = hit.collider;
			if (collider != null && collider.tag == "Flower")
				collider.gameObject.GetComponent<Flower>().ProcessClick();
		}
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
				Vector3 initialPos = mainCam.ScreenToWorldPoint(new Vector3(prevCoords.x, prevCoords.y, plantDistanceFromCam));
//				Debug.Log("initialPos: " + initialPos);
				Vector3 newPos = mainCam.ScreenToWorldPoint(new Vector3(coordinates.x, coordinates.y, plantDistanceFromCam)); 
//				Debug.Log("newPos: " + newPos);
				MoveCamera(initialPos - newPos);
			}
			prevCoords = coordinates;
			prevCoordsSet = true;
		}
	}
	
	private void ProcessZoom(Vector2 coordinates, float delta)
	{
//		Debug.Log("coordinates: " + coordinates);
		Vector3 initialPos = mainCam.ScreenToWorldPoint(new Vector3(coordinates.x, coordinates.y, plantDistanceFromCam));
//		Debug.Log("initialPos: " + initialPos);
		if (mainCam.fieldOfView + delta > maxFOV)
			delta = maxFOV - mainCam.fieldOfView;
		if (mainCam.fieldOfView + delta < minFOV)
			delta = minFOV - mainCam.fieldOfView;
		mainCam.fieldOfView += delta;
		Vector3 newCoords = mainCam.WorldToScreenPoint(initialPos);
		mainCam.fieldOfView -= delta;
		Vector3 newPosOriginalFOV = mainCam.ScreenToWorldPoint(new Vector3(newCoords.x, newCoords.y, plantDistanceFromCam));
//		Debug.Log("newCoords: " + newCoords);
//		Vector3 coordsDelta = (Vector2)newCoords - coordinates;
//		Vector3 posDelta = initialPos - newPosOriginalFOV;
		MoveCamera(newPosOriginalFOV - initialPos);
		mainCam.fieldOfView += delta;
//		Debug.Log("coordsDelta: " + coordsDelta);
//		Debug.Log("posDelta: " + posDelta);
		CalculateEdges();
	}
	
	void MoveCamera(Vector3 movement)
	{
//		Debug.Log("Move cam: " + movement);
		mainCam.transform.position += movement;
		Vector3 basePos = mainCam.WorldToViewportPoint(plant.BasePosisiton);
		Vector3 topPos = mainCam.WorldToViewportPoint(plant.TopPosisiton);
		if (topPos.y < topScrollBuffer ||  basePos.y > (1 - bottomScrollBuffer) || basePos.x  < xScrollBuffer || basePos.x > (1 - xScrollBuffer))
		{
			mainCam.transform.position -= movement;
			return;
		}
		CalculateEdges();
	}
	
	void SetCameraY(float y)
	{
		Vector3 pos = mainCam.transform.position;
		pos.y = y;
		mainCam.transform.position = pos;
		Vector3 basePos = mainCam.WorldToViewportPoint(plant.BasePosisiton);
		Vector3 topPos = mainCam.WorldToViewportPoint(plant.TopPosisiton);
		if (topPos.y < topScrollBuffer ||  basePos.y > (1 - bottomScrollBuffer) || basePos.x  < xScrollBuffer || basePos.x > (1 - xScrollBuffer))
		{
//			mainCam.transform.position -= movement;
			return;
		}
		CalculateEdges();
	}
	
	void CalculateEdges()
	{
		scrollEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, scrollEdgePercent, plantDistanceFromCam)).y;
		topEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, 1, plantDistanceFromCam)).y;
	}
	#endregion
}
