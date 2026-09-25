using UnityEngine;

public class Level : MonoBehaviour
{
	public int trackPartsTillNextLevel = 1;

	public int gewichtungNormalParts = 1;

	public int gewichtungSpecialParts;

	public bool specialPartsAfterEachOther;

	public int minElementsPerPart = 1;

	public int maxElementsPerPart = 3;

	public int gewichtungZombies = 6;

	public int gewichtungRaben = 2;

	public int gewichtungHaende = 1;

	public int gewichtungGhosts = 4;

	public int gewichtungPowerUps = 1;

	public int maxZombiesAfterEachOther;

	public int maxHaendeAfterEachOther;

	public int maxRabenAfterEachOther;

	public int maxGhostsAfterEachOther;

	public int maxOnSameLaneAfterEachOther = 1;

	public bool rabenHaendeAfterEachOther;

	public bool twoZombiesOnRow;

	public float twoZombiesOnRowProbability = 0.1f;
}
