﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : SingletonMonoBehaviour<TutorialManager> {

	[System.Serializable]
	public class TutorialEvent
	{
		public string message;
		public int ID;
	}
	
	#region Attributes
	public TutorialEvent[] tutorialEvents;
	#endregion
	
	#region Unity
	void Awake()
	{
		gm = GUIManager.Instance;
		queue = new Queue<TutorialEvent>();
		
		foreach(TutorialEvent te in tutorialEvents)
		{
			if (PlayerPrefs.GetInt(string.Format("tut{0}", te.ID)) == 1)
				PlayerPrefs.SetInt(string.Format("tut{0}", te.ID), 0);
		}
	}
	#endregion
	
	#region Actions
	public void TriggerTutorial(int ID)
	{
//		Debug.Log ("triggering " + ID + ". value: " + PlayerPrefs.GetInt(string.Format("tut{0}", ID)));
		if (PlayerPrefs.GetInt(string.Format("tut{0}", ID)) != 0)
			return;
			
		TutorialEvent currentTE = new TutorialEvent();
		bool idFound = false;
		
		foreach(TutorialEvent te in tutorialEvents)
		{
			if (te.ID == ID)
			{
				currentTE = te;
				idFound = true;
				break;
			}
		}
		
		if (!idFound)
		{
			Debug.LogWarning("TutorialEvent with ID " + ID + " not found!");
			return;
		}
		
		if (gm.TutorialOpen)
		{
			Debug.Log ("enqueueing " + ID);
			queue.Enqueue(currentTE);
		}
		else
		{
			Debug.Log ("poping up " + ID);
			currentID = ID;
			gm.TutorialPopup(currentTE.message);
		}
	}
	
	public void DismissTutorial(bool dontShowAgain)
	{
		StartCoroutine(Dismiss(dontShowAgain));
	}
	#endregion

	#region Private
	private Queue<TutorialEvent> queue;
	private GUIManager gm;
	private int currentID;
	
	private IEnumerator Dismiss(bool dontShowAgain)
	{
		yield return null;
		if (dontShowAgain)
		{
			PlayerPrefs.SetInt(string.Format("tut{0}", currentID), 2);
		}
		else
		{
			PlayerPrefs.SetInt(string.Format("tut{0}", currentID), 1);
		}
		Debug.Log ("queue.Count: " + queue.Count);
		while (queue.Count > 0)
		{
			TutorialEvent nextTE = queue.Dequeue();
			currentID = nextTE.ID;
			if ( PlayerPrefs.GetInt(string.Format("tut{0}", currentID)) != 0)
				continue;
			gm.TutorialPopup(nextTE.message);
		}
	}
	#endregion
}