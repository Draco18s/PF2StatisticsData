﻿using System;

[Flags]
public enum SaveIncrease {
	NONE = 0,
	CRIT_FAIL_IS_FAIL = 1<<0,
	FAIL_IS_SUCCESS = 1<<1,
	SUCCESS_IS_CRIT_SUCCESS = 1<<2,
	REROLL_FAILURE_PLUS_2 = 1<<3,
	REROLL_CRITICAL_FAILURE_PLUS_2 = 1<<4,
	CRIT_ON_19 = 1<<5,
	MINIMUM_10 = 1<<6,
	DISADVANTAGE = 1<<7,
	REROLL_FAILURE = 1 << 8,
	REROLL_CRITICAL_FAILURE = 1 << 9,
	HAZARD_FINDER = 1<<10,
	MONSTER_HUNTER = 1 << 11,
	MASTER_MONSTER_HUNTER = 1 << 12,
	LEGENDARY_MONSTER_HUNTER = 1 << 13,
}