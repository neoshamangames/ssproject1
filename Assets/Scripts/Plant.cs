using UnityEngine;
using System.Collections;
using Vectrosity;

public class Plant : MonoBehaviour {

	#region Attributes
	public bool drawDebugMarks;
	public bool bezierCurves;
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
	[Range(.1f, 20)]public float lineWidth = 1f;
	[Range(.1f, 3f)]public float glowLineWidthScaler = 1.5f;
	[Range(0f, 1f)]public float widthGrowFactor = 0f;
	[Range(.1f, 20)]public float maxWidth = 3f;
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
	
	#region Properties
	public float Height
	{
		get { return line.points2[line.drawEnd].y; }
	}
	#endregion

	#region Unity
	void Awake()
	{
		//setup camera
		screenHeight = CameraManager.Instance.Height;
		line = new VectorLine ("Plant", new Vector2[MAX_POINTS - 2], healthyColor, normalMaterial, lineWidth, LineType.Continuous, Joins.Weld);
		glowAlphaColor = new Color(veryHealthyColor.r, veryHealthyColor.g, veryHealthyColor.b, 0);
		glowLine = new VectorLine ("PlantGlow", new Vector2[MAX_POINTS - 2], glowAlphaColor, glowMaterial, lineWidth * glowLineWidthScaler, LineType.Continuous, Joins.Weld);
		glowLine.active = false;
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
		UpdateWidth();
		line.smoothWidth = true;
		
		splineSide = (Random.Range(0, 2) == 0);

		//DrawPlant ();
		AddCurve();
		lowPoint = line.points2[0];
		highPoint = line.points2[1];
		glowLine.Draw();
		line.Draw();
		points = new VectorPoints("Points", new Vector2[]{lowPoint, highPoint}, dotMaterial, 2.0f);
		//points.Draw();
	}

	void Update()
	{
		growth += Time.deltaTime * growSegmentsPerSecond;
		int intPart = Mathf.FloorToInt(growth);
		float decPart = growth % 1;

		if (intPart >= endSegment) {
			//Debug.Log("growth: " + growth + ". intPart: " + intPart + ". decPart: " + decPart);
//			if (decPart < SKIP_TO_DEC)
//			{
//				decPart = SKIP_TO_DEC;
//				growth = intPart + decPart;
//			}
			if (intPart == lastSegment)
			{
				AddCurve();
			}
			line.points2[intPart] = Vector2.Lerp(lowPoint, highPoint, DROP_BACK_PERCENT);;
			lowPoint = highPoint;
			highPoint = line.points2[intPart + 1];
			endSegment = intPart + 1;
			line.drawEnd = endSegment;
			glowLine.drawEnd = endSegment;
			//VectorLine.Destroy(ref points);
			//points = new VectorPoints("Points", new Vector2[]{lowPoint, highPoint}, dotMaterial, 2.0f);
			//points.Draw();
			
			//Debug.Log ("lowPoint: " + lowPoint);
			//Debug.Log ("highPoint: " + highPoint);
		}
		UpdateWidth();
		line.points2[intPart + 1] = Vector2.Lerp(lowPoint, highPoint, decPart);

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
		if (glowTransitioning)
		{
			glowTransitionTimer += Time.deltaTime;
			float t;
			if (glowTransitionTimer < stateTransitionSeconds)
			{
				t = glowTransitionTimer/stateTransitionSeconds;
				glowLine.SetColor(Color.Lerp(previousGlowColor, targetGlowColor, t));
			}
			else
			{
				glowLine.SetColor(targetGlowColor);
				glowTransitioning = false;
				if (glowLine.color != veryHealthyColor)
					glowLine.active = false;
			}
		}
		glowLine.Draw();
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
			
		if (GUI.Button (new Rect (10, 270, 90, 30), "Print Points"))
		{
			for(int i=0; i < line.points2.Length; i++)
				Debug.Log("points[" + i + "]: " + line.points2[i]);
		}
			
	}
	#endregion

	#region Private
	private const int MAX_POINTS = 16384;
	private const float DROP_BACK_PERCENT = .95f;
	private Vector2[] curvePoints;
	private VectorLine line;
	private VectorLine glowLine;
	private VectorLine controlLine1;
	private VectorLine controlLine2;
	private Vector2 startPoint;
	private float screenHeight;
	private float growth = 0;
	private int endSegment = 1;
	private int lastSegment = 0;
	private Vector2 finishPoint;
	private float lastAngle;
	private bool lastControlPointFlipped;
	private bool splineSide;
	private bool glow;
	
	private Vector2 lowPoint;
	private Vector2 highPoint;
	
	VectorPoints points;
	
	//color transition
	private bool transitioning;
	private Color targetColor;
	private Color previousColor;
	private float transitionTimer;
	
	
	//glow transition
	private bool glowTransitioning;
	private Color targetGlowColor;
	private Color previousGlowColor;
	private float glowTransitionTimer;
	private Color glowAlphaColor;
	
	private void Restart()
	{
		growth = 0;
		endSegment = 1;
		line.drawEnd = 1;
		lastSegment = 0;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		AddCurve();
		lowPoint = line.points2[0];
		highPoint = line.points2[1];
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
		float upperYCoord = splineCurveHeight * screenHeight + startPoint.y;
		splineSide = !splineSide;
		splinePoints[1] = new Vector2(startPoint.x + Random.Range(minSplineOffset, maxSplineOffset) * screenHeight * (splineSide ? 1 : -1), 
		                              Random.Range(startPoint.y + minSplineYSeperation * splineCurveHeight * screenHeight, upperYCoord - minSplineYSeperation * splineCurveHeight * screenHeight));
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
		lastControlPointFlipped = flipControlPoint;
		Debug.Log ("flipControlPoint: " + flipControlPoint);
		Debug.Log ("angle 1: " + angle);
		float controlLength = Random.Range (minBezierControlLength, maxBezierControlLength) * screenHeight;
		Debug.Log ("controlLength 1: " + controlLength / screenHeight);
		Vector2 controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		curvePoints [1].x = startPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints [1].y = startPoint.y + controlPointOffset.y;
		float bezierCurveHeight =  Random.Range(minBezierCurveHeight, maxBezierCurveHeight);
		finishPoint = new Vector2(startPoint.x, startPoint.y + bezierCurveHeight * screenHeight);
		curvePoints[2] = finishPoint;
		angle = Random.Range (minBezierAngle, maxBezierAngle);
		lastAngle = angle;
		Debug.Log ("angle 2: " + angle);
		controlLength = Random.Range (minBezierControlLength, maxBezierControlLength) * screenHeight;
		Debug.Log ("controlLenght 2: " + controlLength / screenHeight);
		controlPointOffset = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength);
		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
		int segments = Mathf.RoundToInt(segmentsPerScreen * bezierCurveHeight);
		Debug.Log("segments: " + segments);
		line.MakeCurve (curvePoints, segments, lastSegment);
		glowLine.points2 = line.points2;
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
				targetGlowColor = veryHealthyColor;
				glowLine.active = true;
			}
			else
			{
				targetGlowColor = glowAlphaColor;
			}
		}
		previousGlowColor = glowLine.color;
		glowTransitioning = true;
		glowTransitionTimer = 0;
	}
	
	private void UpdateWidth()
	{
		float[] widths = new float[line.points2.Length - 1];
		float[] glowWidths = new float[line.points2.Length - 1];
		float widest = lineWidth + growth * widthGrowFactor;
		float max = line.drawEnd;
		for(int i=0; i < max; i++)
		{
			//widths[(int)i] = Mathf.Lerp(widest, lineWidth, (float)i/max);
			widths[(int)i] = Mathf.Clamp(Mathf.Lerp(widest, lineWidth, (float)i/max), 0, maxWidth);
			glowWidths[i] = Mathf.Clamp(widths[i] * glowLineWidthScaler, 0, maxWidth * glowLineWidthScaler);
		}
		float glowLineWidth = lineWidth * glowLineWidthScaler;
		for(int i=(int)max; i< widths.Length; i++)
		{
			widths[i] = lineWidth;
			glowWidths[i] = glowLineWidth;
		}
		line.SetWidths(widths);
		glowLine.SetWidths(glowWidths);
	}
	#endregion
}
