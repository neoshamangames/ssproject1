using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Plant : MonoBehaviour {

	#region Attributes
	public Transform pointMarker;
	public CameraManager cam;
	public GameObject flowerPrefab;
	public bool drawDebugMarks;
	public bool drawPoints;
	public bool bezierCurves;
	public bool useDebugSettings;
	
	[System.Serializable]
	public class Settings
	{
		public GrowthAttributes growth;
		public StemmingAttributes stemming;
	}
	
	[System.Serializable]
	public class AppearanceAttributes {
		[Range(0, 500)]public float minWidth = 1f;
		[Range(.1f, 500)]public float maxWidth = 30f;	
		[Range(1f, 10000f)]public float widthGrowStretch = 1000f;
		[Range(.1f, 3f)]public float glowLineWidthScaler = .1f;
		[Range(0, 5)]public float stateTransitionSeconds = 1f;
		public Material normalMaterial;
		public Material glowMaterial;
		public Material dotMaterial;
		public Texture2D frontCapTexture;
		public Color healthyColor;
		public Color veryHealthyColor;
		public Color drowningColor;
		public Color witheringColor;
		public Color deadColor;
	}
	
	[System.Serializable]
	public class GrowthAttributes {
		[Range(0, 50)] public float maxGrowthPerSecond = 1f;
		[Range(5, 1000)]public int segmentsPerScreen = 500;
		[Range(0,1)]public float healthyGrowthFactor = .5f;
		[Range(0,1)]public float unHealthyGrowthFactor = .25f;
		[Range(0, 1)]public float veryHealthyRange = .05f;
		[Range(0, 1)]public float healthyRange = .125f;
		[Range(0, 1)]public float aliveRange = .35f;
		public float waterPerSecond = .01f;
		public float dryPerSecond = .001f;
	}
	
	[System.Serializable]
	public class BezierCurveAttributes {
		[Range(0, 45)] public float minAngle = 30f;
		[Range(0, 45)] public float maxAngle = 45f;
		[Range(0, 1)] public float minControlLength = .025f;
		[Range(0, 1)] public float maxControlLength = .05f;
		[Range(0, 1)]public float minCurveHeight = .1f;
		[Range(0, 1)]public float maxCurveHeight = .45f;
	}
	
	[System.Serializable]
	public class SplineAttributes {
		[Range(0, 1)]public float minHeight = .1f;
		[Range(0, 1)]public float maxHeight = .5f;
		[Range(0, 1)]public float minOffset = 0f;
		[Range(0, 1)]public float maxOffset = .25f;
		[Range(0, 1)]public float minYSeperation = .25f;
	}
	
	[System.Serializable]
	public class StemmingAttributes {
		[Range(5, 100)]public int segmentsPer = 15;
		[Range(0, 50)] public float maxGrowthPerSecond = .05f;
		[Range(1, 10)]public float maxWidth = 1f;
		[Range(0, 50)] public float widthGrowth = .05f;
		[Range(0, 1000)]public int startingPlantHeight = 250;
		[Range(0, 1000)]public int minSeperation = 50;
		[Range(0, 1000)]public int maxSeperation = 150;
		[Range(.1f, 3)]public float minCurveWidth = .5f;
		[Range(.1f, 3)]public float maxCurveWidth = 1.5f;
		[Range(-2, 2)]public float minCurveHeight = -.2f;
		[Range(-2, 2)]public float maxCurveHeight = .4f;
		[Range(0, 90)] public float minControlAngle1 = 0f;
		[Range(0, 90)] public float maxControlAngle1 = 45f;
		[Range(0, 90)] public float minControlAngle2 = 45;
		[Range(0, 90)] public float maxControlAngle2 = 90f;
		[Range(0, 50)] public float minControlLength = 10f;
		[Range(0, 50)] public float maxControlLength = 10f;
		[Range(0, 2)]public float minFlowerSize = 0f;
		[Range(0, 2)]public float maxFlowerSize = .75f;
		[Range(0, 1)]public float flowerGrowthFactor = 1f;
		public float buddingTime = 1f;
		public float harvestingTime = 1f;
		public float flowerDelay;
		public float newFlowerDelay;
		public float minDeathTime = 10000f;
		public float maxDeathTime = 100000f;
		public float fallingGravity = .05f;
		public float fallingHorizontalMovement = -2f;
	}
	
	public Settings debugSettings;
	public Settings releaseSettings;
	public AppearanceAttributes appearance;
	public BezierCurveAttributes bezier;
	public SplineAttributes spline;
	
	#endregion
	
	#region Properties
	public Vector3 TopPosisiton
	{
		get { return line.points3[line.drawEnd]; }
	}
	
	public Vector3 BasePosisiton
	{
		get { return line.points3[0]; }
	}
	#endregion

	#region Unity
	void Start()
	{
		//settings
		if (useDebugSettings)
		{
			growth = debugSettings.growth;
			stemming = debugSettings.stemming;
		}
		else
		{
			growth = releaseSettings.growth;
			stemming = releaseSettings.stemming;
		}
	
		//setup camera
		depth = transform.position.z;
		line = new VectorLine ("Plant", new Vector3[MAX_POINTS - 2], appearance.veryHealthyColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		line.depth = 1;
		glowAlphaColor = new Color(appearance.veryHealthyColor.r, appearance.veryHealthyColor.g, appearance.veryHealthyColor.b, 0);
		glowLine = new VectorLine ("PlantGlow", new Vector3[MAX_POINTS - 2], glowAlphaColor, appearance.glowMaterial, appearance.minWidth * appearance.glowLineWidthScaler, LineType.Continuous, Joins.Weld);
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
		VectorLine.SetEndCap ("Point", EndCap.Back, appearance.normalMaterial, appearance.frontCapTexture);
		line.endCap = "Point";
		line.drawStart = 0;
		line.drawEnd = endSegment;
		line.smoothWidth = true;
		UpdateWidth();
		
		splineSide = (Random.Range(0, 2) == 0);

		AddCurve();
		lowPoint = line.points3[0];
		highPoint = line.points3[2];
		glowLine.Draw3D();
		line.Draw3D();
		
		//appearance
		stateTransitionSeconds = appearance.stateTransitionSeconds;
		
		witheredThreshold = OPTIMUM_SATURATION - growth.aliveRange/2;
		witheringThreshold = OPTIMUM_SATURATION - growth.healthyRange/2;
		healthyDryThreshold = OPTIMUM_SATURATION - growth.veryHealthyRange/2;
		healthyWetThreshold = OPTIMUM_SATURATION + growth.veryHealthyRange/2;
		drowningThreshold = OPTIMUM_SATURATION + growth.healthyRange/2;
		drownedThreshold = OPTIMUM_SATURATION + growth.aliveRange/2;
		
		//stemming
		stemSide = (Random.Range(0, 2) == 0);
		stemHeight = stemming.startingPlantHeight;
		stems = new List<Stem>();
		stemSegmentsPer = stemming.segmentsPer;
		targetColor = appearance.veryHealthyColor;
		stemParent = new GameObject();
		stemParent.name = "stemParent";
	}

	void Update()
	{
		float deltaTime = Time.deltaTime;
		if (state != PlantState.Dead)
		{
			height += deltaTime * growth.maxGrowthPerSecond * growthFactor;
//			Debug.Log ("growth: " + growth);
			int intPart = Mathf.FloorToInt(height);
			float decPart = height % 1;
	
			if (intPart >= endSegment) {
			
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
			
			if (state == PlantState.Withering || state == PlantState.Withering)
			{
				unhealthyTimer += deltaTime;
				if (unhealthyTimer > stemDeathTime)
				{
					KillStem();
				}
			}
			
			
			UpdateWidth();
			UpdateStems();
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
	
			UpdateSaturation();
		}

		if (transitioning)
		{
			transitionTimer += deltaTime;
			if (transitionTimer < stateTransitionSeconds)
			{
				float t = transitionTimer/stateTransitionSeconds;
				Color newColor = Color.Lerp(previousColor, targetColor, t);
				line.SetColor(newColor);
				SetStemColor(newColor);
			}
			else
			{
				line.SetColor(targetColor);
				SetStemColor(targetColor);
				transitioning = false;
			}
		}
		if (glowTransitioning)
		{
			glowTransitionTimer += deltaTime;
			if (glowTransitionTimer < stateTransitionSeconds)
			{
				float t = glowTransitionTimer/stateTransitionSeconds;
				glowLine.SetColor(Color.Lerp(previousGlowColor, targetGlowColor, t));
			}
			else
			{
				glowLine.SetColor(targetGlowColor);
				glowTransitioning = false;
				if (glowLine.color != appearance.veryHealthyColor)
					glowLine.active = false;
			}
		}
		
		if (stemDying)
		{
			stemDyingTimer += deltaTime;
			if (stemDyingTimer < stateTransitionSeconds)
			{
				float t = stemDyingTimer/stateTransitionSeconds;
				stems[dyingStemIndex].SetColor(Color.Lerp(targetColor, appearance.deadColor, t));
			}
			else
			{
				float fallingTime = stemDyingTimer - stateTransitionSeconds;
				//dyingStemTransform.Translate(stemXMovement * deltaTime, stemming.fallingGravity * fallingTime, 0);
				Vector3 translate = new Vector3(stemXMovement * deltaTime, stemming.fallingGravity * fallingTime, 0);
				dyingStemTransform.localPosition += translate;
//				dyingStemTransform.Rotate(Vector3.forward, stemRotation * deltaTime);
				if (stems[dyingStemIndex].line.vectorObject.transform.position.y < STEM_REMOVE_HEIGHT)
				{
					RemoveStem();
				}
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
	
	#region Actions
	public void Water()
	{
		saturation += Time.deltaTime * growth.waterPerSecond;
	}
	#endregion

	#region Private	
	private const int MAX_POINTS = 16384;
	private const float DROP_BACK_PERCENT = .95f;
	private float HEIGHT_MULTIPLIER = 10f;
	private float growthFactor = 1f;
	
	private GrowthAttributes growth;
	private StemmingAttributes stemming;
	
	private VectorLine line;
	private VectorLine glowLine;
	
	private Vector3 startPoint;
	private float height = 0;
	private int endSegment = 1;
	private int lastSegment = 0;
	private Vector3 finishPoint;
	private float lastAngle;
	private bool lastControlPointFlipped;
	private bool splineSide;
	private bool glow;
	private float depth;
	private float[] widths;
	
	private Vector3 lowPoint;
	private Vector3 highPoint;
	
	//bezier curve
	private VectorLine controlLine1;
	private VectorLine controlLine2;
	private float controlLength;
	private float controlLength2;
	private Vector3[] curvePoints;
	
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
	
	//stemming
	private float STEM_REMOVE_HEIGHT = -20;
	private bool stemSide;
	private int stemHeight;
	private List<Stem> stems;
	private int stemSegmentsPer;
	private int stemWidthGrowth;
	private float stemDeathTime;
	private float unhealthyTimer;
	private bool stemDying;
	private int dyingStemIndex;
	private float stemDyingTimer;
	private float stemXMovement;
	private Transform dyingStemTransform;
	private GameObject stemParent;
	
	//appearanec
	private float stateTransitionSeconds;
	
	private void Restart()
	{
		height = 0;
		endSegment = 1;
		line.drawEnd = 1;
		lastSegment = 0;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		finishPoint.z = depth;
		AddCurve();
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
		stemSide = (Random.Range(0, 2) == 0);
		stemHeight = stemming.startingPlantHeight;
		stems = new List<Stem>();
		targetColor = appearance.veryHealthyColor;
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
		float splineCurveHeight = Random.Range(spline.minHeight, spline.maxHeight);
		float upperYCoord = splineCurveHeight * HEIGHT_MULTIPLIER + startPoint.y;
		splineSide = !splineSide;
		splinePoints[1] = new Vector3(startPoint.x + Random.Range(spline.minOffset, spline.maxOffset) * HEIGHT_MULTIPLIER * (splineSide ? 1 : -1), 
		                              Random.Range(startPoint.y + spline.minYSeperation * splineCurveHeight * HEIGHT_MULTIPLIER, upperYCoord - spline.minYSeperation * splineCurveHeight * HEIGHT_MULTIPLIER));
		finishPoint = new Vector3(startPoint.x, upperYCoord);
		splinePoints[2] = finishPoint;
		
		if (drawDebugMarks)
		{
			VectorPoints points = new VectorPoints("Spline Marks", splinePoints, Color.red, appearance.dotMaterial, .5f);
			points.Draw();
		}
		
		int segments = Mathf.RoundToInt(growth.segmentsPerScreen * splineCurveHeight);
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
			angle = Random.Range (bezier.minAngle, bezier.maxAngle);
			flipControlPoint = (Random.Range(0, 2) == 0);
			controlLength = Random.Range (bezier.minControlLength, bezier.maxControlLength) * HEIGHT_MULTIPLIER;
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
		float bezierCurveHeight =  Random.Range(bezier.minCurveHeight, bezier.maxCurveHeight);
		finishPoint = new Vector3(startPoint.x, startPoint.y + bezierCurveHeight * HEIGHT_MULTIPLIER, depth);
		curvePoints[2] = finishPoint;
		angle = Random.Range (bezier.minAngle, bezier.maxAngle);
		lastAngle = angle;
//		Debug.Log ("angle 2: " + angle);
		controlLength2 = Random.Range (bezier.minControlLength, bezier.maxControlLength) * HEIGHT_MULTIPLIER;
//		Debug.Log ("controlLenght 2: " + controlLength / screenHeight);
		controlPointOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * controlLength2, Mathf.Sin(angle * Mathf.Deg2Rad) * controlLength2, depth);
//		Debug.Log ("controlPointOffset 2 : " + controlPointOffset);
		curvePoints[3].x = finishPoint.x + controlPointOffset.x * (flipControlPoint ? 1 : -1);
		curvePoints[3].y = finishPoint.y - controlPointOffset.y;
		curvePoints[3].z = depth;
		int segments = Mathf.RoundToInt(growth.segmentsPerScreen * bezierCurveHeight);
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
				Instantiate(pointMarker, line.points3[i], transform.rotation);
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
				targetColor = appearance.deadColor;
				growthFactor = 0;
				break;
			case PlantState.Drowning:
				ResetStemDeath();
				targetColor = appearance.drowningColor;
				growthFactor = growth.unHealthyGrowthFactor;
				break; 
			case PlantState.Withering:
				ResetStemDeath();
				targetColor = appearance.witheringColor;
				growthFactor = growth.unHealthyGrowthFactor;
				break;
			case PlantState.HealthyDry:
			case PlantState.HealthyWet:
				targetColor = appearance.healthyColor;
				growthFactor = growth.healthyGrowthFactor;
				break;
			case PlantState.VeryHealthy:
				targetColor = appearance.veryHealthyColor;
				growthFactor = 1;
				break;
			}
			
			previousColor = line.color;
			transitioning = true;
			transitionTimer = 0;
		}
	}
	
	private void ResetStemDeath()
	{
		unhealthyTimer = 0;
		stemDeathTime = Random.Range(stemming.minDeathTime, stemming.maxDeathTime);
	}
	
	private void SetStemColor(Color color)
	{
		foreach(Stem stem in stems)
		{
			stem.SetColor(color);
		}
	}
	
	private void SetGlow(bool newGlow)
	{
		if (glow != newGlow)
		{
			glow = newGlow;
			if (glow)
			{
				targetGlowColor = appearance.veryHealthyColor;
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
		return Mathf.Atan((float)i/appearance.widthGrowStretch) * appearance.maxWidth * 2/Mathf.PI + appearance.minWidth;
	}
	
	private void UpdateWidth()
	{
		widths = new float[line.points3.Length - 1];
		float[] glowWidths = new float[line.points3.Length - 1];
		
		int max = line.drawEnd;
		for(int i=0; i <= (float)max; i++)
		{
//			widths[(int)i] = Mathf.Lerp(widest, lineWidth, (float)i/max);
//			widths[i] = 5f;
//			widths[i] = Mathf.Clamp(Mathf.Lerp(widest, lineWidth, (float)i/max), 0, maxWidth);
			widths[max - i] = GetWidth(i);
			glowWidths[i] = widths[i] * appearance.glowLineWidthScaler;
		}
//		Debug.Log("maxWidth: " + widths[0]);
		float glowLineWidth = appearance.minWidth * appearance.glowLineWidthScaler;
		for(int i=(int)max + 1; i< widths.Length; i++)
		{
			widths[i] = appearance.minWidth;
			glowWidths[i] = glowLineWidth;
		}
		line.SetWidths(widths);
		glowLine.SetWidths(glowWidths);
	}
	
	private void UpdateSaturation()
	{
		saturation -= growth.dryPerSecond * Time.deltaTime;
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
	
	private void UpdateStems()
	{
		float newGrowth = Time.deltaTime * stemming.maxGrowthPerSecond * growthFactor;
		
		foreach(Stem stem in stems)
		{	
			stem.Grow(newGrowth, widths[stem.height]);
			stem.line.Draw3D();
		}
		
		if (height > stemHeight)
		{
			NewStem();
			stemHeight += Random.Range(stemming.minSeperation, stemming.maxSeperation);
			stemSide = !stemSide;
		}
	}
	
	private void NewStem()
	{
		VectorLine stemLine = new VectorLine ("Stem", new Vector3[MAX_POINTS - 2], appearance.veryHealthyColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		stemLine.depth = 0;
		stemLine.endCap = "Point";
		stemLine.drawStart = 0;
		//		line.drawEnd = 0;
		stemLine.drawEnd = 1;
		line.smoothWidth = true;
		stemLine.lineWidth = 1f;//temp
		Vector3[] curve = new Vector3[4];
		Vector3 point = line.points3[stemHeight];
		curve[0] = point;
		curve[2] = point + new Vector3(Random.Range(stemming.minCurveWidth, stemming.maxCurveWidth) * (stemSide ? 1: -1), Random.Range(stemming.minCurveHeight, stemming.maxCurveHeight));
		float angle1 = Random.Range(stemming.minControlAngle1, stemming.maxControlAngle1) *  Mathf.Deg2Rad;
		float angle2 = Random.Range(stemming.minControlAngle2, stemming.maxControlAngle2) *  Mathf.Deg2Rad;
		float controlLength = Random.Range(stemming.minControlLength, stemming.maxControlLength);
		curve[1] = point + new Vector3(Mathf.Sin(angle1) * controlLength * (stemSide ? 1: -1), Mathf.Cos(angle1) * controlLength);
		curve[3] = curve[2] + new Vector3(Mathf.Sin(angle2) * controlLength * (stemSide ? 1: -1), Mathf.Cos(angle2) * controlLength);
		stemLine.MakeCurve (curve, stemming.segmentsPer, 0);
		GameObject flower = Instantiate(flowerPrefab) as GameObject;
		//Debug.Log ("instantiating new flower: " + flower);
		Stem newStem = new Stem(stemLine, stemHeight, stemSegmentsPer, stemming, appearance.minWidth, targetColor, stemSide, flower.GetComponent<Flower>());
		stems.Add(newStem);
		
		if (drawPoints)
		{
			for(int i=0; i < stemLine.points3.Length; i++)
			{
				Debug.Log ("i");
				Instantiate(pointMarker, stemLine.points3[i], transform.rotation);
				//				marker.transform.position = Vector3.zero;
			}
		}
		
		if (drawDebugMarks)
		{
			controlLine1.points3 = new Vector3[] {point, new Vector3(curve[1].x, curve[1].y, depth)};
			controlLine2.points3 = new Vector3[] {curve[2], new Vector3(curve[3].x, curve[3].y, depth)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
	}
	
	private void KillStem()
	{
		int numOfStems = stems.Count;
		if (!stemDying && numOfStems > 0)
		{
			stemDying = true;
			stemDyingTimer = 0;
			dyingStemIndex = Random.Range(0, numOfStems);
			Debug.Log("stem " + dyingStemIndex + "is dying");
			stemXMovement = stemming.fallingHorizontalMovement * (stems[dyingStemIndex].leftSide ? -1 : 1);
			dyingStemTransform = stems[dyingStemIndex].line.vectorObject.transform;
		}
		unhealthyTimer = 0;
	}
	
	private void RemoveStem()
	{
		stemDying = false;
		VectorLine.Destroy(ref stems[dyingStemIndex].line);
		stems.RemoveAt(dyingStemIndex);
	}
	#endregion
}
