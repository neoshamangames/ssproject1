using UnityEngine;
using System.Collections;

public class CloudCamera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		cloudCam = GetComponent<Camera>();
		cloudCam.clearFlags = CameraClearFlags.Depth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	Camera cloudCam;
}
