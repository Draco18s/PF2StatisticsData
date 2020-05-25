using Assets.draco18s.ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour {
	// Start is called before the first frame update
	private int diffSetting;
	public Transform topBar;
	public GameObject[] resultWindow;
	public GradientAsset gradient;
	public GradientAsset[] gradients;
	public static Main instance;
	public GameObject tooltip;
	public ItemBonusMode itemBonusMode = ItemBonusMode.ITEM;
	private Character[] allChar;
	private Monster[] allMons;
	private Hazard[] allHaz;
	[NonSerialized] public List<Dropdown.OptionData> levelOpts;
	[NonSerialized] public List<Dropdown.OptionData> classOpts;
	protected Dropdown diffDropdown {
		get {
			return topBar.Find("DifficultySelect").GetComponent<Dropdown>();
		}
	}
	private int cycle = 1;

	void Start() {
		instance = this;
		allChar = Resources.LoadAll<Character>("classes");
		allMons = Resources.LoadAll<Monster>("specific_monsters");
		allHaz = Resources.LoadAll<Hazard>("hazards");
		List<int> availLevels = new List<int>();
		classOpts = new List<Dropdown.OptionData>();
		foreach(Character c in allChar) {
			if(!classOpts.Any(ooo => ooo.text == c.name)) {
				Dropdown.OptionData opt = new Dropdown.OptionData(c.name);
				classOpts.Add(opt);
			}
			if(!availLevels.Contains(c.level)) {
				availLevels.Add(c.level);
			}
		}
		levelOpts = new List<Dropdown.OptionData>();
		availLevels.Sort();
		foreach(int lv in availLevels) {
			levelOpts.Add(new Dropdown.OptionData(lv.ToString()));
		}
		diffDropdown.onValueChanged.AddListener(v => {
			diffSetting = v;
		});
		Transform legend = topBar.Find("Legend");
		legend.Find("Hover").GetComponent<Image>().AddHover(p => {
			ShowTooltip(legend.transform.position+new Vector3(-50,-100,0), "Indicates an approximate result of making a roll against challenges of the indicated difficulty as an average between all possible outcomes (nat-1 to nat-20).\nE.g. if only two results are possible (fail or success) then the average will be a blend between the two (yellow).", 4, 1, false);
		});
		legend.Find("Hover2").GetComponent<Image>().AddHover(p => {
			ShowTooltip(legend.transform.position + new Vector3(-50, -100, 0), "Black: Critical Failure\nRed: Failure\nGreen: Success\nBlue: Critical Success\nBlend: Some mixture.", 4, 1, false);
		});
		Color[] col = new Color[20];
		for(int i=0;i<20;i++) {
			col[i] = gradient.gradient.Evaluate((float)i / 19);
		}
		legend.GetComponentInChildren<BreakdownBar>().SetBitColors(col);
		legend.GetComponentInChildren<Dropdown>().onValueChanged.AddListener(v => {
			gradient = gradients[v];
			Color[] cols = new Color[20];
			for(int i = 0; i < 20; i++) {
				cols[i] = gradient.gradient.Evaluate((float)i / 19);
			}
			legend.GetComponentInChildren<BreakdownBar>().SetBitColors(cols);
			foreach(GameObject window in resultWindow) {
				window.GetComponent<ResultWindow>().RefreshColor();
			}
		});
		//legend.GetComponentInChildren<BreakdownBar>().SetNotches(-1, -1);
		int ext = 0;
		int high = 0;
		int mod = 0;
		int low = 0;
		int ter = 0;
		foreach(Monster mon in allMons) {
			foreach(MStatAttr atr in Enum.GetValues(typeof(MStatAttr))) {
				MTEML teml = mon.GetAttribute(atr);
				switch(teml) {
					case MTEML.TERRIBLE:
					case MTEML.THE_WORST:
						ter++;
						break;
					case MTEML.LOW:
						low++;
						break;
					case MTEML.MODERATE:
						mod++;
						break;
					case MTEML.HIGH:
						high++;
						break;
					case MTEML.JUST_BONKERS:
					case MTEML.EXTREME:
						ext++;
						break;
				}
			}
		}
		Debug.Log("Monsters, ter: " + ter);
		Debug.Log("Monsters, low: " + low);
		Debug.Log("Monsters, mod: " + mod);
		Debug.Log("Monsters, high: " + high);
		Debug.Log("Monsters, ext: " + ext);
		topBar.Find("UpdateBtn").GetComponent<Button>().onClick.AddListener(() => {
			foreach(GameObject window in resultWindow) {
				window.GetComponent<ResultWindow>().UpdateDifficulty();
			}
		});
		topBar.Find("CycleToggle").GetComponent<Toggle>().onValueChanged.AddListener(b => {
			if(b) {
				StartCoroutine(CycleLevelValues());
			}
		});
	}

	private IEnumerator CycleLevelValues() {
		while(topBar.Find("CycleToggle").GetComponent<Toggle>().isOn) {
			foreach(GameObject window in resultWindow) {
				window.GetComponent<ResultWindow>().levelDropdown.value = cycle;
			}
			cycle = (cycle % 20) + 1;
			yield return new WaitForSeconds(0.5f);
		}
	}

	public static Character GetCharacter(string className, int levelSetting) {
		return instance.allChar.FirstOrDefault(chr => chr.name == className && chr.level == levelSetting);
	}

	public bool Test(Character character, ResultWindow window) {
		if(character == null) return true;
		GetDifficultyLevel(character.level, diffSetting, out int minLv, out int maxLv);
		if(maxLv < -5) return true;
		StatisticsResults result = ComputeStatistics(character, allMons, minLv, maxLv, itemBonusMode);
		result = ComputeStatistics(character, allHaz, minLv, maxLv, result, itemBonusMode);
		CalcStatsForSkills(character, minLv, maxLv, result, itemBonusMode);
		window.transform.Find("ClassLevel").GetComponent<Text>().text = character.name + " " + character.level + " (" + diffDropdown.options[diffSetting].text + ")";
		window.DisplayResult(result, gradient);
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
				minLv = maxLv = level - 4;
				break;
			case 8:
				minLv = maxLv = level - 3;
				break;
			case 9:
				minLv = maxLv = level - 2;
				break;
			case 10:
				minLv = maxLv = level - 1;
				break;
			case 11:
				minLv = maxLv = level;
				break;
			case 12:
				minLv = maxLv = level + 1;
				break;
			case 13:
				minLv = maxLv = level + 2;
				break;
			case 14:
				minLv = maxLv = level + 3;
				break;
			case 15:
				minLv = maxLv = level + 4;
				break;
			default:
				minLv = maxLv = -10;
				break;
		}
	}

	public static StatisticsResults ComputeStatistics(Character character, Monster[] allMons, int minlvl, int maxlvl, ItemBonusMode mode) {
		StatisticsResults result = new StatisticsResults();
		result.attack[21]= 20;
		result.armorClass[21]= 20;
		result.classSpellDC[21]= 20;
		result.perception[21]= 20;
		result.fort[21]= 20;
		result.refx[21]= 20;
		result.will[21]= 20;
		result.skillSpecialist[21]= 20;
		result.skillDecent[21]= 20;
		result.skillDabbler[21]= 20;
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
		int skil;
		for(int level = minLv; level <= maxLv; level++) {
			skil = Character.GetStatValue(chr, chr.perception, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.PERCEPTION);
			result.totSkills++;
			result.perceptiontot++;
			int diff = 14 + level + (level / 3) + (level >= 5 ? (level >= 23 ? (level >= 25 ? 3 : 2) : 1) : 0);
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff) {
					result.perception[i - 1] += (int)RollResult.SUCCESS;
				}
				else {
					if(result.perception[20] < i) result.perception[20] = i;
					result.perception[i - 1] += (int)RollResult.FAIL;
				}
			}
			TEML maxTeml = GetBestTeml(chr, chr.level);
			StatAttr skillStat = (chr.classStat != StatAttr.CON && level < maxLv ? chr.classStat : GetBestSkillStat(chr, 0));
			skil = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_BEST);
			//diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				result.skillSpecialist[i - 1] += GetRollResult(i, skil, diff, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillSpecialist[20], ref result.skillSpecialist[21]);
			}
			maxTeml = GetBestTeml(chr, chr.level / 2);
			skillStat = GetBestSkillStat(chr, 1);
			skil = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_DECENT);
			//diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				result.skillDecent[i - 1] += GetRollResult(i, skil, diff, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillDecent[20], ref result.skillDecent[21]);
			}
			skillStat = GetBestSkillStat(chr, 3);
			skil = Character.GetStatValue(chr, TEML.TRAINED, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_LOWEST);
			//diff = 14 + level + (level / 3);
			for(int i = 1; i <= 20; i++) {
				result.skillDabbler[i - 1] += GetRollResult(i, skil, diff, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillDabbler[20], ref result.skillDabbler[21]);
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
			baseSkill += 2;
			if(level >= 7) {
				baseSkill += 2;
				if(level >= 15) {
					baseSkill += 2;
				}
			}
		}
		return (TEML)baseSkill;
	}

	private static void CalcStatsForPair(Character chr, Monster mon, StatisticsResults result, ItemBonusMode mode) {
		int off;
		if(chr.attackStat == StatAttr.STR || chr.attackStat == StatAttr.DEX)
			off = Character.GetStatValue(chr, chr.attacks, chr.attackStat, mode)+Character.GetItemBonus(chr,mode,ItemBonusType.WEAPON);
		else
			off = Character.GetStatValue(chr, chr.classSpellDC, chr.attackStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SPELLDC);
		int def = Monster.GetArmor(mon.armorClass, mon.level);
		result.attacktot+=mon.weight;
		bool attackIsDex = chr.attackStat == StatAttr.DEX || chr.attackStat == StatAttr.INT || chr.attackStat == StatAttr.WIS || chr.attackStat == StatAttr.CHA;
		if(chr.attackBoost.HasFlag(SaveIncrease.MONSTER_HUNTER)) {
			int diff = mon.level - chr.level;
			int monsterHunter = 1;
			if(chr.attackBoost.HasFlag(SaveIncrease.LEGENDARY_MONSTER_HUNTER)) {
				monsterHunter = 2;
			}
			for(int i = 1; i <= 20; i++) {
				float total = 0;
				for(int s = 1; s <= 20; s++) {
					//basing recall knowledge off an average between a decent skill and a dabbler
					//to account for there being 4 different skills (odds are high that a ranger isn't going to be best at more than 1)

					TEML maxTeml = GetBestTeml(chr, chr.level / 2);
					StatAttr skillStat = GetBestSkillStat(chr, 1);
					int skil1 = Character.GetStatValue(chr, maxTeml, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_DECENT);
					skillStat = GetBestSkillStat(chr, 3);
					int skil2 = Character.GetStatValue(chr, TEML.TRAINED, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_LOWEST);
					int skil = (skil1 + skil2) / 2;

					diff = 14 + mon.level + (mon.level / 3) + (mon.level >= 5 ? (mon.level >= 23 ? (mon.level >= 25 ? 3 : 2) : 1) : 0);
					float a = 0, b = 0;
					if(GetRollResult(s, skil, diff, SaveIncrease.NONE, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref a, ref b) >= (chr.attackBoost.HasFlag(SaveIncrease.MASTER_MONSTER_HUNTER) ? 2 : 1)) {
						total += mon.weight * GetRollResult(i, off + monsterHunter, def, chr.attackBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.attack[20], ref result.attack[21]);
					}
					else {
						total += mon.weight * GetRollResult(i, off, def, chr.attackBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.attack[20], ref result.attack[21]);
					}
				}
				result.attack[i - 1] += total/20f;
			}
		}
		else {
			for(int i = 1; i <= 20; i++) {
				float r = mon.weight * GetRollResult(i, off, def, chr.attackBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.attack[20], ref result.attack[21]);
				result.attack[i - 1] += r;
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
			result.spellDCtot += mon.weight;
			sve = Monster.GetSavingThrow(mon.fort, mon.level);
			for(int i = 1; i <= 20; i++) {
				result.classSpellDC[i - 1] += mon.weight * GetRollResult(i, sve, sdc, SaveIncrease.NONE, SaveIncrease.NONE, ref result.classSpellDC[20], ref result.classSpellDC[21]);
			}
		}
		if(chr.canAffectsSaves.HasFlag(AffectType.REFX)) {
			result.spellDCtot += mon.weight;
			sve = Monster.GetSavingThrow(mon.refx, mon.level);
			for(int i = 1; i <= 20; i++) {
				result.classSpellDC[i - 1] += mon.weight * GetRollResult(i, sve, sdc, SaveIncrease.NONE, SaveIncrease.NONE, ref result.classSpellDC[20], ref result.classSpellDC[21]);
			}
		}
		if(chr.canAffectsSaves.HasFlag(AffectType.WILL)) {
			result.spellDCtot += mon.weight;
			sve = Monster.GetSavingThrow(mon.will, mon.level);
			for(int i = 1; i <= 20; i++) {
				result.classSpellDC[i - 1] += mon.weight * GetRollResult(i, sve, sdc, SaveIncrease.NONE, SaveIncrease.NONE, ref result.classSpellDC[20], ref result.classSpellDC[21]);
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.FORT) || mon.canAffectsSaves.HasFlag(AffectType.FORT_LIVING)) {
			sve = Character.GetStatValue(chr, chr.fort, StatAttr.CON, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.forttot += mon.weight;
			for(int i = 1; i <= 20; i++) {
				result.fort[i - 1] += mon.weight * GetRollResult(i, sve, sdc, chr.fortSaveBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.fort[20], ref result.fort[21]);
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.REFX)) {
			sve = Character.GetReflexSave(chr, chr.refx, StatAttr.DEX, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.refxtot += mon.weight;
			for(int i = 1; i <= 20; i++) {
				result.refx[i - 1] += mon.weight * GetRollResult(i, sve, sdc, chr.refxSaveBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.refx[20], ref result.refx[21]);
			}
		}
		if(mon.canAffectsSaves.HasFlag(AffectType.WILL)) {
			sve = Character.GetStatValue(chr, chr.will, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS);
			sdc = Monster.GetAbilityDC(mon.abilitySaveDC, mon.level);
			result.willtot += mon.weight;
			for(int i = 1; i <= 20; i++) {
				result.will[i - 1] += mon.weight * GetRollResult(i, sve, sdc, chr.willSaveBoost, attackIsDex ? SaveIncrease.NONE : mon.globalDefenseBoost, ref result.will[20], ref result.will[21]);
			}
		}
		if(mon.attacks != MTEML.NONE) {
			off = Monster.GetAttack(mon.attacks, mon.level, mon.attacksAreSpells);
			def = Character.GetArmorClass(chr, chr.armorClass, StatAttr.DEX, mode) + 10;
			int b = Character.GetItemBonus(chr, mode, chr.armorType == ArmorType.HEAVY ? ItemBonusType.HEAVY_ARMOR : ItemBonusType.ARMOR);
			def += b;
			result.armortot += mon.weight;
			for(int i = 1; i <= 20; i++) {
				result.armorClass[i - 1] += mon.weight * GetRollResult(i, off, def, chr.defenseBoost, SaveIncrease.NONE, ref result.armorClass[20], ref result.armorClass[21]) / 2;
				result.armorClass[i - 1] += mon.weight * GetRollResult(i, off, def + chr.shieldBonus, SaveIncrease.NONE, chr.defenseBoost, ref result.armorClass[20], ref result.armorClass[21]) / 2;
			}
		}
		if(mon.stealth != MTEML.NONE) {
			int skil = Character.GetStatValue(chr, chr.perception, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.PERCEPTION);
			int diff = 10 + Monster.GetStealth(mon.stealth, mon.level);
			result.perceptiontot += mon.weight;
			for(int i = 1; i <= 20; i++) {
				if(skil + i >= diff) {
					result.perception[i - 1] += mon.weight * (int)RollResult.SUCCESS;
				}
				else {
					if(result.perception[20] < i) result.perception[20] = i;
					result.perception[i - 1] += mon.weight * (int)RollResult.FAIL;
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
		int hazardFinder = chr.defenseBoost.HasFlag(SaveIncrease.HAZARD_FINDER) ? 1 : 0;
		if(haz.canBeAttacked) {
			off = Character.GetStatValue(chr, chr.attacks, chr.attackStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.WEAPON);
			def = Hazard.GetArmorClass(haz.armorClass, haz.level);
			result.attacktot++;
			for(int i = 1; i <= 20; i++) {
				result.attack[i - 1] += GetRollResult(i, off, def, chr.attackBoost, SaveIncrease.NONE, ref result.attack[20], ref result.attack[21]);
			}
		}
		if(haz.usesAttack) {
			off = Hazard.GetAttack(haz.level, haz.isComplex);
			def = Character.GetArmorClass(chr, chr.armorClass, StatAttr.DEX, mode) + 10 + Character.GetItemBonus(chr, mode, chr.armorType == ArmorType.HEAVY ? ItemBonusType.HEAVY_ARMOR : ItemBonusType.ARMOR) + hazardFinder;
			result.armortot++;
			for(int i = 1; i <= 20; i++) {
				result.armorClass[i - 1] += GetRollResult(i, off, def, SaveIncrease.NONE, SaveIncrease.NONE, ref result.armorClass[20], ref result.armorClass[21]);
			}
		}
		if(haz.usesSavingThrow) {
			if(haz.canAffectsSaves.HasFlag(AffectType.FORT) || haz.canAffectsSaves.HasFlag(AffectType.FORT_LIVING)) {
				sve = Character.GetStatValue(chr, chr.fort, StatAttr.CON, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS)+hazardFinder;
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.forttot++;
				for(int i = 1; i <= 20; i++) {
					result.fort[i - 1] += GetRollResult(i, sve, sdc, chr.fortSaveBoost, SaveIncrease.NONE, ref result.fort[20], ref result.fort[21]);
				}
			}
			if(haz.canAffectsSaves.HasFlag(AffectType.REFX)) {
				sve = Character.GetReflexSave(chr, chr.refx, StatAttr.DEX, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS) + hazardFinder;
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.refxtot++;
				for(int i = 1; i <= 20; i++) {
					result.refx[i - 1] += GetRollResult(i, sve, sdc, chr.refxSaveBoost, SaveIncrease.NONE, ref result.refx[20], ref result.refx[21]);
				}
			}
			if(haz.canAffectsSaves.HasFlag(AffectType.WILL)) {
				sve = Character.GetStatValue(chr, chr.will, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SAVING_THROWS) + hazardFinder;
				sdc = Hazard.GetSaveDC(haz.effectDifficultyClass, haz.level);
				result.willtot++;
				for(int i = 1; i <= 20; i++) {
					result.will[i - 1] += GetRollResult(i, sve, sdc, chr.willSaveBoost, SaveIncrease.NONE, ref result.will[20], ref result.will[21]);
				}
			}
		}
		if(haz.canBeAttacked && chr.canAffectsSaves.HasFlag(AffectType.FORT)) {
			result.spellDCtot++;
			sve = Hazard.GetFortReflexBonus(haz.fortSave, haz.level);
			for(int i = 1; i <= 20; i++) {
				result.classSpellDC[i - 1] += GetRollResult(i, sve, sdc, SaveIncrease.NONE, SaveIncrease.NONE, ref result.classSpellDC[20], ref result.classSpellDC[21]);
			}
		}
		if(haz.canBeAttacked && chr.canAffectsSaves.HasFlag(AffectType.REFX)) {
			result.spellDCtot++;
			sve = Hazard.GetFortReflexBonus(haz.refxSave, haz.level);
			for(int i = 1; i <= 20; i++) {
				result.classSpellDC[i - 1] += GetRollResult(i, sve, sdc, SaveIncrease.NONE, SaveIncrease.NONE, ref result.classSpellDC[20], ref result.classSpellDC[21]);
			}
		}
		result.totSkills++;
		StatAttr skillStat = GetBestSkillStat(chr, 0);
		off = Character.GetSkillValue(chr, GetBestTeml(chr, chr.level), skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_BEST);
		def = Hazard.GetSkillDC(haz.disable, haz.level);
		for(int i = 1; i <= 20; i++) {
			result.skillSpecialist[i - 1] += GetRollResult(i, off, def, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillSpecialist[20], ref result.skillSpecialist[21]);
		}
		skillStat = GetBestSkillStat(chr, 1);
		off = Character.GetSkillValue(chr, GetBestTeml(chr, chr.level - 7), skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_DECENT);
		for(int i = 1; i <= 20; i++) {
			result.skillDecent[i - 1] += GetRollResult(i, off, def, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillDecent[20], ref result.skillDecent[21]);
		}
		skillStat = GetBestSkillStat(chr, 3);
		off = Character.GetSkillValue(chr, TEML.TRAINED, skillStat, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.SKILL_LOWEST);
		for(int i = 1; i <= 20; i++) {
			result.skillDabbler[i - 1] += GetRollResult(i, off, def, SaveIncrease.NONE, SaveIncrease.NONE, ref result.skillDabbler[20], ref result.skillDabbler[21]);
		}
		int skil = Character.GetStatValue(chr, chr.perception, StatAttr.WIS, mode) + Character.GetItemBonus(chr, mode, ItemBonusType.PERCEPTION) + hazardFinder;
		int diff = Hazard.GetSkillDC(haz.stealth, haz.level);
		result.perceptiontot++;
		for(int i = 1; i <= 20; i++) {
			if(skil + i >= diff) {
				result.perception[i - 1] += (int)RollResult.SUCCESS;
			}
			else {
				if(result.perception[20] < i) result.perception[20] = i;
				result.perception[i - 1] += (int)RollResult.FAIL;
			}
		}
	}

	private static float GetRollResult(int i, int off, int def, SaveIncrease saveBoost, SaveIncrease monsterSaveBoost, ref float maxMiss, ref float minCrit) {
		if(saveBoost.HasFlag(SaveIncrease.MINIMUM_10)) {
			i = Math.Max(i, 10);
		}
		if(i == 1) {
			if(saveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
				return GetRerollResult(RollResult.FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			else
				return GetRerollResult(RollResult.CRIT_FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
		}
		if(i == 20 || (saveBoost.HasFlag(SaveIncrease.CRIT_ON_19) && i == 19)) {
			if(off + i >= def) {
				return GetRerollResult(RollResult.CRIT_SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
			else if(off + i >= def - 10 && i == 20) {
				if(saveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
					return GetRerollResult(RollResult.CRIT_SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
				else
					return GetRerollResult(RollResult.SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
			else {
				if(saveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
					return GetRerollResult(RollResult.SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
				else
					return GetRerollResult(RollResult.FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
		}
		if(off + i >= def) { //save
			if(off + i >= def + 10) { //crit
				return GetRerollResult(RollResult.CRIT_SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
			else {
				if(saveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
					return GetRerollResult(RollResult.CRIT_SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
				else
					return GetRerollResult(RollResult.SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
		}
		else { //fail
			if(off + i <= def - 10) {
				if(saveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
					return GetRerollResult(RollResult.FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
				else
					return GetRerollResult(RollResult.CRIT_FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
			else { //crit
				if(saveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
					return GetRerollResult(RollResult.SUCCESS, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
				else
					return GetRerollResult(RollResult.FAIL, off, def, i, saveBoost, monsterSaveBoost, ref maxMiss, ref minCrit);
			}
		}
	}

	private static float GetRerollResult(RollResult baseResult, int off, int def, int roll, SaveIncrease saveBoost, SaveIncrease monsterSaveBoost, ref float maxMiss, ref float minCrit) {
		if(monsterSaveBoost.HasFlag(SaveIncrease.AURA_OF_MISFORTUNE)) {
			float result = 0;
			for(int i=1;i<=20;i++) {
				result += GetRollResult(Math.Min(i, roll), off, def, saveBoost, SaveIncrease.NONE, ref maxMiss, ref minCrit);
			}
			return result / 20f;
		}
		if(baseResult < RollResult.SUCCESS && maxMiss < roll) {
			maxMiss = roll;
		}
		if(baseResult > RollResult.SUCCESS && minCrit > roll) {
			minCrit = roll;
		}
		float total = 0;
		if((saveBoost.HasFlag(SaveIncrease.REROLL_FAILURE_PLUS_2) || saveBoost.HasFlag(SaveIncrease.REROLL_FAILURE)) && baseResult == RollResult.FAIL) {
			total = RollSimpleSave(baseResult, off+(saveBoost.HasFlag(SaveIncrease.REROLL_FAILURE_PLUS_2) ? 2 : 0), def, saveBoost, (int)RollResult.FAIL);
			return ((int)baseResult * 3 + (total / 20)) / 4;
		}
		if((saveBoost.HasFlag(SaveIncrease.REROLL_CRITICAL_FAILURE_PLUS_2) || saveBoost.HasFlag(SaveIncrease.REROLL_CRITICAL_FAILURE)) && baseResult == RollResult.CRIT_FAIL) {
			total = RollSimpleSave(baseResult, off+ (saveBoost.HasFlag(SaveIncrease.REROLL_CRITICAL_FAILURE_PLUS_2) ? 2 : 0), def, saveBoost);
			return ((int)baseResult * 3 + (total / 20)) / 4;
		}
		if(saveBoost.HasFlag(SaveIncrease.DISADVANTAGE) && baseResult == RollResult.SUCCESS) {
			total = RollSimpleSave(baseResult, off, def, saveBoost, (int)RollResult.CRIT_FAIL, (int)baseResult);
			return Math.Min(total, (int)baseResult);
		}
		return (int)baseResult;
	}

	private static float RollSimpleSave(RollResult baseResult, int off, int def, SaveIncrease saveBoost, int minimumResult = 0, int maximumResult = 3) {
		float total = 0;
		for(int i = 1; i <= 20; i++) {
			if(i == 1) {
				if(saveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
					total += Math.Max(Math.Min((int)RollResult.FAIL, maximumResult), minimumResult);
				else
					total += Math.Max(Math.Min((int)RollResult.CRIT_FAIL, maximumResult), minimumResult);
				continue;
			}
			if(i == 20) {
				if(off + i >= def) {
					total += Math.Max(Math.Min((int)RollResult.CRIT_SUCCESS, maximumResult), minimumResult);
				}
				else if(off + i >= def - 10) {
					if(saveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
						total += Math.Max(Math.Min((int)RollResult.CRIT_SUCCESS, maximumResult), minimumResult);
					else
						total += Math.Max(Math.Min((int)RollResult.SUCCESS, maximumResult), minimumResult);
				}
				else {
					if(saveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
						total += Math.Max(Math.Min((int)RollResult.SUCCESS, maximumResult), minimumResult);
					else
						total += Math.Max(Math.Min((int)RollResult.FAIL, maximumResult), minimumResult);
				}
				continue;
			}
			if(off + i >= def) { //save
				if(off + i >= def + 10) { //crit
					total += Math.Max(Math.Min((int)RollResult.CRIT_SUCCESS, maximumResult), minimumResult);
				}
				else {
					if(saveBoost.HasFlag(SaveIncrease.SUCCESS_IS_CRIT_SUCCESS))
						total += Math.Max(Math.Min((int)RollResult.CRIT_SUCCESS, maximumResult), minimumResult);
					else
						total += Math.Max(Math.Min((int)RollResult.SUCCESS, maximumResult), minimumResult);
				}
			}
			else { //fail
				if(off + i <= def - 10) {
					if(saveBoost.HasFlag(SaveIncrease.CRIT_FAIL_IS_FAIL))
						total += Math.Max(Math.Min((int)RollResult.FAIL, maximumResult), minimumResult);
					else
						total += Math.Max(Math.Min((int)RollResult.CRIT_FAIL, maximumResult), minimumResult);
				}
				else { //crit
					if(saveBoost.HasFlag(SaveIncrease.FAIL_IS_SUCCESS))
						total += Math.Max(Math.Min((int)RollResult.SUCCESS, maximumResult), minimumResult);
					else
						total += Math.Max(Math.Min((int)RollResult.FAIL, maximumResult), minimumResult);
				}
			}
		}
		return total;
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
