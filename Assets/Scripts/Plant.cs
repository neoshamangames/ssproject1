using UnityEngine;
using System.Collections;
using Vectrosity;

public class Plant : MonoBehaviour {

	#region Attributes
	public bool drawDebugMarks;
	public bool bezierCurves;
	[Range(.1f, 100)] public float lineWidth = 1f;
	public Material normalMaterial;
	public Material glowMaterial;
	public Material dotMaterial;
	public Texture2D backCapTexture;
	public Texture2D frontCapTexture;
	public Color healthyColor;
	public Color veryHealthyColor;
	public Color drowningColor;
	public Color witheringColor;
	public Color deadColor;
	
	[Range(0, 50)] public float growSegmentsPerSecond = .1f;
	[Range(0, 5)]public float stateTransitionSeconds = 1f;
	[Range(5, 1000)]public int segmentsPerScreen = 50;
	[Range(0, 45)] public float minBezierAngle = 5f;
	[Range(0, 45)] public float maxBezierAngle = 30f;
	[Range(0, 1)] public float minBezierControlLength = .05f;
	[Range(0, 1)] public float maxBezierControlLength = .1f;
	[Range(0, 1)]public float minBezierCurveHeight = .1f;
	[Range(0, 1)]public float maxBezierCurveHeight = .5f;
	[Range(5, 1000)]public int segmentsPerSpline = 50;
	[Range(0, 1)]public float minSplineHeight = .1f;
	[Range(0, 1)]public float maxSplineHeight = .5f;
	[Range(0, 1)]public float minSplineOffset = 0f;
	[Range(0, 1)]public float maxSplineOffset = .25f;
	[Range(0, 1)]public float minSplineYSeperation = .25f;
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
		line = new VectorLine ("Plant", new Vector2[MAX_POINTS - 2], healthyColor, normalMaterial, lineWidth, LineType.Continuous, Joins.Weld);
		controlLine1 = new VectorLine ("Control Line 1", new Vector2[2], Color.red, null, .02f);
		controlLine2 = new VectorLine ("Control Line 2", new Vector2[2], Color.red, null, .02f);
		curvePoints = new Vector2[4];
		startPoint.x = transform.position.x;
		startPoint.y = transform.position.y;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;

		//End Cap
		VectorLine.SetEndCap ("Point", EndCap.Back, normalMaterial, frontCapTexture);
		line.endCap = "Point";
		line.drawStart = 0;
		line.drawEnd = endSegment;
		
		splineSide = (Random.Range(0, 2) == 0);

		//DrawPlant ();
		AddCurve ();
		line.Draw();
	}

	void Update()
	{
		bool redraw = false;
		growth += Time.deltaTime * growSegmentsPerSecond;
		int newSegment = Mathf.FloorToInt (growth);

		if (newSegment > endSegment) {
			if (newSegment > lastSegment)
			{
				AddCurve();
			}
			endSegment = newSegment;
			line.drawEnd = endSegment;
			redraw = true;
			//Debug.Log ("endSegment: " + endSegment);
		}
		if (transitioning)
		{
			transitionTimer += Time.deltaTime;
			float t;
			if (transitionTimer < stateTransitionSeconds)
			{
				t = transitionTimer/stateTransitionSeconds;
				line.SetColor(Color.Lerp(previousColor, targetColor, t));
			}
			else
			{
				line.SetColor(targetColor);
				transitioning = false;
			}
		}
		if (redraw)
			line.Draw();
	}

	void OnGUI()
	{
		if (GUI.Button (new Rect (10, 30, 90, 30), "Healthy")) {
			TransitionColor (healthyColor);
			SetGlow(false);
		}
		if (GUI.Button (new Rect (10, 70, 90, 30), "Very Healthy"))
		{
			TransitionColor (veryHealthyColor);
			SetGlow(true);
		}
		if (GUI.Button (new Rect (10, 110, 90, 30), "Drowning"))
		{
			TransitionColor (drowningColor);
			SetGlow(false);
			
		}
		if (GUI.Button (new Rect (10, 150, 90, 30), "Withering"))
		{
			TransitionColor (witheringColor);
			SetGlow(false);
		}
		if (GUI.Button (new Rect (10, 190, 90, 30), "Dead"))
		{
			TransitionColor (deadColor);
			SetGlow(false);
		}
		if (GUI.Button (new Rect (10, 230, 90, 30), "Reset Plant"))
			Restart();
			
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
	private float lastAngle;
	private bool lastControlPointFlipped;
	private Color targetColor;
	private Color previousColor;
	private bool glow;
	private bool transitioning;
	private float transitionTimer;
	private bool splineSide;

	private void Restart()
	{
		growth = 0;
		endSegment = 1;
		lastSegment = 0;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		AddCurve();
	}

	private void AddCurve()
	{		
		if (bezierCurves)
			AddBezierCurve();
		else
			AddSpline();
	}
	
	private void AddSpline()
	{
		startPoint = finishPoint;
		Vector2[] splinePoints = new Vector2[3];;
		splinePoints[0] = startPoint;
		float splineCurveHeight = Random.Range(minSplineHeight, maxSplineHeight);
		float upperYCoord = splineCurveHeight * height + startPoint.y;
		splineSide = !splineSide;
		splinePoints[1] = new Vector2(startPoint.x + Random.Range(minSplineOffset, maxSplineOffset) * height * (splineSide ? 1 : -1), 
		                              Random.Range(startPoint.y + minSplineYSeperation * splineCurveHeight * height, upperYCoord - minSplineYSeperation * splineCurveHeight * height));
		finishPoint = new Vector2(startPoint.x, upperYCoord);
		splinePoints[2] = finishPoint;
		
		if (drawDebugMarks)
		{
			VectorPoints points = new VectorPoints("Spline Marks", splinePoints, Color.red, dotMaterial, .5f);
			points.Draw();
		}
		
		int segments = Mathf.RoundToInt(segmentsPerScreen * splineCurveHeight);
		line.MakeSpline(splinePoints, segments, lastSegment);
		lastSegment += segments;
	}
	
	private void AddBezierCurve()
	{
		startPoint = finishPoint;
		curvePoints[0] = startPoint;
		float angle;
		bool flipControlPoint;
		if (lastSegment == 0) {
			angle = Random.Range (minBezierAngle, maxBezierAngle);
			flipControlPoint = (Random.Range(0, 2) == 0);
		} else {
			angle = lastAngle;
			flipControlPoint = !lastControlPointFlipped;
		}
		Debug.Log ("angle 1: " + angle);
		float controlLength = Random.Range (minBezierControlLength, maxBezierControlLength) * height;
		Debug.Log ("controlLength 1: " + controlLength / height);
		Vector2 controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		curvePoints [1].x = startPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints [1].y = startPoint.y + controlPointOffset.y;
		float bezierCurveHeight =  Random.Range(minBezierCurveHeight, maxBezierCurveHeight);
		finishPoint = new Vector2(startPoint.x, startPoint.y + bezierCurveHeight * height);
		curvePoints[2] = finishPoint;
		angle = Random.Range (minBezierAngle, maxBezierAngle);
		lastAngle = angle;
		Debug.Log ("angle 2: " + angle);
		controlLength = Random.Range (minBezierControlLength, maxBezierControlLength) * height;
		Debug.Log ("controlLenght 2: " + controlLength / height);
		controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		lastControlPointFlipped = (Random.Range (0, 2) == 0);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (lastControlPointFlipped ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
		int segments = Mathf.RoundToInt(segmentsPerScreen * bezierCurveHeight);
		Debug.Log("segments: " + segments);
		line.MakeCurve (curvePoints, segments, lastSegment);
		lastSegment += segments;
		
		if (drawDebugMarks)
		{
			controlLine1.points2 = new Vector2[] {startPoint, new Vector2 (curvePoints [1].x, curvePoints [1].y)};
			controlLine2.points2 = new Vector2[] {finishPoint, new Vector2(curvePoints [3].x, curvePoints [3].y)};
			controlLine1.Draw();
			controlLine2.Draw();
		}
	}

	private void TransitionColor(Color newColor)
	{
		if (line.color != newColor)
		{
			previousColor = line.color;
			targetColor = newColor;
			transitioning = true;
			transitionTimer = 0;
		}
	}
	
	private void SetGlow(bool newGlow)
	{
		if (glow != newGlow)
		{
			glow = newGlow;
			if (glow)
			{
				line.material = glowMaterial;
				//line.joins = Joins.None;
			}
			else
			{
				line.material = normalMaterial;
				//line.joins = Joins.Weld;
			}
		}
	}
	#endregion
}
