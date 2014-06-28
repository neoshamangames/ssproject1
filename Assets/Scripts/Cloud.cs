using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Vectrosity;

public class Cloud : MonoBehaviour {

	#region Attributes
	public Material raindropMaterial;
	public Transform raindropRoot;
	public Plant plant;
	public bool useDebugRefillTime;
	[Range(1, 200000)]public float debugRefillTime = 10f;
	[Range(1, 200000)]public float releaseRefillTime = 10f;//43200 (12 hours)
	[Range(1, 60)]public float depletionTime;
	[Range(0, 2)]public float startSize = 1f;
	[Range(0, 5)]public float minSize = .25f;
	[Range(0, 5)]public float maxSize = 2f;
	[Range(0, 1)]public float darkestGrey = .25f;
	[Range(0, 100)]public float dropsPerFrameMax = .05f;
	[Range(0, 100)]public float dropsPerFrameMin = 1f;
	public float raindropTopEdge = 3.7f;
	public float raindropBottomEdge = -2.4f;
	public float raindropAreaMaxWidth = 3.5f;
	public float raindropAreaMinWidth = .5f;
	[Range(0, 10)]public float bottomRaindropThickness = .1f;
	[Range(0, 10)]public float topRaindropThickness = .5f;
	[Range(0, 500)]public float minRaindropLength = .1f;
	[Range(0, 500)]public float maxRaindropLength = 1f;
	[Range(0, 1)]public float raindropLengthVariety = .01f;
	[Range(0, 500)]public float raindropSpeed = 10f;
	public Vector3 cloudOffset;
	#endregion
	
	#region Properties
	public float Size
	{
		get { return transform.localScale.x; }
		set { transform.localScale = new Vector3(value, value, 1); }
	}
	#endregion

	#region Unity
	void Awake()
	{
		cloudCam = CloudCamera.Instance.camera;
		mainCam = Camera.main;
		transform.localScale = new Vector3(startSize, startSize, 1);
		Debug.Log ("cloud Awake");
		spriteRenderer = GetComponent<SpriteRenderer>();
		raindrops = new List<VectorLine>();
		raindropTimers = new List<float>();
		raindropLengths = new List<float>();
		float distanceFromCam = transform.position.z - cloudCam.transform.position.z;
		screenHeight = cloudCam.ViewportToWorldPoint(new Vector3(0, 1, distanceFromCam)).y - cloudCam.ViewportToWorldPoint(new Vector3(0, 0, distanceFromCam)).y;
		growthRate = (maxSize - minSize) / (useDebugRefillTime ? debugRefillTime : releaseRefillTime);
		depletionRate = (maxSize - minSize) / depletionTime;
	}

	void Update()
	{
		max = plant.TopPosisiton.y;
		cloudScreenPos = mainCam.WorldToViewportPoint(plant.TopPosisiton + cloudOffset);
		transform.position = cloudCam.ViewportToWorldPoint(cloudScreenPos);
		prevYPos = cloudScreenPos.y;
		currentScale = transform.localScale;
		cloudPercentage = currentScale.x/maxSize;
		float cloudGrey = Mathf.Lerp(1, darkestGrey, cloudPercentage);
		spriteRenderer.color = new Color(cloudGrey, cloudGrey, cloudGrey);
		
		ProcessRain();
		if (!raining)
		{
			if (currentScale.x < maxSize)
			{
				float grow = Time.deltaTime * growthRate;
				ScaleCloud(grow);
			}
		}
		raining = false;
	}
	
	#endregion
	
	#region Actions
	public void Rain(float deltaTime)
	{
		if (currentScale.x > minSize)
		{
			raining = true;
			float rain = -deltaTime * depletionRate;
			ScaleCloud(rain);
			plant.Water(deltaTime / depletionTime);
		}
	}
	#endregion
	
	#region Private
	private Camera cloudCam;
	private Camera mainCam;
	private float screenHeight;
	private Vector3 cloudScreenPos;
	private const int MAX_RAIN_DROPS = 500;
	private Vector3 currentScale;
	private bool raining;
	private SpriteRenderer spriteRenderer;
	private float cloudPercentage;
	private List<VectorLine> raindrops;
	private List<float> raindropTimers;
	private List<float> raindropLengths;
	Touch firstTouch;
	Touch secondTouch;
	private float prevYPos;
	private float max;
	private float growthRate, depletionRate;
	
	private void ScaleCloud(float scale)
	{
		transform.localScale = new Vector3(currentScale.x + scale, currentScale.y + scale, 1);
	}
	
	private void ProcessRain()
	{
		int raindropsCount = raindrops.Count;
		if (raining)
		{
			float rand = Random.value;
			float numberOfDrops = Mathf.Lerp(dropsPerFrameMin, dropsPerFrameMax, cloudPercentage);
			while (numberOfDrops > rand)
			{
				SpawnRainDrop();
				numberOfDrops--;
			}
		}
		float deltaTime = Time.deltaTime;
		for(int i=0; i < raindropsCount; i++)
		{
			float raindropTimer = raindropTimers[i] += deltaTime;
			float raindropTime = raindropLengths[i] / raindropSpeed;
			if(raindropTimer < raindropTime / 2)
			{
				float bottomDistance = deltaTime/raindropTime * raindropLengths[i];
				raindrops[i].points2[1].y -= bottomDistance/2;
				raindrops[i].points2[2].y -= bottomDistance;
				raindrops[i].Draw();
			}
			else if (raindropTimer < raindropTime)
			{
				float topDistance = deltaTime/raindropTime * raindropLengths[i];
				raindrops[i].points2[1].y -= topDistance/2;
				raindrops[i].points2[0].y -= topDistance;
				raindrops[i].Draw();
			}
			else
			{
				VectorLine line = raindrops[i];
				VectorLine.Destroy(ref line);
				raindrops.RemoveAt(i);
				raindropTimers.RemoveAt(i);
				raindropLengths.RemoveAt(i);
				raindropsCount--;
			}
		}
	}
	
	private void SpawnRainDrop()
	{
		float width = Mathf.Lerp(raindropAreaMinWidth, raindropAreaMaxWidth, cloudPercentage);
		float xCoor = Random.Range(transform.position.x - width, transform.position.x + width);
//		float yCoor = Random.Range(raindropBottomEdge, raindropTopEdge);
		float yCoor = Random.Range(transform.position.y - screenHeight, transform.position.y);
		float length = Mathf.Lerp(minRaindropLength, maxRaindropLength, cloudPercentage)
			* (1 + Random.Range(-raindropLengthVariety, raindropLengthVariety));
		Vector2 startPoint = new Vector2(xCoor, yCoor);
		Vector2 endPoint = new Vector2(xCoor, yCoor);
		Vector2[] points = {startPoint, (startPoint + endPoint)/2, endPoint};
		VectorLine drop = new VectorLine("Rainddrop", points, raindropMaterial, bottomRaindropThickness, LineType.Continuous);
		drop.smoothWidth = true;
		drop.SetWidths(new float[]{topRaindropThickness, bottomRaindropThickness});
		drop.vectorObject.transform.parent = raindropRoot;
		drop.vectorObject.layer = LayerMask.NameToLayer("Cloud");
//		Vector3 pos = drop.vectorObject.transform.localPosition;
//		pos.y = 0;
//		drop.vectorObject.transform.localPosition = pos;
		drop.Draw();
		raindrops.Add(drop);
		raindropTimers.Add(0);
		raindropLengths.Add(length);
	}
	#endregion
}
