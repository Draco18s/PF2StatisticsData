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
		attack = new float[20];
		armorClass = new float[20];
		classSpellDC = new float[20];
		perception = new float[20];
		fort = new float[20];
		refx = new float[20];
		will = new float[20];
		skillSpecialist = new float[20];
		skillDecent = new float[20];
		skillDabbler = new float[20];
	}
}