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
	public Font font;
	public Texture menuButton;
	public Texture muteButton;
	public Texture unmuteButton;
	public Texture frame;
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
	[Range(-1, 1)]public float powerup1_YPercent = -.25f;
	[Range(-1, 1)]public float powerup2_YPercent = -.05f;
	[Range(-1, 1)]public float powerup3_YPercent = .1f;
	[Range(-1, 1)]public float piecesFinishX = .25f;
	[Range(-1, 1)]public float piecesStartX = .05f;
	[Range(-1, 1)]public float multiplierXPercent = .5f;
	[Range(-1, 1)]public float multiplierYPercent = .75f;
	[Range(0, 1)]public float powerUpLabelXPercent = 0;
	[Range(-1, 1)]public float powerUpLabelYPercent = 0;
	[Range(0, 1)]public float textButtonXPercent = .5f;
	[Range(-1, 1)]public float textButtonYPercent = .5f;
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
	[Range(0, 5)]public float powpupFadeTime = 1f;
	public float awaredPieceShiftPercent = 0f;
	#endregion
	
	#region Actions
	public void PrizePopup(ItemManager.Prize prize, int pieceIndex)
	{
		popup = true;
		menuOpen = false;
		awardePrize = prize;
		awardedPieceIndex = pieceIndex;
		popupTimer = 0;
		Debug.Log ("awardedPieceIndex: " + awardedPieceIndex);
		
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		im = ItemManager.Instance;
		am = AudioManager.Instance;
		cm = GetComponent<CameraManager>();
		
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
		
		multiplierStyle = new GUIStyle();
		multiplierStyle.font = font;
		multiplierStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		multiplierStyle.normal.textColor = Color.white;
		multiplierStyle.alignment = TextAnchor.UpperCenter;
		
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
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(centerX - frameWidth / 2, 0, frameWidth, height), frame, ScaleMode.ScaleToFit);
			
			switch (menuState)
			{
			case MenuState.POWERUPS:
				
				DrawPowerUpGUI(im.powerups[0], powerup1Y);
				DrawPowerUpGUI(im.powerups[1], powerup2Y);
				DrawPowerUpGUI(im.powerups[2], powerup3Y);
				
				DrawCreditsButton();
				DrawShopButton();
				
				break;
								
			case MenuState.CREDITS:
				DrawCredits();
				DrawBackButton(textButton2X);
				DrawShopButton();
				break;
				
			case MenuState.SHOP:
			
				DrawCreditsButton();
				DrawBackButton(textButton1X);
				
				for(int i=0; i<numberOfStoreItems; i++)
				{
					DrawStoreItem(i);
				}
				
				break;
			
			}
			
			
			if (GUIButtonTexture(new Rect(muteButtonX, muteButtonY, muteButtonSize, muteButtonSize), muted ? unmuteButton : muteButton))
//			if (GUI.Button(new Rect(muteButtonX, muteButtonY, muteButtonSize, muteButtonSize), muted ? unmuteButton : muteButton, buttonStyle))
			{
				muted = !muted;
				am.ToggleMute();
			}	
		}
		else
		{
			DrawMenuButton();
		}
		
		if (popup)
		{
			GUI.Label(new Rect(0, popupTextY, width, 30), "You've got a prize!", creditsStyle);
			
			GUI.color = Color.white;
			GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardePrize.popupTexture);
			
			GUI.DrawTexture(new Rect(awardedPrizeX[awardedPieceIndex], awardedPrizeY[awardedPieceIndex], awardedPrizeSize, awardedPrizeSize), awardePrize.pieces[awardedPieceIndex].texture);
			
			
			for(int i = 0; i < 4; i++)
			{
				byte t = (byte)Mathf.RoundToInt(popupTimer/ powpupFadeTime * 255);
				if (popupTimer < powpupFadeTime && i == awardedPieceIndex && awardePrize.pieces[i].inventory < 2)
				{
					GUI.color = new Color32(255, 255, 255, t);
					GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardePrize.pieces[i].texture);//fade in
					GUI.color = Color.white;
					popupTimer += Time.deltaTime;
				}
				else
					if (awardePrize.pieces[i].inventory > 0)
						GUI.DrawTexture(new Rect(centerX - popupPrizeSize/2, popupPrizeY, popupPrizeSize, popupPrizeSize), awardePrize.pieces[i].texture);
			}
					
			if (GUI.Button(new Rect(0, 0, width, height), "", buttonStyle))
				popup = false;
				
			
		}
		
		if (plant.state == Plant.PlantState.Dead)
		{
			if (GUI.Button(new Rect(centerX, 15, 100, 50), "Reset Plant", buttonStyle))//temp
			{
				plant.Reset();
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
	private enum MenuState {POWERUPS, CREDITS, SHOP};
	private const int SECONDS_PER_HOUR = 3600;
	private const int SECONDS_PER_MINUTE = 60;
	private ItemManager im;
	private CameraManager cm;
	private AudioManager am;
	private MenuState menuState = MenuState.POWERUPS;
	private float height, width;
	private float centerX, centerY;
	private bool menuOpen;
	private GUIStyle buttonStyle, labelStyle, creditsStyle, multiplierStyle, scoreStyle;
	private float frameWidth;
	private int iconSize;
	private float iconX, iconMultiplierX, iconMultiplierYOffset;
	private float[] pieceX;
	private float[] pieceYOffset;
	private float[] multiplierX;
	private float multiplierYOffset;
	private float powerup1Y, powerup2Y, powerup3Y;
	private float powerUpLabelX, powerUpLabelY;
	private float textButton1X, textButton2X, textButtonY;
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
	private bool popup;
	private ItemManager.Prize awardePrize;
	private int awardedPieceIndex;
	private int awardedPrizeSize, popupPrizeSize;
	private float popupPrizeY, popupTextY;
	private float[] awardedPrizeX, awardedPrizeY;
	private float popupTimer;
	
	private bool GUIButtonTexture( Rect r, Texture t)
	{
		GUI.DrawTexture( r, t);
		return GUI.Button( r, "", buttonStyle);
	}
	
	private void CalculateValues()
	{
		frameWidth = width * frameWidthPercent;
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
			pieceX[0] + width * multiplierXPercent,
			pieceX[1] + width * multiplierXPercent,
			pieceX[2] + width * multiplierXPercent + iconSize/4,
			pieceX[3] + width * multiplierXPercent + iconSize/4
		};
		
		multiplierYOffset = height * multiplierYPercent;
		
		powerup1Y = centerY + height * powerup1_YPercent;
		powerup2Y = centerY + height * powerup2_YPercent;
		powerup3Y = centerY + height * powerup3_YPercent;
		
		powerUpLabelX = powerUpLabelXPercent * frameWidth;
		powerUpLabelY = powerUpLabelYPercent * frameWidth;
		
		textButton1X = centerX + textButtonXPercent * frameWidth - 50;
		textButton2X = centerX - textButtonXPercent * frameWidth - 50;
		textButtonY = centerY + textButtonYPercent * frameWidth;
		
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
		
		awardedPrizeX[0] = centerX - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[1] = centerX - awardedPrizeSize/ 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[2] = centerX - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeX[3] = centerX - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		
		awardedPrizeY[0] = awardedPrizeYPercent * height - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[1] = awardedPrizeYPercent * height - awardedPrizeSize/ 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[2] = awardedPrizeYPercent * height - awardedPrizeSize / 2 - awardedPrizeSize * awaredPieceShiftPercent;
		awardedPrizeY[3] = awardedPrizeYPercent * height - awardedPrizeSize / 2 + awardedPrizeSize * awaredPieceShiftPercent;
		
	}
	
	private void DrawMenuButton()
	{
//		if (GUI.Button(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSizeY), menuButton, buttonStyle))
		if (GUIButtonTexture(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSize), menuButton))
		{
			menuOpen = !menuOpen;
			popup = false;
		}

//		GUI.DrawTexture(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSize), menuButton);
	}
	
	private void DrawShopButton()
	{
		if (GUI.Button(new Rect(textButton1X, textButtonY, 100, 50), "SHOP", buttonStyle))
		{
			SoomlaStore.StartIabServiceInBg();
			menuState = MenuState.SHOP;
		}
	}
	
	private void DrawCreditsButton()
	{
		if (GUI.Button(new Rect(textButton2X, textButtonY, 100, 50), "CREDITS", buttonStyle))
		{
			SoomlaStore.StopIabServiceInBg();
			menuState = MenuState.CREDITS;
		}
	}
	
	private void DrawBackButton(float x)
	{
		if (GUI.Button(new Rect(x, textButtonY, 100, 50), "BACK", buttonStyle))
		{
			SoomlaStore.StopIabServiceInBg();
			menuState = MenuState.POWERUPS;
		}
	}
	
	private void DrawPowerUpGUI(ItemManager.Prize prize, float yCoordinate)
	{
		GUI.Label(new Rect(powerUpLabelX, yCoordinate + powerUpLabelY, 100, 50), prize.name, labelStyle);
		int prizeInv = prize.inventory;
		
		if (prizeInv > 0)
		{
			if (prizeInv > 1)
			{
				if (GUIButtonTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.multipleTexture))
//				if (GUI.Button(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.multipleTexture, buttonStyle))
					im.Activate(prize);
				GUI.Label(new Rect(iconMultiplierX, yCoordinate + iconMultiplierYOffset, iconSize, iconSize), prizeInv.ToString(), multiplierStyle);
			}
			else
			{
				if (GUIButtonTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.texture))
//				if (GUI.Button(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.texture, buttonStyle))
					im.Activate(prize);
			}
		}
			
		else
			GUI.DrawTexture(new Rect(iconX, yCoordinate, iconSize, iconSize), prize.offTexture);
//			GUI.Button(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.offTexture, buttonStyle);
		
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
//		if (GUI.Button(new Rect(storeItemX, storeItemY[i], storeItemSize, storeItemSize), storeItems[i].texture, buttonStyle))
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
	#endregion
}
