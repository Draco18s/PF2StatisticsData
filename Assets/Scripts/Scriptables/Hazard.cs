using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Hazard", menuName = "Pathfinder2/Hazard", order = 1)]
public class Hazard : ScriptableObject {
	new public string name;
	public int level;
	public MTEML stealth;
	public MTEML disable;
	public bool canBeAttacked;
	public MTEML armorClass;
	public MTEML fortSave;
	public MTEML refxSave;
	public bool isComplex;
	public bool usesAttack;
	public bool usesSavingThrow;
	public MTEML effectDifficultyClass;
	public AffectType canAffectsSaves;

	public static int GetAttack(int level, bool isComplex) {
		if(isComplex) {
			return 8 + level + (level / 2);
		}
		return 11 + level + ((level+1)/2);
	}

	public static int GetArmorClass(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 18 + level + (level / 2) + (level < 2 ? 1 : 0);
			case MTEML.HIGH:
				return 15 + level + (level / 2) + (level < 2 ? 1 : 0);
			case MTEML.MODERATE:
				return 15 + level + (level / 2) + (level < 2 ? 1 : 0);
			case MTEML.LOW:
				return 12 + level + (level / 2) + (level < 2 ? 1 : 0);
			case MTEML.TERRIBLE:
				return 9 + level + (level / 2) + (level < 2 ? 1 : 0);
		}
		return 0;
	}

	public static int GetFortReflexBonus(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 10 + level + ((level - 1+ (level > 21 ? (1) : 0)) / 2);
			case MTEML.HIGH:
				return 9 + level + ((level - 1 + (level > 5 ? (level > 10 ? (level > 18 ? (3) : 2) : 1) : 0)) / 3);
			case MTEML.MODERATE:
				return 7 + level + ((level - 1 + (level > 5 ? (level > 10 ? (level > 18 ? (3) : 2) : 1) : 0)) / 3);
			case MTEML.LOW:
				return 3 + level + ((level - 1 + (level > 5 ? (level > 10 ? (level > 15 ? (3) : 2) : 1) : 0)) / 3);
			case MTEML.TERRIBLE:
				return 0 + level + ((level - 1 + (level > 5 ? (level > 10 ? (level > 15 ? (3) : 2) : 1) : 0)) / 3);
		}
		return 0;
	}

	public static int GetSaveDC(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 19 + level + ((level + 3) / 5) + ((level - 1 - (level > 23 ? 1 : 0)) / 5);
			case MTEML.HIGH:
				return 16 + level + (level / 3);
			case MTEML.MODERATE:
				return 14 + level + (level / 3);
			case MTEML.LOW:
				return 12 + level + (level / 3);
			case MTEML.TERRIBLE:
				return 10 + level + (level / 3);
		}
		return 0;
	}

	public static int GetSkillDC(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 19 + level + (level / 3) + ((level-1) / 3);
			case MTEML.HIGH:
				return 16 + level + (level / 3) + ((level - 1) / 3);
			case MTEML.MODERATE:
				return 13 + level + ((level - 1) / 2);
			case MTEML.LOW:
				return 12 + level + ((level - 1) / 3);
			case MTEML.TERRIBLE:
				return 7 + level + ((level - 1) / 3);
		}
		return 0;
	}

	public MTEML GetAttribute(MStatAttr stat) {
		switch(stat) {
			case MStatAttr.AC:
				return armorClass;
			case MStatAttr.ABILITY_DC:
				return effectDifficultyClass;
			case MStatAttr.STEALTH:
				return stealth;
			case MStatAttr.FORT:
				return fortSave;
			case MStatAttr.REFX:
				return refxSave;
			case MStatAttr.DISABLE:
				return disable;
		}
		return MTEML.TERRIBLE;
	}
}
