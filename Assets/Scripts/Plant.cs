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
		[Range(0, 20)] public float maxGrowthPerSecond = 1f;
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
		get {
			return lines[lineIndex].points3[lines[lineIndex].drawEnd];
		}
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
	
	public int[] StemLineIndices
	{
		get
		{
			int[] indices = new int[stemCount];
			for(int i=0; i<stemCount; i++)
			{
				indices[i] = stems[i].lineIndex;
			}
			return indices;
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
	
	public bool LastControlPointFlipped
	{
		get { return lastControlPointFlipped; }
	}
	
	public float ControlLength
	{
		get { return controlLength2; }
	}
	
	public float ControlAngle
	{
		get { return lastAngle; }
	}
	#endregion
	
	#region Actions
	public void Reset()
	{
		Initialize(true);
	}
	#endregion

	#region Unity
	void Awake()
	{
		ItemManager.OnRevive += Revive;
		im = ItemManager.Instance;
		dm = DataManager.Instance;
		stateTime = Time.time;
		stemBirthTimes = new List<float>();
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
		
		
		//End Cap
		VectorLine.SetEndCap ("Point", EndCap.Back, appearance.normalMaterial, appearance.frontCapTexture);
		//VectorLine.SetEndCap ("None", EndCap.None, appearance.normalMaterial, appearance.frontCapTexture);
		
		//glowAlphaColor = new Color(appearance.veryHealthyColor.r, appearance.veryHealthyColor.g, appearance.veryHealthyColor.b, 0);
		//glowLine = new VectorLine ("PlantGlow", new Vector3[MAX_POINTS - 2], glowAlphaColor, appearance.glowMaterial, appearance.minWidth * appearance.glowLineWidthScaler, LineType.Continuous, Joins.Weld);
		//glowLine.active = true;
		controlLine1 = new VectorLine ("Control Line 1", new Vector3[2], Color.red, null, 1f);
		controlLine2 = new VectorLine ("Control Line 2", new Vector3[2], Color.red, null, 1f);
		curvePoints = new Vector3[4];
		
		
		//growth
		widthSegments = (float)appearance.widthSegments;
		
		//appearance
		stateTransitionSeconds = appearance.stateTransitionSeconds;
		
		depth = transform.position.z;
		
		//stemming
		stemFallingFadeTime = stemming.fallingFadeTime;
		stemParent = new GameObject();
		lineParent = new GameObject();
		stemParent.name = "stems";
		lineParent.name = "lines";
		
		Initialize();
	}

	void Update()
	{
		//	Debug.Log ("saturation: " + saturation);
		float deltaTime = Time.deltaTime;
		if (state != PlantState.Dead)
//		if (true)
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
	
	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			dm.SaveData();
		}
		else
		{
			//TODO: trigger "while you were away here
		}
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
	
	#region Handlers
	private void Revive(ItemManager.Prize prize)
	{
		saturation = OPTIMUM_SATURATION;
		TransitionState(PlantState.VeryHealthy);
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
	private bool flipControlPoint;
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
	private GameObject stemParent, lineParent;
	private float stemFallingFadeTime;
	private int stemCount;
	private float stateTime;
	
	//catchup
	private const int NEXT_STEM_HEIGHT_MAX = 20;
	private float catchupGrowth;
	private float drowningSeconds, witheringSeconds = 0;
	private float witheringStartTime = -1f;
	List<float> stemBirthTimes;
	List<float> newStemsBirthTimes;
	List<int> nextStemHeights;
	
	//test
	VectorLine test;
	
	//appearanece
	private float stateTransitionSeconds;
	
	private void Initialize(bool reset = false)
	{
		height = 0;
		currentLineBaseHeight = 0;
		endSegment = 1;
		
		startPoint.x = transform.position.x;
		startPoint.y = transform.position.y;
		startPoint.z = depth;
		finishPoint.x = transform.position.x;
		finishPoint.y = transform.position.y;
		finishPoint.z = depth;
		
		
		targetColor = appearance.veryHealthyColor;
		splineSide = (Random.Range(0, 2) == 0);
		
		lines = new List<VectorLine>();
		lastSegments = new List<int>();
		lineIndex = 0;
		
		//stemming
		nextStemHeight = stemming.startingPlantHeight;
		stemSide = (Random.Range(0, 2) == 0);
		stems = new List<Stem>();
		ResetStemDeath();
		
		stemCount = 0;
		if (reset)
		{
			Destroy(lineParent);
			Destroy(stemParent);
			lineParent = new GameObject();
			lineParent.name = "lines";
			stemParent = new GameObject();
			stemParent.name = "stems";
		}
		if(!reset && dm.curvePointsLoaded.Count > 0)
		{
			LoadCurves();
			finishPoint = lines[lines.Count - 1].points3[lastSegments[lines.Count - 1]];
			
			InitializeValuesFromFile();
			
			Catchup();
			PopulateWidthsPrevious();
			UpdateWidth();
			if (dm.stemLengthsLoaded.Count > 0)
				LoadStems();
			CatchupStems();
		}
		else
		{
			dm.Reset();
			//start with 2 lines
			NewLine(true);
			lastSegment = lastSegments[0];
			NewLine(false);
			UpdateWidth();
			dm.SaveData();
			state = PlantState.VeryHealthy;
			growthFactor = 1;
			saturation = OPTIMUM_SATURATION;
			lowPoint = lines[0].points3[0];
			highPoint = lines[0].points3[2];
		}
		
	}
	
	private void InitializeValuesFromFile()
	{
		lastControlPointFlipped = dm.lastControlPointFlippedLoaded;
		controlLength2 = dm.controlLengthLoaded;
		lastAngle = dm.controlAngleLoaded;
		nextStemHeight = dm.stemNextHeightLoaded;
		int numberOfStemsLoaded = dm.stemLengthsLoaded.Count;
		if (numberOfStemsLoaded > 0)
			stemSide = (dm.stemCurvePointsLoaded[(numberOfStemsLoaded - 1)*4].x - dm.stemCurvePointsLoaded[(numberOfStemsLoaded - 1)*4 + 2].x) > 0;
	}
	
	private void Catchup()
	{
		if (state == PlantState.Dead)
			return;

//		float secondsToAdvance = dm.secondsSinceSave;
		//TODO: use anti-cheat plugin
		float secondsToAdvance = 40;
		AdvancePlant(secondsToAdvance);
	}
	
	private void AdvancePlant(float seconds)
	{
		
		saturation = dm.saturationLoaded;
		FindState();
		
		nextStemHeights = new List<int>();
		newStemsBirthTimes = new List<float>();
		int nextStemHeightToFind = nextStemHeight;
		Debug.Log ("nextStemHeight: " + nextStemHeight);
		
		float newGrowth = 0;
		float totalHeight = height;
		float totalDry = 0;
		float totalSeconds = 0;
		float saturationToNextState = 0;
		float saturationThisPeriod = 0;
		float saturationRemaining = saturation;
		float periodGrowthFactor = 0;
		float secondsToNextState = 0;
		float secondsToNextPeriod = 0;
		float secondsRemaining = seconds;
		float boostTimer = im.powerups[0].powerupTimeRemaining;
		float slowTimer = im.powerups[1].powerupTimeRemaining;
		float growMultiplier = (boostTimer > 0) ? im.powerups[0].powerupMultiplier : 1;
		float dryMultiplier = (slowTimer > 0) ? im.powerups[1].powerupMultiplier : 1;
		
		PlantState periodState, nextState = state;
		
		Debug.Log ("boostTimer: " + boostTimer);
		Debug.Log ("slowTimer: " + slowTimer);
		
		do
		{
			//the following statement do nothing the first iteration
			newGrowth += periodGrowthFactor * secondsToNextPeriod;
			
			float newTotalHeight = newGrowth * growth.maxGrowthPerSecond + height;
			
			if (nextStemHeightToFind < newTotalHeight )
			{
				float heightAboveTotal = nextStemHeightToFind - totalHeight;
				float secondsToGrowHeight = heightAboveTotal / growth.maxGrowthPerSecond / periodGrowthFactor;
				float secondsSinceStart = totalSeconds + secondsToGrowHeight;
				nextStemHeights.Add(nextStemHeightToFind);
				newStemsBirthTimes.Add(secondsSinceStart);
				Debug.Log("found nextStemHeightToFind: " + nextStemHeightToFind);
				nextStemHeightToFind += Random.Range(stemming.minSeperation, stemming.maxSeperation);
			}
			
			totalHeight = newTotalHeight;
			totalSeconds += secondsToNextPeriod;	
			periodState = nextState;
			secondsRemaining -= secondsToNextPeriod;
			saturationRemaining -= saturationThisPeriod;
			totalDry += saturationThisPeriod;
			
			boostTimer -= secondsToNextPeriod;
			slowTimer -= secondsToNextPeriod;
			if (boostTimer <= 0)
			{
				Debug.Log ("boostTimer: " + boostTimer);
				boostTimer = 0;
				growMultiplier = 1;
				im.powerups[0].powerupValue = 1;
			}
			if (slowTimer <= 0)
			{
				Debug.Log ("slowTimer: " + slowTimer);
				slowTimer = 0;
				dryMultiplier = 1;
				im.powerups[1].powerupValue = 1;
			}
			
		
			switch (periodState)
			{
			case PlantState.Drowning:
				saturationToNextState = saturationRemaining - drowningThreshold;
				periodGrowthFactor = growth.unHealthyGrowthFactor;
				nextState = PlantState.HealthyWet;
				break; 
				
			case PlantState.HealthyWet:
				saturationToNextState = saturationRemaining - healthyWetThreshold;
				periodGrowthFactor = growth.healthyGrowthFactor;
				nextState = PlantState.VeryHealthy;
				break;
				
			case PlantState.VeryHealthy:
				saturationToNextState = saturationRemaining - healthyDryThreshold;
				periodGrowthFactor = 1;
				nextState = PlantState.HealthyDry;
				break;		
				
			case PlantState.HealthyDry:
				saturationToNextState = saturationRemaining - witheringThreshold;
				periodGrowthFactor = growth.healthyGrowthFactor;
				nextState = PlantState.Withering;
				break;
				
			case PlantState.Withering:
				if (witheringStartTime == -1)
					witheringStartTime = totalSeconds;
				saturationToNextState = saturationRemaining - witheredThreshold;
				periodGrowthFactor = growth.unHealthyGrowthFactor;
				nextState = PlantState.Dead;
				break;
				
			case PlantState.Dead:
				secondsRemaining = 0;
				break;
			}
			
			secondsToNextState = saturationToNextState / growth.dryPerSecond / dryMultiplier;
			periodGrowthFactor *= growMultiplier;
			
			float lowestTimer = Mathf.Min(boostTimer, slowTimer);
			if (lowestTimer == 0 || secondsToNextState < lowestTimer)
			{
				secondsToNextPeriod = secondsToNextState;
				saturationThisPeriod = saturationToNextState;
			}
			else
			{
				secondsToNextPeriod = lowestTimer;
				nextState = periodState;
				saturationThisPeriod = secondsToNextPeriod * growth.dryPerSecond * dryMultiplier;
			}
			
			if (periodState == PlantState.Drowning)
				drowningSeconds += Mathf.Min(secondsToNextPeriod, secondsRemaining);
			else if (periodState == PlantState.Withering)
				witheringSeconds += Mathf.Min(secondsToNextState, secondsRemaining);
			
				
			Debug.Log ("secondsToNextPeriod: " + secondsToNextPeriod + ", saturationThisPeriod: " + saturationThisPeriod);
			Debug.Log ("secondsToNextState: " + secondsToNextState + ", saturationRemaining: " + saturationRemaining + ", saturationToNextState: " + saturationToNextState);
			Debug.Log ("periodState: " + periodState + ", periodGrowthFactor: " + periodGrowthFactor + ", secondsToNextState: " + secondsToNextState + ", secondsRemaining: " + secondsRemaining);
		}
		while (secondsToNextPeriod < secondsRemaining);
				
//		Debug.Log("newGrowth: " + newGrowth + ", periodGrowthFactor: " + periodGrowthFactor + ", secondsRemaining: " + secondsRemaining);
		totalDry += secondsRemaining * growth.dryPerSecond * dryMultiplier;
		newGrowth += periodGrowthFactor * secondsRemaining;
		catchupGrowth = newGrowth;
		float heightToAdd = newGrowth * growth.maxGrowthPerSecond;
		Debug.Log("newGrowth total: " + newGrowth + ", heightToAdd: " + heightToAdd);

		totalHeight = newGrowth * growth.maxGrowthPerSecond + height;
		totalSeconds += secondsRemaining;
		
		boostTimer -= secondsRemaining;
		slowTimer -= secondsRemaining;
		
		im.powerups[0].powerupTimeRemaining = Mathf.Max(0, boostTimer);
		im.powerups[1].powerupTimeRemaining = Mathf.Max(0, slowTimer);
		
		while (nextStemHeightToFind < totalHeight )
		{
			float difference = totalHeight - nextStemHeightToFind;
			float secondsToGrowDifference = difference / growth.maxGrowthPerSecond / periodGrowthFactor;
			Debug.Log("found nextStemHeightToFind: " + nextStemHeightToFind);
			Debug.Log ("secondsToGrowDifference: " + secondsToGrowDifference);
			nextStemHeights.Add(nextStemHeightToFind);
			newStemsBirthTimes.Add(seconds - secondsToGrowDifference);
			nextStemHeightToFind += Random.Range(stemming.minSeperation, stemming.maxSeperation);
		}

		
		AddHeight(heightToAdd);
		saturation -= totalDry;
		UpdateState();
	}
	
	private void AddHeight(float heightToAdd)
	{
		float newHeight = height + heightToAdd;
		int intPart = Mathf.FloorToInt(newHeight) - currentLineBaseHeight;
		int totalSegments = lastSegments[lineIndex];
		while (intPart > totalSegments)
		{
			Debug.Log("adding new line in AddHeight");
			currentLineBaseHeight += lastSegment;
			NewLine(false);
			lines[lineIndex].endCap = null;
			lines[lineIndex].drawEnd = lastSegments[lineIndex];
			Debug.Log ("lineIndex: " + lineIndex + ", drawEnd: " + lines[lineIndex].drawEnd);
			lineIndex++;
			lastSegment = lastSegments[lineIndex];
			totalSegments += lastSegment;
			
		}
		lines[lineIndex].drawEnd = Mathf.FloorToInt(newHeight) - currentLineBaseHeight;
		height = newHeight;
	}
	
	private void CatchupStems()
	{
	
		float stemGrowth = catchupGrowth * stemming.maxGrowthPerSecond;
		
		foreach(Stem stem in stems)
		{
			int stemLineIndex = stem.lineIndex;
			int stemHeight = stem.height;
			float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemHeight] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemHeight] : appearance.maxWidth;
			Debug.Log ("lineIndex: " + stemLineIndex + ", height: " + stem.height + ", stemGrowth: " + stemGrowth);
			
			stem.CatchupGrowth(stemGrowth, plantWidth);
			stem.line.Draw3D();
				
			stem.flower.CatchupGrowth(stemGrowth);
			
			stemBirthTimes.Add(0);
		}
		
		for(int i=0; i<nextStemHeights.Count(); i++)
		{
			nextStemHeight = nextStemHeights[i];
			Debug.Log ("adding stem in at height: " + nextStemHeight);
			NewStem();
			
			Stem stem = stems.Last();
			int stemLineIndex = stem.lineIndex;
			int stemHeight = stem.height;
			float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemHeight] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemHeight] : appearance.maxWidth;
			float growthSince = GetGrowthSinceHeight(stemLineIndex, stem.height) * stemming.maxGrowthPerSecond;
			
			stem.CatchupGrowth(growthSince, plantWidth);
			stem.line.Draw3D();
				
			stem.flower.CatchupGrowth(growthSince);
			
			nextStemHeight += Random.Range(stemming.minSeperation, stemming.maxSeperation); //for last iteration
			stemSide = !stemSide;
			
			stemBirthTimes.Add(newStemsBirthTimes[i]);
		}

		for(int i=0; i < stemBirthTimes.Count; i++)
		{
			Debug.Log("i: " + i + ", stemBirthTimes[i]: " + stemBirthTimes[i]);
		}
		
		if (drowningSeconds > 0)
		{
			float timeSinceStart = 0;
			while (drowningSeconds > stemDeathTime)
			{
				Debug.Log ("drowningSeconds: " + drowningSeconds + ", stemDeathTime: " + stemDeathTime);
				drowningSeconds -= stemDeathTime;
				timeSinceStart += stemDeathTime;
				stemDeathTime = Random.Range(stemming.minDeathTime, stemming.maxDeathTime);
				RemoveStemThatWasAliveAtTime(timeSinceStart);
			}
			if (state == PlantState.Drowning)
				unhealthyTimer = drowningSeconds;
		}
		else if (witheringSeconds > 0)
		{
			float timeSinceStart = witheringStartTime;
			while (witheringSeconds > stemDeathTime)
			{
				Debug.Log ("witheringSeconds: " + witheringSeconds + ", stemDeathTime: " + stemDeathTime);
				witheringSeconds -= stemDeathTime;
				timeSinceStart += stemDeathTime;
				stemDeathTime = Random.Range(stemming.minDeathTime, stemming.maxDeathTime);
				RemoveStemThatWasAliveAtTime(timeSinceStart);
			}
			if (state == PlantState.Drowning)
				unhealthyTimer = witheringSeconds;
		}
		
		Debug.Log ("unhealthyTimer: " + unhealthyTimer + ", stemDeathTime: " + stemDeathTime);
	}
	
	private void RemoveStemThatWasAliveAtTime(float secondsAfterSave)
	{
		if (stemCount == 0)
			return;
			
		Debug.Log ("RemoveStemThatWasAliveAtTime: " + secondsAfterSave);
		List<Stem> aliveStems = new List<Stem>();
		for(int i=0; i < stemCount; i++)
			if (stemBirthTimes[i] < secondsAfterSave)
				aliveStems.Add(stems[i]);
		
		int removeIndex = Random.Range(0, aliveStems.Count());
		Debug.Log ("removing stem: " + removeIndex);
		VectorLine.Destroy(ref stems[removeIndex].line);
		stems.RemoveAt(removeIndex);
		stemCount--;
		dm.RemoveStem(removeIndex);
		
	}
	
	private float GetGrowthSinceHeight(int l, int h)
	{
		int segmentsFromTop = -h;
		for(int i = l; i <= lineIndex; i++)
		{
			segmentsFromTop += lines[i].drawEnd;
		}
//		Debug.Log ("GetGrowthSinceHeight i: " + i + ", h: " + h);
		return segmentsFromTop / growth.maxGrowthPerSecond;
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
		if (height == 0)
			height = 1;
		NewLine(true, true);
		int numberOfCurves = dm.curvePointsLoaded.Count;
		Debug.Log ("numberOfCurves: " + numberOfCurves);
		while(curveLoadIndex < numberOfCurves)
			NewLine(false, true);
		if (lines.Count > 2)
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
	
	/*
	private void TestLine()
	{
		Color lineColor = appearance.veryHealthyColor;
		test =  new VectorLine ("TestLine", new Vector3[POINTS_PER_LINE], lineColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		Vector3[] points = new Vector3[4];
		Vector3 startPointTest =  new Vector3(0f, 100.6f, -5f);
		points[0] = startPointTest;
		//		Debug.Log ("flipControlPoint: " + flipControlPoint);
		//		Debug.Log ("angle 1: " + angle);
		//		Debug.Log ("controlLength 1: " + controlLength / screenHeight);
		float length1 = Random.Range (bezier.minControlLength, bezier.maxControlLength) * HEIGHT_MULTIPLIER;
		float angle = Random.Range (bezier.minAngle, bezier.maxAngle);
		
		Vector3 controlPointOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * length1, Mathf.Sin(angle * Mathf.Deg2Rad) * length1, depth);
		//		Debug.Log ("controlPointOffset 1 : " + controlPointOffset);
		points [1].x = startPointTest.x + controlPointOffset.x * (true ? 1 : -1);
		points [1].y = startPointTest.y + controlPointOffset.y;
		points[1].z = depth;
		float bezierCurveHeight =  Random.Range(bezier.minCurveHeight, bezier.maxCurveHeight);
		Vector3 finishPointTest = new Vector3(startPointTest.x, startPointTest.y + bezierCurveHeight * HEIGHT_MULTIPLIER, depth);
		points[2] = finishPointTest;
		angle = Random.Range (bezier.minAngle, bezier.maxAngle);
		float length2 = Random.Range (bezier.minControlLength, bezier.maxControlLength) * HEIGHT_MULTIPLIER;
		controlPointOffset = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad) * length2, Mathf.Sin(angle * Mathf.Deg2Rad) * length2, depth);
		points[3].x = finishPointTest.x + controlPointOffset.x * (true ? 1 : -1);
		points[3].y = finishPointTest.y - controlPointOffset.y;
		points[3].z = depth;
		int segments = Mathf.RoundToInt(growth.segmentsPerScreen * bezierCurveHeight);
		test.drawEnd = segments;
		test.MakeCurve (points, segments, 0);
		test.Draw3D();
		
		if (drawDebugMarks)
		{
			controlLine1.points3 = new Vector3[] {startPointTest, new Vector3(points [1].x, points [1].y, depth)};
			controlLine2.points3 = new Vector3[] {finishPointTest, new Vector3(points [3].x, points [3].y, depth)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
		if (drawPoints)
		{
			for(int i=0; i < segments; i++)
			{
				Instantiate(pointMarker, test.points3[i], transform.rotation);
			}
		}
	}
	*/
	
	private void NewLine(bool firstLine, bool loadCurves=false)
	{
	
		Debug.Log ("NewLine(firstLine: " + firstLine + ", loadCurves: " + loadCurves + ", lines.Count: " + lines.Count);
		Color lineColor = firstLine ? appearance.veryHealthyColor : lines[0].color;
		VectorLine line =  new VectorLine ("Plant " + lines.Count, new Vector3[POINTS_PER_LINE], lineColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		line.vectorObject.transform.parent = lineParent.transform;
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
				Debug.Log ("height: " + height + ", currentLineBaseHeight: " + currentLineBaseHeight + ", drawEnd: " + line.drawEnd);
				line.drawEnd = Mathf.CeilToInt(height) - currentLineBaseHeight;
				lowPoint = line.points3[line.drawEnd - 1];//TODO: check if it is possible for this to be < 0
				highPoint = line.points3[line.drawEnd];
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
		
//		Debug.Log ("creating line " + lines.Count + " segments: " + lastSegments[lines.Count] + " curves: " + curves);
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
		if (controlLength == -1 && dm.curvePointsLoaded.Count == 0)
		{
			Debug.Log ("random flipControlPoint and controlLength");
			flipControlPoint = (Random.Range(0, 2) == 0);
			controlLength = Random.Range (bezier.minControlLength, bezier.maxControlLength) * HEIGHT_MULTIPLIER;
			angle = Random.Range (bezier.minAngle, bezier.maxAngle);					
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
		/*
		Debug.Log ("segments: " + segments);
		Debug.Log("new curve startPoint : " + startPoint);
		Debug.Log("curvePoints[0] : " + curvePoints[0]);
		Debug.Log("new curve finishPoint : " + finishPoint);
		Debug.Log("curvePoints[0] : " + curvePoints[3]);
		Debug.Log("first point: " + line.points3[lastSegments[index]]);
		*/
		
				
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
		if (state != newState)
		{
			stateTime = Time.time;
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
	
	private IEnumerator WaitAndSetPlantColor(Color color)
	{
		yield return null;
		SetPlantColor(color);
	}
	
	private void SetPlantColor(Color color)
	{
		foreach(VectorLine line in lines)
		{
			try{
				line.SetColor(color);
			}
			catch
			{
				Debug.LogWarning ("color not set");
			}
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
				int thislastSegment = lastSegments[lineIndex - 1];
				start = thislastSegment + low;
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
				for(int i = start; i < thislastSegment; i++)
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
	
	private void PopulateWidthsPrevious()
	{
		if (lineIndex > 0)
		{
			widthsPrev = new float[lines[lineIndex - 1].points3.Length - 1];
			float maxWidth = appearance.maxWidth;
			int lastSegment = lastSegments[lineIndex - 1];
			for(int i = 0; i < lastSegment; i++)
			{
				widthsPrev[i] = maxWidth;
			}
		}
	}
	
	private void FindState()
	{
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
	}
	
	private void UpdateState()
	{
		Color plantColor = appearance.veryHealthyColor;
		
		FindState();
		
		switch (state)
		{
		case PlantState.Dead:
			plantColor = appearance.deadColor;
			growthFactor = 0;
			break;
		case PlantState.Drowning:
			plantColor = appearance.drowningColor;
			growthFactor = growth.unHealthyGrowthFactor;
			break; 
		case PlantState.Withering:
			plantColor = appearance.witheringColor;
			growthFactor = growth.unHealthyGrowthFactor;
			break;
		case PlantState.HealthyDry:
		case PlantState.HealthyWet:
			plantColor = appearance.healthyColor;
			growthFactor = growth.healthyGrowthFactor;
			break;
		case PlantState.VeryHealthy:
			plantColor = appearance.veryHealthyColor;
			growthFactor = 1;
			break;
		}
		
		StartCoroutine(WaitAndSetPlantColor(plantColor));
//		SetPlantColor(plantColor);
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
		GrowStems();
		
		if (height > nextStemHeight)
		{
			Debug.Log ("height: " + height + ", nextStemHeight: " + nextStemHeight);
			NewStem();
			nextStemHeight += Random.Range(stemming.minSeperation, stemming.maxSeperation);
			stemSide = !stemSide;
		}
	}
	
	private void GrowStems()
	{
		float newGrowth = Time.deltaTime * stemming.maxGrowthPerSecond * growthFactor * im.GrowMultiplier;
		
		foreach(Stem stem in stems)
		{
			if (stem.state == Stem.State.Growing)
			{
				int stemLineIndex = stem.lineIndex;
				int stemHeight = stem.height;
				float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemHeight] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemHeight] : appearance.maxWidth;
				stem.Grow(newGrowth, plantWidth);
				stem.line.Draw3D();
			}
			if (stem.flower.state != Flower.FlowerState.Fruited)
				stem.flower.Grow(newGrowth);
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
		for(int i=0; i<numberOfStemsLoaded; i++)
		{
			Vector3[] curve = {dm.stemCurvePointsLoaded[i*4], dm.stemCurvePointsLoaded[i*4 + 1], dm.stemCurvePointsLoaded[i*4 + 2], dm.stemCurvePointsLoaded[i*4 + 3]};
			NewStem(curve, dm.stemLengthsLoaded[i], dm.stemLineIndicesLoaded[i], dm.stemHeightsLoaded[i]);
			stems.Last().flower.LoadGrowthState(dm.flowerGrowthStatesLoaded[i]);
		}
				
		if(state == PlantState.Withering || state == PlantState.Drowning)
		{
			stemDeathTime = dm.timeUntilStemDeathLoaded;
			Debug.Log ("stemDeathTime loaded: " + stemDeathTime);
		}
			
		if (state == PlantState.Dead)
			GrowStems();
	}

	private void NewStem(Vector3[] curveLoaded=null, float growthLoaded=0, int indexLoaded = 0, ushort heightLoaded=0)
	{
		int h = 0;
		int l = 0;
		float plantWidth = appearance.minWidth;
		
		VectorLine stemLine = new VectorLine ("Stem", new Vector3[POINTS_PER_LINE], targetColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
		stemLine.vectorObject.transform.parent = stemParent.transform;
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
			
			if (indexLoaded == lineIndex)
			{
				plantWidth = widths[h];
			}
			else if (indexLoaded == lineIndex - 1)
			{
//				Debug.Log("lineIndex: " + lineIndex + ", widthsPrev: " + widthsPrev );
				plantWidth = widthsPrev[h];
			}
			else
			{
				plantWidth = appearance.maxWidth;
			}
			
			l = indexLoaded;
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
//				Debug.Log ("if nextStemHeight: " + nextStemHeight + ", currentLineBaseHeight: " + currentLineBaseHeight);
			}
			else
			{
//				Debug.Log ("else nextStemHeight: " + nextStemHeight + ", currentLineBaseHeight: " + currentLineBaseHeight);
				int lineHeightTotal = 0;
				for(int i=0; i <= lineIndex; i++)
				{
					lineHeightTotal += lines[i].drawEnd;
					if (nextStemHeight < lineHeightTotal)
					{
						l = i;
						h = nextStemHeight - lineHeightTotal + lines[i].drawEnd;	//add drawEnd back to get the base height of the line
						break;
					}
				}
			}
			
			Debug.Log("l: " + l + ", h: " + h + ", plantWidth: " + plantWidth + ", drawEnd: " + lines[l].drawEnd);
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
