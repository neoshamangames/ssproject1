/*Sean Maltz 2014*/

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Soomla.Store;

public class GUIManager : SingletonMonoBehaviour<GUIManager> {

	[System.Serializable]
	public class StoreItem
	{
		public Texture texture;
		public string itemName;
		public string priceText;
	}
	
	#region Attributes
	public Plant plant;
	public Cloud cloud;
	public Font font;
	public Texture title;
	public Texture menuButton;
	public Texture muteButton;
	public Texture unmuteButton;
	public Texture musicOn;
	public Texture musicMute;
	public Texture frame;
	public Texture tabPowerups;
	public Texture tabCollectables;
	public Texture tabShop;
	public Texture frameTop;
	public Texture nextPage;
	public Texture previousPage;
	public Texture tutorialFrame;
	public Texture tutorialCheckbox;
	public Texture tutorialCheck;
	public Texture resetPlant;
	public Texture revivePlantActive;
	public Texture revivePlantInactive;
	public Texture dolbyLogo;
	public Color activeMenuButtonTint;
	public Color scoreColor;
	public bool updateValuesInPlayMode = true;
	[Range(0, 10)]public float titleFadeInTime;
	[Range(0, 10)]public float titleStillTime;
	[Range(0, 10)]public float titleFlyUpTime;
	[Range(0, 10)]public float highScoreTime;
	[Range(0, 1)]public float titleWidthPercent = .75f;
	[Range(0, 1)]public float iconSizePercent = .2f;
	[Range(0, 1)]public float menuButtonSizePercent = .2f;
	[Range(0, 1)]public float muteButtonSizePercent = .2f;
	[Range(0, 1)]public float buttonsIn = .05f;
	[Range(0, 1)]public float buttonBottomsHeight = .05f;
	[Range(0, 1)]public float musicButtonXPercent = .05f;
	[Range(0, 1)]public float iconXPercent = .2f;
	[Range(0, 1)]public float iconMultiplierXPercent = .2f;
	[Range(0, 1)]public float iconMultiplierYPercent = .2f;
	[Range(0, 1)]public float frameWidthPercent = .85f;
	[Range(0, 1)]public float frameAlpha = .5f;
	[Range(0, 1)]public float tabButtonYPercent = .0f;
	[Range(0, 1)]public float tabButtonOffsetPercent = 0f;
	[Range(0, 1)]public float tabButtonSpacingPercent = 0f;
	[Range(0, 1)]public float tabButtonWidthPercent = .33f;
	[Range(0, 1)]public float tabButtonHeightPercent = .9f;
	[Range(-.5f, .5f)]public float powerup1_YPercent = -.25f;
	[Range(-.5f, .5f)]public float powerup2_YPercent = -.05f;
	[Range(-.5f, .5f)]public float powerup3_YPercent = .1f;
	[Range(-.5f, .5f)]public float piecesFinishX = .25f;
	[Range(-.5f, .5f)]public float piecesStartX = .05f;
	[Range(-.5f, .5f)]public float piecesYPercent = 0f;
	[Range(-1, 1)]public float multiplierXPercent = .5f;
	[Range(-1, 1)]public float multiplierYPercent = .75f;
	[Range(-5f, .5f)]public float powerUpLabelXPercent = 0;
	[Range(-.5f, .5f)]public float powerUpLabelYPercent = 0;
	public int collectablesPerPage = 3;
	[Range(-.5f, .5f)]public float collectablesStartYPercent = .1f;
	[Range(-5f, .5f)]public float collectablesFinishYPercent = .9f;
	[Range(0, 1)]public float pageButtonSizePercent;
	[Range(-.5f, .5f)]public float pageButtonsYPercent;
	[Range(0, 1)]public float pageButtonsInPercent;
	[Range(-.5f, .5f)]public float creditsButtonXPercent = .5f;
	[Range(-.5f, .5f)]public float creditsButtonYPercent = .5f;
	[Range(0, 1)]public float backButtonXPercent = .5f;
	[Range(0, 1)]public float backButtonYPercent = .5f;
	[Range(0, 1)]public float scoreXPercent = .5f;
	[Range(0, 1)]public float scoreYPercent = .5f;
	public float fontSizeInverse = 20f;
	public StoreItem[] storeItems;
	[Range(0, 1)]public float storeItemSizePercent = .2f;
	[Range(-.5f, .5f)]public float storeStartY = .2f;
	[Range(-.5f, .5f)]public float storeFinishY = .8f;
	[Range(-.5f, .5f)]public float storeItemXPercent = .25f;
	[Range(-.5f, .5f)]public float storePriceXPercent = .25f;
	[Range(-1, 1)]public float storePriceYPercent = 0;
	[Range(-.5f, .5f)]public float storeNameXPercent = .25f;
	[Range(-1, 1)]public float storeNameYPercent = 0;
	[Multiline]public string creditsText;
	[Range(0, 1)]public float creditsXPercent = .5f;
	[Range(0, 1)]public float creditsYPercent = .5f;
	[Range(0, 1)]public float powerupTime1XPercent = .5f;
	[Range(0, 1)]public float powerupTime2XPercent = .5f;
	[Range(-.1f, 1)]public float powerupTimeYPercent = .5f;
	[Range(0, 1)]public float powerupTimeSizePercent = .2f;
	[Range(-1, 1)]public float poweruptTimeLabelXPercent = 0;
	[Range(-1, 1)]public float poweruptTimeLabelYPercent = 0;
	[Range(0,255)]public byte activePowerupTransparency = 100;
	[Range(0, 1)]public float awardedPrizeSizePercent = .1f;
	[Range(0, 1)]public float awardedPrizeYPercent = .5f;
	[Range(0, 1)]public float popupPrizeSizePercent = .1f;
	[Range(0, 1)]public float popupPrizeYPercent = .5f;
	[Range(0, 1)]public float popupTextYPercent = .5f;
	[Range(0, 1)]public float popupCompletePrizeSizePercent = .5f;
	[Range(0, 5)]public float prizeFadeTime = 1f;
	[Range(0, 5)]public float prizeGrowTime = 1f;
	public float awaredPieceShiftPercent = 0f;
	public float tutorialFontSizeInverse = 20f;
	public float okFontSizeInverse = 10f;
	public float tutCheckTextSizeInverse = 20f;
	[Range(0, 1)]public float tutorialFrameWidthPercent = .8f;
	[Range(-.5f, .5f)]public float tutorialTextXPercent = .1f;
	[Range(-.5f, .5f)]public float tutorialTextYPercent = .1f;
	[Range(0, 1)]public float tutorialTextWidthPercent = .7f;
	[Range(-.5f, .5f)]public float tutorialCheckboxXPercent, tutorialCheckboxYPercent;
	[Range(-.5f, .5f)]public float tutorialCheckXPercent, tutorialCheckYPercent;
	[Range(0, 1)]public float tutorialCheckSizePercent, tutorialCheckboxSizePercent;
	[Range(-.5f, .5f)]public float tutorialCheckTextXPercent, tutorialCheckTextYPercent;
	[Range(-.5f, .5f)]public float tutorialAcceptXPercent, tutorialAcceptYPercent;
	[Range(0, 1)]public float tutorialAcceptWidthPercent, tutorialAcceptHeightPercent;
	[Range(-1, .5f)]public float secretButtonYPercent;
	[Range(0, 1)]public float resetButtonSizePercent;
	[Range(-.5f, .5f)]public float resetButtonXPercent;
	[Range(0, 1)]public float resetButtonYPercent;
	[Range(0, 1)]public float reviveButtonYPercent;
	[Range(0,255)]public byte resetButtonTransparency = 100;
	[Range(0, 1)]public float dolbyLogoWidthPercent;
	[Range(-1, 1)]public float dolbyLogoXPercent;
	[Range(-1, 1)]public float dolbyLogoYPercent;
	[Range(0, 1)]public float highScoreLeftPercent;
	[Range(0, 1)]public float highScoreTopPercent;
	[Range(0, 1)]public float highScoreYPercent;
	#endregion
	
	#region Properties
	public bool TutorialOpen
	{
		get { return tutorialPopup; }
	}
	
	public bool MenuOpen
	{
		get { return menuOpen; }
	}
	
	#endregion
	
	#region Actions
	public void PrizePopup(ItemManager.Prize prize, int pieceIndex, bool complete)
	{
		prizePopup = true;
		menuOpen = false;
		awardedPrize = prize;
		awardedPieceIndex = pieceIndex;
		awardedPrizeComplete = complete;
		popupTimer = 0;
	}
	
	public void TutorialPopup(string message, bool checkbox)
	{
		tutorialPopup = true;
		tutorialText = message;
		checkboxChecked = false;
		drawCheckbox = checkbox;
		cloud.StopSound();
	}
	
	public void DisplayTitle()
	{
		titleTimer = 0;
		titleVisible = true;
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		im = ItemManager.Instance;
		am = AudioManager.Instance;
		cm = GetComponent<CameraManager>();
		tm = TutorialManager.Instance;
		dm = DataManager.Instance;
		sm = SoundManager.Instance;
		
		ItemManager.OnPowerupActivated += OnPowerupActivated;
		
		powerup1 = im.prizes[0];
		powerup2 = im.prizes[1];
		powerup1Name = powerup1.name;
		
		sfxMuted = (PlayerPrefs.GetInt("sfxMute") == 1);
		musicMuted = (PlayerPrefs.GetInt("musicMute") == 1);
		
		width = Screen.width;
		height = Screen.height;
		centerX = width / 2;
		centerY = height / 2;
		
		labelStyle = new GUIStyle();
		labelStyle.font = font;
		labelStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		labelStyle.normal.textColor = Color.white;
		labelStyle.alignment = TextAnchor.UpperLeft;
		
		creditsStyle = new GUIStyle();
		creditsStyle.font = font;
		creditsStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		creditsStyle.normal.textColor = Color.white;
		creditsStyle.alignment = TextAnchor.UpperCenter;
		
		scoreStyle = new GUIStyle();
		scoreStyle.font = font;
		scoreStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		scoreStyle.normal.textColor = scoreColor;
		scoreStyle.alignment = TextAnchor.UpperRight;
		
		quanitityStyle = new GUIStyle();
		quanitityStyle.font = font;
		quanitityStyle.fontSize = Mathf.RoundToInt(Screen.width * .9f / fontSizeInverse);
		quanitityStyle.normal.textColor = Color.white;
		quanitityStyle.alignment = TextAnchor.UpperCenter;
		
		multiplierStyle = new GUIStyle();
		multiplierStyle.font = font;
		multiplierStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		multiplierStyle.normal.textColor = Color.white;
		multiplierStyle.alignment = TextAnchor.UpperLeft;
		
		buttonStyle = new GUIStyle();
		buttonStyle.font = font;
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		buttonStyle.alignment = TextAnchor.UpperLeft;
		buttonStyle.border = new RectOffset(0, 0, 0, 0);
		buttonStyle.stretchWidth = true;
		buttonStyle.stretchHeight = true;
		buttonStyle.fixedHeight = 0;
		buttonStyle.fixedWidth = 0;
		
		tutorialStyle = new GUIStyle();
		tutorialStyle.font = font;
		tutorialStyle.fontSize = Mathf.RoundToInt(Screen.width / tutorialFontSizeInverse);
		tutorialStyle.normal.textColor = Color.white;
		tutorialStyle.alignment = TextAnchor.UpperCenter;
		tutorialStyle.wordWrap = true;
		
		okStyle = new GUIStyle();
		okStyle.font = font;
		okStyle.fontSize = Mathf.RoundToInt(Screen.width / okFontSizeInverse);
		okStyle.normal.textColor = Color.white;
		okStyle.alignment = TextAnchor.UpperCenter;
		okStyle.wordWrap = true;
		
		tutCheckTextStyle = new GUIStyle();
		tutCheckTextStyle.font = font;
		tutCheckTextStyle.fontSize = Mathf.RoundToInt(Screen.width / tutCheckTextSizeInverse);
		tutCheckTextStyle.normal.textColor = Color.white;
		tutCheckTextStyle.alignment = TextAnchor.UpperLeft;
		tutCheckTextStyle.wordWrap = true;
		
		numberOfStoreItems = storeItems.Length;
		productIDs = new string[] {Constants.REVIVE_3_PACK_ID, Constants.REVIVE_7_PACK_ID, Constants.REVIVE_20_PACK_ID, Constants.REVIVE_150_PACK_ID};
		storeItemY = new float[numberOfStoreItems];
		storePriceY = new float[numberOfStoreItems];
		storeNameY = new float[numberOfStoreItems];
		itemPrices = new List<string>();
		itemNames = new List<string>();
		
		awardedPrizeX = new float[4];
		awardedPrizeY = new float[4];
		
		frameProportions = (float)frame.width/(float)frame.height;
		tabProportions = (float)tabPowerups.width/(float)tabPowerups.height;
		tutorialFrameProportions = (float)tutorialFrame.width/(float)tutorialFrame.height;
		titleProportions = (float)title.width/(float)title.height;
		dolbyLogoProporitions = (float)dolbyLogo.width/(float)dolbyLogo.height;
		
		tabButtonXs = new float[3];
		
		collectablesY = new float[collectablesPerPage];
		
		CalculateValues();
	}
	
	void Start()
	{		
		for(int i=0; i<numberOfStoreItems; i++)
		{
			PurchasableVirtualItem pvi = StoreInfo.GetItemByItemId(productIDs[i]) as PurchasableVirtualItem;
			if (pvi != null)
			{
				MarketItem marketItem = (pvi.PurchaseType as PurchaseWithMarket).MarketItem;
				itemPrices.Add(String.Format("${0:0.00}", marketItem.Price));
				Debug.Log ("itemPrices[i]: " + itemPrices[i]);
			}
			else
			{
				Debug.Log ("can't access store database in Editor. Using placeholder values.");
				itemPrices.Add(storeItems[i].priceText);
			}
			itemNames.Add(storeItems[i].itemName);
		}
		
		//private bool prizeTutorialsDisplayed, powerupTutorialDisplayed, collectableTutorialDisplayed;
		//private bool completePrizeTutorialsDisplayed, completePowerupTutorialDisplayed, completeReviveTutorialDisplayed, completeCollectableTutorialDisplayed;
		powerupTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", POWERUP_PIECE_TUTORIAL_ID)) == 2);
		collectableTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", COLLECTABLE_PIECE_TUTORIAL_ID)) == 2);
		prizeTutorialsDisplayed = (powerupTutorialDisplayed && collectableTutorialDisplayed);
		storeTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", STORE_TUT_ID)) == 2);
        completePowerupTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", POWERUP_COMPLETE_TUTORIAL_ID)) == 2);
		completeReviveTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", REVIVE_COMPLETE_TUTORIAL_ID)) == 2);
		completeCollectableTutorialDisplayed = (PlayerPrefs.GetInt(string.Format("tut{0}", COLLECTABLE_COMPLETE_TUTORIAL_ID)) == 2);
		completePrizeTutorialsDisplayed = (completePowerupTutorialDisplayed && completeReviveTutorialDisplayed && completeCollectableTutorialDisplayed);
		growFasterUsedTutDisp = (PlayerPrefs.GetInt(string.Format("tut{0}", GROW_FASTER_USED_TUT_ID)) == 2);
		drySlowerUsedTutDisp = (PlayerPrefs.GetInt(string.Format("tut{0}", DRY_SLOWER_USED_TUT_ID)) == 2);
		powerupsMenuTutNotDisp = !(PlayerPrefs.GetInt(string.Format("tut{0}", POWERUP_MENU_TUT_ID)) == 2);
		powerupsUsedTutsDisp = (growFasterUsedTutDisp && powerupsUsedTutsDisp);
		
		numberOfCollectables = im.collectables.Count;
	}
	
	void OnGUI()
	{
	
		#if UNITY_EDITOR
		if (updateValuesInPlayMode)
			CalculateValues();
		#endif
		
		if (titleVisible)
		{
			titleTimer += Time.deltaTime;
			float titleY = initTitleY;
			if (titleTimer < titleFadeInTime)
			{
				float alpha = titleTimer/titleFadeInTime;
				GUI.color = new Color(1f, 1f, 1f, alpha);
			}
			else
			{
				float titleStillTimer = titleTimer - titleFadeInTime;
				if (titleStillTimer > titleStillTime)
				{
					float upTimer = titleStillTimer - titleFadeInTime;
					float t = upTimer/titleFlyUpTime;
					titleY = Mathf.Lerp(initTitleY, -titleHeight, t);
					if (t > 1)
						titleVisible = false;
				}
			}
			GUI.DrawTexture(new Rect(titleX, titleY, titleWidth, titleHeight), title);
			GUI.color = Color.white;
		}
		
		if (tutorialPopup)
			GUI.enabled = false;
		
		if (menuOpen)
		{
			if (GUIButtonTexture(new Rect(muteButtonX, muteButtonY, muteButtonSize, muteButtonSize), sfxMuted ? unmuteButton : muteButton))
			{
				sfxMuted = !sfxMuted;
				am.ToggleSFX();
			}
			if (GUIButtonTexture(new Rect(musicButtonX, muteButtonY, muteButtonSize, muteButtonSize), musicMuted ? musicMute : musicOn))
			{
				musicMuted = !musicMuted;
				am.ToggleMusic();
			}	
		
			GUI.color = activeMenuButtonTint;
			DrawMenuButton();
//			GUI.color = Color.white;
			GUI.color = new Color(1f, 1f, 1f, frameAlpha);
			GUI.DrawTexture(new Rect(frameX, 0, frameWidth, height), frame, ScaleMode.ScaleToFit);
			
			switch (menuState)
			{
			case MenuState.POWERUPS:
				DrawTab(tabPowerups);
				DrawPrizeGUI(im.powerups[0], powerup1Y);
				DrawPrizeGUI(im.powerups[1], powerup2Y);
				DrawPrizeGUI(im.powerups[2], powerup3Y);
				
				DrawCollectablesButton();
				DrawShopButton();
				DrawCreditsButton();
				
				break;
				
			case MenuState.COLLECTABLES:
				DrawTab(tabCollectables);
				
				DrawCollectables();
				
				DrawShopButton();
				DrawPowerupsButton();
				
				break;
								
				
			case MenuState.SHOP:
			
				DrawTab(tabShop);
				
				for(int i=0; i<numberOfStoreItems; i++)
				{
					DrawStoreItem(i);
				}
				
				DrawPowerupsButton();
				DrawCollectablesButton();
				
				break;
				
			case MenuState.CREDITS:
				
				DrawTab(frameTop);
				DrawCredits();
				DrawBackButton();
				
				break;
			}
		}
		else
		{
			DrawMenuButton();
		}
		
		if (prizePopup)
			DrawPrizePopup();
		
		if (plant.state == Plant.PlantState.Dead && !menuOpen)
		{
			GUI.color = new Color32(255, 255, 255, resetButtonTransparency);
			if (GUIButtonTexture(new Rect(resetButtonX, resetButtonY, resetButtonSize, resetButtonSize), resetPlant))
			{
				plant.Reset();
				cloud.Reset();
				cm.Reset();
			}
			
			if (im.powerups[Constants.REVIVE_INDEX].inventory > 0)
			{
				if (GUIButtonTexture(new Rect(resetButtonX, reviveButtonY, resetButtonSize, resetButtonSize), revivePlantActive))
				{
					ActivatePowerup(im.powerups[2]);
				}
			}
			else
			{
				if (GUIButtonTexture(new Rect(resetButtonX, reviveButtonY, resetButtonSize, resetButtonSize), revivePlantInactive))
				{
					menuOpen = true;
					menuState = MenuState.SHOP;
				}
			}
			
			GUI.color = Color.white;
		}
		
		GUI.Label(new Rect(scoreX, scoreY, width, 50), String.Format("Score: {0}", Mathf.RoundToInt(plant.Height)), scoreStyle);
		
		if (powerup1Active)
			DrawActivePowerup(powerup1, powerupTime1X);
		if (powerup2Active)
			DrawActivePowerup(powerup2, powerupTime2X);
		
		if (tutorialPopup)
		{
			GUI.enabled = true;
			DrawTutorialPopup();
		}
		
		if (GUI.Button(new Rect(highScoreButtonX, 0, width, highScoreButtonY), "", buttonStyle))
		{
			highScoreTimer = highScoreTime;
		}
		
		if (highScoreTimer > 0)
		{
			highScoreTimer -= Time.deltaTime;
			GUI.Label(new Rect(scoreX, highScoreY, width, 50), String.Format("Your Best: {0}", plant.HighScore), scoreStyle);
		}
	}
	
	void OnDestroy()
	{
		ItemManager.OnPowerupActivated -= OnPowerupActivated;
	}
	#endregion
	
	#region Handlers
	private void OnPowerupActivated(ItemManager.Prize powerup)
	{
		if (powerup.name == powerup1Name)
		{
			powerup1Active = true;
			sm.PlaySound(sm.powerupBoost);
		}
		else
		{
			powerup2Active = true;
			sm.PlaySound(sm.powerupSlow);
		}
		
	}
	#endregion
	
	#region Private
	private enum MenuState {POWERUPS, COLLECTABLES, SHOP, CREDITS};
	private const int SECONDS_PER_HOUR = 3600;
	private const int SECONDS_PER_MINUTE = 60;
	private const int POWERUP_PIECE_TUTORIAL_ID = 13;
	private const int COLLECTABLE_PIECE_TUTORIAL_ID = 14;
	private const int POWERUP_COMPLETE_TUTORIAL_ID = 7;
	private const int REVIVE_COMPLETE_TUTORIAL_ID = 8;
	private const int COLLECTABLE_COMPLETE_TUTORIAL_ID = 15;
	private const int GROW_FASTER_USED_TUT_ID = 16;
	private const int DRY_SLOWER_USED_TUT_ID = 17;
	private const int HAVE_REVIVE_TUT_ID = 19;
	private const int NO_REVIVE_TUT_ID = 20;
	private const int STORE_TUT_ID = 21;
	private const int POWERUP_MENU_TUT_ID = 27;
	private const int LAST_COLLECTABLE_TUT_ID = 29;
	private ItemManager im;
	private CameraManager cm;
	private AudioManager am;
	private TutorialManager tm;
	private DataManager dm;
	private SoundManager sm;
	private MenuState menuState, lastMenuState = MenuState.POWERUPS;
	private float height, width;
	private float centerX, centerY;
	private bool menuOpen;
	private GUIStyle buttonStyle, labelStyle, creditsStyle, quanitityStyle, multiplierStyle, scoreStyle, tutorialStyle, okStyle, tutCheckTextStyle;
	private float frameX, frameWidth, frameHeight, frameProportions;
	private float tabY, tabButtonY, tabButtonWidth, tabButtonHeight, tabHeight, tabProportions;
	private float[] tabButtonXs;
	private int iconSize;
	private float iconX, iconMultiplierX, iconMultiplierYOffset;
	private float[] pieceX;
	private float[] pieceYOffset;
	private float[] multiplierX;
	private float multiplierYOffset;
	private float powerup1Y, powerup2Y, powerup3Y;
	private float powerUpLabelX, powerUpLabelY;
	private float creditsButtonX, creditsButtonY;
	private float backButtonX, backButtonY;
	private float menuButtonSize, muteButtonSize;
	private float menuButtonX, menuButtonY;
	private float muteButtonX, muteButtonY, musicButtonX;
	private float scoreX, scoreY;
	private bool sfxMuted, musicMuted;
	private int numberOfStoreItems;
	private string[] productIDs;
	private float storeItemSize;
	private float[] storeItemY, storePriceY, storeNameY;
	private float storeItemX, storePriceX, storeNameX;
	private List<string> itemPrices;
	private List<string> itemNames;
	private float creditsX, creditsY;
	private float powerupTime1X, powerupTime2X, powerupTimeY, powerupTimeSize;
	private string powerup1Name;
	private ItemManager.Prize powerup1, powerup2;
	private bool powerup1Active, powerup2Active;
	private float poweruptTimeLabelX, poweruptTimeLabelY;
	private bool prizePopup, tutorialPopup;
	private ItemManager.Prize awardedPrize;
	private int awardedPieceIndex;
	private int awardedPrizeSize, popupPrizeSize, completePrizeSize;
	private bool awardedPrizeComplete;
	private float popupPrizeY, popupTextY;
	private float[] awardedPrizeX, awardedPrizeY;
	private float popupTimer;
	private float tutorialFrameHeight, tutorialFrameWidth, tutorialFrameProportions;
	private float tutorialTextX, tutorialTextY, tutorialTextWidth;
	private string tutorialText;
	private float tutorialCheckboxX, tutorialCheckboxY, tutorialCheckboxSize;
	private float tutorialCheckX, tutorialCheckY, tutorialCheckSize;
	private float tutorialCheckTextX, tutorialCheckTextY;
	private float tutorialAcceptX, tutorialAcceptY, tutorialAcceptWidth, tutorialAcceptHeight;
	private bool drawCheckbox, checkboxChecked;
	private float tutorialAcceptTextX, tutorialAcceptTextY;
	private bool prizeTutorialsDisplayed, powerupTutorialDisplayed, collectableTutorialDisplayed, storeTutorialDisplayed;
	private bool completePrizeTutorialsDisplayed, completePowerupTutorialDisplayed, completeReviveTutorialDisplayed, completeCollectableTutorialDisplayed;
	private bool growFasterUsedTutDisp, drySlowerUsedTutDisp, powerupsUsedTutsDisp;
	private bool powerupsMenuTutNotDisp = true;
	private int collectablesPage = 0;
	private	int numberOfCollectables;
	private float collectablesStartY, collectablesFinishY;
	private float[] collectablesY;
	private float pageButtonSize, pageButtonsY, prevPageX, nextPageX;
	private float secretButtonX1, secretButtonX2, secretButtonY, secretButtonHeight, secretButtonWidth;
	private float highScoreButtonX, highScoreButtonY, highScoreTimer, highScoreY;
	private int secretButtonStep = 0;
	private bool titleVisible;
	private float titleTimer;
	private float titleX, initTitleY, titleWidth, titleHeight, titleProportions;
	private float resetButtonX, resetButtonY, resetButtonSize, reviveButtonY;
	private float dolbyLogoWidth, dolbyLogoHeight, dolbyLogoProporitions, dolbyLogoX, dolbyLogoY;
	private float frameTopEdge, frameBottomEdge, frameRightEdge;
	
	private bool GUIButtonTexture( Rect r, Texture t)
	{
		GUI.DrawTexture( r, t);
		return GUI.Button( r, "", buttonStyle);
	}
	
	private void CalculateValues()
	{
		frameWidth = width * frameWidthPercent;
		frameX = centerX - frameWidth / 2;
		frameRightEdge = width - frameX;
		frameHeight = Mathf.RoundToInt(frameWidth/frameProportions);
		frameBottomEdge = height - (height - frameHeight)/2;
		tabHeight = Mathf.RoundToInt(frameWidth/tabProportions);
		tabY = -frameHeight/2 - tabHeight/2 + 1;
		
		titleWidth = Mathf.RoundToInt(width * titleWidthPercent);
		titleHeight = Mathf.RoundToInt(titleWidth/titleProportions);
		titleX = centerX - titleWidth / 2;
		initTitleY = centerY - titleHeight / 2;
		
		tabButtonY = centerY - frameHeight/2 - tabHeight + tabButtonYPercent * height;
		tabButtonWidth = frameWidth * tabButtonWidthPercent;
		tabButtonHeight = tabHeight * tabButtonHeightPercent;
		for(int i=0; i < 3; i++)
			tabButtonXs[i] = frameX + tabButtonOffsetPercent * width + (tabButtonWidth + tabButtonSpacingPercent * width) * i;
		iconX = centerX - frameWidth * iconXPercent;
		iconSize = Mathf.RoundToInt(width * iconSizePercent);
		iconMultiplierX = iconX + iconMultiplierXPercent * iconSize;
		iconMultiplierYOffset = iconMultiplierYPercent * iconSize;
		
		menuButtonSize = Mathf.RoundToInt(width * menuButtonSizePercent);
		menuButtonX = buttonsIn * width - menuButtonSize / 2;
		menuButtonY = height - menuButtonSize / 2 - buttonBottomsHeight * height;
		
		muteButtonSize = Mathf.RoundToInt(width * muteButtonSizePercent);
		muteButtonX = width - muteButtonSize / 2 - buttonsIn * width;
		muteButtonY = height - muteButtonSize / 2 - buttonBottomsHeight * height;
		musicButtonX = musicButtonXPercent * width;
		
		creditsButtonX = xRelativeToFrame(creditsButtonXPercent);
		creditsButtonY = yRelativeToFrame(creditsButtonYPercent);
		
		backButtonX = xRelativeToFrame(backButtonXPercent);
		backButtonY = yRelativeToFrame(backButtonYPercent);
		
		float startX = xRelativeToFrame(piecesStartX);
		float finishX = xRelativeToFrame(piecesFinishX);
		pieceX = new float[] {
			Mathf.Lerp(startX, finishX, 0),
			Mathf.Lerp(startX, finishX, .333f),
			Mathf.Lerp(startX, finishX, .666f),
			Mathf.Lerp(startX, finishX, 1)
		};
		
		pieceYOffset = new float[] {
			iconSize/2 + frameHeight * piecesYPercent,
			frameHeight * piecesYPercent,
			frameHeight * piecesYPercent,
			iconSize/2 + frameHeight * piecesYPercent
		};
		
		multiplierX = new float[] {
			pieceX[0] + frameWidth * multiplierXPercent - iconSize * .2f,
			pieceX[1] + frameWidth * multiplierXPercent - iconSize * .15f,
			pieceX[2] + frameWidth * multiplierXPercent + iconSize * .15f,
			pieceX[3] + frameWidth * multiplierXPercent + iconSize * .1f
		};
		
		multiplierYOffset = frameHeight * multiplierYPercent;
		
		powerup1Y = yRelativeToFrame(powerup1_YPercent);
		powerup2Y = yRelativeToFrame(powerup2_YPercent);
		powerup3Y = yRelativeToFrame(powerup3_YPercent);
		
		powerUpLabelX = yRelativeToFrame(powerUpLabelXPercent);
		powerUpLabelY = yRelativeToFrame(powerUpLabelYPercent);
		
		collectablesStartY = yRelativeToFrame(collectablesStartYPercent);
		collectablesFinishY = yRelativeToFrame(collectablesFinishYPercent);
		
		for(int i=0; i<collectablesPerPage; i++)
		{
			collectablesY[i] = Mathf.Lerp(collectablesStartY, collectablesFinishY, (float)i/(float)(collectablesPerPage-1));
		}
		
		pageButtonSize = Mathf.RoundToInt(pageButtonSizePercent * width);
		pageButtonsY = yRelativeToFrame(pageButtonsYPercent);
		prevPageX = pageButtonsInPercent * width - pageButtonSize / 2;
		nextPageX = width - pageButtonSize / 2 - pageButtonsInPercent * width;
		
		scoreX =  -width * scoreXPercent;
		scoreY = height * scoreYPercent;
		highScoreY = height * highScoreYPercent;
		
		storeItemSize = Mathf.RoundToInt(width * storeItemSizePercent);
		for(int i=0; i<numberOfStoreItems; i++)
		{
			storeItemY[i] = Mathf.Lerp(storeStartY, storeFinishY, (float)i/(float)(numberOfStoreItems - 1)) * frameHeight + centerY;
			storePriceY[i] = storeItemY[i] + storePriceYPercent * frameHeight;
			storeNameY[i] = storeItemY[i] + storeNameYPercent * frameHeight;
		}
		storeItemX = xRelativeToFrame(storeItemXPercent);
		storePriceX = xRelativeToFrame(storePriceXPercent);
		storeNameX = xRelativeToFrame(storeNameXPercent);
		
		creditsX = creditsXPercent * width;
		creditsY = creditsYPercent * height;
		
		powerupTime1X = height * powerupTime1XPercent;
		powerupTime2X = height * powerupTime2XPercent;
		powerupTimeY= height * powerupTimeYPercent;
		powerupTimeSize = Mathf.RoundToInt(width * powerupTimeSizePercent);
		poweruptTimeLabelX = width *.2f * poweruptTimeLabelXPercent;
		poweruptTimeLabelY = width *.2f * poweruptTimeLabelYPercent;
		
		awardedPrizeSize = Mathf.RoundToInt(awardedPrizeSizePercent * width);
		popupPrizeSize = Mathf.RoundToInt(popupPrizeSizePercent * width);
		popupPrizeY = height * popupPrizeYPercent;
		popupTextY = height * popupTextYPercent;
		completePrizeSize = Mathf.RoundToInt(popupCompletePrizeSizePercent * width);
		
		awardedPrizeX[0] = centerX - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[1] = centerX - awardedPrizeSize/ 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[2] = centerX - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[3] = centerX - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		
		awardedPrizeY[0] = awardedPrizeYPercent * height - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[1] = awardedPrizeYPercent * height - awardedPrizeSize/ 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[2] = awardedPrizeYPercent * height - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[3] = awardedPrizeYPercent * height - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		
		tutorialFrameWidth = Mathf.RoundToInt(tutorialFrameWidthPercent * width);
		tutorialFrameHeight = Mathf.RoundToInt(tutorialFrameWidth/tutorialFrameProportions);
		tutorialTextX = xRelativeToFrame(tutorialTextXPercent, tutorialFrameWidth);
		tutorialTextY = yRelativeToFrame(tutorialTextYPercent, tutorialFrameHeight);
		tutorialTextWidth = Mathf.RoundToInt(tutorialTextWidthPercent * width);
		tutorialCheckboxX = xRelativeToFrame(tutorialCheckboxXPercent, tutorialFrameWidth);
		tutorialCheckboxY = yRelativeToFrame(tutorialCheckboxYPercent, tutorialFrameHeight);
		tutorialCheckboxSize = Mathf.RoundToInt(tutorialCheckboxSizePercent * width);
		tutorialCheckX = xRelativeToFrame(tutorialCheckXPercent, tutorialFrameWidth);
		tutorialCheckY = yRelativeToFrame(tutorialCheckYPercent, tutorialFrameHeight);
		tutorialCheckSize = Mathf.RoundToInt(tutorialCheckSizePercent * width);
		tutorialCheckTextX = xRelativeToFrame(tutorialCheckTextXPercent, tutorialFrameWidth);
		tutorialCheckTextY = yRelativeToFrame(tutorialCheckTextYPercent, tutorialFrameHeight);
		tutorialAcceptX = xRelativeToFrame(tutorialAcceptXPercent, tutorialFrameWidth);
		tutorialAcceptY = yRelativeToFrame(tutorialAcceptYPercent, tutorialFrameHeight);
		tutorialAcceptWidth = Mathf.RoundToInt(tutorialAcceptWidthPercent * width);
		tutorialAcceptHeight = Mathf.RoundToInt(tutorialAcceptHeightPercent * height);
		
		secretButtonWidth = width * .15f;
		secretButtonHeight = height * .1f;
		secretButtonX1= centerX + width * .35f - secretButtonWidth/2;
		secretButtonX2= centerX - width * .35f - secretButtonWidth/2;
		secretButtonY = yRelativeToFrame(secretButtonYPercent);
		
		resetButtonSize = Mathf.RoundToInt(width * resetButtonSizePercent);
		resetButtonX = xRelativeToFrame(resetButtonXPercent);
		resetButtonY = Mathf.RoundToInt(height * resetButtonYPercent);
		reviveButtonY = Mathf.RoundToInt(height * reviveButtonYPercent);
		
		dolbyLogoWidth = Mathf.RoundToInt(width * dolbyLogoWidthPercent);
		dolbyLogoHeight = Mathf.RoundToInt(dolbyLogoWidth/dolbyLogoProporitions);
		dolbyLogoX = xRelativeToFrame(dolbyLogoXPercent);
		dolbyLogoY = yRelativeToFrame(dolbyLogoYPercent);
		
		highScoreButtonX = Mathf.RoundToInt(highScoreLeftPercent * width);
		highScoreButtonY = Mathf.RoundToInt(highScoreTopPercent * height);
	}
	
	private int xRelativeToFrame(float percent, float? fw = null)
	{
		if (fw == null)
			fw = frameWidth;
		return Mathf.RoundToInt(percent * (float)fw + centerX);
	}
	
	private int yRelativeToFrame(float percent, float? fh = null)
	{
		if (fh == null)
			fh = frameHeight;
		return Mathf.RoundToInt(percent * (float)fh + centerY);
	}
	
	private void DrawMenuButton()
	{
		if (menuOpen)
		{
			if (GUI.Button(new Rect(0, 0, frameX, height), "", buttonStyle) || GUI.Button(new Rect(frameRightEdge, 0, width, height), "", buttonStyle)
			    || GUI.Button(new Rect(0, 0, width, tabButtonY), "", buttonStyle) || GUI.Button(new Rect(0, frameBottomEdge, width, height), "", buttonStyle))
			{
				menuOpen = false;
			}
		}
		if (GUIButtonTexture(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSize), menuButton))
		{
			menuOpen = !menuOpen;
			prizePopup = false;
						
			if (menuOpen)
			{
				if (powerupsMenuTutNotDisp)
				{
					tm.TriggerTutorial(POWERUP_MENU_TUT_ID);
					powerupsMenuTutNotDisp = false;
					plant.openMenuTutorialNotDisplayed = false;
				}
			
				if (plant.state == Plant.PlantState.Dead)
					if (im.powerups[Constants.REVIVE_INDEX].inventory > 0)
						tm.TriggerTutorial(HAVE_REVIVE_TUT_ID);
					else
						tm.TriggerTutorial(NO_REVIVE_TUT_ID);
				
				sm.PlaySound(sm.menuOpen);
			}
			else
				sm.PlaySound(sm.menuClose);
			
		}
	}
	
	private void DrawPowerupsButton()
	{
		if (DrawTabButton(0))
		{
			if (menuState == MenuState.SHOP)
				SoomlaStore.StopIabServiceInBg();
			menuState = MenuState.POWERUPS;
			lastMenuState = menuState;
			sm.PlaySound(sm.menuTab);
		}
	}
	
	private void DrawCollectablesButton()
	{
		if (DrawTabButton(1))
		{
			if (menuState == MenuState.SHOP)
				SoomlaStore.StopIabServiceInBg();
			menuState = MenuState.COLLECTABLES;
			lastMenuState = menuState;
			sm.PlaySound(sm.menuTab);
		}
	}
	
	private void DrawShopButton()
	{
		if (DrawTabButton(2))
		{
			SoomlaStore.StartIabServiceInBg();
			menuState = MenuState.SHOP;
			lastMenuState = menuState;
			if (!storeTutorialDisplayed)
				tm.TriggerTutorial(STORE_TUT_ID);
			sm.PlaySound(sm.menuTab);
		}
	}
	
	private bool DrawTabButton(int tabNumber)
	{
		return GUI.Button(new Rect(tabButtonXs[tabNumber], tabButtonY, tabButtonWidth, tabButtonHeight), "", buttonStyle);
	}
	
	private void DrawCreditsButton()
	{
		if (GUI.Button(new Rect(creditsButtonX, creditsButtonY, 100, 50), "CREDITS", buttonStyle))
		{
			SoomlaStore.StopIabServiceInBg();
			menuState = MenuState.CREDITS;
			secretButtonStep = 0;
		}
	}
	
	private void DrawBackButton()
	{
		if (GUI.Button(new Rect(backButtonX, backButtonY, 100, 50), "BACK", buttonStyle))
		{
			menuState = lastMenuState;
			if (menuState == MenuState.SHOP)
			{
				SoomlaStore.StartIabServiceInBg();
			}
		}
	}
	
	private void DrawTab(Texture texture)
	{
		GUI.DrawTexture(new Rect(frameX, tabY, frameWidth, height), texture, ScaleMode.ScaleToFit);
		GUI.color = Color.white;
	}
	
	private void DrawPrizeGUI(ItemManager.Prize prize, float yCoordinate)
	{
		GUI.Label(new Rect(powerUpLabelX, yCoordinate + powerUpLabelY, 100, 50), prize.name, labelStyle);
		int prizeInv = prize.inventory;
		
		if (prizeInv > 0)
		{
			if (prizeInv > 1)
			{
				if (GUIButtonTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.multipleTexture))
				{
					ActivatePowerup(prize);
				}
				GUI.Label(new Rect(iconMultiplierX, yCoordinate + iconMultiplierYOffset, iconSize, iconSize), prizeInv.ToString(), quanitityStyle);
			}
			else
			{
				if (GUIButtonTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.texture))
					ActivatePowerup(prize);
			}
		}
			
		else
			GUI.DrawTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.offTexture);
		
		for(int i=0; i<prize.pieces.Length; i++)
		{
			int inventory = prize.pieces[i].inventory;
			ItemManager.Piece piece = prize.pieces[i];
			if (inventory > 0)
			{
				GUI.DrawTexture(new Rect(pieceX[i], yCoordinate + pieceYOffset[i], iconSize, iconSize), piece.texture);
				if (inventory > 1)
					GUI.Label(new Rect(multiplierX[i], yCoordinate + multiplierYOffset, 100, 50), String.Format("x{0}", inventory), multiplierStyle);
			}
			else
			{
				GUI.DrawTexture(new Rect(pieceX[i], yCoordinate + pieceYOffset[i], iconSize, iconSize), piece.offTexture);
			}
		}
	}
	
	private void ActivatePowerup(ItemManager.Prize powerup)
	{
		if (im.Activate(powerup))
			menuOpen = false;
		
		if (!powerupsUsedTutsDisp)
		{
			if (powerup.name == Constants.DRY_SLOWER_NAME)
			{
				tm.TriggerTutorial(DRY_SLOWER_USED_TUT_ID);
				drySlowerUsedTutDisp = true;
				if (growFasterUsedTutDisp == true)
					powerupsUsedTutsDisp = true;
			}
			else if (powerup.name == Constants.GROW_FASTER_NAME)
			{
				tm.SetDontShowAgain(POWERUP_MENU_TUT_ID);
				
				tm.TriggerTutorial(GROW_FASTER_USED_TUT_ID);
				if (drySlowerUsedTutDisp == true)
					powerupsUsedTutsDisp = true;
			}
		}
	}
	
	private void DrawCollectables()
	{
		int start = collectablesPage * collectablesPerPage;
		int numCollectablesToDraw = Mathf.Min(collectablesPerPage, numberOfCollectables - start);
		for(int i=0; i<numCollectablesToDraw; i++)
		{
			DrawPrizeGUI(im.collectables[start + i], collectablesY[i]);
		}
		
		if (collectablesPage > 0) 
			if (GUIButtonTexture(new Rect(prevPageX, pageButtonsY, pageButtonSize, pageButtonSize), previousPage))
			{
				collectablesPage--;
				sm.PlaySound(sm.menuBack);
			}
		
		if (start + numCollectablesToDraw < numberOfCollectables)
			if (GUIButtonTexture(new Rect(nextPageX, pageButtonsY, pageButtonSize, pageButtonSize), nextPage))
			{
				collectablesPage++;
				sm.PlaySound(sm.menuForward);
			}
	}
	
	private void DrawActivePowerup(ItemManager.Prize powerup, float x)
	{			
		float timeRemaining = powerup.powerupTimeRemaining;
		
		
		if (timeRemaining > 0)
		{
			GUI.color = new Color32(255, 255, 255, activePowerupTransparency);
			GUI.DrawTexture(new Rect(x, powerupTimeY, powerupTimeSize, powerupTimeSize), powerup.activeTexture);
			GUI.color = Color.white;
			GUI.Label(new Rect(x + poweruptTimeLabelX, powerupTimeY + poweruptTimeLabelY, 500, 20), FormatTime(timeRemaining), labelStyle);
		}
		else
		{
			if (powerup.name == powerup1Name)
				powerup1Active = false;
			else
				powerup2Active = false;
		}
	}
	
	private string FormatTime(float seconds)
	{
		string timeString;
		if (seconds > SECONDS_PER_HOUR * 2)
			timeString = String.Format("{0} hours", Mathf.FloorToInt(seconds/SECONDS_PER_HOUR));
		else if (seconds > SECONDS_PER_MINUTE * 2)
			timeString = String.Format("{0} minutes", Mathf.FloorToInt(seconds/SECONDS_PER_MINUTE));
		else
			timeString = String.Format("{0} seconds", Mathf.FloorToInt(seconds));
		
		return timeString;
	}
	
	private void DrawStoreItem(int i)
	{
		if (GUIButtonTexture(new Rect(storeItemX, storeItemY[i], storeItemSize, storeItemSize), storeItems[i].texture))
		{
			StoreInventory.BuyItem(productIDs[i]);
		}
		GUI.Label(new Rect(storePriceX, storePriceY[i], 100, 100), itemPrices[i], labelStyle);
		GUI.Label(new Rect(storeNameX, storeNameY[i], 100, 100), itemNames[i], labelStyle);
	}
	
	private void DrawCredits()
	{
		GUI.Label(new Rect(creditsX, creditsY, width, height), creditsText, creditsStyle);
		if (GUI.Button(new Rect(secretButtonX1, secretButtonY, secretButtonWidth, secretButtonHeight), "", buttonStyle))
		{
			ProcessSecretButton(1);
		}
		if (GUI.Button(new Rect(secretButtonX2, secretButtonY, secretButtonWidth, secretButtonHeight), "", buttonStyle))
		{
			ProcessSecretButton(0);
		}
		GUI.DrawTexture(new Rect(dolbyLogoX, dolbyLogoY, dolbyLogoWidth, dolbyLogoHeight), dolbyLogo);
	}
	
	private void ProcessSecretButton(int buttonPressed)
	{
		switch (buttonPressed)
		{
		case 0:
			switch (secretButtonStep)
			{
			case 0:
			case 2:
			case 3:
			case 6:
			case 7:
			case 8:
			case 12:
			case 13:
			case 14:
			case 15:
				secretButtonStep++;
				break;
			default:
				secretButtonStep = 0;
				break;
			}
			break;
		case 1:
			switch (secretButtonStep)
			{
			case 1:
			case 4:
			case 5:
			case 9:
			case 10:
			case 11:
			case 16:
			case 17:
			case 18:
				secretButtonStep++;
				break;
			case 19:
				secretButtonStep = 0;
				ResetEverything();
				break;
			default:
				secretButtonStep = 0;
				break;
			}
			break;
		}
		Debug.Log ("step: " + secretButtonStep);
	}
	
	private void ResetEverything()
	{
		im.Reset();
		tm.ResetStates();
		plant.Reset();
		dm.DeleteFile();
		sm.PlaySound(sm.powerupRevive);
	}
	
	private void DrawPrizePopup()
	{
		GUI.Label(new Rect(0, popupTextY, width, 30), "You've got a prize!", creditsStyle);
		
		GUI.color = Color.white;
		GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardedPrize.popupTexture);
		
		GUI.DrawTexture(new Rect(awardedPrizeX[awardedPieceIndex], awardedPrizeY[awardedPieceIndex], awardedPrizeSize, awardedPrizeSize), awardedPrize.pieces[awardedPieceIndex].texture);
		
		if (awardedPrizeComplete && popupTimer > prizeFadeTime)
		{
			float growTimer = popupTimer - prizeFadeTime;
			float t = Mathf.Clamp01((growTimer/prizeGrowTime));
			{
				float prizeSize = Mathf.Lerp(popupPrizeSize, completePrizeSize, t);
				float yOffset = (popupPrizeSize - prizeSize)/2;
				GUI.DrawTexture(new Rect(centerX - prizeSize/2, popupPrizeY + yOffset, prizeSize, prizeSize),
								awardedPrize.type == ItemManager.Type.Powerup ? awardedPrize.activeTexture : awardedPrize.texture);
			}
		}
		else
		{
			for(int i = 0; i < 4; i++)
			{
				byte t = (byte)Mathf.RoundToInt(popupTimer/ prizeFadeTime * 255);
				if (popupTimer < prizeFadeTime && i == awardedPieceIndex && awardedPrize.pieces[i].inventory < 2)
				{
					GUI.color = new Color32(255, 255, 255, t);
					GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardedPrize.pieces[i].texture);//fade in
					GUI.color = Color.white;
				}
				else
					if (awardedPrize.pieces[i].inventory > 0 || awardedPrizeComplete)
						GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardedPrize.pieces[i].texture);
			}
		}
		popupTimer += Time.deltaTime;
		
		if (GUI.Button(new Rect(0, 0, width, height), "", buttonStyle))
		{
			prizePopup = false;
			if (awardedPrizeComplete)
			{
				if (!completePrizeTutorialsDisplayed)
				{
					switch (awardedPrize.type)
					{
					case ItemManager.Type.Collectable:
						tm.TriggerTutorial(COLLECTABLE_COMPLETE_TUTORIAL_ID);
						completeCollectableTutorialDisplayed = true;
						break;
					case ItemManager.Type.Powerup:
						if (awardedPrize.name == Constants.REVIVE_NAME)
						{
							tm.TriggerTutorial(REVIVE_COMPLETE_TUTORIAL_ID);
							completeReviveTutorialDisplayed = true;
						}
						else
						{
							tm.TriggerTutorial(POWERUP_COMPLETE_TUTORIAL_ID);
							completePowerupTutorialDisplayed = true;
						}
						break;
					}
					if (completeCollectableTutorialDisplayed == true && completePowerupTutorialDisplayed == true && completeReviveTutorialDisplayed == true)
						completePrizeTutorialsDisplayed = true;
				}
				if (awardedPrize.type == ItemManager.Type.Collectable)
				{
					if (im.CollectablesRemain())
					{
						tm.TriggerTutorial(LAST_COLLECTABLE_TUT_ID);
					}
				}
			}
			else
			{
				if (!prizeTutorialsDisplayed)
					switch (awardedPrize.type)
					{
					case ItemManager.Type.Collectable:
						tm.TriggerTutorial(COLLECTABLE_PIECE_TUTORIAL_ID);
						collectableTutorialDisplayed = true;
						if (powerupTutorialDisplayed == true)
							prizeTutorialsDisplayed = true;
						break;
					case ItemManager.Type.Powerup:
						tm.TriggerTutorial(POWERUP_PIECE_TUTORIAL_ID);
						powerupTutorialDisplayed = true;
						if (collectableTutorialDisplayed == true)
							prizeTutorialsDisplayed = true;
						break;
					}
			}
		}
	}
	
	private void DrawTutorialPopup()
	{
		GUI.DrawTexture(new Rect(centerX - tutorialFrameWidth/2, centerY - tutorialFrameHeight/2, tutorialFrameWidth, tutorialFrameHeight), tutorialFrame);
		GUI.Label(new Rect(tutorialTextX, tutorialTextY, tutorialTextWidth, height), tutorialText, tutorialStyle);
		
		if (drawCheckbox)
		{
			if (GUIButtonTexture(new Rect(tutorialCheckboxX, tutorialCheckboxY, tutorialCheckboxSize, tutorialCheckboxSize), tutorialCheckbox))
				checkboxChecked = !checkboxChecked;
			GUI.Label(new Rect(tutorialCheckTextX, tutorialCheckTextY, 1000, 400), "check to never show again", tutCheckTextStyle);
			
			if (checkboxChecked)
				GUI.DrawTexture(new Rect(tutorialCheckX, tutorialCheckY, tutorialCheckSize, tutorialCheckSize), tutorialCheck);
		}
		
		if (GUI.Button(new Rect(tutorialAcceptX, tutorialAcceptY, tutorialAcceptWidth, tutorialAcceptHeight), "ok", okStyle))
		{
			tm.DismissTutorial(checkboxChecked);
			tutorialPopup = false;
		}
	}
	#endregion
}
