using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TutorialManager))]
public class TutorialManagerEditor : Editor {

	#region Unity
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		if(GUILayout.Button("Reset 'Don't Show Again' states"))
		{
			TutorialManager tm = (TutorialManager)target;
			tm.ResetStates();
		}
	}
	#endregion
}
