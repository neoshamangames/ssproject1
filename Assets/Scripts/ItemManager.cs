using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Soomla;
using Soomla.Store;

#if UNITY_EDITOR
using UnityEditor;
#endif
public class ItemManager : SingletonMonoBehaviour<ItemManager> {
	
	public enum Type {Powerup, Collectable};
	
	#region Events
	public delegate void PowerUpEvent(Prize powerup);
	public static event PowerUpEvent OnRevive;
	public static event PowerUpEvent OnPowerupActivated;
	#endregion
	
	#region Attributes
	public Plant plant;
	public List<Prize> powerups;
	public List<Prize> collectables;
	[Range(0,1)]public float chanceOfPowerup = .5f;
	public float growFasterTime;
	[Range(1,3)]public float growFasterMultiplier = 1.5f;
	public float drySlowerTime;
	[Range(0,1)]public float drySlowerMultiplier = .5f;
	
	#if UNITY_EDITOR
	[CustomPropertyDrawer (typeof(Prize))]
	class PrizeDrawer : PropertyDrawer
	{
		
		float piecesHeight;
		
		public override float GetPropertyHeight (SerializedProperty prop, GUIContent label)
		{
			
			SerializedProperty pieces = prop.FindPropertyRelative("pieces");
			float height = 200;
			piecesHeight = 0;
			if (pieces.isExpanded)
			{
				int count = pieces.CountInProperty();
				piecesHeight = count * 20;
				height += piecesHeight;
			}
			
			if (prop.FindPropertyRelative ("type").enumValueIndex == (int)Type.Powerup)
				height += 30;
			
			
			return height;
		}
		
		public override void OnGUI (Rect pos, SerializedProperty prop, GUIContent label) {
			
			SerializedProperty type = prop.FindPropertyRelative ("type");
			SerializedProperty name = prop.FindPropertyRelative ("name");
			SerializedProperty activeTexture = prop.FindPropertyRelative("activeTexture");
			SerializedProperty offTexture = prop.FindPropertyRelative("offTexture");
			SerializedProperty texture = prop.FindPropertyRelative("texture");
			SerializedProperty multipleTexture = prop.FindPropertyRelative("multipleTexture");
			SerializedProperty rarity = prop.FindPropertyRelative ("rarity");
			SerializedProperty pieces = prop.FindPropertyRelative("pieces");
			SerializedProperty inventory = prop.FindPropertyRelative("inventory");
			
			Rect nameRect = new Rect (pos.x, pos.y + 5, pos.width, 15);
			Rect typeRect = new Rect (pos.x, pos.y + 25, pos.width, 15);
			Rect activeTextureRect = new Rect (pos.x, pos.y + 45, pos.width, 15);
			Rect offTextureRect = new Rect (pos.x, pos.y + 65, pos.width, 15);
			Rect textureRect = new Rect (pos.x, pos.y + 85, pos.width, 15);
			Rect multipleTextureRect = new Rect (pos.x, pos.y + 105, pos.width, 15);
			Rect rarityRect = new Rect (pos.x, pos.y + 125, pos.width, 15);
			Rect piecesRect = new Rect(pos.x, pos.y + 145, pos.width, 15);
			Rect inventoryRect = new Rect(pos.x, pos.y + 165 + piecesHeight, pos.width, 15);
			
			int indent = EditorGUI.indentLevel;
			//			EditorGUI.indentLevel = 0;
			
			EditorGUI.PropertyField(nameRect, name);
			EditorGUI.PropertyField(typeRect, type);
			if (type.enumValueIndex == (int)Type.Powerup)
			{
				EditorGUI.PropertyField(activeTextureRect, activeTexture);
				EditorGUI.PropertyField(multipleTextureRect, multipleTexture);
			}
			EditorGUI.PropertyField(offTextureRect, offTexture);
			EditorGUI.PropertyField(textureRect, texture);
			EditorGUI.PropertyField(rarityRect, rarity);
			EditorGUI.PropertyField(inventoryRect, inventory);
			EditorGUI.PropertyField(piecesRect, pieces, true);
			
			//			Debug.Log (type.enumValueIndex);
			if (type.enumValueIndex == (int)Type.Powerup)
			{
				SerializedProperty powerupMultiplier = prop.FindPropertyRelative("powerupMultiplier");
				SerializedProperty powerupActiveTime = prop.FindPropertyRelative("powerupActiveTime");
				SerializedProperty powerupTimeRemaining = prop.FindPropertyRelative("powerupTimeRemaining");
				SerializedProperty powerupValue = prop.FindPropertyRelative("powerupValue");
				
				Rect powerupMultiplierRect = new Rect(pos.x, pos.y + 185 + piecesHeight, pos.width/2, 15);
				Rect powerupActiveTimeRect = new Rect(pos.x, pos.y + 205 + piecesHeight, pos.width/2, 15);
				Rect powerupTimeRemainingRect = new Rect(pos.x + pos.width/2, pos.y + 205 + piecesHeight, pos.width/2, 15);
				Rect powerupValueRect = new Rect(pos.x + pos.width/2, pos.y + 185 + piecesHeight, pos.width/2, 15);
				
				EditorGUI.PropertyField(powerupMultiplierRect, powerupMultiplier);
				EditorGUI.PropertyField(powerupActiveTimeRect, powerupActiveTime);
				EditorGUI.PropertyField(powerupTimeRemainingRect, powerupTimeRemaining);
				EditorGUI.PropertyField(powerupValueRect, powerupValue);			
			}
			
			
			//			EditorGUI.PropertyField (
			//				new Rect (pos.width - curveWidth, pos.y, curveWidth, pos.height),
			//				curve, GUIContent.none);
			EditorGUI.indentLevel = indent;
			
			//			EditorGUI.EndProperty ();
		}
	}
	#endif
	
	[System.Serializable]
	public class Prize
	{
		public Type type;
		public string name;
		public Texture2D activeTexture;
		public Texture2D offTexture;
		public Texture2D texture;
		public Texture2D multipleTexture;
		[Range(.1f, 2)]public float rarity = 1f;
		public Piece[] pieces;
		public int inventory;
		public float powerupMultiplier = 1;
		public float powerupActiveTime = 60;
		public float powerupTimeRemaining;
		public float powerupValue = 1;
	}
	
	[System.Serializable]
	public class Piece
	{
		public Texture2D texture;
		public Texture2D offTexture;
		public int inventory;
	}
	
	public Prize[] prizes;
	#endregion
	
	#region Properties
	public float GrowMultiplier { get{return powerups[0].powerupValue;} }
	public float DryMultiplier { get{return powerups[1].powerupValue;} }
	#endregion
	
	#region Unity
	void Awake()
	{
		dm = DataManager.Instance;
		StoreEvents.OnMarketPurchase += onMarketPurchase;
		StoreEvents.OnItemPurchased += onItemPurchased;
		StoreEvents.OnGoodBalanceChanged += onGoodBalanceChanged;
		
		powerups = new List<Prize>();
		collectables = new List<Prize>();
		foreach(Prize prize in prizes)
		{
			if (prize.type == Type.Powerup)
			{
				powerupRarirtyTotal += prize.rarity;
				prize.powerupValue = 1f;
				powerups.Add(prize);
			}
			else
			{
				collectableRarirtyTotal += prize.rarity;
				collectables.Add(prize);
			}
		}
		
		for(int i=0; i < powerups.Count; i++)
		{
			if (powerups[i].name  == REVIVE_NAME)
				{
					reviveIndex = i;
					break;
				}
		}
	}
	
	void Start()
	{
		UpdateBalances();
		foreach(Prize powerup in powerups)
		{
			if (powerup.powerupTimeRemaining > 0)
			{
				powerup.powerupValue = powerup.powerupMultiplier;
				OnPowerupActivated(powerup);
			}
		}
	}
	
	void Update()
	{
		float deltaTime = Time.deltaTime;
		foreach(Prize powerup in powerups)
		{
			if (powerup.powerupTimeRemaining > 0)
			{
				powerup.powerupTimeRemaining -= deltaTime;
				if (powerup.powerupTimeRemaining < 0)
				{
					powerup.powerupValue = 1;
					powerup.powerupTimeRemaining = 0;
				}
			}
		}
	}
	
	/*
	void OnGUI()//temp
	{
		float y = 10;
		GUI.enabled = false;
		foreach(Prize prize in prizes)
		{
			if (prize.type == Type.Powerup && prize.inventory > 0 && IsPowerupUseable(prize))
				GUI.enabled = true;
			else
				GUI.enabled = false;
			if (GUI.Button (new Rect (10, y, 200, 25), FormatPrizeString(prize)))
				Activate(prize);
			y += 35;
		}
		if (powerups[0].powerupTimeRemaining > 0)
		{
			GUI.Label(new Rect(25, Screen.height - 50, 500, 25), "grow faster active " + (int)powerups[0].powerupTimeRemaining + " seconds remain");
			Debug.Log("grow faster active " + (int)powerups[0].powerupTimeRemaining + " seconds remain");
		}
		if (powerups[1].powerupTimeRemaining > 0)
			GUI.Label(new Rect(25, Screen.height - 100, 500, 25), "dry slower active " + (int)powerups[1].powerupTimeRemaining + " seconds remain");
	}
	*/
	#endregion
	
	#region Actions
	public void AwardPrize()
	{
		bool powerupSelected = (Random.Range(0.0f, 1.0f) < chanceOfPowerup);
		float selection = Random.Range(0, powerupSelected ? powerupRarirtyTotal : collectableRarirtyTotal);
		List<Prize> selectedPrizes = powerupSelected ? powerups : collectables;
		for(int i=0; i < selectedPrizes.Count; i++)
		{
			Prize selectedPrize = selectedPrizes[i];
			if (selection < selectedPrize.rarity)
			{
				int piece = Random.Range(0, selectedPrize.pieces.Length);
				selectedPrize.pieces[piece].inventory++;
				CheckForCompletePrize(selectedPrize);
				if (!powerupSelected)
					dm.StoreCollectableIndex((ushort)i);
				break;
			}
			selection -= selectedPrizes[i].rarity;
		}
	}
	
	public void Activate(Prize prize)
	{
		if (plant.state == Plant.PlantState.Dead)
		{
			if (prize.name == REVIVE_NAME)
			{
				OnRevive(prize);
				prize.inventory--;
				StoreInventory.TakeItem(Constants.REVIVE_ID, 1);
			}
		
		}
		else
		{
			if (prize.name != REVIVE_NAME)
			{
				prize.powerupValue = prize.powerupMultiplier;
				prize.powerupTimeRemaining += prize.powerupActiveTime;
				prize.inventory--;
				OnPowerupActivated(prize);
			}
		}
		
	}
	#endregion
	
	#region Handlers
	private void onMarketPurchase(PurchasableVirtualItem pvi, string purchaseToken, string payload)
	{
		Debug.Log ("purchase successful name: " + pvi.Name + ", itemId: " + pvi.ItemId + ", ID: " + pvi.ID);
		int reviveBalance = StoreInventory.GetItemBalance("revive"); 
		int packBalance = StoreInventory.GetItemBalance(Constants.REVIVE_3_PACK_ID);
		Debug.Log ("reviveBalance: " + reviveBalance);
		Debug.Log ("packBalance: " + packBalance);
	}

	
	private void onItemPurchased(PurchasableVirtualItem pvi, string payload)
	{
		Debug.Log ("onItemPurchased");
	}
		
	private void onGoodBalanceChanged(VirtualGood good, int balance, int amountAdded) {
		Debug.Log ("Balance of good " + good.ItemId + " changed to " + balance + ". Added " + amountAdded);
		UpdateBalances();
	}
	
	private void UpdateBalances()
	{
		prizes[REVIVE_INDEX].inventory = StoreInventory.GetItemBalance(Constants.REVIVE_ID);
		prizes[0].inventory = 10;//TODO
		prizes[1].inventory = 10;//TODO
	}
	#endregion
		
	#region Private
	private const string REVIVE_NAME = "Revive Plant";
	private const int REVIVE_INDEX = 2;
	private int reviveIndex;
	private DataManager dm;
	private float powerupRarirtyTotal = 0;
	private float collectableRarirtyTotal = 0;
	
	private void CheckForCompletePrize(Prize prize)
	{
		bool haveAllPieces = true;
		foreach(Piece piece in prize.pieces)
		{
			if (piece.inventory == 0)
			{
				haveAllPieces = false;
				break;
			}
		}
		if (haveAllPieces)
		{
			foreach(Piece piece in prize.pieces)
				piece.inventory--;
			
			prize.inventory++;
		}
		
	}
	
	private string FormatPrizeString(Prize prize)
	{
		string s = prize.name + "(" + prize.inventory + ") pieces: ";
		int length = prize.pieces.Length;
		for(int i=0; i<length; i++)
		{
			s += prize.pieces[i].inventory;
			if (i < length - 1)
				s += ", ";
		}
		return s;
	}
	
	private bool IsPowerupUseable(Prize powerup)
	{
		switch (powerup.name)
		{
		case "revive":
			return (plant.state == Plant.PlantState.Dead);
		default:
			return (plant.state != Plant.PlantState.Dead);
		}
	}
	#endregion
	}
