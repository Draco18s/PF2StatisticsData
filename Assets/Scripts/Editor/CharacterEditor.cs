using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Character), true)]
public class CharacterEditor : Editor {
	private GUIStyle red;

	void OnEnable() {
		try {
			red = new GUIStyle(EditorStyles.boldLabel);
			red.normal.textColor = new Color(1, 0, 0, 1);
		}
		catch(NullReferenceException) { }
	}

	public override void OnInspectorGUI() {
		Character c = (Character)serializedObject.targetObject;
		int origLevel = c.level;
		base.OnInspectorGUI();
		if(c.level > 20) c.level = 20;
		SerializedProperty statProp = serializedObject.FindProperty("statGen");
		EditorGUILayout.BeginHorizontal();
		Rect rOriginal = EditorGUILayout.GetControlRect();
		Rect r = rOriginal;
		r.y += 50;
		r.xMin += Screen.width -200;

		if(origLevel/5 < c.level/5) {
			FillInBoosts(statProp, origLevel+5);
			if((origLevel+5) / 5 < c.level / 5) {
				FillInBoosts(statProp, origLevel + 10);
				if((origLevel + 10) / 5 < c.level / 5) {
					FillInBoosts(statProp, origLevel + 15);
					if((origLevel + 15) / 5 < c.level / 5) {
						FillInBoosts(statProp, origLevel + 20);
					}
				}
			}
		}

		GUIUtility.RotateAroundPivot(-90f, new Vector2(r.xMin, rOriginal.yMin+50));
		EditorGUI.LabelField(r, "Ancestry");
		r.y += 18;
		EditorGUI.LabelField(r, "Flaw");
		r.y += 18;
		EditorGUI.LabelField(r, "Background");
		r.y += 18;
		EditorGUI.LabelField(r, "Class");
		r.y += 18;
		EditorGUI.LabelField(r, "4 Free");
		r.y += 18;
		EditorGUI.LabelField(r, "Level 5");
		r.y += 18;
		EditorGUI.LabelField(r, "Level 10");
		r.y += 18;
		EditorGUI.LabelField(r, "Level 15");
		r.y += 18;
		EditorGUI.LabelField(r, "Level 20");
		GUIUtility.RotateAroundPivot(90f, new Vector2(r.xMin, rOriginal.yMin + 50));
		rOriginal.y += 55;
		rOriginal.x += 1;
		rOriginal = DrawStatEditorGui(rOriginal, "STR", statProp, c.level);
		rOriginal = DrawStatEditorGui(rOriginal, "DEX", statProp, c.level);
		rOriginal = DrawStatEditorGui(rOriginal, "CON", statProp, c.level);
		rOriginal = DrawStatEditorGui(rOriginal, "INT", statProp, c.level);
		rOriginal = DrawStatEditorGui(rOriginal, "WIS", statProp, c.level);
		rOriginal = DrawStatEditorGui(rOriginal, "CHA", statProp, c.level);
		bool showError = Validate(c.level, statProp, out string stringError);
		serializedObject.ApplyModifiedProperties();
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space(155);

		EditorUtility.SetDirty(serializedObject.targetObject);
		if(showError) {
			GUILayout.Label(stringError, red);
		}
		EditorGUILayout.LabelField("Approximated Stats");

		//Character c = (Character)serializedObject.targetObject;

		EditorGUILayout.BeginHorizontal();
		EditorGUI.indentLevel++;
		EditorGUILayout.LabelField("STR");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.STR, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("DEX");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.DEX, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("CON");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.CON, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("INT");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.INT, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("WIS");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.WIS, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("CHA");
		EditorGUILayout.LabelField(Character.GetRawAttributeValue(c, StatAttr.CHA, ItemBonusMode.NONE).ToString());
		EditorGUILayout.EndHorizontal();
		EditorGUI.indentLevel--;
	}

	private static void FillInBoosts(SerializedProperty property, int level) {
		SerializedProperty strProp = property.FindPropertyRelative("STR");
		SerializedProperty dexProp = property.FindPropertyRelative("DEX");
		SerializedProperty conProp = property.FindPropertyRelative("CON");
		SerializedProperty intProp = property.FindPropertyRelative("INT");
		SerializedProperty wisProp = property.FindPropertyRelative("WIS");
		SerializedProperty chaProp = property.FindPropertyRelative("CHA");
		int l1 = (level - level % 5) - 5;
		int l2 = level - level % 5;
		strProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = strProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
		dexProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = dexProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
		conProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = conProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
		intProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = intProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
		wisProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = wisProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
		chaProp.GetArrayElementAtIndex(4 + (l2 / 5)).boolValue = chaProp.GetArrayElementAtIndex(4 + (l1 / 5)).boolValue;
	}

	private bool Validate(int level, SerializedProperty property, out string stringError) {
		stringError = "";
		SerializedProperty strProp = property.FindPropertyRelative("STR");
		SerializedProperty dexProp = property.FindPropertyRelative("DEX");
		SerializedProperty conProp = property.FindPropertyRelative("CON");
		SerializedProperty intProp = property.FindPropertyRelative("INT");
		SerializedProperty wisProp = property.FindPropertyRelative("WIS");
		SerializedProperty chaProp = property.FindPropertyRelative("CHA");
		int t = (strProp.GetArrayElementAtIndex(0).boolValue? 1 : 0)
			+ (dexProp.GetArrayElementAtIndex(0).boolValue ? 1 : 0)
			+ (conProp.GetArrayElementAtIndex(0).boolValue ? 1 : 0)
			+ (intProp.GetArrayElementAtIndex(0).boolValue ? 1 : 0)
			+ (wisProp.GetArrayElementAtIndex(0).boolValue ? 1 : 0)
			+ (chaProp.GetArrayElementAtIndex(0).boolValue ? 1 : 0);
		if(t > 2) {
			stringError = "Must have 2 attributes from Ancestry";
			return true;
		}
		t = (strProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0)
			+ (dexProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0)
			+ (conProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0)
			+ (intProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0)
			+ (wisProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0)
			+ (chaProp.GetArrayElementAtIndex(1).boolValue ? 1 : 0);
		if(t > 1) {
			stringError = "Can only have 1 flaw";
			return true;
		}
		t = (strProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0)
			+ (dexProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0)
			+ (conProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0)
			+ (intProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0)
			+ (wisProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0)
			+ (chaProp.GetArrayElementAtIndex(2).boolValue ? 1 : 0);
		if(t != 2) {
			stringError = "Must have 2 attributes from Background";
			return true;
		}
		t = (strProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0)
			+ (dexProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0)
			+ (conProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0)
			+ (intProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0)
			+ (wisProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0)
			+ (chaProp.GetArrayElementAtIndex(3).boolValue ? 1 : 0);
		if(t != 1) {
			stringError = "Must have 1 attribute from Class";
			return true;
		}
		for(int i=4;i<5+(level/5);i++) {
			t = (strProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0)
				+ (dexProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0)
				+ (conProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0)
				+ (intProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0)
				+ (wisProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0)
				+ (chaProp.GetArrayElementAtIndex(i).boolValue ? 1 : 0);
			if(t != 4) {
				stringError = "Must have 4 attributes from boosts";
				return true;
			}
		}
		return false;
	}

	private Rect DrawStatEditorGui(Rect rect, string statString, SerializedProperty property, int level) {
		SerializedProperty statProp = property.FindPropertyRelative(statString);
		if(statProp == null) return rect;
		Rect orig = rect;
		EditorGUI.LabelField(rect, statString);
		rect.xMin += Screen.width - 200;
		rect.width = 18;
		for(int i = 0; i < 5 + (level / 5); i++) {
			SerializedProperty bProp = statProp.GetArrayElementAtIndex(i);
			bProp.boolValue = EditorGUI.Toggle(rect, bProp.boolValue);
			
			rect.x += 18;
			if(i == 0 && bProp.boolValue) {
				statProp.GetArrayElementAtIndex(1).boolValue = false;
			}
			if(i == 1 && bProp.boolValue) {
				statProp.GetArrayElementAtIndex(0).boolValue = false;
			}
		}
		for(int i = 5 + (level / 5); i < 9; i++) {
			statProp.GetArrayElementAtIndex(i).boolValue = false;
		}
		//statProp.boolValue = EditorGUI.Toggle(rect, statProp.boolValue);
		orig.y += 20;
		return orig;
	}
}
