using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : SingletonMonoBehaviour<AudioManager> {

	#region Attributes
	public Plant plant;
	public AudioSource sfx;
	public AudioClip musicDead;
	public AudioClip musicUnhealthy;
	public AudioClip musicHealthy;
	public AudioClip musicVeryHealthy;
	[Range(0, 1)]public float defaultMusicVolume = .7f;
	[Range(0, 1)]public float defaultSFXVolume = .7f;
	public float musicTransitionTime = 1f;
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
	public void SelectMusic(Plant.PlantState state)
	{
		int newIndex = 0;
		switch (state)
		{
		case Plant.PlantState.Dead:
			newIndex = 0;
			break;
		case Plant.PlantState.Drowning:
		case Plant.PlantState.Withering:
			newIndex = 1;
			break;
		case Plant.PlantState.HealthyDry:
		case Plant.PlantState.HealthyWet:
			newIndex = 2;
			break;
		case Plant.PlantState.VeryHealthy:
			newIndex = 3;
			break;
		}
		StartCoroutine(TransitionMusic(newIndex));
	}
	
	public void ToggleMusic()
	{
		musicMute = !musicMute;
		
		if (musicMute)
		{
			for(int i=0; i<4; i++)
			{
				musicSources[i].volume = 0;
			}
			
		}
		else
		{
			for(int i=0; i<4; i++)
			{
				if (i == musicIndex)
					musicSources[i].volume = defaultMusicVolume;
			}
		}
			
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
	void Start () {
		switch (plant.state)
		{
		case Plant.PlantState.Dead:
			musicIndex = 0;
			break;
		case Plant.PlantState.Drowning:
		case Plant.PlantState.Withering:
			musicIndex = 1;
			break;
		case Plant.PlantState.HealthyDry:
		case Plant.PlantState.HealthyWet:
			musicIndex = 2;
			break;
		case Plant.PlantState.VeryHealthy:
			musicIndex = 3;
			break;
		}
	
		musicMute = (PlayerPrefs.GetInt("musicMute") == 1);
		musicSources = new List<AudioSource>();
		for(int i=0; i<4; i++)
		{
			AudioSource audioSource = gameObject.AddComponent<AudioSource>();
			audioSource.playOnAwake = true;
			audioSource.loop = true;
			audioSource.volume = musicMute ? 0 : (musicIndex == i ? defaultMusicVolume : 0);
			musicSources.Add(audioSource);
		}
		
		musicSources[0].clip = musicDead;
		musicSources[1].clip = musicUnhealthy;
		musicSources[2].clip = musicHealthy;
		musicSources[3].clip = musicVeryHealthy;
		
		for(int i=0; i<4; i++)
			musicSources[i].Play();
		
		sfxMute = (PlayerPrefs.GetInt("sfxMute") == 1);
//		
		sfx.volume = sfxMute ? 0 : defaultSFXVolume;
	}
	#endregion
	
	#region Private
	private List<AudioSource> musicSources;
	private int musicIndex;
	private bool sfxMute, musicMute;
	private float transitionTimer;
	
	private IEnumerator TransitionMusic(int newIndex)
	{
		if (!musicMute)
		{
//			Debug.Log ("transiton music to newState: " + newIndex);
			transitionTimer = 0;
			while (transitionTimer < musicTransitionTime)
			{
				float t = transitionTimer/musicTransitionTime;
				musicSources[musicIndex].volume = Mathf.Lerp(defaultMusicVolume, 0, t);
				musicSources[newIndex].volume = Mathf.Lerp(0, defaultMusicVolume, t);
				yield return null;
				transitionTimer += Time.deltaTime;
			}
			musicSources[musicIndex].volume = 0;
			musicSources[newIndex].volume = defaultMusicVolume;
		}
		musicIndex = newIndex;
	}
	#endregion
}
