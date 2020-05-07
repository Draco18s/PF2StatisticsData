using System;

[Flags]
public enum AffectType {
	NONE=0,
	FORT=1<<0,
	REFX=1<<1,
	WILL=1<<2
}