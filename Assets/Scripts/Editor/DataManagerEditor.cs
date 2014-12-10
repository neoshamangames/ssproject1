/*Sean Maltz 2014*/

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
		dm = (DataManager)target;
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		
		
		if(GUILayout.Button("Delete Save File"))
		{
			Debug.Log ("filePath: " + filePath);
			dm.DeleteFile(filePath);
		}
	}
	#endregion
	
	#region Private
	string filePath;
	DataManager dm;
	#endregion
}
