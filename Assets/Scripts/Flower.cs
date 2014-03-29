using UnityEngine;
using System.Collections;

public class Flower : MonoBehaviour {

	#region Attributes
	public Plant.StemmingAttributes stemming;
	#endregion

	#region Unity
	void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		pm = PrizeManager.Instance;
	}
	
	void Start()
	{
		nextFlowerDelay = stemming.flowerDelay;
	}
	#endregion
	
	#region Actions
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
			float t = transitionTime/stemming.buddingTime;
			if (transitionTime >= stemming.buddingTime)
			{
				t = 1;
				state = FlowerState.Fruited;
			}
			transitionTime += deltaTime;
			sr.color = Color.Lerp(Color.white, Color.red, t);
			//animation will play here
			break;
		case FlowerState.Fruited:
			break;
		case FlowerState.Harvested:
			float t2 = transitionTime/stemming.harvestingTime;
			sr.color = Color.Lerp(Color.red, Color.clear, t2);
			if (transitionTime >= stemming.harvestingTime)
			{
				PrepareNextBud();
			}
			transitionTime += deltaTime;
			break;
			
		}
	}
	
	public void ProcessClick()
	{
		if (state == FlowerState.Fruited)
		{
			Debug.Log ("harvesting flower");
			transitionTime = 0;
			state = FlowerState.Harvested;
			pm.AwardPrize();
			
		}
	}
	#endregion
	
	#region Private
	private SpriteRenderer sr;
	private enum FlowerState {PreBloom, Budding, Blooming, Fruited, Harvested};
	private FlowerState state = FlowerState.PreBloom;
	private float transitionTime = 0;
	private float growthCounter = 0;
	private float nextFlowerDelay;
	private PrizeManager pm;
	
	private void PrepareNextBud()
	{
		transitionTime = 0;
		growthCounter = 0;
		nextFlowerDelay = stemming.newFlowerDelay;
		transform.localScale = new Vector3(0, 0, 0);
		sr.color = Color.white;
		state = FlowerState.PreBloom;
	}
	#endregion
}
