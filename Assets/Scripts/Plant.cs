using UnityEngine;
using System.Collections;
using Vectrosity;

public class Plant : MonoBehaviour {

	#region Attributes
	//[Range(0, 1)] public float test;
	[Range(.1f, 100)] public float lineWidth = 1f;
	public Material lineMaterial;
	public Texture2D backCapTexture;
	public Texture2D frontCapTexture;
	public Color color;
	[Range(0, 10)] public float growSegmentsPerSecond = .1f;
	[Range(5, 1000)]public int segmentsPerCurve = 50;
	[Range(0, 45)] public float minAngle = 5f;
	[Range(0, 45)] public float maxAngle = 30f;
	[Range(0, 1)] public float minControlLength = .05f;
	[Range(0, 1)] public float maxControlLength = .1f;
	[Range(0, 1)]public float minCurveHeight = .1f;
	[Range(0, 1)]public float maxCurveHeight = .5f;
	#endregion

	#region Unity
	void Awake()
	{
		//setup camera
		Camera vectorCam = VectorLine.SetCamera ();
		Camera mainCam = Camera.main;
		vectorCam.orthographic = mainCam.orthographic;
		vectorCam.transform.position = mainCam.transform.position;
		vectorCam.transform.localScale = mainCam.transform.localScale;
		vectorCam.orthographicSize = mainCam.orthographicSize;
		vectorCam.nearClipPlane = 0;
		vectorCam.farClipPlane = 5000;
		height = vectorCam.orthographicSize * 2;
		width = height * Screen.width / Screen.height;
		Debug.Log ("width: " + width);
		line = new VectorLine ("Plant", new Vector2[MAX_POINTS - 2], color, lineMaterial, lineWidth, LineType.Continuous, Joins.Weld);
		controlLine1 = new VectorLine ("Control Line 1", new Vector2[2], Color.red, null, .02f);
		controlLine2 = new VectorLine ("Control Line 2", new Vector2[2], Color.red, null, .02f);
		curvePoints = new Vector2[4];
		startPoint.x = transform.position.x;
		startPoint.y = transform.position.y;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;

		//End Cap
		VectorLine.SetEndCap ("Point", EndCap.Back, lineMaterial, frontCapTexture);
		line.endCap = "Point";

		DrawPlant ();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown (0))
			DrawPlant ();

		growth += Time.deltaTime * growSegmentsPerSecond;
		Debug.Log ("growth: " + growth);
		int newSegment = Mathf.FloorToInt (growth);
		if (newSegment > endSegment) {
			if (newSegment > lastSegment)
			{
				AddCurve();
			}
			endSegment = newSegment;
			line.drawEnd = endSegment;
			line.Draw();
		}
	}
	#endregion

	#region Private
	private const int MAX_POINTS = 16384;
	private Vector2[] curvePoints;
	private VectorLine line;
	private VectorLine controlLine1;
	private VectorLine controlLine2;
	private Vector2 startPoint;
	private float width;
	private float height;
	private float growth = 0;
	private int endSegment = 1;
	private int lastSegment = 0;
	private Vector2 finishPoint;

	private void DrawPlant()
	{

		curvePoints[0] = startPoint;
		float angle = Random.Range (minAngle, maxAngle);
		Debug.Log ("angle 1: " + angle);
		float controlLength = Random.Range (minControlLength, maxControlLength) * height;
		Debug.Log ("controlLength 1: " + controlLength / height);
		Vector2 controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		curvePoints [1].x = startPoint.x + controlPointOffset.x * (Random.Range(0, 2) == 0 ? 1 : -1);
		curvePoints [1].y = startPoint.y + controlPointOffset.y;
		finishPoint = new Vector2(startPoint.x, startPoint.y + Random.Range(minCurveHeight * height, maxCurveHeight * height));
		curvePoints[2] = finishPoint;
		angle = Random.Range (minAngle, maxAngle);
		Debug.Log ("angle 2: " + angle);
		controlLength = Random.Range (minControlLength, maxControlLength) * height;
		Debug.Log ("controlLenght 2: " + controlLength / height);
		controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (Random.Range(0, 2) == 0 ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
		line.MakeCurve (curvePoints, segmentsPerCurve, 0);
		lastSegment += segmentsPerCurve;
		line.drawStart = 0;
		line.drawEnd = endSegment;

		//draw control lines (DEBUG)
		controlLine1.points2 = new Vector2[] {startPoint, new Vector2 (curvePoints [1].x, curvePoints [1].y)};
		controlLine2.points2 = new Vector2[] {finishPoint, new Vector2(curvePoints [3].x, curvePoints [3].y)};
		controlLine1.Draw();
		controlLine2.Draw();

		line.Draw();
	}

	private void AddCurve()
	{
		startPoint = finishPoint;
		float angle = Random.Range (minAngle, maxAngle);
		Debug.Log ("angle 1: " + angle);
		float controlLength = Random.Range (minControlLength, maxControlLength) * height;
		Debug.Log ("controlLength 1: " + controlLength / height);
		Vector2 controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		curvePoints [1].x = startPoint.x + controlPointOffset.x * (Random.Range(0, 2) == 0 ? 1 : -1);
		curvePoints [1].y = startPoint.y + controlPointOffset.y;
		finishPoint = new Vector2(startPoint.x, startPoint.y + Random.Range(minCurveHeight * height, maxCurveHeight * height));
		curvePoints[2] = finishPoint;
		angle = Random.Range (minAngle, maxAngle);
		Debug.Log ("angle 2: " + angle);
		controlLength = Random.Range (minControlLength, maxControlLength) * height;
		Debug.Log ("controlLenght 2: " + controlLength / height);
		controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (Random.Range(0, 2) == 0 ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
	}
	#endregion
}
