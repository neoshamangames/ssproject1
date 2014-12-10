/*Sean Maltz 2014*/

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
		[Range(0, 5)]public float stateTransitionSeconds = 1f;
		public Material normalMaterial;
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
		[Range(0, 1)]public float initialSaturation = .5f;
		public float sproutingGrowthFactor = 1f;
		public float sproutingTime;
		[Range(0, 20)] public float maxGrowthPerSecond = 1f;
		[Range(5, 1000)]public int segmentsPerScreen = 500;
		[Range(0,1)]public float healthyGrowthFactor = .5f;
		[Range(0,1)]public float unHealthyGrowthFactor = .25f;
//		[Range(0, 1)]public float veryHealthyRange = .05f;
//		[Range(0, 1)]public float healthyRange = .125f;
//		[Range(0, 1)]public float aliveRange = .35f;
		[Range(0, 1)]public float witheredThreshold;
		[Range(0, 1)]public float witheringThreshold;
		[Range(0, 1)]public float healthyDryThreshold;
		[Range(0, 1)]public float healthyWetThreshold;
		[Range(0, 1)]public float drowningThreshold;
		[Range(0, 1)]public float drownedThreshold;
		[Range(0, 1)]public float waterPerCloud = .1f;
		[Range(0, 1)]public float dryPerSecond = .001f;
		[Range(0, 1)]public float healthyWetDryPerSecond = .001f;
		[Range(0, 1)]public float drowningDryPerSecond = .001f;
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
	public float welcomeTutorialDelay;
	public float panZoomTutorialDelay;
	public float waterTutorialDelay;
	public float openMenuTutorialDelay;
	
	#if UNITY_EDITOR
	public bool noDehydrationDuringCatchup;
	#endif
	
	#endregion
	
	#region Public
	public enum PlantState {Dead, Drowning, HealthyWet, VeryHealthy, HealthyDry, Withering};
	[System.NonSerialized]public PlantState state = PlantState.VeryHealthy;
	[System.NonSerialized] public bool openMenuTutorialNotDisplayed;
	#endregion
	
	#region Properties
	public int HighScore {
		set { highScore = value; }
		get {
			highScore = Mathf.Max(highScore, Mathf.RoundToInt(height));
			return highScore;
		}
	}
	
	public float greatestWidthOnScreen
	{
		get
		{
			if (lineIndex > 0)
			{
				float lowestPointYPercent = Camera.main.WorldToViewportPoint(lines[lineIndex - 1].points3[0]).y;
				if (lowestPointYPercent > 0)
				{
					return appearance.maxWidth;
				}
				else
				{
					float currentLineBasePercent = Camera.main.WorldToViewportPoint(lines[lineIndex].points3[0]).y;
					if (currentLineBasePercent > 0)
					{
						float percentRange = currentLineBasePercent - lowestPointYPercent;
						float pointPercent = Mathf.Abs(lowestPointYPercent) / percentRange;
						return widthsPrev[Mathf.RoundToInt(pointPercent * (lines[lineIndex - 1].drawEnd - 1))];
					}
					else
					{
						float percentRange = Camera.main.WorldToViewportPoint(lines[lineIndex].points3[endSegment]).y - currentLineBasePercent;
						float pointPercent = Mathf.Abs(currentLineBasePercent) / percentRange;
						int widthsIndex = Mathf.Clamp(Mathf.RoundToInt(pointPercent * endSegment), 0, widths.Length - 1);
						#if UNITY_EDITOR
//						Debug.Log ("lines[lineIndex].points3[endSegment]: " + lines[lineIndex].points3[endSegment]);
//						Debug.Log("currentLineBasePercent: " + currentLineBasePercent);
//						Debug.Log("percentRange: " + percentRange);
//						Debug.Log("pointPercent: " + pointPercent);
//						Debug.Log("Mathf.RoundToInt(pointPercent * endSegment): " + Mathf.RoundToInt(pointPercent * endSegment));
//						Debug.Log("widths.Length: " + widths.Length);
						#endif
						return widths[widthsIndex];
					}
				}
			}
			else
			{
				float lowestPointYPercent = Camera.main.WorldToViewportPoint(lines[0].points3[0]).y;
				if (lowestPointYPercent > 0)
				{
					return widths[0];
				}
				else
				{
					float percentRange = Camera.main.WorldToViewportPoint(lines[0].points3[endSegment]).y - lowestPointYPercent;
					float pointPercent = Mathf.Abs(lowestPointYPercent) / percentRange;
					float width = widths[Mathf.Clamp(Mathf.RoundToInt(pointPercent * endSegment), 0, widths.Length - 1)];//TODO: find out why it can be > 1
					#if UNITY_EDITOR
//					Debug.Log("percentRange: " + percentRange);
//					Debug.Log("pointPercent: " + pointPercent);
//					Debug.Log("Mathf.RoundToInt(pointPercent * endSegment): " + Mathf.RoundToInt(pointPercent * endSegment));
//					Debug.Log("widths.Length: " + widths.Length);
					#endif
					return width;
				}
			}
		}
	}
	
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
		SaveHighScore();
		am.SelectMusic(PlantState.HealthyDry);
		Initialize(true);
	}
	#endregion

	#region Unity
	void Awake()
	{
		ItemManager.OnRevive += Revive;
		im = ItemManager.Instance;
		dm = DataManager.Instance;
		tm = TutorialManager.Instance;
		sm = SoundManager.Instance;
		gm = GUIManager.Instance;
		am = AudioManager.Instance;
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
		
		witheredThreshold = growth.witheredThreshold;
		witheringThreshold = growth.witheringThreshold;
		healthyDryThreshold = growth.healthyDryThreshold;
		healthyWetThreshold = growth.healthyWetThreshold;
		drowningThreshold = growth.drowningThreshold;
		drownedThreshold = growth.drownedThreshold;
		
		//End Cap
		VectorLine.SetEndCap ("Point", EndCap.Back, appearance.normalMaterial, appearance.frontCapTexture);
		//VectorLine.SetEndCap ("None", EndCap.None, appearance.normalMaterial, appearance.frontCapTexture);
		
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
		
		openMenuTutorialNotDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", POWERUP_MENU_TUT_ID)) == 0);
		
		Initialize();
		highScore = Mathf.Max(PlayerPrefs.GetInt("hs", 0), Mathf.RoundToInt(height));
	}

	void Update()
	{		
		if (resuming)
		{
			Debug.Log ("resuming is true");
			resumingTimer += Time.deltaTime;
			if (resumingTimer > RESUME_TIMEOUT)
			{
				resuming = false;
			}
			return;
		}
//		else
//		{
//			Debug.Log ("not resuming");
//		}
		
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
//					if (lineIndex > 1)
//						HalvePoints(lines[lineIndex - 2]);
					NewLine(false);
					lines[lineIndex].drawEnd = endSegment;
					lines[lineIndex].endCap = null;
					lineIndex++;
//					Debug.Log ("lineIndex: " + lineIndex);
					lastSegment = lastSegments[lineIndex];
//					Debug.Log ("new lineIndex: " + lineIndex);
					lines[lineIndex].endCap = "Point";
					lowestLineToDraw = lineIndex - 1;
					intPart = 0;
				}
				else
				{
					lines[lineIndex].points3[intPart] = Vector3.Lerp(lines[lineIndex].points3[intPart - 1], lines[lineIndex].points3[intPart], DROP_BACK_PERCENT);
				}
				#if UNITY_ANDROID
//				Debug.Log ("lowestLineToDraw: " + lowestLineToDraw);
				#endif
				lowPoint = lines[lineIndex].points3[intPart];
				highPoint = lines[lineIndex].points3[intPart + 1];
				endSegment = intPart + 1;
				lines[lineIndex].drawEnd = endSegment;
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
				SetPlantColor(newColor, true);
			}
			else
			{
				SetPlantColor(targetColor);
				transitioning = false;
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
				Vector3 translate = new Vector3(stemXMovement * deltaTime, stemming.fallingGravity * fallingTime, 0);
				dyingStem.RotateAndTranslate(stemming.fallingRotation * deltaTime, translate);
				float t = fallingTime/stemFallingFadeTime;
				dyingStem.SetColor(Color.Lerp(appearance.deadColor, Color.clear, t));
				dyingStem.flower.SetAlpha(1 - t);
				dyingStem.line.Draw3D();
				if(fallingTime > stemFallingFadeTime)
				{
					RemoveStem();
				}
			}
		}		
		
		for(int i=lowestLineToDraw; i<=lineIndex; i++)
		{
//			if (lines[i].vectorObject.renderer.isVisible)
//			{
				lines[i].Draw3D();
//			}
		}
	}
	
	void OnApplicationQuit()
	{
		Suspend();
	}
	
	void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			Suspend();
		}
		else
		{
			#if !UNITY_EDITOR
			resuming = true;
			resumingTimer = 0;
			Debug.Log("setting resuming to true");
			Resume();
			#endif
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
		currentDryPerSecond = growth.dryPerSecond;
		TransitionState(PlantState.VeryHealthy);
		sm.PlaySound(sm.powerupRevive);
	}
	#endregion

	#region Private
	private const int WELCOME_FIRST_TUT_ID = 0;
	private const int WATER_TUT_ID = 25;
	private const int OPEN_MENU_TUT_ID = 26;
	private const int ZOOM_PAN_TUT_ID = 18;
	private const int WELCOME_BACK_TUT_ID = 23;
	private const int STEM_SPROUTING_TUT_ID = 5;
	private const int DEAD_TUT_ID = 9;
	private const int WITHERING_TUT_ID = 3;
	private const int HEALTHY_DRY_TUT_ID = 2;
	private const int VERY_HEALTHY_TUT_ID = 12;
	private const int HEALTHY_WET_TUT_ID = 11;
	private const int DROWNING_TUT_ID = 4;
	private const int POWERUP_MENU_TUT_ID = 27;
	
	private const int MAX_POINTS_PER_LINE = 5000;
	private const float DROP_BACK_PERCENT = .98f;
	private float HEIGHT_MULTIPLIER = 10f;
	
	private float growthFactor = 1f;
	
	private int highScore;
	
	private GrowthAttributes growth;
	private StemmingAttributes stemming;
	private ItemManager im;
	private DataManager dm;
	private TutorialManager tm;
	private SoundManager sm;
	private GUIManager gm;
	private AudioManager am;
	
	private List<VectorLine> lines;
	private List<int>lastSegments;
	private int lastSegment;
	private int lineIndex;
	private int lowestLineToDraw = 0;
	
	private Vector3 startPoint;
	private float height = 0;
	private int currentLineBaseHeight = 0;
	private int endSegment = 1;
	private Vector3 finishPoint;
	private float lastAngle;
	private bool lastControlPointFlipped;
	private bool splineSide;
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
	private float saturation;
	private float healthyWetThreshold;
	private float healthyDryThreshold;
	private float drowningThreshold;
	private float drownedThreshold;
	private float witheringThreshold;
	private float witheredThreshold;
	private float currentDryPerSecond;
	
	//color transition
	private bool transitioning;
	private Color targetColor;
	private Color previousColor;
	private Color stemDyingColor;
	private float transitionTimer;
	
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
	private Stem dyingStem;
	private GameObject stemParent, lineParent;
	private float stemFallingFadeTime;
	private int stemCount;
	
	//catchup
	private const int NEXT_STEM_HEIGHT_MAX = 20;
	private const float RESUME_TIMEOUT = 5f;
	private float catchupGrowth;
	private float drowningSeconds, witheringSeconds = 0;
	private float witheringStartTime = -1f;
	List<float> stemBirthTimes;
	List<float> newStemsBirthTimes;
	List<int> nextStemHeights;
	private bool resuming;
	private float resumingTimer;
	
	//appearanece
	private float stateTransitionSeconds;
	
	private void Suspend()
	{
		if (dyingStem)
			RemoveStem();
		dm.SaveData();
		SaveHighScore();
	}
	
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
			SetLineWidths();
			UpdateWidth();
			if (dm.stemLengthsLoaded.Count > 0)
				LoadStems();
			CatchupStems();
			cam.CatchupBackground();
			DrawAllLines();
			tm.TriggerTutorial(WELCOME_BACK_TUT_ID);
			StartCoroutine(TriggerTutorialAfterDelay(ZOOM_PAN_TUT_ID, panZoomTutorialDelay));
			if (state == PlantState.Dead)
				UpdateWidth();
		}
		else
		{
//			Debug.Log ("reset");
			dm.Reset();
			//start with 2 lines
			NewLine(true);
			lastSegment = lastSegments[0];
			NewLine(false);
			UpdateWidth();
			dm.SaveData();
			
			saturation = growth.initialSaturation;
			FindState();
			UpdateState();
			growthFactor = growth.sproutingGrowthFactor;
			
			int intPart = Mathf.FloorToInt(height);
			lowPoint = lines[0].points3[intPart];
			highPoint = lines[0].points3[intPart + 1];
			if (intPart > 0)
				lines[lineIndex].points3[intPart] = Vector3.Lerp(lines[lineIndex].points3[intPart - 1], lines[lineIndex].points3[intPart], DROP_BACK_PERCENT);
			UpdateWidth();
			lowestLineToDraw = 0;
			StartCoroutine("Sprout");
		}
		
	}
	
	private void Resume()
	{
		
		Debug.Log ("resuming");
		dm.LoadResumeData();
		Catchup();
		SetLineWidths();
		UpdateWidth();
		CatchupStems();
		cam.CatchupBackground();
		cam.GoToPlantTop();
		DrawAllLines();
		lowPoint = lines[lineIndex].points3[lines[lineIndex].drawEnd - 1];
		highPoint = lines[lineIndex].points3[lines[lineIndex].drawEnd];
		resuming = false;
		Debug.Log ("resuming done");
	}
	
	private void DrawAllLines()
	{
		for(int i=0; i < lineIndex; i++) 
			lines[i].Draw3D();
		lowestLineToDraw = (int)Mathf.Clamp(lineIndex - 1, 0, Mathf.Infinity);
	}
	
	private void SaveHighScore()
	{
		highScore = Mathf.Max(PlayerPrefs.GetInt("hs"), Mathf.RoundToInt(height));
		PlayerPrefs.SetInt("hs", highScore);
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

		float secondsToAdvance = (float)dm.secondsSinceSave;
		AdvancePlant(secondsToAdvance);
	}
	
	private void AdvancePlant(float seconds)
	{
		
		saturation = dm.saturationLoaded;
		FindState();
		
		nextStemHeights = new List<int>();
		newStemsBirthTimes = new List<float>();
		int nextStemHeightToFind = nextStemHeight;
//		Debug.Log ("nextStemHeight: " + nextStemHeight);
		
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
		float stateDryPerSecond = 0;
		
		PlantState periodState, nextState = state;
		
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
//				Debug.Log("found nextStemHeightToFind: " + nextStemHeightToFind);
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
				boostTimer = 0;
				growMultiplier = 1;
				im.powerups[0].powerupValue = 1;
			}
			if (slowTimer <= 0)
			{
//				Debug.Log ("slowTimer: " + slowTimer);
				slowTimer = 0;
				dryMultiplier = 1;
				im.powerups[1].powerupValue = 1;
			}
			
		
			switch (periodState)
			{
			case PlantState.Drowning:
				saturationToNextState = saturationRemaining - drowningThreshold;
				periodGrowthFactor = growth.unHealthyGrowthFactor;
				stateDryPerSecond = growth.drowningDryPerSecond;
				nextState = PlantState.HealthyWet;
				break; 
				
			case PlantState.HealthyWet:
				saturationToNextState = saturationRemaining - healthyWetThreshold;
				periodGrowthFactor = growth.healthyGrowthFactor;
				stateDryPerSecond = growth.healthyWetDryPerSecond;
				nextState = PlantState.VeryHealthy;
				break;
				
			case PlantState.VeryHealthy:
				saturationToNextState = saturationRemaining - healthyDryThreshold;
				periodGrowthFactor = 1;
				stateDryPerSecond = growth.dryPerSecond;
				nextState = PlantState.HealthyDry;
				break;		
				
			case PlantState.HealthyDry:
				saturationToNextState = saturationRemaining - witheringThreshold;
				periodGrowthFactor = growth.healthyGrowthFactor;
				stateDryPerSecond = growth.dryPerSecond;
				nextState = PlantState.Withering;
				break;
				
			case PlantState.Withering:
				if (witheringStartTime == -1)
					witheringStartTime = totalSeconds;
				saturationToNextState = saturationRemaining - witheredThreshold;
				periodGrowthFactor = growth.unHealthyGrowthFactor;
				stateDryPerSecond = growth.dryPerSecond;
				nextState = PlantState.Dead;
				break;
				
			case PlantState.Dead:
				secondsRemaining = 0;
				break;
			}
			
			secondsToNextState = saturationToNextState / stateDryPerSecond / dryMultiplier;
			#if UNITY_EDITOR
			if (noDehydrationDuringCatchup)
				secondsToNextState = Mathf.Infinity;
			#endif
			periodGrowthFactor *= growMultiplier;
			
			float lowestActiveTimer = 0;
			if (boostTimer > 0)
				if (slowTimer > 0)
					lowestActiveTimer = Mathf.Min(boostTimer, slowTimer);
				else
					lowestActiveTimer = boostTimer;
			else
				lowestActiveTimer = slowTimer;
					
			if (lowestActiveTimer == 0 || secondsToNextState < lowestActiveTimer)
			{
				secondsToNextPeriod = secondsToNextState;
				saturationThisPeriod = saturationToNextState;
			}
			else
			{
				secondsToNextPeriod = lowestActiveTimer;
				nextState = periodState;
				saturationThisPeriod = secondsToNextPeriod * stateDryPerSecond * dryMultiplier;
			}
			
			if (periodState == PlantState.Drowning)
				drowningSeconds += Mathf.Min(secondsToNextPeriod, secondsRemaining);
			else if (periodState == PlantState.Withering)
				witheringSeconds += Mathf.Min(secondsToNextState, secondsRemaining);
			
//			Debug.Log ("secondsToNextPeriod: " + secondsToNextPeriod + ", secondsRemaining: " + secondsRemaining);
//			Debug.Log ("secondsToNextState: " + secondsToNextState + ", saturationRemaining: " + saturationRemaining + ", saturationToNextState: " + saturationToNextState);
//			Debug.Log ("periodState: " + periodState + ", periodGrowthFactor: " + periodGrowthFactor + ", secondsToNextState: " + secondsToNextState + ", secondsRemaining: " + secondsRemaining);
		}
		while (secondsToNextPeriod < secondsRemaining);
				
//		Debug.Log("newGrowth: " + newGrowth + ", periodGrowthFactor: " + periodGrowthFactor + ", secondsRemaining: " + secondsRemaining);
		totalDry += secondsRemaining * stateDryPerSecond * dryMultiplier;
		newGrowth += periodGrowthFactor * secondsRemaining;
		catchupGrowth = newGrowth;
		float heightToAdd = newGrowth * growth.maxGrowthPerSecond;

//		Debug.Log ("adding growth: " + newGrowth * growth.maxGrowthPerSecond);
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
//			Debug.Log("found nextStemHeightToFind: " + nextStemHeightToFind);
//			Debug.Log ("secondsToGrowDifference: " + secondsToGrowDifference);
			nextStemHeights.Add(nextStemHeightToFind);
			newStemsBirthTimes.Add(seconds - secondsToGrowDifference);
			nextStemHeightToFind += Random.Range(stemming.minSeperation, stemming.maxSeperation);
		}

		
		AddHeight(heightToAdd);
		#if UNITY_EDITOR
		if (!noDehydrationDuringCatchup)
		#endif
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
//			Debug.Log("adding new line in AddHeight");
			currentLineBaseHeight += lastSegment;
			NewLine(false);
			lines[lineIndex].endCap = null;
			lines[lineIndex].drawEnd = lastSegments[lineIndex];
			lineIndex++;
			lastSegment = lastSegments[lineIndex];
			totalSegments += lastSegment;
			
		}
		lines[lineIndex].drawEnd = Mathf.FloorToInt(newHeight) - currentLineBaseHeight;
		lines[lineIndex].endCap = "Point";
		height = newHeight;
	}
	
	private void CatchupStems()
	{
	
		float stemGrowth = catchupGrowth * stemming.maxGrowthPerSecond;
		
		foreach(Stem stem in stems)
		{
			if (stem.state != Stem.State.Grown)
			{
				int stemLineIndex = stem.lineIndex;
				int stemHeight = stem.height;
				float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemHeight] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemHeight] : appearance.maxWidth;
	//			Debug.Log ("lineIndex: " + stemLineIndex + ", height: " + stem.height + ", stemGrowth: " + stemGrowth);
				stem.CatchupGrowth(stemGrowth, plantWidth);
			}
			stem.line.Draw3D();
				
			stem.flower.CatchupGrowth(stemGrowth);
			
			stemBirthTimes.Add(0);
		}
		
		for(int i=0; i<nextStemHeights.Count(); i++)
		{
			nextStemHeight = nextStemHeights[i];
//			Debug.Log ("adding stem in at height: " + nextStemHeight);
			NewStem();
			tm.TriggerTutorial(STEM_SPROUTING_TUT_ID);
			
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

//		for(int i=0; i < stemBirthTimes.Count; i++)
//		{
//			Debug.Log("i: " + i + ", stemBirthTimes[i]: " + stemBirthTimes[i]);
//		}
		
		if (drowningSeconds > 0)
		{
			float timeSinceStart = 0;
			while (drowningSeconds > stemDeathTime)
			{
//				Debug.Log ("drowningSeconds: " + drowningSeconds + ", stemDeathTime: " + stemDeathTime);
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
//				Debug.Log ("witheringSeconds: " + witheringSeconds + ", stemDeathTime: " + stemDeathTime);
				witheringSeconds -= stemDeathTime;
				timeSinceStart += stemDeathTime;
				stemDeathTime = Random.Range(stemming.minDeathTime, stemming.maxDeathTime);
				RemoveStemThatWasAliveAtTime(timeSinceStart);
			}
			if (state == PlantState.Drowning)
				unhealthyTimer = witheringSeconds;
		}
		
//		Debug.Log ("unhealthyTimer: " + unhealthyTimer + ", stemDeathTime: " + stemDeathTime);
	}
	
	private void RemoveStemThatWasAliveAtTime(float secondsAfterSave)
	{
		if (stemCount == 0)
			return;
			
//		Debug.Log ("RemoveStemThatWasAliveAtTime: " + secondsAfterSave);
		List<Stem> aliveStems = new List<Stem>();
		for(int i=0; i < stemCount; i++)
			if (stemBirthTimes[i] < secondsAfterSave)
				aliveStems.Add(stems[i]);
		
		int removeIndex = Random.Range(0, aliveStems.Count());
//		Debug.Log ("removing stem: " + removeIndex);
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
//		Debug.Log ("numberOfCurves: " + numberOfCurves);
		while(curveLoadIndex < numberOfCurves)
			NewLine(false, true);
	}
	
	private void SetLineWidths()
	{
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
	
	private void NewLine(bool firstLine, bool loadCurves=false)
	{
	
//		Debug.Log ("NewLine(firstLine: " + firstLine + ", loadCurves: " + loadCurves + ", lines.Count: " + lines.Count);
		Color lineColor = firstLine ? appearance.veryHealthyColor : lines[0].color;
		VectorLine line =  new VectorLine ("Plant " + lines.Count, new Vector3[MAX_POINTS_PER_LINE], lineColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
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
		while (lastSegments[lines.Count] < widthSegments);
		
		List<Vector3> newPoints3 = new List<Vector3>();
		for(int i=0; i < lastSegments[lines.Count] + 1; i++)
		{
			newPoints3.Add(line.points3[i]);
//			Debug.Log (string.Format("point {0}: {1}", i, line.points3[i]));
		}
		
		line.Resize(newPoints3.ToArray());
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
				lowPoint = line.points3[line.drawEnd - 1];
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
			for(int i=0; i < line.points3.Length; i++)
			{
				Instantiate(pointMarker, line.points3[i], transform.rotation);
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
			state = newState;

			switch (state)
			{
			case PlantState.Dead:
				targetColor = appearance.deadColor;
				growthFactor = 0;
				currentDryPerSecond = growth.dryPerSecond;
				tm.TriggerTutorial(DEAD_TUT_ID);
				break;
			case PlantState.Drowning:
				ResetStemDeath();
				targetColor = appearance.drowningColor;
				growthFactor = growth.unHealthyGrowthFactor;
				currentDryPerSecond = growth.drowningDryPerSecond;
				tm.TriggerTutorial(DROWNING_TUT_ID);
				break; 
			case PlantState.Withering:
				ResetStemDeath();
				targetColor = appearance.witheringColor;
				growthFactor = growth.unHealthyGrowthFactor;
				currentDryPerSecond = growth.dryPerSecond;
				tm.TriggerTutorial(WITHERING_TUT_ID);
				break;
			case PlantState.HealthyDry:
				targetColor = appearance.healthyColor;
				growthFactor = growth.healthyGrowthFactor;
				currentDryPerSecond = growth.dryPerSecond;
				break;
			case PlantState.HealthyWet:
				targetColor = appearance.healthyColor;
				growthFactor = growth.healthyGrowthFactor;
				currentDryPerSecond = growth.healthyWetDryPerSecond;
				break;
			case PlantState.VeryHealthy:
				targetColor = appearance.veryHealthyColor;
				growthFactor = 1;
				currentDryPerSecond = growth.dryPerSecond;
				tm.TriggerTutorial(VERY_HEALTHY_TUT_ID);
				if (openMenuTutorialNotDisplayed)
				{
					StartCoroutine(TriggerTutorialAfterDelay(OPEN_MENU_TUT_ID, openMenuTutorialDelay));
					openMenuTutorialNotDisplayed = false;
				}
				break;
			}
			
			previousColor = lines[0].color;
			transitioning = true;
			transitionTimer = 0;
			am.SelectMusic(newState);
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
	
	private void SetPlantColor(Color color, bool transition=false)
	{
		foreach(VectorLine line in lines)
		{
			if (!transition || line.vectorObject.renderer.isVisible)
			{
				try{
					line.SetColor(color);
				}
				catch
				{
					Debug.LogWarning ("color not set");
				}
			}
		}
//		int stemCount=0;
		foreach(Stem stem in stems)
		{
			if (stem.state != Stem.State.Dead)
			{
				if (!transition || stem.line.vectorObject.renderer.isVisible)
				{
					stem.SetColor(color);
//					stemCount++;
				}
			}
		}
//		Debug.Log("stemCount: " + stemCount);
	}
	
	private void UpdateWidth()
	{
		float maxWidth = appearance.maxWidth;
		widths = new float[lines[lineIndex].points3.Length - 1];

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
		
		for(int i = start; i < finish; i++)
		{
			float t = ((float)i - (float)low)/widthSegments;
//			Debug.Log (string.Format("i: {0}, t: {1}", i,t));
			widths[i] = Mathf.Lerp(appearance.maxWidth, appearance.minWidth, t);
		}
			
		lines[lineIndex].SetWidths(widths);
	}
	
	/*
	private void HalvePoints(VectorLine line)
	{
		Debug.Log ("halving points of line");
		List<Vector3> newPoints3 = new List<Vector3>();
		for(int i=0; i < line.points3.Length; i++)
		{
			if ((i%2) == 0)
				newPoints3.Add(line.points3[i]);
		}
		
		line.Resize(newPoints3.ToArray());
		line.drawStart = 0;
		line.drawEnd = line.points3.Length;
		line.Draw3D();
	}
	*/
	
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
		
		Plant.PlantState prevState = state;
		
		FindState();
		
		bool displayTut = (prevState != state);
						
		
		switch (state)
		{
		case PlantState.Dead:
			plantColor = appearance.deadColor;
			growthFactor = 0;
			currentDryPerSecond = growth.dryPerSecond;
			if (displayTut)
				tm.TriggerTutorial(DEAD_TUT_ID);
			break;
		case PlantState.Drowning:
			plantColor = appearance.drowningColor;
			growthFactor = growth.unHealthyGrowthFactor;
			currentDryPerSecond = growth.drowningDryPerSecond;
			if (displayTut)
				tm.TriggerTutorial(DROWNING_TUT_ID);
			break; 
		case PlantState.Withering:
			plantColor = appearance.witheringColor;
			growthFactor = growth.unHealthyGrowthFactor;
			currentDryPerSecond = growth.dryPerSecond;
			if (displayTut)
				tm.TriggerTutorial(WITHERING_TUT_ID);
			break;
		case PlantState.HealthyDry:
			plantColor = appearance.healthyColor;
			growthFactor = growth.healthyGrowthFactor;
			currentDryPerSecond = growth.dryPerSecond;
			if (displayTut)
				tm.TriggerTutorial(HEALTHY_DRY_TUT_ID);
			break;
		case PlantState.HealthyWet:
			plantColor = appearance.healthyColor;
			growthFactor = growth.healthyGrowthFactor;
			currentDryPerSecond = growth.healthyWetDryPerSecond;
			break;
		case PlantState.VeryHealthy:
			plantColor = appearance.veryHealthyColor;
			growthFactor = 1;
			currentDryPerSecond = growth.dryPerSecond;
			if (displayTut)
				tm.TriggerTutorial(VERY_HEALTHY_TUT_ID);
			break;
		}
		
		StartCoroutine(WaitAndSetPlantColor(plantColor));
//		SetPlantColor(plantColor);
	}
	
	private void UpdateSaturation()
	{
		saturation -= currentDryPerSecond * Time.deltaTime * im.DryMultiplier;
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
					tm.TriggerTutorial(HEALTHY_WET_TUT_ID);
				}
				
				if (saturation < healthyDryThreshold)
				{
					TransitionState(PlantState.HealthyDry);
					tm.TriggerTutorial(HEALTHY_DRY_TUT_ID);
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
			NewStem();
			tm.TriggerTutorial(STEM_SPROUTING_TUT_ID);
			nextStemHeight += Random.Range(stemming.minSeperation, stemming.maxSeperation);
			stemSide = !stemSide;
		}
	}
	
	private void GrowStems()
	{
		float newGrowth = Time.deltaTime * stemming.maxGrowthPerSecond * growthFactor * im.GrowMultiplier;
		
//		for (int  i=0; i<stems.Count; i++)
//		{
//			if (stems[i].line.vectorObject.renderer.isVisible)
//				Debug.Log ("stems is visible: " + i);
//		}
		
		foreach(Stem stem in stems)
		{
			if (stem.state == Stem.State.Growing)
			{
				int stemLineIndex = stem.lineIndex;
				int stemHeight = stem.height;
				float plantWidth = (stemLineIndex == lineIndex) ?  widths[stemHeight] : (stemLineIndex == lineIndex - 1) ? widthsPrev[stemHeight] : appearance.maxWidth;
				stem.Grow(newGrowth, plantWidth);
				if (stem.line.vectorObject.renderer.isVisible)
				{
					stem.line.Draw3D();
				}
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
//			Debug.Log ("stem Length Loaded: " + dm.stemLengthsLoaded[i]);
			NewStem(curve, dm.stemLengthsLoaded[i], dm.stemLineIndicesLoaded[i], dm.stemHeightsLoaded[i]);
			stems.Last().flower.LoadGrowthState(dm.flowerGrowthStatesLoaded[i]);
		}
				
		if(state == PlantState.Withering || state == PlantState.Drowning)
		{
			stemDeathTime = dm.timeUntilStemDeathLoaded;
//			Debug.Log ("stemDeathTime loaded: " + stemDeathTime);
		}
			
		if (state == PlantState.Dead)
			GrowStems();
	}

	private void NewStem(Vector3[] curveLoaded=null, float growthLoaded=0, int indexLoaded = 0, ushort heightLoaded=0)
	{
	
		int h = 0;
		int l = 0;
		float plantWidth = appearance.minWidth;
		
		VectorLine stemLine = new VectorLine ("Stem", new Vector3[stemming.segmentsPer + 1], targetColor, appearance.normalMaterial, appearance.minWidth, LineType.Continuous, Joins.Weld);
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
//				Debug.LogWarning ("stem is higher than height!");
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
			
//			Debug.Log("l: " + l + ", h: " + h + ", plantWidth: " + plantWidth + ", drawEnd: " + lines[l].drawEnd);
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
	
	private IEnumerator Sprout()
	{
		gm.DisplayTitle();
		yield return new WaitForSeconds(growth.sproutingTime);
		growthFactor = growth.healthyGrowthFactor;
		StartCoroutine(TriggerTutorialAfterDelay(WELCOME_FIRST_TUT_ID, welcomeTutorialDelay));
		StartCoroutine(TriggerTutorialAfterDelay(ZOOM_PAN_TUT_ID, panZoomTutorialDelay));
		StartCoroutine(TriggerTutorialAfterDelay(WATER_TUT_ID, waterTutorialDelay));
	}
	
	private IEnumerator TriggerTutorialAfterDelay(int tutorialID, float delay)
	{
		yield return new WaitForSeconds(delay);
		tm.TriggerTutorial(tutorialID);
//		Debug.Log ("triggering: " + tutorialID);
	}
	#endregion
}
