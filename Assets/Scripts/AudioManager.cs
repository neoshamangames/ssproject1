using UnityEngine;
using System.Collections;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	#region Attributes
	public AudioSource sfx;
	[Range(0, 1)]public float defaultMusicVolume = .7f;
	[Range(0, 1)]public float defaultSFXVolume = .7f;
	#endregion
	
	#region Properties
	public bool SFXMute
	{
		get{ return sfxMute; }
	}
	public bool MusicMute
	{
		get{ return musicMute; }
	}
	#endregion
	
	#region Actions
	public void ToggleMusic()
	{
		musicMute = !musicMute;
		
		if (musicMute)
			music.volume = 0;
		else
			music.volume = defaultMusicVolume;
			
		PlayerPrefs.SetInt("musicMute", musicMute ? 1: 0);
	}
	
	public void ToggleSFX()
	{
		sfxMute = !sfxMute;
		
		if (sfxMute)
			sfx.volume = 0;
		else
			sfx.volume = defaultSFXVolume;
		
		PlayerPrefs.SetInt("sfxMute", sfxMute ? 1: 0);
	}
	#endregion
	
	#region Unity	
	void Awake () {
		music = GetComponent<AudioSource>();
		musicMute = (PlayerPrefs.GetInt("musicMute") == 1);
		sfxMute = (PlayerPrefs.GetInt("sfxMute") == 1);
		music.volume = musicMute ? 0 : defaultMusicVolume;
		sfx.volume = sfxMute ? 0 : defaultSFXVolume;
	}
	#endregion
	
	#region Private
	private AudioSource music;
	private bool sfxMute, musicMute;
	#endregion
}
