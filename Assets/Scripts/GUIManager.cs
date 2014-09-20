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
	public Texture menuButton;
	public Texture muteButton;
	public Texture unmuteButton;
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
	public Texture tutorialAcceptButton;
	public Color activeMenuButtonTint;
	public Color scoreColor;
	public bool updateValuesInPlayMode = true;
	[Range(0, 1)]public float iconSizePercent = .2f;
	[Range(0, 1)]public float menuButtonSizePercent = .2f;
	[Range(0, 1)]public float muteButtonSizePercent = .2f;
	[Range(0, 1)]public float buttonsIn = .05f;
	[Range(0, 1)]public float buttonBottomsHeight = .05f;
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
	[Range(-1, 1)]public float powerup1_YPercent = -.25f;
	[Range(-1, 1)]public float powerup2_YPercent = -.05f;
	[Range(-1, 1)]public float powerup3_YPercent = .1f;
	[Range(-1, 1)]public float piecesFinishX = .25f;
	[Range(-1, 1)]public float piecesStartX = .05f;
	[Range(-1, 1)]public float multiplierXPercent = .5f;
	[Range(-1, 1)]public float multiplierYPercent = .75f;
	[Range(0, 1)]public float powerUpLabelXPercent = 0;
	[Range(-1, 1)]public float powerUpLabelYPercent = 0;
	public int collectablesPerPage = 3;
	[Range(0, 1)]public float collectablesStartYPercent = .1f;
	[Range(0, 1)]public float collectablesFinishYPercent = .9f;
	[Range(0, 1)]public float pageButtonSizePercent;
	[Range(0, 1)]public float pageButtonsYPercent;
	[Range(0, 1)]public float pageButtonsInPercent;
	[Range(0, 1)]public float creditsButtonXPercent = .5f;
	[Range(0, 1)]public float creditsButtonYPercent = .5f;
	[Range(0, 1)]public float backButtonXPercent = .5f;
	[Range(0, 1)]public float backButtonYPercent = .5f;
	[Range(0, 1)]public float scoreXPercent = .5f;
	[Range(0, 1)]public float scoreYPercent = .5f;
	public float fontSizeInverse = 20f;
	public StoreItem[] storeItems;
	[Range(0, 1)]public float storeItemSizePercent = .2f;
	[Range(0, 1)]public float storeStartY = .2f;
	[Range(0, 1)]public float storeFinishY = .8f;
	[Range(0, 1)]public float storeItemXPercent = .25f;
	[Range(0, 1)]public float storePriceXPercent = .25f;
	[Range(-1, 1)]public float storePriceYPercent = 0;
	[Range(0, 1)]public float storeNameXPercent = .25f;
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
	[Range(0, 1)]public float tutorialFrameWidthPercent = .8f;
	[Range(0, 1)]public float tutorialTextXPercent = .1f;
	[Range(0, 1)]public float tutorialTextYPercent = .1f;
	[Range(0, 1)]public float tutorialTextWidthPercent = .7f;
	[Range(0, 1)]public float tutorialCheckboxXPercent, tutorialCheckboxYPercent, tutorialCheckboxSizePercent;
	[Range(0, 1)]public float tutorialCheckXPercent, tutorialCheckYPercent, tutorialCheckSizePercent;
	[Range(0, 1)]public float tutorialCheckTextXPercent, tutorialCheckTextYPercent;
	[Range(0, 1)]public float tutorialAcceptXPercent, tutorialAcceptYPercent, tutorialAcceptWidthPercent;
	[Range(0, 1)]public float tutorialAcceptTextXPercent, tutorialAcceptTextYPercent;
	#endregion
	
	#region Properties
	public bool TutorialOpen
	{
		get { return tutorialPopup; }
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
	
	public void TutorialPopup(string message)
	{
		Debug.Log ("popup message: " + message);
		tutorialPopup = true;
		tutorialText = message;
		checkboxChecked = false;
//		menuOpen = false;
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		im = ItemManager.Instance;
		am = AudioManager.Instance;
		cm = GetComponent<CameraManager>();
		tm = TutorialManager.Instance;
		
		ItemManager.OnPowerupActivated += OnPowerupActivated;
		
		powerup1 = im.prizes[0];
		powerup2 = im.prizes[1];
		powerup1Name = powerup1.name;
		
		muted = am.Muted;
		
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
		scoreStyle.alignment = TextAnchor.UpperLeft;
		
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
		buttonStyle.alignment = TextAnchor.UpperCenter;
		buttonStyle.border = new RectOffset(0, 0, 0, 0);
		buttonStyle.stretchWidth = true;
		buttonStyle.stretchHeight = true;
		buttonStyle.fixedHeight = 0;
		buttonStyle.fixedWidth = 0;
		
		tutorialStyle = new GUIStyle();
		tutorialStyle.font = font;
		tutorialStyle.fontSize = Mathf.RoundToInt(Screen.width / tutorialFontSizeInverse);
		tutorialStyle.normal.textColor = Color.white;
		tutorialStyle.alignment = TextAnchor.UpperLeft;
		tutorialStyle.wordWrap = true;
		
		numberOfStoreItems = storeItems.Length;
		productIDs = new string[] {Constants.REVIVE_3_PACK_ID, Constants.REVIVE_5_PACK_ID, Constants.REVIVE_10_PACK_ID, Constants.REVIVE_15_PACK_ID};
		storeItemY = new float[numberOfStoreItems];
		storePriceY = new float[numberOfStoreItems];
		storeNameY = new float[numberOfStoreItems];
		PVIs = new List<PurchasableVirtualItem>();
		itemPrices = new List<string>();
		itemNames = new List<string>();
		
		awardedPrizeX = new float[4];
		awardedPrizeY = new float[4];
		
		frameProportions = (float)frame.width/(float)frame.height;
		tabProportions = (float)tabPowerups.width/(float)tabPowerups.height;
		tutorialFrameProportions = (float)tutorialFrame.width/(float)tutorialFrame.height;
		tutorialAcceptProportions = (float)tutorialAcceptButton.width/(float)tutorialAcceptButton.height;
		
		tabButtonXs = new float[3];
		
		collectablesY = new float[collectablesPerPage];
		
		CalculateValues();
	}
	
	void Start()
	{
		List<VirtualGood> virtualGoods = StoreInfo.GetVirtualGoods();
		
//		foreach(VirtualGood virtualGood in virtualGoods)
//		{
//			Debug.Log ("virtualGood.Name: " + virtualGood.Name + ", virtualGood.ItemId: " + virtualGood.ItemId + ", virtualGood.ID: " + virtualGood.ID);
//		}
		
		for(int i=0; i<numberOfStoreItems; i++)
		{
			PurchasableVirtualItem pvi = StoreInfo.GetItemByItemId(productIDs[i]) as PurchasableVirtualItem;
			if (pvi != null)
			{
				MarketItem marketItem = (pvi.PurchaseType as PurchaseWithMarket).MarketItem;
				itemPrices.Add(String.Format("${0:0.00}", marketItem.Price));
//				Debug.Log ("itemPrices[i]: " + itemPrices[i]);
			}
			else
			{
//				Debug.Log ("can't access store database in Editor. Using placeholder values.");
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
		powerupsUsedTutsDisp = (growFasterUsedTutDisp && powerupsUsedTutsDisp);
		
		numberOfCollectables = im.collectables.Count;
	}
	
	void OnGUI()
	{
	
		#if UNITY_EDITOR
		if (updateValuesInPlayMode)
			CalculateValues();
		#endif
		
		if (menuOpen)
		{
		
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
			
			
			if (GUIButtonTexture(new Rect(muteButtonX, muteButtonY, muteButtonSize, muteButtonSize), muted ? unmuteButton : muteButton))
			{
				muted = !muted;
				am.ToggleMute();
			}	
		}
		else
		{
			DrawMenuButton();
		}
		
		if (prizePopup)
			DrawPrizePopup();
		
		if (tutorialPopup)
			DrawTutorialPopup();
		
		if (plant.state == Plant.PlantState.Dead)
		{
			if (GUI.Button(new Rect(centerX, 15, 100, 50), "Reset Plant", buttonStyle))//TODO: replace with better
			{
				plant.Reset();
				cloud.Reset();
				cm.Reset();
			}
		}
		
		GUI.Label(new Rect(scoreX, scoreY, 200, 50), Mathf.RoundToInt(plant.Height).ToString(), scoreStyle);
		
		if (powerup1Active)
			DrawActivePowerup(powerup1, powerupTime1X);
		if (powerup2Active)
			DrawActivePowerup(powerup2, powerupTime2X);
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
			powerup1Active = true;
		else
			powerup2Active = true;
		
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
	private ItemManager im;
	private CameraManager cm;
	private AudioManager am;
	private TutorialManager tm;
	private MenuState menuState, lastMenuState = MenuState.POWERUPS;
	private float height, width;
	private float centerX, centerY;
	private bool menuOpen;
	private GUIStyle buttonStyle, labelStyle, creditsStyle, quanitityStyle, multiplierStyle, scoreStyle, tutorialStyle;
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
	private float muteButtonX, muteButtonY;
	private float scoreX, scoreY;
	private bool muted;
	private int numberOfStoreItems;
	private string[] productIDs;
	private float storeItemSize;
	private float[] storeItemY, storePriceY, storeNameY;
	private float storeItemX, storePriceX, storeNameX;
	private List<PurchasableVirtualItem> PVIs;
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
	private float tutorialAcceptX, tutorialAcceptY, tutorialAcceptWidth, tutorialAcceptHeight, tutorialAcceptProportions;
	private bool checkboxChecked;
	private float tutorialAcceptTextX, tutorialAcceptTextY;
	private bool prizeTutorialsDisplayed, powerupTutorialDisplayed, collectableTutorialDisplayed, storeTutorialDisplayed;
	private bool completePrizeTutorialsDisplayed, completePowerupTutorialDisplayed, completeReviveTutorialDisplayed, completeCollectableTutorialDisplayed;
	private bool growFasterUsedTutDisp, drySlowerUsedTutDisp, powerupsUsedTutsDisp;
	private int collectablesPage = 0;
	private	int numberOfCollectables;
	private float collectablesStartY, collectablesFinishY;
	private float[] collectablesY;
	private float pageButtonSize, pageButtonsY, prevPageX, nextPageX;
	
	private bool GUIButtonTexture( Rect r, Texture t)
	{
		GUI.DrawTexture( r, t);
		return GUI.Button( r, "", buttonStyle);
	}
	
	private void CalculateValues()
	{
		frameWidth = width * frameWidthPercent;
		frameX = centerX - frameWidth / 2;
		frameHeight = Mathf.RoundToInt(frameWidth/frameProportions);
		tabHeight = Mathf.RoundToInt(frameWidth/tabProportions);
		tabY = -frameHeight/2 - tabHeight/2 + 1;
		
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
		
		creditsButtonX = Mathf.RoundToInt(width * creditsButtonXPercent);
		creditsButtonY = Mathf.RoundToInt(height * creditsButtonYPercent);
		
		backButtonX = Mathf.RoundToInt(width * backButtonXPercent);
		backButtonY = Mathf.RoundToInt(height * backButtonYPercent);
		
		float startX = centerX + frameWidth * piecesStartX;
		float finishX = centerX + frameWidth * piecesFinishX;
		pieceX = new float[] {
			Mathf.Lerp(startX, finishX, 0),
			Mathf.Lerp(startX, finishX, .333f),
			Mathf.Lerp(startX, finishX, .666f),
			Mathf.Lerp(startX, finishX, 1)
		};
		
		pieceYOffset = new float[] {
			iconSize/2,
			0,
			0,
			iconSize/2
		};
		
		multiplierX = new float[] {
			pieceX[0] + width * multiplierXPercent - iconSize *.25f,
			pieceX[1] + width * multiplierXPercent - iconSize * .15f,
			pieceX[2] + width * multiplierXPercent + iconSize * .15f,
			pieceX[3] + width * multiplierXPercent + iconSize * .25f
		};
		
		multiplierYOffset = height * multiplierYPercent;
		
		powerup1Y = centerY + height * powerup1_YPercent;
		powerup2Y = centerY + height * powerup2_YPercent;
		powerup3Y = centerY + height * powerup3_YPercent;
		
		powerUpLabelX = powerUpLabelXPercent * frameWidth;
		powerUpLabelY = powerUpLabelYPercent * frameWidth;
		
		collectablesStartY = collectablesStartYPercent * height;
		collectablesFinishY = collectablesFinishYPercent * height;
		
		for(int i=0; i<collectablesPerPage; i++)
		{
			collectablesY[i] = Mathf.Lerp(collectablesStartY, collectablesFinishY, (float)i/(float)(collectablesPerPage-1));
		}
		
		pageButtonSize = Mathf.RoundToInt(pageButtonSizePercent * width);
		pageButtonsY = pageButtonsYPercent * height;
		prevPageX = pageButtonsInPercent * width - pageButtonSize / 2;
		nextPageX = width - pageButtonSize / 2 - pageButtonsInPercent * width;
		
		scoreX = width - width * scoreXPercent;
		scoreY = height * scoreYPercent;
		
		storeItemSize = Mathf.RoundToInt(width * storeItemSizePercent);
		for(int i=0; i<numberOfStoreItems; i++)
		{
			storeItemY[i] = Mathf.Lerp(storeStartY, storeFinishY, (float)i/(float)(numberOfStoreItems - 1)) * height;
			storePriceY[i] = storeItemY[i] + storePriceYPercent * height;
			storeNameY[i] = storeItemY[i] + storeNameYPercent * height;
		}
		storeItemX = Mathf.RoundToInt(width * storeItemXPercent);
		storePriceX = Mathf.RoundToInt(width * storePriceXPercent);
		storeNameX = Mathf.RoundToInt(width * storeNameXPercent);
		
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
		tutorialTextX = Mathf.RoundToInt(tutorialTextXPercent * width);
		tutorialTextY = Mathf.RoundToInt(tutorialTextYPercent * height);
		tutorialTextWidth = Mathf.RoundToInt(tutorialTextWidthPercent * width);
		tutorialCheckboxX = Mathf.RoundToInt(tutorialCheckboxXPercent * width);
		tutorialCheckboxY = Mathf.RoundToInt(tutorialCheckboxYPercent * height);
		tutorialCheckboxSize = Mathf.RoundToInt(tutorialCheckboxSizePercent * width);
		tutorialCheckX = Mathf.RoundToInt(tutorialCheckXPercent * width);
		tutorialCheckY = Mathf.RoundToInt(tutorialCheckYPercent * height);
		tutorialCheckSize = Mathf.RoundToInt(tutorialCheckSizePercent * width);
		tutorialCheckTextX = Mathf.RoundToInt(tutorialCheckTextXPercent * width);
		tutorialCheckTextY = Mathf.RoundToInt(tutorialCheckTextYPercent * height);
		tutorialAcceptX = Mathf.RoundToInt(tutorialAcceptXPercent * width);
		tutorialAcceptY = Mathf.RoundToInt(tutorialAcceptYPercent * height);
		tutorialAcceptWidth = Mathf.RoundToInt(tutorialAcceptWidthPercent * width);
		tutorialAcceptHeight = Mathf.RoundToInt(tutorialAcceptWidth/tutorialAcceptProportions);
		tutorialAcceptTextX = Mathf.RoundToInt(tutorialAcceptTextXPercent * width);
		tutorialAcceptTextY = Mathf.RoundToInt(tutorialAcceptTextYPercent * height);
	}
	
	private void DrawMenuButton()
	{
		if (GUIButtonTexture(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSize), menuButton))
		{
			menuOpen = !menuOpen;
			prizePopup = false;
			tutorialPopup = false;
			
			if (plant.state == Plant.PlantState.Dead && menuOpen)
				if (im.powerups[Constants.REVIVE_INDEX].inventory > 0)
					tm.TriggerTutorial(HAVE_REVIVE_TUT_ID);
				else
					tm.TriggerTutorial(NO_REVIVE_TUT_ID);
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
					im.Activate(prize);
					
					if (!powerupsUsedTutsDisp)
					{
						if (prize.name == Constants.DRY_SLOWER_NAME)
						{
							tm.TriggerTutorial(DRY_SLOWER_USED_TUT_ID);
							drySlowerUsedTutDisp = true;
							if (growFasterUsedTutDisp == true)
								powerupsUsedTutsDisp = true;
						}
						else if (prize.name == Constants.GROW_FASTER_NAME)
						{
							tm.TriggerTutorial(GROW_FASTER_USED_TUT_ID);
							if (drySlowerUsedTutDisp == true)
								powerupsUsedTutsDisp = true;
						}
						menuOpen = false;
					}
				}
				GUI.Label(new Rect(iconMultiplierX, yCoordinate + iconMultiplierYOffset, iconSize, iconSize), prizeInv.ToString(), quanitityStyle);
			}
			else
			{
				if (GUIButtonTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.texture))
					im.Activate(prize);
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
				collectablesPage--;
		
		if (start + numCollectablesToDraw < numberOfCollectables)
			if (GUIButtonTexture(new Rect(nextPageX, pageButtonsY, pageButtonSize, pageButtonSize), nextPage))
				collectablesPage++;
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
		if (GUIButtonTexture(new Rect(tutorialCheckboxX, tutorialCheckboxY, tutorialCheckboxSize, tutorialCheckboxSize), tutorialCheckbox))
			checkboxChecked = !checkboxChecked;
		GUI.Label(new Rect(tutorialCheckTextX, tutorialCheckTextY, 1000, 400), "Don't tell me again", tutorialStyle);
		if (checkboxChecked)
			GUI.DrawTexture(new Rect(tutorialCheckX, tutorialCheckY, tutorialCheckSize, tutorialCheckSize), tutorialCheck);
		if (GUIButtonTexture(new Rect(tutorialAcceptX, tutorialAcceptY, tutorialAcceptWidth, tutorialAcceptHeight), tutorialAcceptButton))
		{
			tm.DismissTutorial(checkboxChecked);
			tutorialPopup = false;
		}
		GUI.Label(new Rect(tutorialAcceptTextX, tutorialAcceptTextY, 1000, 400), "Gotcha", tutorialStyle);
	}
	#endregion
}
