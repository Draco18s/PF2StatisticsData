using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(Monster), true)]
public class MonsterEditor : Editor
{
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		EditorGUILayout.LabelField("Approximated Stats");
		//EditorGUILayout.LabelField("STR");
		Monster c = (Monster)serializedObject.targetObject;
		EditorGUILayout.IntField("Perception", Monster.GetPerception(c.GetAttribute(MStatAttr.PERCEPT), c.level));
		EditorGUILayout.IntField("Stealth", Monster.GetStealth(c.GetAttribute(MStatAttr.STEALTH), c.level));
		EditorGUILayout.IntField("AC", Monster.GetArmor(c.GetAttribute(MStatAttr.AC), c.level));
		EditorGUILayout.IntField("Attack", Monster.GetAttack(c.GetAttribute(MStatAttr.ATK),c.level,c.attacksAreSpells));
		EditorGUILayout.IntField("Save DC", Monster.GetAbilityDC(c.GetAttribute(MStatAttr.ABILITY_DC), c.level));
		EditorGUILayout.IntField("Fort", Monster.GetSavingThrow(c.GetAttribute(MStatAttr.FORT), c.level));
		EditorGUILayout.IntField("Ref", Monster.GetSavingThrow(c.GetAttribute(MStatAttr.REFX), c.level));
		EditorGUILayout.IntField("Will", Monster.GetSavingThrow(c.GetAttribute(MStatAttr.WILL), c.level));
	}
}
