/*Sean Maltz 2014*/

using UnityEngine;
using System.Collections;

public abstract class Mover : MonoBehaviour {
	
	#region Private
	public abstract void Move();
	public virtual void Touched()
	{
	
	}
	#endregion
}
