using UnityEngine;
using System.Collections;

public class Flower : MonoBehaviour {

	public enum FlowerState {PreBloom, Budding, Blooming, Fruited, Harvested};
	
	#region Attributes
	public Plant.StemmingAttributes stemming;
	public FlowerState state = FlowerState.PreBloom;
	#endregion

	#region Unity
	void Awake()
	{
		sr = GetComponent<SpriteRenderer>();
		im = ItemManager.Instance;
	}
	
	void Start()
	{
		nextFlowerDelay = stemming.flowerDelay;
	}
	#endregion
	
	#region Actions
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
			transitionTime = 0;
			state = FlowerState.Harvested;
			im.AwardPrize();
			
		}
	}
	#endregion
	
	#region Private
	private SpriteRenderer sr;
	private float transitionTime = 0;
	private float growthCounter = 0;
	private float nextFlowerDelay;
	private ItemManager im;
	
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
