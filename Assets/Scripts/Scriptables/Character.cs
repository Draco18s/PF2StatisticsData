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
	public SaveIncrease attackBoost;
	public TEML armorClass;
	public SaveIncrease defenseBoost;
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
	//public StatArray stats;
	public StatGen statGen;

	public static int GetSkillValue(Character chr, TEML teml, StatAttr stat, ItemBonusMode mode) {
		int b = (int)teml;
		if(teml > TEML.UNTRAINED) {
			b += chr.level;
		}
		b += GetAttributeValue(chr, stat, mode);
		return b;
	}

	public static int GetAttributeValue(Character chr, StatAttr stat, ItemBonusMode mode) {
		//TODO: items by level
		return StatGen.CalcForStat(chr.statGen, stat) + (stat == chr.classStat ? GetItemBonus(chr, mode, ItemBonusType.ATTRIBUTE) : 0);
	}

	public static int GetRawAttributeValue(Character chr, StatAttr stat, ItemBonusMode mode) {
		//TODO: items by level
		return StatGen.CalcForStatRaw(chr.statGen, stat) + (stat == chr.classStat ? GetItemBonus(chr, mode, ItemBonusType.ATTRIBUTE) : 0);
	}

	public static int GetStatValue(Character chr, TEML teml, StatAttr stat, ItemBonusMode mode) {
		int b = (int)teml;
		if(teml > TEML.UNTRAINED) {
			b += chr.level;
		}
		b += GetAttributeValue(chr, stat, mode);
		return b;
	}

	public static int GetReflexSave(Character chr, TEML teml, StatAttr stat, ItemBonusMode mode) {
		int baseSave = GetStatValue(chr, teml, stat, mode);
		int bulwark = 0;
		switch(chr.armorType) {
			case ArmorType.HEAVY:
				bulwark = 3;
				break;
		}
		return baseSave + bulwark;
	}

	public static int GetArmorClass(Character chr, TEML teml, StatAttr stat, ItemBonusMode mode) {
		int dex = GetAttributeValue(chr, StatAttr.DEX, mode);
		int baseAC = GetStatValue(chr, teml, stat, mode) - dex;
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
		switch(rank) {
			case StatRank.KEY:
				return chr.classStat;
		}
		return StatGen.GetBestFor(chr.statGen, rank, chr.classStat);
	}

	public static StatAttr GetSecondaryStat(Character chr, StatRank rank, StatAttr not) {
		return StatGen.GetBestFor(chr.statGen, rank, not);
	}

	public static int GetItemBonus(Character chr, ItemBonusMode mode, ItemBonusType bonus) {
		switch(mode) {
			case ItemBonusMode.ITEM:
				return GetItemBonus(chr, bonus);
			case ItemBonusMode.ABP:
				return GetAPB(chr, bonus);
			default:
				return 0;
		}
	}

	private static int GetItemBonus(Character chr, ItemBonusType bonus) {
		return GetAPB(chr,bonus); //TODO
	}

	private static int GetAPB(Character chr, ItemBonusType bonus) {
		switch(bonus) {
			case ItemBonusType.WEAPON:
				if(chr.level < 2)
					return 0;
				if(chr.level < 10)
					return 1;
				if(chr.level < 16)
					return 2;
				return 3;
			case ItemBonusType.ARMOR:
			case ItemBonusType.HEAVY_ARMOR:
				if(chr.level < 5)
					return 0;
				if(chr.level < 11)
					return 1;
				if(chr.level < 18)
					return 2;
				return 3;
			case ItemBonusType.SKILL_BEST:
				if(chr.level < 3)
					return 0;
				if(chr.level < 9)
					return 1;
				if(chr.level < 17)
					return 2;
				return 3;
			case ItemBonusType.SKILL_DECENT:
				if(chr.level < 6)
					return 0;
				if(chr.level < 13)
					return 1;
				if(chr.level < 20)
					return 2;
				return 3;
			case ItemBonusType.SKILL_LOWEST:
				if(chr.level < 17)
					return 0;
				return 1;
			case ItemBonusType.PERCEPTION:
				if(chr.level < 7)
					return 0;
				if(chr.level < 13)
					return 1;
				if(chr.level < 19)
					return 2;
				return 3;
			case ItemBonusType.ATTRIBUTE:
				if(chr.level < 17)
					return 0;
				return 1;
			case ItemBonusType.SPELLDC:
				return 0;
			case ItemBonusType.SAVING_THROWS:
				if(chr.level < 8)
					return 0;
				if(chr.level < 14)
					return 1;
				if(chr.level < 20)
					return 2;
				return 3;
		}
		return 0;
	}
}
