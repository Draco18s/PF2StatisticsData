using UnityEditor;

[CustomEditor(typeof(Hazard), true)]
public class HazardEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.LabelField("Approximated Stats");

		Hazard c = (Hazard)serializedObject.targetObject;
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Stealth", UnityEngine.GUILayout.MaxWidth(UnityEngine.Screen.width*.31f));
		EditorGUILayout.LabelField("+", UnityEngine.GUILayout.MaxWidth(10));
		EditorGUILayout.IntField(Hazard.GetSkillDC(c.GetAttribute(MStatAttr.STEALTH), c.level)-10);
		EditorGUILayout.LabelField("/", UnityEngine.GUILayout.MaxWidth(10));
		EditorGUILayout.IntField(Hazard.GetSkillDC(c.GetAttribute(MStatAttr.STEALTH), c.level));
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.IntField("Disable", Hazard.GetSkillDC(c.GetAttribute(MStatAttr.DISABLE), c.level));
		EditorGUILayout.IntField("AC", Hazard.GetArmorClass(c.GetAttribute(MStatAttr.AC), c.level));
		EditorGUILayout.IntField("Attack", Hazard.GetAttack(c.level, c.isComplex));
		EditorGUILayout.IntField("Save DC", Hazard.GetSaveDC(c.GetAttribute(MStatAttr.ABILITY_DC), c.level));
		EditorGUILayout.IntField("Fort", Hazard.GetFortReflexBonus(c.GetAttribute(MStatAttr.FORT), c.level));
		EditorGUILayout.IntField("Ref", Hazard.GetFortReflexBonus(c.GetAttribute(MStatAttr.REFX), c.level));
	}
}
