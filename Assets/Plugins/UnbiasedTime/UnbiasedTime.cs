using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class UnbiasedTime : MonoBehaviour {
	
	private static UnbiasedTime instance;
	public static UnbiasedTime Instance {
		get {
			if (instance == null) {
				GameObject g = new GameObject ("UnbiasedTimeSingleton");
			    instance = g.AddComponent<UnbiasedTime> ();
			    DontDestroyOnLoad(g);
			}
			return instance;
		}
	}

	// Estimated difference in seconds between device time and real world time
	// timeOffset = deviceTime - worldTime;
	[HideInInspector]
	public long timeOffset = 0;
	
	void Awake() {
		onStart();
	}
	
	void OnApplicationPause (bool pause) {
		if (pause) {
			onEnd();
		}
		else {
			onStart();
		}
	}

	void OnApplicationQuit () {
		onEnd();
	}
	
	private void onStart () {
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) { 
			using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
			using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) 
			{
				var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (playerActivityContext != null && unbiasedTimeClass != null) {
					unbiasedTimeClass.CallStatic ("vtcOnSessionStart", playerActivityContext);
					timeOffset = unbiasedTimeClass.CallStatic <long> ("vtcTimestampOffset");
				}
			}
		}
		#endif
	}
	
	private void onEnd () {
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
			using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) 
			{
				var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (playerActivityContext != null && unbiasedTimeClass != null) {
					unbiasedTimeClass.CallStatic ("vtcOnSessionEnd", playerActivityContext);
				}
			}
		}
		#endif
	}

	// Returns estimated DateTime value taking into account possible device time changes
	public DateTime Now() {
		return DateTime.Now.AddSeconds ( -1.0f * timeOffset );
	}


	// timeOffset value is cached for performance reasons (calls to native plugins can be expensive). 
	// This method is used to update offset value in cases if you think device time was changed by user. 
	// 
	// However, time offset is updated automatically when app gets backgrounded or foregrounded. 
	// By the time of writing there is no way to have more than one active app on iOS, 
	// so there is no need to call this method explicitly. 
	//
	public void UpdateTimeOffset() {
		#if UNITY_ANDROID
		if (Application.platform == RuntimePlatform.Android) {
			
			using (var activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
			using (var unbiasedTimeClass = new AndroidJavaClass("com.vasilij.unbiasedtime.UnbiasedTime")) 
			{
				var playerActivityContext = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				if (playerActivityContext != null && unbiasedTimeClass != null) {
					timeOffset = unbiasedTimeClass.CallStatic <long> ("vtcTimestampOffset");
				}
			}
		}
		#endif
	}
}