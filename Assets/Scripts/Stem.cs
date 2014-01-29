using UnityEngine;
using System.Collections;
using Vectrosity;

public class Stem : Object {

	#region Attributes
	public VectorLine line;
	public float growth = 0;
	public int height;
	public bool leftSide;
	#endregion
	
	#region Constructors
	public Stem(VectorLine line_, int height, int segments, float widthGrowth, float lineWidth, Color color, bool leftSide)
	{
		line = line_;
		lowPoint = line.points3[0];
		highPoint = line.points3[1];
		this.height = height;
		this.segments = segments;
		this.widthGrowth = widthGrowth;
		maxWidth = this.lineWidth = lineWidth;
		this.leftSide = leftSide;
		SetColor(color);
	}
	#endregion
	
	#region Actions
	public void Grow(float newGrowth, float currentWidth)
	{
		if (growth < segments-1)
		{
			growth += newGrowth;
	//		Debug.Log("growth: " + growth);
			
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
			//Debug.Log("intPart: " + intPart);
			line.points3[intPart + 1] = Vector3.Lerp(lowPoint, highPoint, decPart);
		}
		maxWidth = Mathf.Clamp(maxWidth + widthGrowth, 0, currentWidth);
//		Debug.Log("maxWidth: " + maxWidth);
		UpdateWidth();
	}
	
	public void SetColor(Color color)
	{
		line.SetColor(color);
	}
	#endregion
	
	#region Private
	private float DROP_BACK_PERCENT = .95f;
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
