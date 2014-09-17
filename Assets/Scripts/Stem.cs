using UnityEngine;
using System.Collections;
using Vectrosity;

public class Stem : Object {

	public enum State {Growing, Grown, Dead};
	
	#region Attributes
	public VectorLine line;
	public float growth = 0;
	public bool rightSide;
	public Flower flower;
	public Plant.StemmingAttributes stemming;
	public State state = State.Growing;
	public int lineIndex;
	public int height;
	#endregion
	
	#region Constructors
	public Stem(VectorLine line, int lineIndex, int height, Plant.StemmingAttributes stemming, float lineWidth, Color color, bool rightSide, Flower flower, float growth, float plantWidth)
	{
		this.line = line;
		this.lineIndex = lineIndex;
		this.height = height;
		
		this.segments = stemming.segmentsPer;
		this.stemming = stemming;
		maxWidth = this.lineWidth = lineWidth;
		
		if (growth < segments - 1)
		{
			int low = Mathf.FloorToInt(growth);
			lowPoint = line.points3[low];
			highPoint = line.points3[low + 1];
			line.drawEnd =  low + 1;
		}
		else
		{
			int low = segments - 2;
			lowPoint = line.points3[low];
			highPoint = line.points3[low + 1];
			line.drawEnd = segments - 1;
			lengthening = false;
		}
		
		maxWidth = Mathf.Clamp(growth * stemming.widthGrowth, 1, plantWidth);
		if (maxWidth > stemming.maxWidth)
		{
			maxWidth = stemming.maxWidth;
			state = State.Grown;
			UpdateWidth();
			line.Draw3D();
		}
		
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
		this.growth = growth;
		SetFlowerPositonToEnd();
	}
	#endregion
	
	#region Actions
	public bool CatchupGrowth(float newGrowth, float plantWidth)
	{
		int low;
		float currentGrowth = growth + newGrowth;
		if (currentGrowth > segments - 1)
		{
			low = segments - 2;
		}
		else
		{
			low = Mathf.FloorToInt(currentGrowth);
		}
		
		lowPoint = line.points3[low];
		highPoint = line.points3[low + 1];
		
		return Grow(newGrowth, plantWidth);
	}
	
	
	public bool Grow(float newGrowth, float plantWidth) //returns true when fully grown
	{
		
		growth += newGrowth;
		if (lengthening)
		{
//		if (growth < segments - 1)
//		{		
			int intPart = Mathf.FloorToInt(growth);
			float decPart = growth % 1;
			
			if (growth >= segments - 1)
			{
				lengthening = false;
				intPart = segments - 2;
				int low = intPart;
				lowPoint = line.points3[low];
				highPoint = line.points3[low + 1];
				decPart = 1;
			}
			
			if (intPart >= endSegment)
			{
				line.points3[intPart] = Vector3.Lerp(lowPoint, highPoint, DROP_BACK_PERCENT);
				lowPoint = highPoint;
				highPoint = line.points3[intPart + 1];
				endSegment = intPart + 1;
				line.drawEnd = endSegment;
				
				direction = line.points3[intPart + 1] - line.points3[intPart];
				nextDirection = line.points3[intPart + 2] - line.points3[intPart + 1];
			}
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);//TODO
			flower.transform.position = line.points3[intPart + 1];
			//Debug.Log("stem.line.drawEnd: " + stem.line.drawEnd);
			float angle = Vector3.Angle(direction, Vector3.up);
			float sign = Mathf.Sign(Vector3.Dot(direction, Vector3.right));
			Quaternion startRotation = Quaternion.Euler(0, 0, -angle * sign);
			float nextAngle = Vector3.Angle(nextDirection, Vector3.up);
			float nextSign = Mathf.Sign(Vector3.Dot(nextDirection, Vector3.right));
			Quaternion finishRotation = Quaternion.Euler(0, 0, -nextAngle * nextSign);
//			angle = sign * Mathf.Lerp(angle, nextAngle, decPart);
//			prevSign = sign;
			//flower.transform.localEulerAngles = new Vector3(0, 0, -angle);
			flower.transform.rotation = Quaternion.Lerp(startRotation, finishRotation, decPart);
		}
		maxWidth = Mathf.Clamp(growth * stemming.widthGrowth, 1, plantWidth);
		
		if (maxWidth > stemming.maxWidth)
		{
			maxWidth = stemming.maxWidth;
			state = State.Grown;
			UpdateWidth();
			return true;
		}
		UpdateWidth();
		return false;
	}
	
	public void SetFlowerPositonToEnd()
	{
		flower.transform.position = line.points3[segments - 1];
		direction = line.points3[segments] - line.points3[segments - 1];
		float angle = Vector3.Angle(direction, Vector3.up);
		float sign = Mathf.Sign(Vector3.Dot(direction, Vector3.right));
		flower.transform.rotation = Quaternion.Euler(0, 0, -angle * sign);
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
	private bool lengthening = true;
	
	private void UpdateWidth()
	{
	
		widths = new float[line.points3.Length - 1];
		
		int max = line.drawEnd;
		for(int i=0; i <= (float)max; i++)
		{
			widths[max - i] = GetWidth(i);
		}
		line.SetWidths(widths);
	}
	
	private float GetWidth(int i)
	{
		return Mathf.Lerp(lineWidth, maxWidth, (float)i/(float)segments);
	}
	#endregion
}
