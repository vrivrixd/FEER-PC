using UnityEngine;

public class Level : MonoBehaviour
{
	public int trackPartsTillNextLevel;

	public int gewichtungNormalParts;

	public int gewichtungSpecialParts;

	public bool specialPartsAfterEachOther;

	public int minElementsPerPart;

	public int maxElementsPerPart;

	public int gewichtungZombies;

	public int gewichtungRaben;

	public int gewichtungHaende;

	public int gewichtungGhosts;

	public int gewichtungPowerUps;

	public int maxZombiesAfterEachOther;

	public int maxHaendeAfterEachOther;

	public int maxRabenAfterEachOther;

	public int maxGhostsAfterEachOther;

	public int maxOnSameLaneAfterEachOther;

	public bool rabenHaendeAfterEachOther;

	public bool twoZombiesOnRow;

	public float twoZombiesOnRowProbability;
}
