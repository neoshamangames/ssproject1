using UnityEngine;
using System.Collections;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	#region Attributes
	[Range(0, 1)]public float defaultMusicVolue = .7f;
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
			music.volume = defaultMusicVolue;
	}
	#endregion
	
	#region Unity	
	void Awake () {
		music = GetComponent<AudioSource>();
		music.volume = defaultMusicVolue;
	}
	#endregion
	
	#region Private
	private AudioSource music;
	private bool muted;
	#endregion
}
