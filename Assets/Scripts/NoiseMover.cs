/*Sean Maltz 2014*/

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
	public bool rotate, pressToSpin;
	public float rotSpeed = 1f;
	public float spinSpeed = 1f;
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
		transform.position = new Vector3(xNoise, yNoise);
		transform.position += initialPos;
		t += deltaTime * speed;
		
		if (spinning)
		{
			if (totalSpin < 360)
			{
				float spin = spinSpeed * deltaTime;
				transform.Rotate(new Vector3(0,0,1), spin * spinDir);
				totalSpin += spin;
			}
			else
			{
				transform.Rotate(new Vector3(0,0,1), (360 - totalSpin) * spinDir);
				spinning = false;
			}
		}
		else if (rotate)
		{
			rotateT += deltaTime * rotSpeed;
			float angle = noise.coherentNoise(rotOffset + rotateT, 0, 0, rotNoiseOctaves, rotNoiseMultiplier, rotNoiseAmplitude, rotNoiseLacunarity, rotNoisePersistence);
			transform.Rotate(new Vector3(0,0,1), angle);
		}
	}
	
	public override void Touched()
	{
		if (!spinning)
		{
			spinning = true;
			totalSpin = 0;
			spinDir = (Random.Range(0, 2) == 0) ? 1 : -1;
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
		initialPos = transform.position;
	}
	#endregion
	
	#region Private
	private SimplexNoiseGenerator noise;
	private Vector3 initialPos;
	private float xOffset;
	private float yOffset;
	private float rotOffset;
	private float t, rotateT;
	private bool spinning;
	private float spinDir;
	private float totalSpin;
	#endregion
}
