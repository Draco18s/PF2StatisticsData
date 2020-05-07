using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Monster_Type", menuName = "Pathfinder2/Monster", order = 1)]
public class Monster : ScriptableObject
{
	new public string name;
	public int level;
	public float weight;
	public MTEML attacks;
	public bool attacksAreSpells;
	public MTEML armorClass;
	public MTEML abilitySaveDC;
	public AffectType canAffectsSaves;
	public MTEML perception;
	public MTEML fort;
	public MTEML refx;
	public MTEML will;
	public MTEML stealth;

	/*public static int GetStatValue(Monster mon, MStatAttr stat) {
		MTEML teml = mon.GetAttribute(stat);
		return 0;// GetValueFor(teml, mon.level);
	}*/

	public static int GetAttack(MTEML teml, int level, bool isSpell) {
		if(isSpell) return GetAbilityDC(teml, level) - 8;
		switch(teml) {
			case MTEML.EXTREME:
				return 10 + level + (level / 2);
			case MTEML.HIGH:
				return 8 + level + (level / 2);
			case MTEML.MODERATE:
				return 6 + level + (level / 2);
			case MTEML.LOW:
				return 4 + level + ((level + 1) / 3);
			case MTEML.TERRIBLE:
				return 2 + level + ((level + 1) / 3);
		}
		return 0;
	}

	public static int GetArmor(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 18 + level + (level / 2) + (level < 1 ? 1 : 0);
			case MTEML.HIGH:
				return 15 + level + (level / 2) + (level < 1 ? 1 : 0);
			case MTEML.MODERATE:
				return 14 + level + (level / 2) + (level < 1 ? 1 : 0);
			case MTEML.LOW:
				return 12 + level + (level / 2) + (level < 1 ? 1 : 0);
			case MTEML.TERRIBLE:
				return 8 + level + (level / 2) + (level < 1 ? 1 : 0);
		}
		return 0;
	}

	public static int GetAbilityDC(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 19 + level + (level / 2);
			case MTEML.HIGH:
				return 16 + level + (level / 3);
			case MTEML.MODERATE:
				return 13 + level + (level / 3);
			case MTEML.LOW:
				return 0;
			case MTEML.TERRIBLE:
				return 0;
		}
		return 0;
	}

	public static int GetPerception(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 10 + level + (level / 2);
			case MTEML.HIGH:
				return 9 + level + ((level - 1) / 5) + ((level + 1) / 5);
			case MTEML.MODERATE:
				return 6 + level + ((level - 1) / 5) + ((level + 1) / 5);
			case MTEML.LOW:
				return 3 + level + ((level - 1) / 5) + ((level + 1) / 5);
			case MTEML.TERRIBLE:
				return 1 + level + ((level - 1) / 3);
		}
		return 0;
	}

	public static int GetSavingThrow(MTEML teml, int level) {
		return GetPerception(teml, level);
	}

	public static int GetStealth(MTEML teml, int level) {
		switch(teml) {
			case MTEML.EXTREME:
				return 9 + level + (level / 3) + ((level - 1) / 3);
			case MTEML.HIGH:
				return 6 + level + (level / 3) + ((level - 1) / 3);
			case MTEML.MODERATE:
				return 5 + level + ((level - 1) / 2);
			case MTEML.LOW:
				return 3 + level + ((level - 1) / 2);
			case MTEML.TERRIBLE:
				return 2 + level + ((level + 1) / 3);
		}
		return 0;
	}

	public MTEML GetAttribute(MStatAttr stat) {
		switch(stat) {
			case MStatAttr.ATK:
				return attacks;
			case MStatAttr.AC:
				return armorClass;
			case MStatAttr.ABILITY_DC:
				return abilitySaveDC;
			case MStatAttr.PERCEPT:
				return perception;
			case MStatAttr.STEALTH:
				return stealth;
			case MStatAttr.FORT:
				return fort;
			case MStatAttr.REFX:
				return refx;
			case MStatAttr.WILL:
				return will;
		}
		return MTEML.TERRIBLE;
	}
}
