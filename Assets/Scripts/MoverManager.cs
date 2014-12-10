/*Sean Maltz 2014*/

using UnityEngine;
using System.Collections;

public class MoverManager : MonoBehaviour {

	#region Parameters
	public float buffer = .2f;
	public float unitsFromCam = 5f;
	#endregion

	#region Unity
	void Awake ()
	{
		mainCam = Camera.main;
		movers = GetComponentsInChildren<Mover>(true);
		screenTop = 1 + buffer;
		screenBottom = -buffer;
	}
	
	void Update ()
	{
		float bottom = mainCam.ViewportToWorldPoint(new Vector3(.5f, screenBottom, unitsFromCam)).y;
		float top = mainCam.ViewportToWorldPoint(new Vector3(.5f, screenTop, unitsFromCam)).y;
		foreach(Mover mover in movers)
		{
			float yPos = mover.transform.position.y;
			
			if (yPos > bottom && yPos < top)
			{
				mover.Move();
			}
		}
	}
	#endregion
	
	private Camera mainCam;
	private Mover[] movers;
	private float screenTop, screenBottom;
}
