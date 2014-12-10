/*Sean Maltz 2014*/

using UnityEngine;
using System.Collections;

public class SoundManager : SingletonMonoBehaviour<SoundManager> {

	public enum Sounds {MenuOpen, MenuBack, MenuClose, MenuForward, MenuTab, FlowerBlossom, FlowerBud, FlowerHarvest, PowerupBoost, PowerupSlow, PowerupRevive};

	#region Properties
	public AudioClip menuOpen;
	public AudioClip menuClose;
	public AudioClip menuBack;
	public AudioClip menuForward;
	public AudioClip menuTab;
	public AudioClip flowerBlossom;
	public AudioClip flowerBud;
	public AudioClip flowerHarvest;
	public AudioClip powerupBoost;
	public AudioClip powerupSlow;
	public AudioClip powerupRevive;
	#endregion

	#region Unity
	void Awake () {
		audioSource = gameObject.GetComponent<AudioSource>();
	}
	
	void Start () {
	}
	#endregion
	
	#region Actions	
	public void PlaySound(AudioClip sound)
	{
		if (audioSource.isPlaying)
			return;
		audioSource.clip = sound;
		audioSource.Play();
	}
	#endregion
	
	#region Private
	private AudioSource audioSource;
	#endregion
}
