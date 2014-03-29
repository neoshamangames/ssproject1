using UnityEngine;
using System.Collections;
using Vectrosity;

public class Stem : Object {

	#region Attributes
	public VectorLine line;
	public float growth = 0;
	public int height;
	public bool leftSide;
	public Flower flower;
	public Transform flowerParent;
	public Plant.StemmingAttributes stemming;
	#endregion
	
	#region Constructors
	public Stem(VectorLine line, int height, int segments, Plant.StemmingAttributes stemming, float lineWidth, Color color, bool leftSide, Flower flower)
	{
		this.line = line;
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
		this.height = height;
		this.segments = segments;
		this.stemming = stemming;
		maxWidth = this.lineWidth = lineWidth;
		this.leftSide = leftSide;
		float angle = Vector3.Angle(highPoint - lowPoint, Vector3.up);
		flower.transform.localEulerAngles = new Vector3(0, 0, angle);
		flower.transform.position = line.points3[0];
		flower.transform.localScale *= stemming.minFlowerSize;
		flower.transform.parent = line.vectorObject.transform;
		flower.stemming = stemming;
		this.flower = flower;
		SetColor(color);
	}
	#endregion
	
	#region Actions	
	public void Grow(float newGrowth, float currentWidth)
	{	
		if (growth < segments-1)
		{
			growth += newGrowth;
			
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
			}
			//Debug.Log("stem.line.drawEnd: " + stem.line.drawEnd);
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
			flower.transform.position = line.points3[intPart + 1];
			Vector3 direction = line.points3[intPart + 1] - line.points3[intPart];
			float angle = Vector3.Angle(direction, Vector3.up);
			float sign = Mathf.Sign(Vector3.Dot(direction, Vector3.right));
			angle *= sign;
			flower.transform.localEulerAngles = new Vector3(0, 0, -angle);
		}
		maxWidth = Mathf.Clamp(Mathf.Clamp(maxWidth + stemming.widthGrowth, 0, currentWidth), 0, stemming.maxWidth);
		UpdateWidth();
		flower.Grow(newGrowth);
	}
	
	
	
	public void SetColor(Color color)
	{
		line.SetColor(color);
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
