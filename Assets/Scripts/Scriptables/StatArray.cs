using System;
using UnityEngine;

//[CreateAssetMenu(fileName = "StatArray", menuName = "Pathfinder2/StatArray", order = 1)]
[Serializable]
public class StatArray {
	public StatRank STR;
	public StatRank DEX;
	public StatRank CON;
	public StatRank INT;
	public StatRank WIS;
	public StatRank CHA;

	public StatRank GetRankFor(StatAttr stat) {
		switch(stat) {
			case StatAttr.STR:
				return STR;
			case StatAttr.DEX:
				return DEX;
			case StatAttr.CON:
				return CON;
			case StatAttr.INT:
				return INT;
			case StatAttr.WIS:
				return WIS;
			case StatAttr.CHA:
				return CHA;
		}
		return StatRank.DUMP;
	}

	public static int GetValue(StatRank rank, int lvl) {
		//TODO: items by level
		if(lvl < 5) {
			switch(rank) {
				case StatRank.DUMP:
					return 0;
				case StatRank.NICE:
					return 1;
				case StatRank.TERTIARY:
					return 1;
				case StatRank.SECONDARY:
					return 3;
				case StatRank.KEY:
					return 4;
			}
		}
		int boosts = lvl / 5;
		switch(rank) {
			case StatRank.DUMP:
				return 0;
			case StatRank.NICE:
				return 1 + (boosts / 2);
			case StatRank.TERTIARY:
				return 1 + boosts;
			case StatRank.SECONDARY:
				return 4 + ((boosts-1) / 2);
			case StatRank.KEY:
				return 4 + (boosts/2);
		}
		return 0;
	}
}