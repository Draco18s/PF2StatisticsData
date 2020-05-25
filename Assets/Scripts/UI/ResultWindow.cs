using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultWindow : MonoBehaviour {
	private int classSetting;
	private int levelSetting;
	public Dropdown classDropdown;
	public Dropdown levelDropdown;
	private StatisticsResults lastResult;

	void Start() {
		levelDropdown.AddOptions(Main.instance.levelOpts);
		levelDropdown.onValueChanged.AddListener(v => {
			int ll = 0;
			int.TryParse(levelDropdown.options[v].text, out ll);
			levelSetting = ll;
			UpdateWindow();
		});
		classDropdown.AddOptions(Main.instance.classOpts);
		classDropdown.onValueChanged.AddListener(v => {
			classSetting = v;
			UpdateWindow();
		});
		ClearWindow(gameObject);
	}

	private void UpdateWindow() {
		if(classSetting == 0 || classSetting == 0 || levelSetting == 0) {
			ClearWindow(gameObject);
			return;
		}
		if(Main.instance.Test(Main.GetCharacter(classDropdown.options[classSetting].text, levelSetting), this)) {
			levelDropdown.SetValueWithoutNotify(0);
			ClearWindow(gameObject);
		}
	}

	public void UpdateDifficulty() {
		UpdateWindow();
	}

	private static void ClearWindow(GameObject window) {
		window.transform.Find("ClassLevel").GetComponent<Text>().text = "";
		Color[] cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			cols[i] = Color.white;
		}
		BreakdownBar bar = window.transform.Find("Attack").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Abilities").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Fort").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Refx").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Will").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Armor").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Perception").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Skill1").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Skill2").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		bar = window.transform.Find("Skill3").GetComponentInChildren<BreakdownBar>();
		bar.SetBitColors(cols);
		bar.SetNotches(-1, -1);
		window.GetComponent<ResultWindow>().lastResult = null;
	}

	public void DisplayResult(StatisticsResults result, GradientAsset gradient) {
		lastResult = result;
		if(result == null) return;
		BreakdownBar bar = transform.Find("Attack").GetComponentInChildren<BreakdownBar>();
		Color[] cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.attacktot > 0) {
				float v = (result.attack[i] / 3f) / result.attacktot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.attack[20] = result.attack[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.attack[20], (int)result.attack[21]);
		bar = transform.Find("Abilities").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.spellDCtot > 0) {
				float v = (result.classSpellDC[i] / 3f) / result.spellDCtot;
				cols[i] = gradient.gradient.Evaluate(1 - v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.classSpellDC[20] = result.classSpellDC[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.classSpellDC[20], (int)result.classSpellDC[21]);
		bar = transform.Find("Fort").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.forttot > 0) {
				float v = (result.fort[i] / 3f) / result.forttot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.fort[20] = result.fort[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.fort[20], (int)result.fort[21]);
		bar = transform.Find("Refx").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.refxtot > 0) {
				float v = (result.refx[i] / 3f) / result.refxtot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.refx[20] = result.refx[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.refx[20], (int)result.refx[21]);
		bar = transform.Find("Will").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.willtot > 0) {
				float v = (result.will[i] / 3f) / result.willtot;
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.will[20] = result.will[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.will[20], (int)result.will[21]);
		bar = transform.Find("Armor").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.armortot > 0) {
				float v = (result.armorClass[i] / 3f) / result.armortot;
				cols[i] = gradient.gradient.Evaluate(1 - v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.armorClass[20] = result.armorClass[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.armorClass[20], (int)result.armorClass[21]);
		bar = transform.Find("Perception").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.perceptiontot > 0) {
				float v = (result.perception[i] / 3f) / (result.perceptiontot);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.perception[20] = result.perception[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.perception[20], -1);
		bar = transform.Find("Skill1").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillSpecialist[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.skillSpecialist[20] = result.skillSpecialist[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.skillSpecialist[20], (int)result.skillSpecialist[21]);
		bar = transform.Find("Skill2").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillDecent[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.skillDecent[20] = result.skillDecent[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.skillDecent[20], (int)result.skillDecent[21]);
		bar = transform.Find("Skill3").GetComponentInChildren<BreakdownBar>();
		cols = new Color[20];
		for(int i = 0; i < 20; i++) {
			if(result.totSkills > 0) {
				float v = (result.skillDabbler[i] / 3f) / (result.totSkills);
				cols[i] = gradient.gradient.Evaluate(v);
			}
			else {
				cols[i] = new Color(.5f, .5f, .5f, 1);
				result.skillDabbler[20] = result.skillDabbler[21] = -1;
			}
		}
		bar.SetBitColors(cols);
		bar.SetNotches((int)result.skillDabbler[20], (int)result.skillDabbler[21]);
	}

	public void RefreshColor() {
		DisplayResult(lastResult, Main.instance.gradient);
	}
}
