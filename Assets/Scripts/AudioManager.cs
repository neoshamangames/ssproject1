using UnityEngine;
using System.Collections;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	#region Attributes
	[Range(0, 1)]public float defaultMusicVolume = .7f;
	#endregion
	
	#region Properties
	public bool Muted
	{
		get{ return muted; }
	}
	#endregion
	
	#region Actions
	public void ToggleMute()
	{
		muted = !muted;
		
		if (muted)
			music.volume = 0;
		else
			music.volume = defaultMusicVolume;
			
		PlayerPrefs.SetInt("mute", muted ? 1: 0);
	}
	#endregion
	
	#region Unity	
	void Awake () {
		music = GetComponent<AudioSource>();
		muted = (PlayerPrefs.GetInt("mute") == 1);
		music.volume = muted ? 0 : defaultMusicVolume;
	}
	#endregion
	
	#region Private
	private AudioSource music;
	private bool muted;
	#endregion
}
