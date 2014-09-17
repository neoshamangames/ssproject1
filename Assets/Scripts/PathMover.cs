using UnityEngine;
using System.Collections;
using Vectrosity;

public class PathMover : Mover {

	#region Parameters
	public int numberOfSegments;
	public bool drawPath;
	public float delay = 5f;
	public float speed = 1f;
	public float startingYRange;
	public float pathWidth;
	public float maxPathHeight;
	public float minControlPoint1X;
	public float maxControlPoint1X;
	public float minControlPoint1Y;
	public float maxControlPoint1Y;
	public float minControlPoint2X;
	public float maxControlPoint2X;
	public float minControlPoint2Y;
	public float maxControlPoint2Y;
	#endregion
	
	#region Actions
	public override void Move()
	{
		if (progressing)
		{
			progress += Time.deltaTime * speed;
			int intPart = Mathf.FloorToInt(progress);
			decPart = progress % 1;
			if (intPart >= endSegment)
			{
				if (intPart >= lastSegment)
				{
					StartCoroutine("WaitAndNewPath");
					return;
				}
				endSegment++;
				lowPoint = points3[intPart];
				highPoint = points3[intPart + 1];
				direction = points3[intPart + 1] - points3[intPart];
				nextDirection = points3[intPart + 2] - points3[intPart + 1];
			}
			transform.position = Vector3.Lerp(lowPoint, highPoint, decPart);
			RotateToPath();
		}
	}
	#endregion
	
	#region Unity
	void Awake()
	{
		initialPos = transform.position;
		points3 = new Vector3[numberOfSegments + 1];
		path = new VectorLine("planePath", points3, Color.grey, null, 1, LineType.Continuous);
		controlLine1 = new VectorLine("planePathControlLine", new Vector3[2], Color.red, null, 1, LineType.Continuous);
		controlLine2 = new VectorLine("planePathControlLine", new Vector3[2], Color.red, null, 1, LineType.Continuous);
		path.vectorObject.transform.parent = transform.parent;
		
		lastSegment = numberOfSegments - 1;
		
		NewPath();
	}
	#endregion
	
	#region Private
	private VectorLine path, controlLine1, controlLine2;
	private Vector3[] points3;
	private Vector3 initialPos;
	private bool leftToRight;
	private float progress;
	private int endSegment = 1;
	private int lastSegment;
	private float decPart;
	private Vector3 lowPoint, highPoint;
	private Vector3 direction, nextDirection;
	private bool progressing;
	
	private void NewPath()
	{
		float startingX = leftToRight ? -pathWidth : pathWidth;
		float startingY = initialPos.y + Random.Range(-startingYRange, startingYRange);
		float finishX = -startingX;
		float finishY = startingY + Random.Range(-maxPathHeight, maxPathHeight);
		Vector3 controlPoint1 = new Vector3(startingX + (leftToRight ? 1 : - 1) * Random.Range(minControlPoint1X, maxControlPoint1X),
		                                    startingY + Random.Range(minControlPoint1Y, maxControlPoint1Y), initialPos.z);
		Vector3 controlPoint2 = new Vector3(finishX + (leftToRight ? 1 : - 1) * Random.Range(minControlPoint2X, maxControlPoint2X),
											finishY + Random.Range(minControlPoint2Y, maxControlPoint2Y), initialPos.z);
		Vector3[] curvePoints = {new Vector3(startingX, startingY, initialPos.z), controlPoint1, new Vector3(finishX, finishY, initialPos.z), controlPoint2};
		Debug.Log ("curvePoints[0]: " + curvePoints[0]);
		Debug.Log ("curvePoints[2]: " + curvePoints[2]);
		path.MakeCurve(curvePoints, numberOfSegments);
		#if UNITY_EDITOR
		if (drawPath)
		{
			path.Draw3D();
			controlLine1.points3 = new Vector3[] {new Vector3(startingX, startingY), new Vector3(curvePoints[1].x, curvePoints[1].y, initialPos.z)};
			controlLine2.points3 = new Vector3[] {new Vector3(finishX, finishY), new Vector3(curvePoints[3].x, curvePoints[3].y, initialPos.z)};
			controlLine1.Draw3D();
			controlLine2.Draw3D();
		}
		#endif
		progress = 0;
		endSegment = 1;
		lowPoint = points3[0];
		highPoint = points3[1];
		direction = highPoint - lowPoint;
		nextDirection = points3[2] - highPoint;
		progressing = true;
	}
	
	private IEnumerator WaitAndNewPath()
	{
		progressing = false;
		yield return new WaitForSeconds(delay);
		leftToRight = !leftToRight;
		Vector3 scale = transform.localScale;
		scale.x = -scale.x;
		transform.localScale = scale;
		NewPath();
	}
	
	private void RotateToPath()
	{
		float angle = Vector3.Angle(direction, Vector3.up);
		float sign = Mathf.Sign(Vector3.Dot(direction, Vector3.right));
		Quaternion startRotation = Quaternion.Euler(0, 0, -angle * sign);
		float nextAngle = Vector3.Angle(nextDirection, Vector3.up);
		float nextSign = Mathf.Sign(Vector3.Dot(nextDirection, Vector3.right));
		Quaternion finishRotation = Quaternion.Euler(0, 0, -nextAngle * nextSign);
		//			angle = sign * Mathf.Lerp(angle, nextAngle, decPart);
		//			prevSign = sign;
		//flower.transform.localEulerAngles = new Vector3(0, 0, -angle);
		transform.rotation = Quaternion.Lerp(startRotation, finishRotation, decPart);
	}
	#endregion
}
