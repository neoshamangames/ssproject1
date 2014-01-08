using UnityEngine;
using System.Collections;
using Vectrosity;

public class Plant : MonoBehaviour {

	#region Attributes
	public Transform pointMarker;
	public CameraManager cam;
	public bool drawDebugMarks;
	public bool drawPoints;
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
	
	[Range(0, 50)] public float maxGrowthPerSecond = .1f;
	[Range(0,1)]public float healthyGrowthFactor = .5f;
	[Range(0,1)]public float unHealthyGrowthFactor = .25f;
	[Range(0, 5)]public float stateTransitionSeconds = 1f;
	[Range(5, 1000)]public int segmentsPerScreen = 50;
	[Range(0, 500)]public float lineWidth = 1f;
	[Range(.1f, 3f)]public float glowLineWidthScaler = 1.5f;
//	[Range(0f, 1f)]public float widthGrowFactor = 0f;
	[Range(1f, 10000f)]public float widthGrowStretch = 100f;
	[Range(.1f, 500)]public float maxWidth = 3f;
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
	[Range(0, 10)]public float widthScaler = 1f;
	[Range(0, 1)]public float veryHealthyRange = .05f;
	[Range(0, 1)]public float healthyRange = .125f;
	[Range(0, 1)]public float aliveRange = .35f;
	public float maxDisplayableWidth = 20f;
	public float waterPerSecond = .001f;
	public float dryPerSecond = .00001f;
	#endregion
	
	#region Properties
	public Vector3 TopPosisiton
	{
		get { return line.points3[line.drawEnd]; }
	}
	
	public Vector3 BasePosisiton
	{
		get { return line.points3[0] + transform.position; }
	}
	#endregion

	#region Unity
	void Start()
	{
		//setup camera
		mainCam = Camera.main;
		depth = transform.position.z;
		initialOneUnit = mainCam.WorldToScreenPoint(new Vector3(1, 0, depth)).x - mainCam.WorldToScreenPoint(new Vector3(0, 0, depth)).x;
		Debug.Log("depth: " + depth);
		screenHeight = CameraManager.Instance.Height;
		line = new VectorLine ("Plant", new Vector3[MAX_POINTS - 2], veryHealthyColor, normalMaterial, lineWidth, LineType.Continuous, Joins.Weld);
//		line.vectorObject.transform.position = transform.position;
		glowAlphaColor = new Color(veryHealthyColor.r, veryHealthyColor.g, veryHealthyColor.b, 0);
		glowLine = new VectorLine ("PlantGlow", new Vector3[MAX_POINTS - 2], glowAlphaColor, glowMaterial, lineWidth * glowLineWidthScaler, LineType.Continuous, Joins.Weld);
		glowLine.active = true;
		controlLine1 = new VectorLine ("Control Line 1", new Vector3[2], Color.red, null, 1f);
		controlLine2 = new VectorLine ("Control Line 2", new Vector3[2], Color.red, null, 1f);
		curvePoints = new Vector3[4];
		startPoint.x = transform.position.x;
		startPoint.y = transform.position.y;
		startPoint.z = depth;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		finishPoint.z = depth;

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
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
		glowLine.Draw3D();
		line.Draw3D();
		points = new VectorPoints("Points", new Vector3[]{lowPoint, highPoint}, dotMaterial, 2.0f);
		//points.Draw();
		
		witheredThreshold = OPTIMUM_SATURATION - aliveRange/2;
		witheringThreshold = OPTIMUM_SATURATION - healthyRange/2;
		healthyDryThreshold = OPTIMUM_SATURATION - veryHealthyRange/2;
		healthyWetThreshold = OPTIMUM_SATURATION + veryHealthyRange/2;
		drowningThreshold = OPTIMUM_SATURATION + healthyRange/2;
		drownedThreshold = OPTIMUM_SATURATION + aliveRange/2;
	}

	void Update()
	{
		if (state != PlantState.Dead)
		{
			growth += Time.deltaTime * maxGrowthPerSecond * growthFactor;
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
				line.points3[intPart] = Vector3.Lerp(lowPoint, highPoint, DROP_BACK_PERCENT);
				lowPoint = highPoint;
				highPoint = line.points3[intPart + 1];
				endSegment = intPart + 1;
				line.drawEnd = endSegment;
				glowLine.drawEnd = endSegment;
				//VectorLine.Destroy(ref points);
				//points = new VectorPoints("Points", new Vector3[]{lowPoint, highPoint}, dotMaterial, 2.0f, 0);
				//points.Draw();
				
				//Debug.Log ("lowPoint: " + lowPoint);
				//Debug.Log ("highPoint: " + highPoint);
			}
			UpdateWidth();
//			AutoZoomOut();
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
	
			UpdateSaturation();
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
		glowLine.Draw3D();
		line.Draw3D();
	}

	/*
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
			for(int i=0; i < line.points3.Length; i++)
				Debug.Log("points[" + i + "]: " + line.points3[i]);
		}
			
	}
	*/
	#endregion
	public void Water()
	{
		saturation += Time.deltaTime * waterPerSecond;
	}
	
	#region Actions
	
	#endregion

	#region Private
	private const int MAX_POINTS = 16384;
	private const float DROP_BACK_PERCENT = .95f;
	private float growthFactor = 1f;
	
	private VectorLine line;
	private VectorLine glowLine;
	
	private Vector3 startPoint;
	private float screenHeight;
	private float growth = 0;
	private int endSegment = 1;
	private int lastSegment = 0;
	private Vector3 finishPoint;
	private float lastAngle;
	private bool lastControlPointFlipped;
	private bool splineSide;
	private bool glow;
	private Camera mainCam;
	private float depth;
	private float initialOneUnit;
	private float[] widths;
	
	private Vector3 lowPoint;
	private Vector3 highPoint;
	
	//bezier curve
	private VectorLine controlLine1;
	private VectorLine controlLine2;
	private float controlLength;
	private float controlLength2;
	private Vector3[] curvePoints;
	
	VectorPoints points;
	private const float OPTIMUM_SATURATION = .5f;
	private float saturation = OPTIMUM_SATURATION;
	private float healthyWetThreshold;
	private float healthyDryThreshold;
	private float drowningThreshold;
	private float drownedThreshold;
	private float witheringThreshold;
	private float witheredThreshold;
	
	//color transition
	private enum PlantState {Dead, Drowning, HealthyWet, VeryHealthy, HealthyDry, Withering};
	private PlantState state = PlantState.VeryHealthy;
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
		finishPoint.z = depth;
		AddCurve();
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
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
		Vector3[] splinePoints = new Vector3[3];;
		splinePoints[0] = startPoint;
		float splineCurveHeight = Random.Range(minSplineHeight, maxSplineHeight);
		float upperYCoord = splineCurveHeight * screenHeight + startPoint.y;
		splineSide = !splineSide;
		splinePoints[1] = new Vector3(startPoint.x + Random.Range(minSplineOffset, maxSplineOffset) * screenHeight * (splineSide ? 1 : -1), 
		                              Random.Range(startPoint.y + minSplineYSeperation * splineCurveHeight * screenHeight, upperYCoord - minSplineYSeperation * splineCurveHeight * screenHeight));
		finishPoint = new Vector3(startPoint.x, upperYCoord);
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
			controlLength = Random.Range (minBezierControlLength, maxBezierControlLength) * screenHeight;
		} else {
			controlLength = controlLength2;
			angle = lastAngle;
			flipControlPoint = !lastControlPointFlipped;
		}
		lastControlPointFlipped = flipControlPoint;
//		Debug.Log ("flipControlPoint: " + flipControlPoint);
//		Debug.Log ("angle 1: " + angle);
//		Debug.Log ("controlLength 1: " + controlLength / screenHeight);
		Vector3 controlPointOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength, depth);
//		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		curvePoints [1].x = startPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints [1].y = startPoint.y + controlPointOffset.y;
		curvePoints[1].z = depth;
		float bezierCurveHeight =  Random.Range(minBezierCurveHeight, maxBezierCurveHeight);
		finishPoint = new Vector3(startPoint.x, startPoint.y + bezierCurveHeight * screenHeight, depth);
		curvePoints[2] = finishPoint;
		angle = Random.Range (minBezierAngle, maxBezierAngle);
		lastAngle = angle;
//		Debug.Log ("angle 2: " + angle);
		controlLength2 = Random.Range (minBezierControlLength, maxBezierControlLength) * screenHeight;
//		Debug.Log ("controlLenght 2: " + controlLength / screenHeight);
		controlPointOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength2, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength2, depth);
//		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
		curvePoints[3].z = depth;
		int segments = Mathf.RoundToInt(segmentsPerScreen * bezierCurveHeight);
//		Debug.Log("segments: " + segments);
		line.MakeCurve (curvePoints, segments, lastSegment);
		glowLine.points3 = line.points3;
		
		if (drawDebugMarks)
		{
			controlLine1.points3 = new Vector3[] {startPoint, new Vector3(curvePoints [1].x, curvePoints [1].y, depth)};
			controlLine2.points3 = new Vector3[] {finishPoint, new Vector3(curvePoints [3].x, curvePoints [3].y, depth)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
		if (drawPoints)
		{
			for(int i=lastSegment; i < lastSegment + segments; i++)
			{
				GameObject marker = Instantiate(pointMarker, line.points3[i], transform.rotation) as GameObject;
				//				marker.transform.position = Vector3.zero;
			}
		}
		
		
		lastSegment += segments;
	}

	private void TransitionState(PlantState newState)
	{
		if (state != newState)
		{
			state = newState;
			if (state == PlantState.VeryHealthy)
				SetGlow(true);
			else
				SetGlow(false);
				
			switch (state)
			{
			case PlantState.Dead:
				targetColor = deadColor;
				growthFactor = 0;
				break;
			case PlantState.Drowning:
				targetColor = drowningColor;
				growthFactor = unHealthyGrowthFactor;
				break;
			case PlantState.Withering:
				targetColor = witheringColor;
				growthFactor = unHealthyGrowthFactor;
				break;
			case PlantState.HealthyDry:
			case PlantState.HealthyWet:
				targetColor = healthyColor;
				growthFactor = healthyGrowthFactor;
				break;
			case PlantState.VeryHealthy:
				targetColor = veryHealthyColor;
				growthFactor = 1;
				break;
			}
			
			previousColor = line.color;
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
	
	private float GetWidth(int i)
	{
		return Mathf.Atan((float)i/widthGrowStretch) * maxWidth * 2/Mathf.PI + lineWidth;
	}
	
	private void UpdateWidth()
	{
		widths = new float[line.points3.Length - 1];
		float[] glowWidths = new float[line.points3.Length - 1];
		float oneUnit = mainCam.WorldToScreenPoint(new Vector3(1, 0, depth)).x - mainCam.WorldToScreenPoint(new Vector3(0, 0, depth)).x;
//		float widthScaler = oneUnit/initialOneUnit;
//		float widest = lineWidth + lineWidth * growth * widthGrowFactor;
		
		int max = line.drawEnd;
		for(int i=0; i <= (float)max; i++)
		{
//			widths[(int)i] = Mathf.Lerp(widest, lineWidth, (float)i/max);
//			widths[i] = 5f;
//			widths[i] = Mathf.Clamp(Mathf.Lerp(widest, lineWidth, (float)i/max), 0, maxWidth);
			widths[max - i] = GetWidth(i);
			glowWidths[i] = widths[i] * glowLineWidthScaler;
		}
//		Debug.Log("maxWidth: " + widths[0]);
		float glowLineWidth = lineWidth * glowLineWidthScaler;
		for(int i=(int)max + 1; i< widths.Length; i++)
		{
			widths[i] = lineWidth;
			glowWidths[i] = glowLineWidth;
		}
		line.SetWidths(widths);
		glowLine.SetWidths(glowWidths);
	}
	
	private void UpdateSaturation()
	{
		saturation -= dryPerSecond * Time.deltaTime;
//		Debug.Log("saturation: " + saturation);
		switch (state)
		{
			case PlantState.Drowning:
				if (saturation > drownedThreshold)
				{
					TransitionState(PlantState.Dead);	
				}
				if (saturation < drowningThreshold)
				{
					TransitionState(PlantState.HealthyWet);	
				}
			break;
		
			case PlantState.HealthyWet:
				if (saturation > drowningThreshold)
				{
					TransitionState(PlantState.Drowning);	
				}
				if (saturation < healthyWetThreshold)
				{
					TransitionState(PlantState.VeryHealthy);	
				}
				break;
		
			case PlantState.VeryHealthy:
				if (saturation > healthyWetThreshold)
				{
					TransitionState(PlantState.HealthyWet);	
				}
				
				if (saturation < healthyDryThreshold)
				{
					TransitionState(PlantState.HealthyDry);	
				}
				
				break;
			
			case PlantState.HealthyDry:
				if (saturation > healthyDryThreshold)
				{
					TransitionState(PlantState.VeryHealthy);	
				}
				if (saturation < witheringThreshold)
				{
					TransitionState(PlantState.Withering);	
				}
				break;
			
			
			case PlantState.Withering:
				if (saturation > witheringThreshold)
				{
					TransitionState(PlantState.HealthyDry);	
				}
				if (saturation < witheredThreshold)
				{
					TransitionState(PlantState.Dead);	
				}
			
				break;
		}
	}
	
	private void AutoZoomOut()
	{
		float screenBottomEdge = mainCam.ViewportToWorldPoint(new Vector3(.5f, 0f, -depth)).y;
		//		Debug.Log("screenBottomEdge: " + screenBottomEdge);
		int drawEnd = line.drawEnd;
		//		Debug.Log("line.points3[drawEnd - 1].y: " + line.points3[drawEnd - 1].y);
		float greatestWidth = 0f;
		for(int i=0; i < line.drawEnd; i++)
		{
			if(line.points3[i].y >= screenBottomEdge)
			{
				greatestWidth = widths[i];
				break;
			}
		}
		Debug.Log("greatestWidth: " + greatestWidth);
		if (greatestWidth > maxDisplayableWidth)
			cam.ZoomOut();
	}
	#endregion
}
