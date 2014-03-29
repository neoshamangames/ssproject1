using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PrizeManager : SingletonMonoBehaviour<PrizeManager> {

	#region Attributes
	public enum Type {Powerup, Collectable}
	[Range(0,1)]public float chanceOfPowerup = .5f;
	
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
	
	#region Unity
	void Awake()
	{
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
		Debug.Log("powerupRarirtyTotal: " + powerupRarirtyTotal);
		Debug.Log("collectableRarirtyTotal: " + collectableRarirtyTotal);
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
				Debug.Log ("awarding piece " + piece + " of prize " + selectedPrize.name);
				selectedPrize.pieces[piece].inventory++;
				CheckForCompletePrize(selectedPrize);
				break;
			}
			selection -= selectedPrizes[i].rarity;
		}
	}
	#endregion
	
	#region Private
	private float powerupRarirtyTotal = 0;
	private float collectableRarirtyTotal = 0;
	private List<Prize> powerups;
	private List<Prize> collectables;
	
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
			Debug.Log(prize.name + " complete!");
		}
			
	}
	#endregion
}
