using UnityEngine;
using System.Collections;
using Vectrosity;

public class Stem : Object {

	public enum State {Growing, Grown, Dead};
	
	#region Attributes
	public VectorLine line;
	public float growth = 0;
	public int height;
	public bool rightSide;
	public Flower flower;
	public Plant.StemmingAttributes stemming;
	public State state = State.Growing;
	#endregion
	
	#region Constructors
	public Stem(VectorLine line, int height, Plant.StemmingAttributes stemming, float lineWidth, Color color, bool rightSide, Flower flower)
	{
		this.line = line;
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
		this.height = height;
		this.segments = stemming.segmentsPer;
		this.stemming = stemming;
		maxWidth = this.lineWidth = lineWidth;
		this.rightSide = rightSide;
		float angle = Vector3.Angle(highPoint - lowPoint, Vector3.up);
		flower.transform.localEulerAngles = new Vector3(0, 0, angle);
		flower.transform.position = line.points3[0];
		flower.transform.localScale *= stemming.minFlowerSize;
		flower.transform.parent = line.vectorObject.transform;
		flower.stemming = stemming;
		this.flower = flower;
		SetColor(color);
		direction = new Vector3();
		//Plant plant = GameObject.Find("Plant").GetComponent<Plant>();
		//for(int i=0; i<line.points3.Length; i++)
		//	Instantiate(plant.pointMarker, line.points3[i], Quaternion.identity);
	}
	#endregion
	
	#region Actions	
	public void Grow(float newGrowth, float currentWidth)
	{	
		growth += newGrowth;
		if (growth < segments - 1)
		{
			int intPart = Mathf.FloorToInt(growth);
			float decPart = growth % 1;
			
			
			if (intPart >= endSegment)
			{
				line.points3[intPart] = Vector3.Lerp(lowPoint, highPoint, DROP_BACK_PERCENT);
				lowPoint = highPoint;
				highPoint = line.points3[intPart + 1];
				endSegment = intPart + 1;
				line.drawEnd = endSegment;
	//			Debug.Log("intPart: " + intPart);
	//			Debug.Log("endSegment: " + endSegment);
				
				direction = line.points3[intPart + 1] - line.points3[intPart];
				nextDirection = line.points3[intPart + 2] - line.points3[intPart + 1];
			}
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
			flower.transform.position = line.points3[intPart + 1];
			//Debug.Log("stem.line.drawEnd: " + stem.line.drawEnd);
			float angle = Vector3.Angle(direction, Vector3.up);
			float sign = Mathf.Sign(Vector3.Dot(direction, Vector3.right));
			Quaternion startRotation = Quaternion.Euler(0, 0, -angle * sign);
			float nextAngle = Vector3.Angle(nextDirection, Vector3.up);
			float nextSign = Mathf.Sign(Vector3.Dot(nextDirection, Vector3.right));
			Quaternion finishRotation = Quaternion.Euler(0, 0, -nextAngle * nextSign);
			angle = sign * Mathf.Lerp(angle, nextAngle, decPart);
			prevSign = sign;
			//flower.transform.localEulerAngles = new Vector3(0, 0, -angle);
			flower.transform.rotation = Quaternion.Lerp(startRotation, finishRotation, decPart);
		}
		maxWidth = Mathf.Clamp(maxWidth + stemming.widthGrowth, 0, currentWidth);
		if (maxWidth > stemming.maxWidth)
		{
			maxWidth = stemming.maxWidth;
			state = State.Grown;
		}
		UpdateWidth();
	}
	
	public void SetColor(Color color)
	{
		line.SetColor(color);
	}
	
	public void Rotate(float angle)
	{
		if (rightSide)
			angle *= -1;
		float angleRad = angle * Mathf.Deg2Rad;
		Vector3 pivot = line.points3[0];
		for(int i=0; i<=endSegment; i++)
		{
			Vector3 point = line.points3[i];
			float newX = pivot.x + (point.x-pivot.x)*Mathf.Cos(angleRad) - (point.y-pivot.y)*Mathf.Sin(angleRad);
			float newY = pivot.y + (point.x-pivot.x)*Mathf.Sin(angleRad) + (point.y-pivot.y)*Mathf.Cos(angleRad);
			line.points3[i] = new Vector3(newX, newY, pivot.z);
		}
		flower.transform.localPosition = line.points3[endSegment];
		flower.transform.localEulerAngles += new Vector3(0, 0, angle);
	}
	#endregion
	
	#region Private	
	private float DROP_BACK_PERCENT = .99f;
	private int endSegment = 1;
	private Vector3 lowPoint;
	private Vector3 highPoint;
	private float[] widths;
	private float lineWidth;
	private int segments;
	private float widthGrowth;
	private float maxWidth;
	private Vector3 direction, nextDirection;
	private float prevSign;
	
	private void UpdateWidth()
	{
		widths = new float[line.points3.Length - 1];
		
		int max = line.drawEnd;
		for(int i=0; i <= (float)max; i++)
		{
			widths[max - i] = GetWidth(i);
		}
		for(int i=(int)max + 1; i< widths.Length; i++)
		{
			widths[i] = lineWidth;
		}
		line.SetWidths(widths);
	}
	
	private float GetWidth(int i)
	{
		return Mathf.Lerp(lineWidth, maxWidth, (float)i/(float)segments);
	}
	#endregion
}
