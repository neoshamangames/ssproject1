using UnityEngine;
using System.Collections;

public class NoiseMover : Mover {

	
	#region Attributes
	public float speed = 1f;
	public int noiseOctaves = 8;
	public int noiseMultiplier = 25;
	public float noiseAmplitude = 0.5f;
	public float noiseLacunarity = 2f;
	public float noisePersistence = 0.9f;
	public bool rotate;
	public float rotSpeed = 1f;
	public int rotNoiseOctaves = 8;
	public int rotNoiseMultiplier = 25;
	public float rotNoiseAmplitude = 0.5f;
	public float rotNoiseLacunarity = 2f;
	public float rotNoisePersistence = 0.9f;
	#endregion
	
	#region Actions
	public override void Move ()
	{
		float deltaTime = Time.deltaTime;
		float xNoise = noise.coherentNoise(xOffset + t, 0, 0, noiseOctaves, noiseMultiplier, noiseAmplitude, noiseLacunarity, noisePersistence);
		float yNoise = noise.coherentNoise(yOffset + t, 0, 0, noiseOctaves, noiseMultiplier, noiseAmplitude, noiseLacunarity, noisePersistence);
		//		Debug.Log ("dragonfly xNoise: " + xNoise);
		transform.position = new Vector3(xNoise, yNoise);
		transform.position += initialPos;
		t += deltaTime * speed;
		
		if (rotate)
		{
			rotateT += deltaTime * rotSpeed;
			float angle = noise.coherentNoise(rotOffset + rotateT, 0, 0, rotNoiseOctaves, rotNoiseMultiplier, rotNoiseAmplitude, rotNoiseLacunarity, rotNoisePersistence);
			transform.Rotate(new Vector3(0,0,1), angle);
		}
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		noise = new SimplexNoiseGenerator();
		xOffset = Random.Range(0, 20000);
		yOffset = Random.Range(0, 20000);
		rotOffset = Random.Range(0, 20000);
		zPos = transform.position.z;
		initialPos = transform.position;
	}
	#endregion
	
	#region Private
	SimplexNoiseGenerator noise;
	Vector3 initialPos;
	float xOffset;
	float yOffset;
	float rotOffset;
	float t, rotateT;
	float zPos;
	#endregion
}
