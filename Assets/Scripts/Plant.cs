using UnityEngine;
using System.Linq;
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
		[Range(5, 1000)]public int widthSegments = 500;
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
		[Range(0, 10)] public float maxGrowthPerSecond = 1f;
		[Range(5, 1000)]public int segmentsPerScreen = 500;
		[Range(0,1)]public float healthyGrowthFactor = .5f;
		[Range(0,1)]public float unHealthyGrowthFactor = .25f;
		[Range(0, 1)]public float veryHealthyRange = .05f;
		[Range(0, 1)]public float healthyRange = .125f;
		[Range(0, 1)]public float aliveRange = .35f;
		[Range(0, 1)]public float waterPerCloud = .1f;
		[Range(0, 1)]public float dryPerSecond = .001f;
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
		public float fallingRotation = 30f;
		public float fallingFadeTime = 2f;
	}
	
	public Settings debugSettings;
	public Settings releaseSettings;
	public AppearanceAttributes appearance;
	public BezierCurveAttributes bezier;
	public SplineAttributes spline;
	
	#endregion
	
	#region Public
	public enum PlantState {Dead, Drowning, HealthyWet, VeryHealthy, HealthyDry, Withering};
	[System.NonSerialized]public PlantState state = PlantState.VeryHealthy;
	#endregion
	
	#region Properties
	public Vector3 TopPosisiton
	{
		get { return lines[lineIndex].points3[lines[lineIndex].drawEnd]; }
	}
	
	public Vector3 BasePosisiton
	{
		get { return lines[0].points3[0]; }
	}
	
	public float Height
	{
		get { return height; }
	}
	
	public float Saturation
	{
		get { return saturation; }
	}
	
	public byte[] StemLineFlags
	{
		get
		{
			byte[] flags = new byte[stemCount];
			for(int i=0; i<stemCount; i++)
			{
				int stemLineIndex = stems[i].lineIndex;
				flags[i] = (stemLineIndex == lineIndex) ?  (byte)0 : (stemLineIndex == lineIndex - 1) ? (byte)1 : (byte)2;
			}
			return flags;
		}
	}
	
	public ushort[] StemHeights
	{
		get
		{
			ushort[] heights = new ushort[stemCount];
			for(int i=0; i<stemCount; i++)
				heights[i] = (ushort)stems[i].height;
			return heights;
		}
	}
	
	public int NextStemHeight
	{
		get { return nextStemHeight; }
	}
	
	public float[] StemLengths
	{
		get {
			float[] lengths = new float[stemCount];
			for(int i=0; i<stemCount; i++)
				lengths[i] = stems[i].growth;
			return lengths;
		}
	}
	
	public float[] FlowerGrowthStates
	{
		get
		{
			float[] states = new float[stemCount];
			for(int i=0; i<stemCount; i++)
				states[i] = stems[i].flower.GrowthState;
			return states;
		}
	}
	
	public float TimeUntilStemDeath
	{
		get { return stemDeathTime - unhealthyTimer; }
	}
	#endregion

	#region Unity
	void Awake()
	{
		ItemManager.OnRevive += Revive;
		im = ItemManager.Instance;
		dm = DataManager.Instance;
	}
	
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
		
		witheredThreshold = OPTIMUM_SATURATION - growth.aliveRange/2;
		witheringThreshold = OPTIMUM_SATURATION - growth.healthyRange/2;
		healthyDryThreshold = OPTIMUM_SATURATION - growth.veryHealthyRange/2;
		healthyWetThreshold = OPTIMUM_SATURATION + growth.veryHealthyRange/2;
		drowningThreshold = OPTIMUM_SATURATION + growth.healthyRange/2;
		drownedThreshold = OPTIMUM_SATURATION + growth.aliveRange/2;
		
		//setup camera
		depth = transform.position.z;
		lines = new List<VectorLine>();
		lastSegments = new List<int>();
		
		//End Cap
		VectorLine.SetEndCap ("Point", EndCap.Back, appearance.normalMaterial, appearance.frontCapTexture);
		//VectorLine.SetEndCap ("None", EndCap.None, appearance.normalMaterial, appearance.frontCapTexture);
		
		//glowAlphaColor = new Color(appearance.veryHealthyColor.r, appearance.veryHealthyColor.g, appearance.veryHealthyColor.b, 0);
		//glowLine = new VectorLine ("PlantGlow", new Vector3[MAX_POINTS - 2], glowAlphaColor, appearance.glowMaterial, appearance.minWidth * appearance.glowLineWidthScaler, LineType.Continuous, Joins.Weld);
		//glowLine.active = true;
		controlLine1 = new VectorLine ("Control Line 1", new Vector3[2], Color.red, null, 1f);
		controlLine2 = new VectorLine ("Control Line 2", new Vector3[2], Color.red, null, 1f);
		curvePoints = new Vector3[4];
		startPoint.x = transform.position.x;
		startPoint.y = transform.position.y;
		startPoint.z = depth;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		finishPoint.z = depth;
		
		splineSide = (Random.Range(0, 2) == 0);
		
		//growth
		widthSegments = (float)appearance.widthSegments;
		
		//appearance
		stateTransitionSeconds = appearance.stateTransitionSeconds;
		targetColor = appearance.veryHealthyColor;
		
		//stemming
		nextStemHeight = stemming.startingPlantHeight;
		stemFallingFadeTime = stemming.fallingFadeTime;
		stemSide = (Random.Range(0, 2) == 0);
		stems = new List<Stem>();
		stemParent = new GameObject();
		stemParent.name = "stemParent";
		
		if(dm.curvePointsLoaded.Count > 0)
		{
			LoadState();
			LoadCurves();
			UpdateWidth();
			if (dm.stemLengthsLoaded.Count > 0)
				LoadStems();
		}
		else
		{
			//start with 2 lines
			NewLine(true);
			lastSegment = lastSegments[0];
			NewLine(false);
			UpdateWidth();
			dm.SaveData();
		}
		
		

		//create 2 lines, each with a certain number of curves


		lowPoint = lines[0].points3[0];
		highPoint = lines[0].points3[2];
	}

	void Update()
	{
		//	Debug.Log ("saturation: " + saturation);
		float deltaTime = Time.deltaTime;
		if (state != PlantState.Dead)
		{
			height += deltaTime * growth.maxGrowthPerSecond * growthFactor * im.GrowMultiplier;
			int intPart = Mathf.FloorToInt(height) - currentLineBaseHeight;
			float decPart = height % 1;
	
			if (intPart >= endSegment) {
			
				if (intPart >= lastSegment)
				{
					currentLineBaseHeight += lastSegment;
					NewLine(false);
					lines[lineIndex].endCap = null;
					lineIndex++;
					lastSegment = lastSegments[lineIndex];
					Debug.Log ("new lineIndex: " + lineIndex);
					lines[lineIndex].endCap = "Point";
					lowestLineToDraw = lineIndex - 1;
					intPart = 0;
				}
				//line.points3[intPart] = Vector3.Lerp(line.points3[intPart - 1], line.points3[intPart], DROP_BACK_PERCENT);
				lowPoint = lines[lineIndex].points3[intPart];
				highPoint = lines[lineIndex].points3[intPart + 1];
				endSegment = intPart + 1;
				lines[lineIndex].drawEnd = endSegment;
				//glowLine.drawEnd = endSegment;
				//VectorLine.Destroy(ref points);
				//points = new VectorPoints("Points", new Vector3[]{lowPoint, highPoint}, dotMaterial, 2.0f, 0);
				//points.Draw();
				
				//Debug.Log ("lowPoint: " + lowPoint);
				//Debug.Log ("highPoint: " + highPoint);
			}
			
			if (state == PlantState.Withering || state == PlantState.Drowning)
			{
				unhealthyTimer += deltaTime;
				if (unhealthyTimer > stemDeathTime)
				{
					Debug.Log ("unhealthyTimer: " + unhealthyTimer);
					Debug.Log ("stemDeathTime: " + stemDeathTime);
					KillStem();
				}
			}
			
			UpdateWidth();
			UpdateStems();
			lines[lineIndex].points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
	
			UpdateSaturation();
		}
		else
		{
			UpdateFlowers();
		}

		if (transitioning)
		{
			transitionTimer += deltaTime;
			if (transitionTimer < stateTransitionSeconds)
			{
				float t = transitionTimer/stateTransitionSeconds;
				Color newColor = Color.Lerp(previousColor, targetColor, t);
				SetPlantColor(newColor);
			}
			else
			{
				SetPlantColor(targetColor);
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
				stems[dyingStemIndex].SetColor(Color.Lerp(stemDyingColor, appearance.deadColor, t));
			}
			else
			{
				float fallingTime = stemDyingTimer - stateTransitionSeconds;
				//dyingStemTransform.Translate(stemXMovement * deltaTime, stemming.fallingGravity * fallingTime, 0);
				Vector3 translate = new Vector3(stemXMovement * deltaTime, stemming.fallingGravity * fallingTime, 0);
				dyingStemTransform.localPosition += translate;
				dyingStem.Rotate(stemming.fallingRotation * deltaTime);
				float t = fallingTime/stemFallingFadeTime;
				stems[dyingStemIndex].SetColor(Color.Lerp(appearance.deadColor, Color.clear, t));
				stems[dyingStemIndex].flower.SetAlpha(1 - t);
				if(fallingTime > stemFallingFadeTime)
				{
					RemoveStem();
				}
			}
		}
		
		//glowLine.Draw3D();
		for(int i=lowestLineToDraw; i<=lineIndex; i++) //TODO: possibly unnecessary. maybe only need to update the line that is growing. (check after width is implemented)
			lines[i].Draw3D(); 
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
	
	void OnApplicationQuit()
	{
		dm.SaveData();
	}
	
	void OnDestroy()
	{
		ItemManager.OnRevive -= Revive;
	}
	#endregion
	
	#region Actions
	public void Water(float percentOfCloud)
	{
		saturation += percentOfCloud * growth.waterPerCloud;
	}
	#endregion

	#region Private	
//	private const int MAX_POINTS = 16384;
	private const int POINTS_PER_LINE = 5000;
	private const float DROP_BACK_PERCENT = .99f;
	private float HEIGHT_MULTIPLIER = 10f;
	private float growthFactor = 1f;
	
	private GrowthAttributes growth;
	private StemmingAttributes stemming;
	private ItemManager im;
	private DataManager dm;
	
	private List<VectorLine> lines;
	private List<int>lastSegments;
	private int lastSegment;
	private int lineIndex;
	private int lowestLineToDraw = 0;
	private VectorLine glowLine;
	
	private Vector3 startPoint;
	private float height = 0;
	private int currentLineBaseHeight = 0;
	private int endSegment = 1;
	private Vector3 finishPoint;
	private float lastAngle;
	private bool lastControlPointFlipped;
	private bool splineSide;
	private bool glow;
	private float depth;
	private float[] widths, widthsPrev;
	
	private Vector3 lowPoint;
	private Vector3 highPoint;
	
	//bezier curve
	private VectorLine controlLine1;
	private VectorLine controlLine2;
	private float controlLength = -1;
	private float controlLength2;
	private Vector3[] curvePoints;
	private int curveLoadIndex;
	private bool currentLineFound;
	
	private const float OPTIMUM_SATURATION = .5f;
	private float saturation = OPTIMUM_SATURATION;
	private float healthyWetThreshold;
	private float healthyDryThreshold;
	private float drowningThreshold;
	private float drownedThreshold;
	private float witheringThreshold;
	private float witheredThreshold;
	private Color loadedColor;
	
	//color transition
	private bool transitioning;
	private Color targetColor;
	private Color previousColor;
	private Color stemDyingColor;
	private float transitionTimer;
	
	
	//glow transition
	private bool glowTransitioning;
	private Color targetGlowColor;
	private Color previousGlowColor;
	private float glowTransitionTimer;
	private Color glowAlphaColor;
	
	//growth
	private float widthSegments;
	
	//stemming
	private const float STEM_DEPTH = .05f;
	private bool stemSide;
	private int nextStemHeight;
	private List<Stem> stems;
	private int stemWidthGrowth;
	private float stemDeathTime;
	private float unhealthyTimer;
	private bool stemDying;
	private int dyingStemIndex;
	private float stemDyingTimer;
	private float stemXMovement;
	private Transform dyingStemTransform;
	private Stem dyingStem;
	private GameObject stemParent;
	private float stemFallingFadeTime;
	private int stemCount;
	
	//appearanece
	private float stateTransitionSeconds;
	
	private void Restart() //TODO: NEEDS TO BE ADAPTED TO MULTIPLE LINE SCHEMA!
	{
		height = 0;
		endSegment = 1;
//		line.drawEnd = 1;
		lastSegment = 0;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		finishPoint.z = depth;
//		lowPoint = line.points3[0];
//		highPoint = line.points3[1];
		stemSide = (Random.Range(0, 2) == 0);
		nextStemHeight = stemming.startingPlantHeight;
		stems = new List<Stem>();
		targetColor = appearance.veryHealthyColor;
	}
	
	private void Revive()
	{
		saturation = OPTIMUM_SATURATION;
		TransitionState(PlantState.VeryHealthy);
	}

	private void AddCurve(int index, VectorLine line)
	{		
		if (bezierCurves)
			AddBezierCurve(index, line);
		else
			AddSpline(index, line);
	}
	
	private void LoadCurves()
	{
		height = dm.heightLoaded;
		NewLine(true, true, loadedColor);
		int numberOfCurves = dm.curvePointsLoaded.Count;
		while(curveLoadIndex < numberOfCurves)
			NewLine(false, true, loadedColor);
		if (numberOfCurves > 2)
		{
			float maxWidth = appearance.maxWidth;
			for(int i=0; i<lines.Count-2; i++)
			{
				widths = new float[lines[i].points3.Length - 1];
				for(int n=0; n<widths.Length; n++)
					widths[n] = maxWidth;
				lines[i].SetWidths(widths);
			}
		}
	}
	
	private void NewLine(bool firstLine, bool loadCurves=false, Color? loadedColor = null)
	{
		Color lineColor = loadCurves ? (Color)loadedColor : firstLine ? appearance.veryHealthyColor : lines[0].color;
		VectorLine line =  new VectorLine ("Plant " + lines.Count, new Vector3[POINTS_PER_LINE], lineColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		line.smoothWidth = true;
		line.depth = 1;
		lastSegments.Add(0);
		
		int curves = 0;
		do
		{
			if (loadCurves)
			{
				Vector3[] cps = {dm.curvePointsLoaded[curveLoadIndex], dm.curvePointsLoaded[curveLoadIndex + 1], dm.curvePointsLoaded[curveLoadIndex + 2], dm.curvePointsLoaded[curveLoadIndex + 3]};
//				Debug.Log ("loading curve: " + cps[0] + cps[1] + cps[2] + cps[3]);
				for(int i=0; i<4; i++)
					cps[i].z = depth;
				AddBezierCurve(lines.Count, line, cps, dm.segmentsLoaded[curveLoadIndex/4]);
				curveLoadIndex+=4;
			}
			else
			{
				AddCurve(lines.Count, line);
			}
			curves++;
		}
		while (lastSegments[lines.Count] < appearance.widthSegments);
		
		line.drawStart = 0;
		
		if (loadCurves)
		{
			if(currentLineFound)
			{
				line.drawEnd = 0;
			}
			else if (height < currentLineBaseHeight + lastSegments[lines.Count])
			{
				currentLineFound = true;
				line.endCap = "Point";
				lineIndex = lines.Count;
				lastSegment = lastSegments[lines.Count];
				line.drawEnd = Mathf.CeilToInt(height) - currentLineBaseHeight;
			}
			else
			{
				currentLineBaseHeight += lastSegments[lines.Count];
				line.drawEnd = lastSegments[lines.Count];
			}
		}
		else
		{
			if (firstLine)
				line.endCap = "Point";
			
			line.drawEnd = firstLine ? endSegment : 0;
		}
		
		Debug.Log ("creating line " + lines.Count + " segments: " + lastSegments[lines.Count] + " curves: " + curves);
		lines.Add(line);
	}
	
	private void AddSpline(int index, VectorLine line)
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
		lastSegments[index] += segments;
	}
	
	private void AddBezierCurve(int index, VectorLine line)
	{
		startPoint = finishPoint;
		curvePoints[0] = startPoint;
		float angle;
		bool flipControlPoint;
		if (controlLength == -1) {
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
		line.MakeCurve (curvePoints, segments, lastSegments[index]);
		//glowLine.points3 = line.points3;
		
		#if UNITY_EDITOR
		if (drawDebugMarks)
		{
			controlLine1.points3 = new Vector3[] {startPoint, new Vector3(curvePoints [1].x, curvePoints [1].y, depth)};
			controlLine2.points3 = new Vector3[] {finishPoint, new Vector3(curvePoints [3].x, curvePoints [3].y, depth)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
		if (drawPoints)
		{
			for(int i=lastSegments[index]; i < lastSegment + segments; i++)
			{
				Instantiate(pointMarker, lines[index].points3[i], transform.rotation);
			}
		}
		#endif
		
		
		lastSegments[index] += segments;
		
		dm.StoreCurve(curvePoints, (ushort)segments);
	}
	
	private void AddBezierCurve(int index, VectorLine line, Vector3[] curvePoints, int segments)
	{
		line.MakeCurve(curvePoints, segments, lastSegments[index]);
		
		lastSegments[index] += segments;
		
		dm.StoreCurve(curvePoints, (ushort)segments);
	}

	private void TransitionState(PlantState newState)
	{
		Debug.Log ("transition state: " + newState);
		if (state != newState)
		{
			state = newState;
			/*
			if (state == PlantState.VeryHealthy)
				SetGlow(true);
			else
				SetGlow(false);
			*/
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
			
			previousColor = lines[0].color;
			transitioning = true;
			transitionTimer = 0;
		}
	}
	
	private void ResetStemDeath()
	{
		unhealthyTimer = 0;
		stemDeathTime = Random.Range(stemming.minDeathTime, stemming.maxDeathTime);
	}
	
	private void SetPlantColor(Color color)
	{
		
		foreach(VectorLine line in lines)
		{
			line.SetColor(color);
		}
		foreach(Stem stem in stems)
		{
			if (stem.state != Stem.State.Dead)
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

	private void UpdateWidth()
	{
		float maxWidth = appearance.maxWidth;
		widths = new float[lines[lineIndex].points3.Length - 1];
			
		//float[] glowWidths = new float[lines[lineIndex].points3.Length - 1];
		
		int drawEnd = lines[lineIndex].drawEnd;
		int low = drawEnd - appearance.widthSegments;
		int start = 0;
		int finish = 0;
		if (low < 0)
		{
			if (lineIndex > 0)//width growth spans more than 1 line
			{
				widthsPrev = new float[lines[lineIndex - 1].points3.Length - 1];
				int lastSegment = lastSegments[lineIndex - 1];
				start = lastSegment + low;
				if (start < 0)
				{
					Debug.LogError("appearance.widthSegments is too high!");
					return;
				}
				//first fill widths with maxWidth until start
				for(int i = 0; i < start; i++)
				{
					widthsPrev[i] = maxWidth;
				}
				float count = 0;
				for(int i = start; i < lastSegment; i++)
				{
					float t = count/widthSegments;
					widthsPrev[i] = Mathf.Lerp(appearance.maxWidth, appearance.minWidth, t);
					count++;
				}
				lines[lineIndex - 1].SetWidths(widthsPrev);
			}
			
			start = 0;
			finish = drawEnd;
		}
		else
		{
			start = low;
			finish = drawEnd;
			for(int i = 0; i < start; i++)
			{
				widths[i] = maxWidth;
			}
		}
		
		for(int i = start; i <= finish; i++)
		{
			float t = ((float)i - (float)low)/widthSegments;
			widths[i] = Mathf.Lerp(appearance.maxWidth, appearance.minWidth, t);
		}
		lines[lineIndex].SetWidths(widths);
		//glowLine.SetWidths(glowWidths);
	}
	
	private void LoadState()
	{
		saturation = dm.saturationLoaded;
		
		if (saturation <=  witheredThreshold)
			state = PlantState.Dead;
		else  if (saturation <=  witheringThreshold)
			state = PlantState.Withering;
		else if (saturation <=  healthyDryThreshold)
			state = PlantState.HealthyDry;
		else if (saturation <=  healthyWetThreshold)
			state = PlantState.VeryHealthy;
		else if (saturation <=  drowningThreshold)
			state = PlantState.HealthyWet;
		else if (saturation <=  drownedThreshold)
			state = PlantState.Drowning;
		else
			state = PlantState.Dead;
			
		switch (state)
		{
		case PlantState.Dead:
			loadedColor = appearance.deadColor;
			growthFactor = 0;
			break;
		case PlantState.Drowning:
			loadedColor = appearance.drowningColor;
			growthFactor = growth.unHealthyGrowthFactor;
			break; 
		case PlantState.Withering:
			loadedColor = appearance.witheringColor;
			growthFactor = growth.unHealthyGrowthFactor;
			break;
		case PlantState.HealthyDry:
		case PlantState.HealthyWet:
			loadedColor = appearance.healthyColor;
			growthFactor = growth.healthyGrowthFactor;
			break;
		case PlantState.VeryHealthy:
			loadedColor = appearance.veryHealthyColor;
			growthFactor = 1;
			break;
		}
		
	}
	
	private void UpdateSaturation()
	{
		saturation -= growth.dryPerSecond * Time.deltaTime * im.DryMultiplier;
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
		float newGrowth = Time.deltaTime * stemming.maxGrowthPerSecond * growthFactor * im.GrowMultiplier;
		
		foreach(Stem stem in stems)
		{
			if (stem.state == Stem.State.Growing)
			{
				int stemLineIndex = stem.lineIndex;
				float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemLineIndex] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemLineIndex] : appearance.maxWidth;
				stem.Grow(newGrowth, plantWidth);
			}
			stem.line.Draw3D();
			if (stem.flower.state != Flower.FlowerState.Fruited)
				stem.flower.Grow(newGrowth);
		}
		
		if (height > nextStemHeight)
		{
			NewStem();
			nextStemHeight += Random.Range(stemming.minSeperation, stemming.maxSeperation);
			stemSide = !stemSide;
		}
	}
	
	private void UpdateFlowers()
	{
		float newGrowth = Time.deltaTime * stemming.maxGrowthPerSecond * growthFactor * im.GrowMultiplier;
		foreach(Stem stem in stems)
		{
			if (stem.flower.state != Flower.FlowerState.Fruited)
				stem.flower.Grow(newGrowth);
		}
	}
	
	private void LoadStems()
	{
		int numberOfStemsLoaded = dm.stemLengthsLoaded.Count;
		nextStemHeight = dm.stemNextHeightLoaded;
		for(int i=0; i<numberOfStemsLoaded; i++)
		{
			Vector3[] curve = {dm.stemCurvePointsLoaded[i*4], dm.stemCurvePointsLoaded[i*4 + 1], dm.stemCurvePointsLoaded[i*4 + 2], dm.stemCurvePointsLoaded[i*4 + 3]};
			NewStem(curve, dm.stemLengthsLoaded[i], dm.stemLineFlagsLoaded[i], dm.stemHeightsLoaded[i]);
			stems.Last().flower.LoadGrowthState(dm.flowerGrowthStatesLoaded[i]);
		}
		
		stemSide = (dm.stemCurvePointsLoaded[(numberOfStemsLoaded - 1)*4].x - dm.stemCurvePointsLoaded[(numberOfStemsLoaded - 1)*4 + 2].x) > 0;
		
		if(state == PlantState.Withering || state == PlantState.Drowning)
			stemDeathTime = dm.timeUntilStemDeathLoaded;
			
		if (state == PlantState.Dead)
			UpdateStems();
			
	}

	private void NewStem(Vector3[] curveLoaded=null, float growthLoaded=0, byte flagLoaded = 0, ushort heightLoaded=0)
	{		
		int h = 0;
		int l = 0;
		float plantWidth = appearance.minWidth;
		
		VectorLine stemLine = new VectorLine ("Stem", new Vector3[POINTS_PER_LINE], targetColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		stemLine.depth = -1;
		stemLine.endCap = "Point";
		stemLine.drawStart = 0;
		stemLine.smoothWidth = true;
		stemLine.lineWidth = appearance.minWidth;//temp
		Vector3[] curve = new Vector3[4];
		
		
		if (curveLoaded != null)
		{
			
			curve = curveLoaded;
			for(int i=0; i<4; i++)
				curve[i].z = depth;
			h = heightLoaded;
			switch (flagLoaded)
			{
				case 0:
					l = lineIndex;
					plantWidth = widths[h];
					break;
				case 1:
					l = lineIndex - 1;
					plantWidth = widthsPrev[h];
					break;
				case 2:
					l = -1;
					plantWidth = appearance.maxWidth;
					break;
			}
			Debug.Log("l: " + l + ", h: " + h + ", plantWidth: " + plantWidth );
		}
		else
		{
		
			if(nextStemHeight > height)
			{
				Debug.LogWarning ("stem is higher than height!");
				return;
			}
			if (nextStemHeight > currentLineBaseHeight)
			{
				l = lineIndex;
				h = nextStemHeight - currentLineBaseHeight;
			}
			else
			{
				for(int i=0; i<lineIndex; i++)
				{
					h += lines[i].drawEnd;
					if (nextStemHeight < h)
					{
						l = i;
						break;
					}
				}
			}
			
			Debug.Log ("new stem. l: " + l);
			Vector3 point = lines[l].points3[h];
			point.z += STEM_DEPTH;
			curve[0] = point;
			curve[2] = point + new Vector3(Random.Range(stemming.minCurveWidth, stemming.maxCurveWidth) * (stemSide ? 1: -1), Random.Range(stemming.minCurveHeight, stemming.maxCurveHeight));
			float angle1 = Random.Range(stemming.minControlAngle1, stemming.maxControlAngle1) *  Mathf.Deg2Rad;
			float angle2 = Random.Range(stemming.minControlAngle2, stemming.maxControlAngle2) *  Mathf.Deg2Rad;
			float controlLength = Random.Range(stemming.minControlLength, stemming.maxControlLength);
			curve[1] = point + new Vector3(Mathf.Sin(angle1) * controlLength * (stemSide ? 1: -1), Mathf.Cos(angle1) * controlLength);
			curve[3] = curve[2] + new Vector3(Mathf.Sin(angle2) * controlLength * (stemSide ? 1: -1), Mathf.Cos(angle2) * controlLength);
		}
		
		stemLine.MakeCurve (curve, stemming.segmentsPer, 0);
		GameObject flower = Instantiate(flowerPrefab) as GameObject;
		//Debug.Log ("instantiating new flower: " + flower);
		Stem newStem = new Stem(stemLine, l, h, stemming, appearance.minWidth, lines[0].color, stemSide, flower.GetComponent<Flower>(), growthLoaded, plantWidth);
		stems.Add(newStem);
		stemCount++;
		
		#if UNITY_EDITOR
		if (drawPoints)
		{
			for(int i=0; i < stemLine.points3.Length; i++)
			{
				Instantiate(pointMarker, stemLine.points3[i], transform.rotation);
				//				marker.transform.position = Vector3.zero;
			}
		}
		
		if (drawDebugMarks)
		{
			controlLine1.points3 = new Vector3[] {curve[0], new Vector3(curve[1].x, curve[1].y, depth)};
			controlLine2.points3 = new Vector3[] {curve[2], new Vector3(curve[3].x, curve[3].y, depth)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
		#endif
		
		dm.StoreStem(curve);
	}
	
	private void KillStem()
	{
		if (!stemDying && stemCount > 0)
		{
			stemDying = true;
			stemDyingTimer = 0;
			dyingStemIndex = Random.Range(0, stemCount);
			stemXMovement = stemming.fallingHorizontalMovement * (stems[dyingStemIndex].rightSide ? -1 : 1);
			dyingStemTransform = stems[dyingStemIndex].line.vectorObject.transform;
			dyingStem = stems[dyingStemIndex];
			stems[dyingStemIndex].state = Stem.State.Dead;
			stemDyingColor = lines[0].color;
		}
		ResetStemDeath();
	}
	
	private void RemoveStem()
	{
		stemDying = false;
		VectorLine.Destroy(ref stems[dyingStemIndex].line);
		stems.RemoveAt(dyingStemIndex);
		stemCount--;
		dm.RemoveStem(dyingStemIndex);
	}
	#endregion
}
