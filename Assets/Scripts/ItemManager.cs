using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemManager : SingletonMonoBehaviour<ItemManager> {

	public enum Type {Powerup, Collectable};

	#region Events
	public delegate void PowerUpEvent();
	public static event PowerUpEvent OnRevive;
	#endregion

	#region Attributes
	public Plant plant;
	[Range(0,1)]public float chanceOfPowerup = .5f;
	public float growFasterTime;
	[Range(1,3)]public float growFasterMultiplier = 1.5f;
	public float drySlowerTime;
	[Range(0,1)]public float drySlowerMultiplier = .5f;
	
	[System.Serializable]
	public class Prize
	{
		public Type type;
		public string name;
		[Range(.1f, 2)]public float rarity = 1f;
		public Piece[] pieces;
		public int inventory;
	}
	
	[System.Serializable]
	public class Piece
	{
		public Texture2D texture;
		public int inventory;
	}
	
	public Prize[] prizes;
	#endregion
	
	#region Properties
	public float GrowMultiplier { get{return grow.value;} }
	public float DryMultiplier { get{return dry.value;} }
	#endregion
	
	#region Unity
	void Awake()
	{
		dry.value = 1;
		grow.value = 1;
		
		powerups = new List<Prize>();
		collectables = new List<Prize>();
		foreach(Prize prize in prizes)
		{
			if (prize.type == Type.Powerup)
			{
				powerupRarirtyTotal += prize.rarity;
				powerups.Add(prize);
			}
			else
			{
				collectableRarirtyTotal += prize.rarity;
				collectables.Add(prize);
			}
		}
	}
	
	void Update()
	{
		float deltaTime = Time.deltaTime;
		if (dry.timeRemaining > 0)
		{
			dry.timeRemaining -= deltaTime;
			if (dry.timeRemaining < 0)
				dry.value = 1f;
		}
		if (grow.timeRemaining > 0)
		{
			Debug.Log("grow faster active " + (int)grow.timeRemaining + " seconds remain");
			grow.timeRemaining -= deltaTime;
			if (grow.timeRemaining < 0)
				grow.value = 1f;
		}
	}
	
	void OnGUI()//temp
	{
		float y = 10;
		GUI.enabled = false;
		foreach(Prize prize in prizes)
		{
			if (prize.type == Type.Powerup && prize.inventory > 0 && IsPrizeUseable(prize))
				GUI.enabled = true;
			else
				GUI.enabled = false;
			if (GUI.Button (new Rect (10, y, 200, 25), FormatPrizeString(prize)))
				Activate(prize);
			y += 35;
		}
		if (grow.timeRemaining > 0)
		{
			GUI.Label(new Rect(25, Screen.height - 50, 500, 25), "grow faster active " + (int)grow.timeRemaining + " seconds remain");
			Debug.Log("grow faster active " + (int)grow.timeRemaining + " seconds remain");
		}
		if (dry.timeRemaining > 0)
			GUI.Label(new Rect(25, Screen.height - 100, 500, 25), "dry slower active " + (int)dry.timeRemaining + " seconds remain");
	}
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
				break;
			}
			selection -= selectedPrizes[i].rarity;
		}
	}
	#endregion
	
	#region Private
	private struct Bonus
	{
		public float value;
		public float timeRemaining;
	}
	
	private float powerupRarirtyTotal = 0;
	private float collectableRarirtyTotal = 0;
	private List<Prize> powerups;
	private List<Prize> collectables;
	private Bonus grow;
	private Bonus dry;
	
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
	
	private bool IsPrizeUseable(Prize prize)
	{
		switch (prize.name)
		{
		case "revive":
			return (plant.state == Plant.PlantState.Dead);
		default:
			return (plant.state != Plant.PlantState.Dead);
		}
	}
	
	private void Activate(Prize prize)
	{
		Debug.Log ("Activating " + prize.name);
		switch (prize.name)
		{
		case "revive":
			OnRevive();
			break;
		case "dry slower":
			dry.value = drySlowerMultiplier;
			dry.timeRemaining = drySlowerTime;
			break;
		case "grow faster":
			Debug.Log ("grow faster case");
			grow.value = growFasterMultiplier;
			grow.timeRemaining = growFasterTime;
			break;
		}
		prize.inventory--;
	}
	#endregion
}
