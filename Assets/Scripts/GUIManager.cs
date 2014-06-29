using UnityEngine;
using System;
using System.Collections;

public class GUIManager : MonoBehaviour {
	
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
	[Range(0, 1)]public float powerUpLabelXPercent = 0;
	[Range(-1, 1)]public float powerUpLabelYPercent = 0;
	[Range(0, 1)]public float textButtonXPercent = .5f;
	[Range(-1, 1)]public float textButtonYPercent = .5f;
	[Range(-1, 1)]public float scoreXPercent = .5f;
	[Range(-1, 1)]public float scoreYPercent = .5f;
	public float fontSizeInverse = 20f;
	#endregion
	
	#region Unity
	void Awake()
	{
		im = ItemManager.Instance;
		am = AudioManager.Instance;
		cm = GetComponent<CameraManager>();
		
		muted = am.Muted;
		
		width = Screen.width;
		height = Screen.height;
		centerX = width / 2;
		centerY = height / 2;
		
		labelStyle = new GUIStyle();
		labelStyle.font = font;
		labelStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		labelStyle.normal.textColor = Color.white;
		labelStyle.alignment = TextAnchor.MiddleCenter;
		
		
		scoreStyle = new GUIStyle();
		scoreStyle.font = font;
		scoreStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		scoreStyle.normal.textColor = scoreColor;
		scoreStyle.alignment = TextAnchor.MiddleRight;
		
		multiplierStyle = new GUIStyle();
		multiplierStyle.font = font;
		multiplierStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		multiplierStyle.normal.textColor = Color.white;
		multiplierStyle.alignment = TextAnchor.MiddleLeft;
		
		buttonStyle = new GUIStyle();
		buttonStyle.font = font;
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.fontSize = Mathf.RoundToInt(Screen.width / fontSizeInverse);
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.border = new RectOffset(0, 0, 0, 0);
		
		CalculateValues();
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
			DrawPowerUpGUI(im.powerups[0], powerup1Y);
			DrawPowerUpGUI(im.powerups[1], powerup2Y);
			DrawPowerUpGUI(im.powerups[2], powerup3Y);
			GUI.Button(new Rect(textButton1X, textButtonY, 100, 50), "SHOP", buttonStyle);
			GUI.Button(new Rect(textButton2X, textButtonY, 100, 50), "CREDITS", buttonStyle);
			if (GUI.Button(new Rect(muteButtonX, muteButtonY, muteButtonSize, muteButtonSize), muted ? unmuteButton : muteButton, buttonStyle))
			{
				muted = !muted;
				am.ToggleMute();
			}
			
		}
		else
		{
			DrawMenuButton();
		}
		
		if (plant.state == Plant.PlantState.Dead)
		{
			if (GUI.Button(new Rect(5, 5, 100, 50), "Reset Plant", buttonStyle))//temp
			{
				plant.Reset();
				cm.Reset();
			}
		}
		
		GUI.Button(new Rect(scoreX, scoreY, 200, 50), Mathf.RoundToInt(plant.Height).ToString(), scoreStyle);
	}
	#endregion
	
	#region Private
	private const float MULTIPLIER_Y_OFFSET_PERCENT = .75f;
	private ItemManager im;
	private CameraManager cm;
	private AudioManager am;
	private float height, width;
	private float centerX, centerY;
	private bool menuOpen;
	private GUIStyle buttonStyle, labelStyle, multiplierStyle, scoreStyle;
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
	
	private void DrawMenuButton()
	{
		if (GUI.Button(new Rect(menuButtonX, menuButtonY, menuButtonSize, menuButtonSize), menuButton, buttonStyle))
			menuOpen = !menuOpen;
	}
	
	private void CalculateValues()
	{
		frameWidth = width * frameWidthPercent;
		iconX = centerX - frameWidth * iconXPercent;
		iconMultiplierX = iconX + iconMultiplierXPercent * iconX;
		iconMultiplierYOffset = iconMultiplierYPercent * iconX;
		
		iconSize = Mathf.RoundToInt(frameWidth * iconSizePercent);
		
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
			pieceX[0] + iconSize * multiplierXPercent,
			pieceX[1] + iconSize * multiplierXPercent,
			pieceX[2] + iconSize * multiplierXPercent,
			pieceX[3] + iconSize * multiplierXPercent
		};
		
		multiplierYOffset = iconSize * MULTIPLIER_Y_OFFSET_PERCENT;
		
		powerup1Y = centerY + height * powerup1_YPercent;
		powerup2Y = centerY + height * powerup2_YPercent;
		powerup3Y = centerY + height * powerup3_YPercent;
		
		powerUpLabelX = powerUpLabelXPercent * frameWidth;
		powerUpLabelY = powerUpLabelYPercent * frameWidth;
		
		textButton1X = centerX + textButtonXPercent * frameWidth - 50;
		textButton2X = centerX - textButtonXPercent * frameWidth - 50;
		textButtonY = centerY + textButtonYPercent * frameWidth;
		
		scoreX = width * scoreXPercent;
		scoreY = height * scoreYPercent;
	}
	
	private void DrawPowerUpGUI(ItemManager.Prize prize, float yCoordinate)
	{
		GUI.Label(new Rect(powerUpLabelX, yCoordinate + powerUpLabelY, 100, 50), prize.name, labelStyle);
		int prizeInv = prize.inventory;
		
		if (prizeInv > 0)
		{
			if (prizeInv > 1)
			{
				if (GUI.Button(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.multipleTexture, buttonStyle))
					im.Activate(prize);
				GUI.Label(new Rect(iconMultiplierX, yCoordinate + iconMultiplierYOffset, iconSize, iconSize), prizeInv.ToString(), multiplierStyle);
			}
			else
			{
				if (GUI.Button(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.texture, buttonStyle))
					im.Activate(prize);
			}
		}
			
		else
			GUI.DrawTexture(new Rect(iconX, yCoordinate, iconSize, iconSize),  prize.offTexture);
		
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
	#endregion
}
