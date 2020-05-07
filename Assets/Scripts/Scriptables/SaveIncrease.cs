using System;

[Flags]
public enum SaveIncrease {
	NONE = 0,
	CRIT_FAIL_IS_FAIL = 1<<0,
	FAIL_IS_SUCCESS = 1<<1,
	SUCCESS_IS_CRIT_SUCCESS = 1<<2,
}