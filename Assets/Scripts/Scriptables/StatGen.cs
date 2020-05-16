using System;
using System.Collections.Generic;

[Serializable]
public class StatGen {
	public bool[] STR;
	public bool[] DEX;
	public bool[] CON;
	public bool[] INT;
	public bool[] WIS;
	public bool[] CHA;

	public StatGen() {
		STR = new bool[9];
		DEX = new bool[9];
		CON = new bool[9];
		INT = new bool[9];
		WIS = new bool[9];
		CHA = new bool[9];
	}

	public static int CalcForStat(StatGen allStats, StatAttr stat) {
		return (CalcForStatRaw(allStats,stat)-10)/ 2;
	}

	public static int CalcForStatRaw(StatGen allStats, StatAttr stat) {
		bool[] arr = GetArray(allStats, stat);
		int val = 10;
		for(int i = 0; i < 9; i++) {
			if(i == 1 && arr[i]) {
				val -= 2;
			}
			else if(val < 18 && arr[i]) {
				val += 2;
			}
			else if(arr[i]) {
				val += 1;
			}
		}
		return val;
	}

	private static bool[] GetArray(StatGen allStats, StatAttr stat) {
		switch(stat) {
			case StatAttr.STR:
				return allStats.STR;
			case StatAttr.DEX:
				return allStats.DEX;
			case StatAttr.CON:
				return allStats.CON;
			case StatAttr.INT:
				return allStats.INT;
			case StatAttr.WIS:
				return allStats.WIS;
			case StatAttr.CHA:
				return allStats.CHA;
		}
		return new bool[9];
	}

	public static StatAttr GetBestFor(StatGen stats, StatRank rank) {
		List<Tuple<StatAttr, int>> t = new List<Tuple<StatAttr, int>>();
		t.Add(new Tuple<StatAttr, int>(StatAttr.STR, CalcForStat(stats, StatAttr.STR)));
		t.Add(new Tuple<StatAttr, int>(StatAttr.DEX, CalcForStat(stats, StatAttr.DEX)));
		t.Add(new Tuple<StatAttr, int>(StatAttr.CON, CalcForStat(stats, StatAttr.CON)));
		t.Add(new Tuple<StatAttr, int>(StatAttr.INT, CalcForStat(stats, StatAttr.INT)));
		t.Add(new Tuple<StatAttr, int>(StatAttr.WIS, CalcForStat(stats, StatAttr.WIS)));
		t.Add(new Tuple<StatAttr, int>(StatAttr.CHA, CalcForStat(stats, StatAttr.CHA)));

		t.Sort((x, y) => y.Item2.CompareTo(x.Item2));
		switch(rank) {
			case StatRank.KEY:
				return t[0].Item1;
			case StatRank.SECONDARY:
				return t[1].Item1;
			case StatRank.TERTIARY:
				return t[2].Item1;
			case StatRank.NICE:
				return t[2].Item1;
			case StatRank.DUMP:
				return t[3].Item1;
		}
		return t[0].Item1;
	}

	public static StatAttr GetBestFor(StatGen stats, StatRank rank, StatAttr not) {
		List<Tuple<StatAttr, int>> t = new List<Tuple<StatAttr, int>>();

		if(not != StatAttr.STR) t.Add(new Tuple<StatAttr, int>(StatAttr.STR, CalcForStat(stats, StatAttr.STR)));
		if(not != StatAttr.DEX) t.Add(new Tuple<StatAttr, int>(StatAttr.DEX, CalcForStat(stats, StatAttr.DEX)));
		if(not != StatAttr.CON) t.Add(new Tuple<StatAttr, int>(StatAttr.CON, CalcForStat(stats, StatAttr.CON)));
		if(not != StatAttr.INT) t.Add(new Tuple<StatAttr, int>(StatAttr.INT, CalcForStat(stats, StatAttr.INT)));
		if(not != StatAttr.WIS) t.Add(new Tuple<StatAttr, int>(StatAttr.WIS, CalcForStat(stats, StatAttr.WIS)));
		if(not != StatAttr.CHA) t.Add(new Tuple<StatAttr, int>(StatAttr.CHA, CalcForStat(stats, StatAttr.CHA)));
		
		t.Sort((x, y) => y.Item2.CompareTo(x.Item2));

		switch(rank) {
			case StatRank.KEY:
				return t[0].Item1;
			case StatRank.SECONDARY:
				return t[1].Item1;
			case StatRank.TERTIARY:
				return t[2].Item1;
			case StatRank.NICE:
				return t[2].Item1;
			case StatRank.DUMP:
				return t[3].Item1;
		}
		return t[0].Item1;
	}
}