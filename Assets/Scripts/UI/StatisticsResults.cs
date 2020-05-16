public class StatisticsResults {
	public float[] attack;
	public float attacktot;
	public float[] armorClass;
	public float armortot;
	public float[] classSpellDC;
	public float spellDCtot;
	public float[] perception;
	public float perceptiontot;
	public float[] fort;
	public float forttot;
	public float[] refx;
	public float refxtot;
	public float[] will;
	public float willtot;
	public float[] skillSpecialist;
	public float[] skillDecent;
	public float[] skillDabbler;
	public float totSkills;

	public StatisticsResults() {
		attack = new float[22];
		armorClass = new float[22];
		classSpellDC = new float[22];
		perception = new float[22];
		fort = new float[22];
		refx = new float[22];
		will = new float[22];
		skillSpecialist = new float[22];
		skillDecent = new float[22];
		skillDabbler = new float[22];
	}
}