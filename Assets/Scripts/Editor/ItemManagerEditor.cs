using UnityEngine;
using System.Collections;
using UnityEditor;

#region Unity
[CustomEditor(typeof(ItemManager))]
public class ItemManagerEditor : Editor {

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		ItemManager im = (ItemManager)target;
		im.plant = (Plant)EditorGUILayout.ObjectField("Plant", im.plant, typeof(Plant), true);
		im.chanceOfPowerup = EditorGUILayout.FloatField("Chance of powerup", im.chanceOfPowerup);
		EditorGUILayout.PropertyField(serializedObject.FindProperty("prizes"), true);
		serializedObject.ApplyModifiedProperties();
	}
}
#endregion

#region Private
#endregion
