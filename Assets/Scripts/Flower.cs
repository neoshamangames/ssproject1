using UnityEngine;
using System.Collections;

public class Flower : MonoBehaviour {

	public enum FlowerState {PreBloom, Budding, Blooming, Fruited, Harvested};
	
	#region Attributes
	public Plant.StemmingAttributes stemming;
	public FlowerState state = FlowerState.PreBloom;
	public Sprite budSprite;
	public Sprite bloomSprite;
	#endregion
	
	#region Properties
	public float GrowthState
	{
		get
		{
			float val = 0;
			switch (state)
			{
			case FlowerState.PreBloom:
				val = growthCounter - nextFlowerDelay;//should be negative
				Debug.Log ("growthCounter: " + growthCounter + ", nextFlowerDelay: " + nextFlowerDelay);
				Debug.Log ("GrowthState PreBloom. val: " + val);
				break;
				
			case FlowerState.Blooming:
				val = transform.localScale.x;//should be positive
				break;
				
			case FlowerState.Budding:				
			case FlowerState.Fruited:
				val = 0;
				break;
				
			case FlowerState.Harvested:
				val = -nextFlowerDelay;
				break;
			}
			return val;
		}
	}
	#endregion

	#region Unity
	void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		im = ItemManager.Instance;
		tm = TutorialManager.Instance;
//		fruitedColor = Color.red;
	}
	
	void Start()
	{
		if (!flowerStateLoaded)
			nextFlowerDelay = stemming.flowerDelay;
	}
	#endregion
	
	#region Actions
	public void LoadGrowthState(float growthState)
	{
		if (growthState == 0)
		{
			state = FlowerState.Fruited;
			float scale = stemming.maxFlowerSize;
			transform.localScale = new Vector3(scale, scale, scale);
//			sr.color = Color.red;
			sr.sprite = bloomSprite;
		}
		else if (growthState < 0)
		{
			state = FlowerState.PreBloom;
			nextFlowerDelay = -growthState;
			flowerStateLoaded = true;
			Debug.Log ("flowerStateLoaded nextFlowerDelay: " + nextFlowerDelay);
		}
		else
		{
			state = FlowerState.Blooming;
			transform.localScale = new Vector3(growthState, growthState, growthState);
			tm.TriggerTutorial(6);
		}
	}
	
	public void SetAlpha(float alpha)
	{
		Color color = sr.color;
		color.a = alpha;
		sr.color = color;
	}
	
	public void Grow(float newGrowth)
	{
		float deltaTime = Time.deltaTime;
		switch (state)
		{
		case FlowerState.PreBloom:
			growthCounter += newGrowth;
			if (growthCounter > nextFlowerDelay)
				state = FlowerState.Blooming;
			break;
		case FlowerState.Blooming:
			float flowerGrowth = newGrowth * stemming.flowerGrowthFactor;
			float scale = transform.localScale.x + flowerGrowth;
			if (scale > stemming.maxFlowerSize)
			{
				scale = stemming.maxFlowerSize;
				state = FlowerState.Budding;
			}
			transform.localScale = new Vector3(scale, scale, scale);
			break;
		case FlowerState.Budding:
//			float t = transitionTime/stemming.buddingTime;
			if (transitionTime >= stemming.buddingTime)
			{
//				t = 1;
				state = FlowerState.Fruited;
				sr.sprite = bloomSprite;
				tm.TriggerTutorial(6);
			}
			transitionTime += deltaTime;
//			sr.color = Color.Lerp(Color.white, fruitedColor, t);
			//animation will play here
			break;
		case FlowerState.Fruited:
			break;
		case FlowerState.Harvested:
			float t2 = transitionTime/stemming.harvestingTime;
			sr.color = Color.Lerp(Color.white, Color.clear, t2);
			if (transitionTime >= stemming.harvestingTime)
			{
				PrepareNextBud();
			}
			transitionTime += deltaTime;
			break;
			
		}
	}
	
	public void CatchupGrowth(float newGrowth)
	{
		nextFlowerDelay = stemming.flowerDelay;
//		Debug.Log ("CatchupGrowth newGrowth: " + newGrowth + ", nextFlowerDelay: " + nextFlowerDelay + ", growthCounter: " + growthCounter);
		
		if (state == FlowerState.PreBloom)
		{
			float growthToBloom = nextFlowerDelay - growthCounter;
			growthCounter += newGrowth;
			if (growthCounter > nextFlowerDelay)
			{
				state = FlowerState.Blooming;
				newGrowth -= growthToBloom;
			}
		}
		if (state == FlowerState.Blooming)
		{
			float flowerGrowth = newGrowth * stemming.flowerGrowthFactor;
			float scale = transform.localScale.x + flowerGrowth;
			if (scale > stemming.maxFlowerSize)
			{
				scale = stemming.maxFlowerSize;
				state = FlowerState.Fruited;
//				sr.color = fruitedColor;
				sr.sprite = bloomSprite;
			}
			transform.localScale = new Vector3(scale, scale, scale);
		}
			
	}
	
	public void ProcessClick()
	{
		if (state == FlowerState.Fruited)
		{
			transitionTime = 0;
			state = FlowerState.Harvested;
			im.AwardPrize();
			
		}
	}
	#endregion
	
	#region Private
	private SpriteRenderer sr;
	private ItemManager im;
	private TutorialManager tm;
	private float transitionTime = 0;
	private float growthCounter = 0;
	private float nextFlowerDelay;
	private bool flowerStateLoaded;
	private Color fruitedColor;
	
	private void PrepareNextBud()
	{
		transitionTime = 0;
		growthCounter = 0;
		nextFlowerDelay = stemming.newFlowerDelay;
		transform.localScale = new Vector3(0, 0, 0);
		sr.color = Color.white;
		state = FlowerState.PreBloom;
		sr.sprite = budSprite;
	}
	#endregion
}
