/*Sean Maltz 2014*/

using UnityEngine;
using System.Collections;

public class SinMover : Mover {

	
	#region Attributes
	public float speed = 1f;
	public float frequency = 1f;
	public float amplitude = 1f;
	[Range(0, 6.283f)]public float initialPhase;
	public float wrapWidth = 10f;
	public bool sway;
	public float swaySpeed;
	public float minAngle, maxAngle;
	#endregion
	
	#region Actions
	public override void Move()
	{
		float deltaTime = Time.deltaTime;
		float deltaT = deltaTime * speed;
		xPos += deltaT;
		t += deltaT;
		float yPos = Mathf.Sin(frequency * t + initialPhase) * amplitude;
		if (speed > 0)
		{
			if (xPos > distanceToRightEdge)
			{
				xPos = distanceToLeftEdge;
			}
		}
		else
		{
			if (xPos < -distanceToLeftEdge)
				xPos = distanceToRightEdge;
		}
		Vector3 offset = new Vector3(xPos, yPos);
		transform.position = initialPos + offset;
		
		if (sway)
		{
			if (swayDirection)
			{
				swayT -= swaySpeed * deltaTime * frequency;
			}
			else
			{
				swayT += swaySpeed * deltaTime * frequency;
			}
			transform.eulerAngles = Vector3.Slerp(minEulers, maxEulers, swayT);
			if (swayT > 1 || swayT < 0)
				swayDirection = !swayDirection;
		}
		
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		initialPos = transform.position;
		distanceToRightEdge = wrapWidth - initialPos.x;
		distanceToLeftEdge = -wrapWidth - initialPos.x;
		minEulers = new Vector3(0, 0, minAngle);
		maxEulers = new Vector3(0, 0, maxAngle);
	}
	#endregion
	
	#region Private
	private Vector3 initialPos;
	private float t, xPos;
	private float distanceToLeftEdge, distanceToRightEdge;
	
	private Vector3 minEulers, maxEulers;
	private float swayT;
	private bool swayDirection;
	#endregion
}
