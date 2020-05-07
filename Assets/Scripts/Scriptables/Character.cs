using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character_Class", menuName = "Pathfinder2/Character", order = 1)]
public class Character : ScriptableObject
{
	new public string name;
	public int level;
	public TEML attacks;
	public StatAttr attackStat;
	public TEML armorClass;
	public int shieldBonus;
	public ArmorType armorType;
	public TEML classSpellDC;
	public bool alchemistUseItemDC;
	public AffectType canAffectsSaves;
	public StatAttr classStat;
	public TEML perception;
	public int firstSkillIncrease;
	public TEML fort;
	public SaveIncrease fortSaveBoost;
	public TEML refx;
	public SaveIncrease refxSaveBoost;
	public TEML will;
	public SaveIncrease willSaveBoost;
	public StatArray stats;

	public static int GetSkillValue(Character chr, TEML teml, StatAttr stat) {
		//TODO: items by level
		if(teml > TEML.UNTRAINED)
			return chr.level + (int)teml + StatArray.GetValue(chr.stats.GetRankFor(stat), chr.level);
		return StatArray.GetValue(chr.stats.GetRankFor(stat), chr.level);
	}

	public static int GetStatValue(Character chr, TEML teml, StatAttr stat) {
		if(teml > TEML.UNTRAINED)
			return chr.level + (int)teml + StatArray.GetValue(chr.stats.GetRankFor(stat), chr.level);
		return StatArray.GetValue(chr.stats.GetRankFor(stat), chr.level);
	}

	public static int GetArmorClass(Character chr, TEML teml, StatAttr stat) {
		int dex = StatArray.GetValue(chr.stats.GetRankFor(StatAttr.DEX), chr.level);
		int baseAC = GetStatValue(chr, teml, stat) - dex;
		int armorAC = 0;
		switch (chr.armorType) {
			case ArmorType.UNARMORED:
				break;
			case ArmorType.LIGHT:
				dex = Mathf.Min(dex, 4);
				armorAC = Mathf.Max(Mathf.Max(5 - dex,0),2);
				break;
			case ArmorType.MEDIUM:
				dex = Mathf.Min(dex, 2);
				armorAC = Mathf.Max(Mathf.Max(5 - dex, 0),4);
				break;
			case ArmorType.HEAVY:
				dex = Mathf.Min(dex, 0);
				armorAC = Mathf.Max(Mathf.Max(6 - dex, 0),6);
				break;
		}
		return baseAC + armorAC + dex;
	}

	public static StatAttr GetSecondaryStat(Character chr, StatRank rank) {
		if(chr.stats.STR == rank) return StatAttr.STR;
		if(chr.stats.DEX == rank) return StatAttr.DEX;
		if(chr.stats.CON == rank) return StatAttr.CON;
		if(chr.stats.INT == rank) return StatAttr.INT;
		if(chr.stats.WIS == rank) return StatAttr.WIS;
		if(chr.stats.CHA == rank) return StatAttr.CHA;
		return chr.classStat;
	}
}
