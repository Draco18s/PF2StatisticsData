public class StatisticsResults {
	public float[] attack;
	public int attacktot;
	public float[] armorClass;
	public int armortot;
	public float[] classSpellDC;
	public int spellDCtot;
	public float[] perception;
	public int perceptiontot;
	public float[] fort;
	public int forttot;
	public float[] refx;
	public int refxtot;
	public float[] will;
	public int willtot;
	public float[] skillSpecialist;
	public float[] skillDecent;
	public float[] skillDabbler;
	public int totSkills;

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