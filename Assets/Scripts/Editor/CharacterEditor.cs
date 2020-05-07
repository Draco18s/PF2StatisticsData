using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character), true)]
public class CharacterEditor : Editor
{
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.LabelField("Approximated Stats");
		//EditorGUILayout.LabelField("STR");
		Character c = (Character)serializedObject.targetObject;

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("STR");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.STR, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("DEX");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.DEX, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("CON");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.CON, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("INT");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.INT, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("WIS");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.WIS, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("CON");
		EditorGUILayout.LabelField((10 + 2 * StatArray.GetValue(c.stats.CON, c.level)).ToString());
		EditorGUILayout.EndHorizontal();
	}
}
