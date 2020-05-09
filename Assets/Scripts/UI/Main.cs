﻿using Assets.draco18s.ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
	// Start is called before the first frame update
	public Dropdown diffDropdown;
	public Dropdown levelDropdown;
	public Dropdown[] dropdowns;
	public GameObject[] resultWindow;
	public GradientAsset gradient;
	public GameObject legend;
	private int diffSetting;
	private int levelSetting;
	public static Main instance;
	public GameObject tooltip;
	public ItemBonusMode itemBonusMode = ItemBonusMode.ITEM;
	private Character[] allChar;

	void Start() {
		instance = this;
		allChar = Resources.LoadAll<Character>("classes");
		List<int> availLevels = new List<int>();
		List<Dropdown.OptionData> opts;
		for(int i = 0; i < dropdowns.Length; i++) {
			int j = i;
			Dropdown dd = dropdowns[i];
			opts = new List<Dropdown.OptionData>();
			foreach(Character c in allChar) {
				if(!opts.Any(ooo => ooo.text == c.name)) {
					Dropdown.OptionData opt = new Dropdown.OptionData(c.name);
					opts.Add(opt);
				}
				if(!availLevels.Contains(c.level)) {
					availLevels.Add(c.level);
				}
			}
			dd.AddOptions(opts);
			dd.onValueChanged.AddListener(v => {
				if(v == 0) {
					ClearWindow(resultWindow[j]);
					return;
				}
				if(Test(GetCharacter(dd.options[v].text, levelSetting), resultWindow[j])) {
					dd.SetValueWithoutNotify(0);
					ClearWindow(resultWindow[j]);
				}
			});
		}
		opts = new List<Dropdown.OptionData>();
		foreach(int lv in availLevels) {
			opts.Add(new Dropdown.OptionData(lv.ToString()));
		}
		levelDropdown.AddOptions(opts);
		levelDropdown.onValueChanged.AddListener(v => {
			int ll = 0;
			int.TryParse(levelDropdown.options[v].text, out ll);
			levelSetting = ll;
		});
		diffDropdown.onValueChanged.AddListener(v => {
			diffSetting = v;
		});
		legend.transform.Find("Hover").GetComponent<Image>().AddHover(p => {
			ShowTooltip(legend.transform.position+new Vector3(-50,-100,0), "Indicates an approximate result of making a roll against challenges of the indicated difficulty as an average between all possible outcomes (nat-1 to nat-20).\nE.g. if only two results are possible (fail or success) then the average will be a blend between the two (yellow).", 4, 1, false);
		});
		legend.transform.Find("Hover2").GetComponent<Image>().AddHover(p => {
			ShowTooltip(legend.transform.position + new Vector3(-50, -100, 0), "Black: Critical Failure\nRed: Failure\nGreen: Success\nBlue: Critical Success\nBlend: Some mixture.", 4, 1, false);
		});
	}

	private Character GetCharacter(string className, int levelSetting) {
		return allChar.FirstOrDefault(chr => chr.name == className && chr.level == levelSetting);
	}

	private void ClearWindow(GameObject window) {
		window.transform.Find("ClassLevel").GetComponent<Text>().text = "";
		Color[] cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			cols[i] = Color.white;
		}
		BreakdownBar bar = window.transform.Find("Attack").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Abilities").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Fort").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Refx").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Will").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Armor").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Perception").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill1").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill2").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill3").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
	}

	private bool Test(Character character, GameObject window) {
		if(character == null) return true;
		Monster[] allMons = Resources.LoadAll<Monster>("specific_monsters");
		Hazard[] allHaz = Resources.LoadAll<Hazard>("hazards");
		int minLv, maxLv;
		GetDifficultyLevel(character.level, diffSetting, out minLv, out maxLv);
		if(maxLv < -5) return true;
		StatisticsResults result = ComputeStatistics(character, allMons, minLv, maxLv, itemBonusMode);
		result = ComputeStatistics(character, allHaz, minLv, maxLv, result, itemBonusMode);
		CalcStatsForSkills(character, minLv, maxLv, result, itemBonusMode);
		window.transform.Find("ClassLevel").GetComponent<Text>().text = character.name + " " + character.level + " (" + diffDropdown.options[diffSetting].text + ")";
		DisplayResult(result, window);
		return false;
	}

	private void GetDifficultyLevel(int level, int diffSetting, out int minLv, out int maxLv) {
		switch(diffSetting) {
			case 1:
				minLv = level - 4;
				maxLv = level + 4;
				break;
			case 2:
				minLv = level - 4;
				maxLv = level;
				break;
			case 3:
				minLv = level - 3;
				maxLv = level + 1;
				break;
			case 4:
				minLv = level - 2;
				maxLv = level + 2;
				break;
			case 5:
				minLv = level - 1;
				maxLv = level + 3;
				break;
			case 6:
				minLv = level;
				maxLv = level + 4;
				break;
			case 7:
				minLv = maxLv = level-2;
				break;
			case 8:
				minLv = maxLv = level - 1;
				break;
			case 9:
				minLv = maxLv = level;
				break;
			case 10:
				minLv = maxLv = level + 1;
				break;
			case 11:
				minLv = maxLv = level + 2;
				break;
			default:
				minLv = maxLv = -10;
				break;
		}
	}

	private void DisplayResult(StatisticsResults result, GameObject window) {
		BreakdownBar bar = window.transform.Find("Attack").GetComponentInChildren<BreakdownBar>();
		Color[] cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.attacktot > 0) {
				float v = (result.attack[i] / 3f) / result.attacktot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Abilities").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.spellDCtot > 0) {
				float v = (result.classSpellDC[i] / 3f) / result.spellDCtot;
				cols[i] = gradient.gradient.Evaluate(1 - v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Fort").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.forttot > 0) {
				float v = (result.fort[i] / 3f) / result.forttot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Refx").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.refxtot > 0) {
				float v = (result.refx[i] / 3f) / result.refxtot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Will").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.willtot > 0) {
				float v = (result.will[i] / 3f) / result.willtot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Armor").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.armortot > 0) {
				float v = (result.armorClass[i] / 3f) / result.armortot;
				cols[i] = gradient.gradient.Evaluate(1 - v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Perception").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.perceptiontot > 0) {
				float v = (result.perception[i] / 3f) / (result.perceptiontot);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill1").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillSpecialist[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill2").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillDecent[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
		bar = window.transform.Find("Skill3").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillDabbler[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
			}
		}
		bar.SetBitColors(cols);
	}

	public static StatisticsResults ComputeStatistics(Character character, Monster[] allMons, int minlvl, int maxlvl, ItemBonusMode mode) {
		StatisticsResults result = new StatisticsResults();
		foreach(Monster m in allMons) {
			if(m.level >= minlvl && m.level <= maxlvl)
				CalcStatsForPair(character, m, result, mode);
		}
		return result;
	}

	public StatisticsResults ComputeStatistics(Character character, Hazard[] allHaz, int minlvl, int maxlvl, StatisticsResults result, ItemBonusMode mode) {
		foreach(Hazard m in allHaz) {
			if(m.level >= minlvl && m.level <= maxlvl)
				CalcStatsForPair(character, m, result, mode);
		}
		return result;
	}

	private static void CalcStatsForSkills(Character chr, int minLv, int maxLv, StatisticsResults result, ItemBonusMode mode) {
		int skil = Character.GetStatValue(chr, chr.perception, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.PERCEPTION);
		for(int level = minLv; level <= maxLv; level++) {
			result.totSkills++;
			result.perceptiontot++;
			int diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff) {
					result.perception[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					result.perception[i - 1] += (int)RollResult.FAIL;
				}
			}
			TEML maxTeml = GetBestTeml(chr, chr.level);
			StatAttr skillStat = (chr.classStat != StatAttr.CON && level < maxLv ? chr.classStat : GetBestSkillStat(chr, 0));
			skil = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_BEST);
			diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff || i == 20) {
					if(skil + i >= diff + 10 || i == 20) {
						result.skillSpecialist[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else
						result.skillSpecialist[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					if(skil + i < diff - 10 || i == 1) {
						result.skillSpecialist[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else
						result.skillSpecialist[i - 1] += (int)RollResult.FAIL;
				}
			}
			maxTeml = GetBestTeml(chr, chr.level - 7);
			skillStat = GetBestSkillStat(chr, 1);
			skil = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_DECENT);
			diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff || i == 20) {
					if(skil + i >= diff + 10 || i == 20) {
						result.skillDecent[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else
						result.skillDecent[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					if(skil + i < diff - 10 || i == 1) {
						result.skillDecent[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else
						result.skillDecent[i - 1] += (int)RollResult.FAIL;
				}
			}
			skillStat = GetBestSkillStat(chr, 3);
			skil = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_LOWEST);
			diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff || i == 20) {
					if(skil + i >= diff + 10 || i == 20) {
						result.skillDabbler[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else
						result.skillDabbler[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					if(skil + i < diff - 10 || i == 1) {
						result.skillDabbler[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else
						result.skillDabbler[i - 1] += (int)RollResult.FAIL;
				}
			}
		}
	}

	private static StatAttr GetBestSkillStat(Character chr, int n) {
		StatRank rank = (StatRank)n;
		StatAttr sec = Character.GetSecondaryStat(chr, rank);
		if(sec != StatAttr.STR && sec != StatAttr.CON) {
			return sec;
		}
		rank = (rank + 1 <= StatRank.NICE ? rank + 1: rank);
		sec = Character.GetSecondaryStat(chr, rank, sec);
		if(sec != StatAttr.STR && sec != StatAttr.CON) {
			return sec;
		}
		rank = (rank + 1 <= StatRank.DUMP ? rank + 1 : rank);
		sec = Character.GetSecondaryStat(chr, rank, sec);
		if(sec != StatAttr.STR && sec != StatAttr.CON) {
			return sec;
		}
		rank = (rank + 1 <= StatRank.DUMP ? rank + 1 : rank);
		return Character.GetSecondaryStat(chr, rank, sec);
	}

	private static TEML GetBestTeml(Character chr, int level) {
		int baseSkill = (int)TEML.TRAINED;
		if(level >= chr.firstSkillIncrease) {
			baseSkill++;
			if(level >= 7) {
				baseSkill++;
				if(level >= 15) {
					baseSkill++;
				}
			}
		}
		return (TEML)baseSkill;
	}

	private static void CalcStatsForPair(Character chr, Monster mon, StatisticsResults result, ItemBonusMode mode) {
		int off = Character.GetStatValue(chr, chr.attacks, chr.attackStat, mode)+Character.GetItemBonus(chr,mode,ItemBonusType.WEAPON);
		int def = Monster.GetArmor(mon.armorClass, mon.level);
		result.attacktot++;
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def) {
				if(i == 1) {
					result.attack[i - 1] += (int)RollResult.FAIL;
					continue;
				}
				if(i == 20) {
					if(off + i >= def) {
						result.attack[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(off + i >= def - 10) {
						result.attack[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.attack[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(off + i >= def + 10) {
					result.attack[i - 1] += (int)RollResult.CRIT_SUCCESS;
				}
				else {
					result.attack[i - 1] += (int)RollResult.SUCCESS;
				}
			}
			else {
				result.attack[i - 1] += (int)RollResult.FAIL;
			}
		}
		int sdc;
		int sve;
		if(chr.alchemistUseItemDC) {
			if(chr.level < 8)
				sdc = 17 + chr.level + (chr.level / 3);
			else
				sdc = Math.Max(Character.GetStatValue(chr, chr.classSpellDC, chr.classStat, mode) + 10 + Character.GetItemBonus(chr, mode, ItemBonusType.SPELLDC), 17 + chr.level + (chr.level / 3));
		}
		else {
			sdc = Character.GetStatValue(chr, chr.classSpellDC, chr.classStat, mode) + 10 + Character.GetItemBonus(chr, mode, ItemBonusType.SPELLDC);
		}
		if(chr.canAffectsSaves.HasFlag(AffectType.FORT) || chr.canAffectsSaves.HasFlag(AffectType.FORT_LIVING)) {
			result.spellDCtot++;
			sve = Monster.GetSavingThrow(mon.fort, mon.level);
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(chr.canAffectsSaves.HasFlag(AffectType.REFX)) {
			result.spellDCtot++;
			sve = Monster.GetSavingThrow(mon.refx, mon.level);
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(chr.canAffectsSaves.HasFlag(AffectType.WILL)) {
			result.spellDCtot++;
			sve = Monster.GetSavingThrow(mon.will, mon.level);
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.FORT) || mon.canAffectsSaves.HasFlag(AffectType.FORT_LIVING)) {
			sve = Character.GetStatValue(chr, chr.fort, StatAttr.CON, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.forttot++;
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					if(chr.fortSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
						result.fort[i - 1] += (int)RollResult.FAIL;
					else
						result.fort[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.fort[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.fort[i - 1] += (int)RollResult.SUCCESS;
						else
							result.fort[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.fort[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.fort[i - 1] += (int)RollResult.FAIL;
						else
							result.fort[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.fort[i - 1] += (int)RollResult.SUCCESS;
						else
							result.fort[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.REFX)) {
			sve = Character.GetStatValue(chr, chr.refx, StatAttr.DEX, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.refxtot++;
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					if(chr.refxSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
						result.refx[i - 1] += (int)RollResult.FAIL;
					else
						result.refx[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.refx[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.refx[i - 1] += (int)RollResult.SUCCESS;
						else
							result.refx[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.refx[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.refx[i - 1] += (int)RollResult.FAIL;
						else
							result.refx[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.refx[i - 1] += (int)RollResult.SUCCESS;
						else
							result.refx[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.WILL)) {
			sve = Character.GetStatValue(chr, chr.will, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.willtot++;
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					if(chr.willSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
						result.will[i - 1] += (int)RollResult.FAIL;
					else
						result.will[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						if(chr.willSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.will[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						if(chr.willSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.will[i - 1] += (int)RollResult.SUCCESS;
						else
							result.will[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						if(chr.willSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
							result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
						else
							result.will[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						if(chr.willSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.will[i - 1] += (int)RollResult.FAIL;
						else
							result.will[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						if(chr.willSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
							result.will[i - 1] += (int)RollResult.SUCCESS;
						else
							result.will[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		off = Monster.GetAttack(mon.attacks, mon.level, mon.attacksAreSpells);
		def = Character.GetArmorClass(chr, chr.armorClass, StatAttr.DEX, mode) + 10;
		int b = Character.GetItemBonus(chr, mode, chr.armorType == ArmorType.HEAVY ? ItemBonusType.HEAVY_ARMOR : ItemBonusType.ARMOR);
		Debug.Log("Def: " + def + "+"+b + " (" + chr.armorType + ")");
		def += b;
		result.armortot++;
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def) {
				if(i == 1) {
					result.armorClass[i - 1] += ((int)RollResult.FAIL) / 2f;
					continue;
				}
				if(i == 20) {
					if(off + i >= def) {
						result.armorClass[i - 1] += ((int)RollResult.CRIT_SUCCESS) / 2f;
					}
					else if(off + i >= def - 10) {
						result.armorClass[i - 1] += ((int)RollResult.SUCCESS) / 2f;
					}
					else {
						result.armorClass[i - 1] += ((int)RollResult.FAIL) / 2f;
					}
					continue;
				}
				if(off + i >= def + 10) {
					result.armorClass[i - 1] += ((int)RollResult.CRIT_SUCCESS) / 2f;
				}
				else {
					result.armorClass[i - 1] += ((int)RollResult.SUCCESS) / 2f;
				}
			}
			else {
				result.armorClass[i - 1] += ((int)RollResult.FAIL) / 2f;
			}
		}
		def += chr.shieldBonus;
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def) {
				if(i == 1) {
					result.armorClass[i - 1] += ((int)RollResult.FAIL)/2f;
					continue;
				}
				if(i == 20) {
					if(off + i >= def) {
						result.armorClass[i - 1] += ((int)RollResult.CRIT_SUCCESS)/ 2f;
					}
					else if(off + i >= def - 10) {
						result.armorClass[i - 1] += ((int)RollResult.SUCCESS)/2f;
					}
					else {
						result.armorClass[i - 1] += ((int)RollResult.FAIL)/2f;
					}
					continue;
				}
				if(off + i >= def + 10) {
					result.armorClass[i - 1] += ((int)RollResult.CRIT_SUCCESS)/2f;
				}
				else {
					result.armorClass[i - 1] += ((int)RollResult.SUCCESS)/2f;
				}
			}
			else {
				result.armorClass[i - 1] += ((int)RollResult.FAIL)/2f;
			}
		}
		if(mon.stealth != MTEML.NONE) {
			int skil = Character.GetStatValue(chr, chr.perception, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.PERCEPTION);
			int diff = 10 + Monster.GetStealth(mon.stealth, mon.level);
			result.perceptiontot++;
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff) {
					result.perception[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					result.perception[i - 1] += (int)RollResult.FAIL;
				}
			}
		}
	}

	private void CalcStatsForPair(Character chr, Hazard haz, StatisticsResults result, ItemBonusMode mode) {
		int off;
		int def;
		int sve;
		int sdc;
		if(chr.alchemistUseItemDC) {
			if(chr.level < 8)
				sdc = 17 + chr.level + (chr.level / 3);
			else
				sdc = Math.Max(Character.GetStatValue(chr, chr.classSpellDC, chr.classStat, mode) + 10 + Character.GetItemBonus(chr, mode, ItemBonusType.SPELLDC), 17 + chr.level + (chr.level / 3));
		}
		else {
			sdc = Character.GetStatValue(chr, chr.classSpellDC, chr.classStat, mode) + 10 + Character.GetItemBonus(chr, mode, ItemBonusType.SPELLDC);
		}
		if(haz.canBeAttacked) {
			off = Character.GetStatValue(chr, chr.attacks, chr.attackStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.WEAPON);
			def = Hazard.GetArmorClass(haz.armorClass, haz.level);
			result.attacktot++;
			for(int i = 1; i <= 20; i++) {
				if(off + i >= def) {
					if(i == 1) {
						result.attack[i - 1] += (int)RollResult.FAIL;
						continue;
					}
					if(i == 20) {
						if(off + i >= def) {
							result.attack[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else if(off + i >= def - 10) {
							result.attack[i - 1] += (int)RollResult.SUCCESS;
						}
						else {
							result.attack[i - 1] += (int)RollResult.FAIL;
						}
						continue;
					}
					if(off + i >= def + 10) {
						result.attack[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.attack[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else {
					result.attack[i - 1] += (int)RollResult.FAIL;
				}
			}
		}
		if(haz.usesAttack) {
			off = Hazard.GetAttack(haz.level, haz.isComplex);
			def = Character.GetArmorClass(chr, chr.armorClass, StatAttr.DEX, mode) + 10 + Character.GetItemBonus(chr, mode, chr.armorType == ArmorType.HEAVY ? ItemBonusType.HEAVY_ARMOR : ItemBonusType.ARMOR);
			result.armortot++;
			for(int i = 1; i <= 20; i++) {
				if(off + i >= def) {
					if(i == 1) {
						result.armorClass[i - 1] += (int)RollResult.FAIL;
						continue;
					}
					if(i == 20) {
						if(off + i >= def) {
							result.armorClass[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else if(off + i >= def - 10) {
							result.armorClass[i - 1] += (int)RollResult.SUCCESS;
						}
						else {
							result.armorClass[i - 1] += (int)RollResult.FAIL;
						}
						continue;
					}
					if(off + i >= def + 10) {
						result.armorClass[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.armorClass[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else {
					result.armorClass[i - 1] += (int)RollResult.FAIL;
				}
			}
		}
		if(haz.usesSavingThrow) {
			if(haz.canAffectsSaves.HasFlag(AffectType.FORT) || haz.canAffectsSaves.HasFlag(AffectType.FORT_LIVING)) {
				sve = Character.GetStatValue(chr, chr.fort, StatAttr.CON, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.forttot++;
				for(int i = 1; i <= 20; i++) {
					if(i == 1) {
						if(chr.fortSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.fort[i - 1] += (int)RollResult.FAIL;
						else
							result.fort[i - 1] += (int)RollResult.CRIT_FAIL;
						continue;
					}
					if(i == 20) {
						if(sve + i >= sdc) {
							result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else if(sve + i >= sdc - 10) {
							if(chr.fortSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.fort[i - 1] += (int)RollResult.SUCCESS;
						}
						else {
							if(chr.fortSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.fort[i - 1] += (int)RollResult.SUCCESS;
							else
								result.fort[i - 1] += (int)RollResult.FAIL;
						}
						continue;
					}
					if(sve + i >= sdc) { //save
						if(sve + i >= sdc + 10) { //crit
							result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else {
							if(chr.fortSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.fort[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.fort[i - 1] += (int)RollResult.SUCCESS;
						}
					}
					else { //fail
						if(sve + i <= sdc - 10) {
							if(chr.fortSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
								result.fort[i - 1] += (int)RollResult.FAIL;
							else
								result.fort[i - 1] += (int)RollResult.CRIT_FAIL;
						}
						else { //crit
							if(chr.fortSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.fort[i - 1] += (int)RollResult.SUCCESS;
							else
								result.fort[i - 1] += (int)RollResult.FAIL;
						}
					}
				}
			}
			if(haz.canAffectsSaves.HasFlag(AffectType.REFX)) {
				sve = Character.GetStatValue(chr, chr.refx, StatAttr.DEX, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.refxtot++;
				for(int i = 1; i <= 20; i++) {
					if(i == 1) {
						if(chr.refxSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.refx[i - 1] += (int)RollResult.FAIL;
						else
							result.refx[i - 1] += (int)RollResult.CRIT_FAIL;
						continue;
					}
					if(i == 20) {
						if(sve + i >= sdc) {
							result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else if(sve + i >= sdc - 10) {
							if(chr.refxSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.refx[i - 1] += (int)RollResult.SUCCESS;
						}
						else {
							if(chr.refxSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.refx[i - 1] += (int)RollResult.SUCCESS;
							else
								result.refx[i - 1] += (int)RollResult.FAIL;
						}
						continue;
					}
					if(sve + i >= sdc) { //save
						if(sve + i >= sdc + 10) { //crit
							result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else {
							if(chr.refxSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.refx[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.refx[i - 1] += (int)RollResult.SUCCESS;
						}
					}
					else { //fail
						if(sve + i <= sdc - 10) {
							if(chr.refxSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
								result.refx[i - 1] += (int)RollResult.FAIL;
							else
								result.refx[i - 1] += (int)RollResult.CRIT_FAIL;
						}
						else { //crit
							if(chr.refxSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.refx[i - 1] += (int)RollResult.SUCCESS;
							else
								result.refx[i - 1] += (int)RollResult.FAIL;
						}
					}
				}
			}
			if(haz.canAffectsSaves.HasFlag(AffectType.WILL)) {
				sve = Character.GetStatValue(chr, chr.will, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.willtot++;
				for(int i = 1; i <= 20; i++) {
					if(i == 1) {
						if(chr.willSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
							result.will[i - 1] += (int)RollResult.FAIL;
						else
							result.will[i - 1] += (int)RollResult.CRIT_FAIL;
						continue;
					}
					if(i == 20) {
						if(sve + i >= sdc) {
							result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else if(sve + i >= sdc - 10) {
							if(chr.willSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.will[i - 1] += (int)RollResult.SUCCESS;
						}
						else {
							if(chr.willSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.will[i - 1] += (int)RollResult.SUCCESS;
							else
								result.will[i - 1] += (int)RollResult.FAIL;
						}
						continue;
					}
					if(sve + i >= sdc) { //save
						if(sve + i >= sdc + 10) { //crit
							result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
						}
						else {
							if(chr.willSaveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
								result.will[i - 1] += (int)RollResult.CRIT_SUCCESS;
							else
								result.will[i - 1] += (int)RollResult.SUCCESS;
						}
					}
					else { //fail
						if(sve + i <= sdc - 10) {
							if(chr.willSaveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
								result.will[i - 1] += (int)RollResult.FAIL;
							else
								result.will[i - 1] += (int)RollResult.CRIT_FAIL;
						}
						else { //crit
							if(chr.willSaveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
								result.will[i - 1] += (int)RollResult.SUCCESS;
							else
								result.will[i - 1] += (int)RollResult.FAIL;
						}
					}
				}
			}
		}
		if(haz.canBeAttacked && chr.canAffectsSaves.HasFlag(AffectType.FORT)) {
			result.spellDCtot++;
			sve = Hazard.GetFortReflexBonus(haz.fortSave, haz.level);
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		if(haz.canBeAttacked && chr.canAffectsSaves.HasFlag(AffectType.REFX)) {
			result.spellDCtot++;
			sve = Hazard.GetFortReflexBonus(haz.refxSave, haz.level);
			for(int i = 1; i <= 20; i++) {
				if(i == 1) {
					result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					continue;
				}
				if(i == 20) {
					if(sve + i >= sdc) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else if(sve + i >= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
					continue;
				}
				if(sve + i >= sdc) { //save
					if(sve + i >= sdc + 10) { //crit
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_SUCCESS;
					}
					else {
						result.classSpellDC[i - 1] += (int)RollResult.SUCCESS;
					}
				}
				else { //fail
					if(sve + i <= sdc - 10) {
						result.classSpellDC[i - 1] += (int)RollResult.CRIT_FAIL;
					}
					else { //crit
						result.classSpellDC[i - 1] += (int)RollResult.FAIL;
					}
				}
			}
		}
		result.totSkills++;
		StatAttr skillStat = GetBestSkillStat(chr, 0);
		off = Character.GetSkillValue(chr, GetBestTeml(chr, chr.level), skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_BEST);
		def = Hazard.GetSkillDC(haz.disable, haz.level);
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def || i == 20) {
				if(off + i >= def + 10 || i == 20) {
					result.skillSpecialist[i - 1] += (int)RollResult.CRIT_SUCCESS;
				}
				else
					result.skillSpecialist[i - 1] += (int)RollResult.SUCCESS;
			}
			else {
				if(off + i < def - 10 || i == 1) {
					result.skillSpecialist[i - 1] += (int)RollResult.CRIT_FAIL;
				}
				else
					result.skillSpecialist[i - 1] += (int)RollResult.FAIL;
			}
		}
		skillStat = GetBestSkillStat(chr, 1);
		off = Character.GetSkillValue(chr, GetBestTeml(chr, chr.level - 7), skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_DECENT);
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def || i == 20) {
				if(off + i >= def + 10 || i == 20) {
					result.skillDecent[i - 1] += (int)RollResult.CRIT_SUCCESS;
				}
				else
					result.skillDecent[i - 1] += (int)RollResult.SUCCESS;
			}
			else {
				if(off + i < def - 10 || i == 1) {
					result.skillDecent[i - 1] += (int)RollResult.CRIT_FAIL;
				}
				else
					result.skillDecent[i - 1] += (int)RollResult.FAIL;
			}
		}
		skillStat = GetBestSkillStat(chr, 3);
		off = Character.GetSkillValue(chr, TEML.TRAINED, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_LOWEST);
		for(int i = 1; i <= 20; i++) {
			if(off + i >= def || i == 20) {
				if(off + i >= def + 10 || i == 20) {
					result.skillDabbler[i - 1] += (int)RollResult.CRIT_SUCCESS;
				}
				else
					result.skillDabbler[i - 1] += (int)RollResult.SUCCESS;
			}
			else {
				if(off + i < def - 10 || i == 1) {
					result.skillDabbler[i - 1] += (int)RollResult.CRIT_FAIL;
				}
				else
					result.skillDabbler[i - 1] += (int)RollResult.FAIL;
			}
		}
	}
	public static void HideTooltip() {
		instance.tooltip.SetActive(false);
	}

	public static void ShowTooltip(Vector3 p, string v) {
		ShowTooltip(p, v, 1);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio) {
		ShowTooltip(pos, v, ratio, 1);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio, float scale) {
		ShowTooltip(pos, v, ratio, scale, true);
	}
	public static void ShowTooltip(Vector3 pos, string v, float ratio, float scale, bool allowMoveDown) {
		if(v.Length == 0) return;

		instance.tooltip.SetActive(true);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 160);
		((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 64);
		instance.tooltip.transform.position = pos;
		Text textArea = instance.tooltip.transform.Find("Text").GetComponent<Text>();
		textArea.text = v;
		//width + 7.5
		//height + 6
		bool fits = false;
		if(textArea.preferredWidth < 610) {
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textArea.preferredWidth);
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textArea.preferredHeight);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (textArea.preferredWidth / 2) + 22);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (textArea.preferredHeight / 2) + 16);
			fits = true;
		}
		/*if(t.preferredHeight < 232) {
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (t.preferredHeight / 4) + 7.5f);
			fits = true;
		}*/
		float w = 64;// t.preferredWidth;
		if(!fits) {
			float ph = 4;
			float h = 68;
			float r = 1;
			do {
				r = (h * ratio) / w;
				w += 64;// * Mathf.CeilToInt(r);
				((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
				ph = h;
				h = textArea.preferredHeight;
			} while(h * ratio > w);
			if(h == ph) {
				w = textArea.preferredWidth + 22;
				//w -= 32 * Mathf.CeilToInt(r);
				((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			}

			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, w);
			((RectTransform)textArea.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, h);
			h = textArea.preferredHeight;
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (w / 2) + 8);
			((RectTransform)instance.tooltip.transform).SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (h / 2) + 15f);
		}
		float wid = ((RectTransform)instance.tooltip.transform).rect.width;
		float hig = ((RectTransform)instance.tooltip.transform).rect.height;
		if(instance.tooltip.transform.position.x + wid * scale > Screen.width) {
			if(allowMoveDown) {
				//shift the tooltip down. No check for off-screen
				if(instance.tooltip.transform.position.y - hig * 1.5f < 35) {
					instance.tooltip.transform.position = new Vector3(Screen.width - 5 - wid, instance.tooltip.transform.position.y + ((RectTransform)instance.tooltip.transform).rect.height, 0);
				}
				else {
					instance.tooltip.transform.position = new Vector3(Screen.width - 5 - wid, instance.tooltip.transform.position.y - ((RectTransform)instance.tooltip.transform).rect.height, 0);
				}
			}
			else {
				//instance.tooltip.transform.position += new Vector3(wid * scale / 2, 0, 0);
				instance.tooltip.transform.position = new Vector3(Screen.width - 5 - wid, instance.tooltip.transform.position.y, 0);
			}
		}
		else {
			instance.tooltip.transform.position += new Vector3(0, 0, 0);
		}
		instance.tooltip.transform.localScale = new Vector3(scale, scale, scale);
	}
}
