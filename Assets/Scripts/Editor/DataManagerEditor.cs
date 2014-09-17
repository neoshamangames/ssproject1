using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;

[CustomEditor(typeof(DataManager))]
public class DataManagerEditor : Editor {
	
	#region Unity
	void Awake()
	{
		filePath = Application.persistentDataPath + "/saplings.data";
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		
		
		if(GUILayout.Button("Delete Save File"))
		{
			File.Delete(filePath);
		}
	}
	#endregion
	
	#region Private
	string filePath;
	#endregion
}
