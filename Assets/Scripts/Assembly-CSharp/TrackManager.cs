using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackManager : MonoBehaviour
{
	public Fog fog;

	public Transform m_TrackTransform;

	public ParticleSystem boostParticles;

	public TrackPartPooler trackPartPooler;

	public ObstaclePooler obstaclePooler;

	public CollectiblePooler collectiblePooler;

	public ConsumablePooler consumablePooler;

	protected MissionLevels[] m_MissionLevels;

	protected MissionLevels m_TutorialLevel;

	protected Level[] m_Levels;

	public Collectible[] collectiblePrefabs;

	public Consumable[] consumablePrefabs;

	public float maxSpeed;

	protected TrackPart[] m_TrackPrefabs;

	protected TrackPart[] m_SpecialTrackPrefabs;

	protected Obstacle[] m_ZombiePrefabs;

	protected Obstacle[] m_AirPrefabs;

	protected Obstacle[] m_GroundPrefabs;

	protected float m_DarkenAt;

	protected float m_TimePassedSinceSpeedIncreased;

	protected float m_Speed;

	protected float m_SumScaledSpeed;

	protected float m_TotalWorldDistance;

	protected List<TrackPart> m_TrackParts;

	protected int m_PreviousPart;

	protected int m_TrackSpawnCount;

	protected const int k_DesiredTrackPartsCount = 4;

	protected const float k_TrackPartRemovalDistance = -10f;

	protected const float k_LaneOffset = 1f;

	protected int m_CurrentLevelSpawned;

	protected int m_CurrentLevel;

	protected List<float> m_NextLevelStarts;

	protected float m_LevelLength;

	protected bool m_IsRunning;

	protected int m_ObstaclesCount;

	protected int m_NextCollectibleAt;

	protected int m_SumGewichtung;

	protected int m_SumGewichtungTrackParts;

	protected List<int> m_LastSpawnedTypes;

	protected int m_MaxListSpawnedTypesSize;

	protected List<int> m_LastLanesUsed;

	protected int m_MaxListLanesSize;

	protected float m_LastConsumableSpawnedAt;

	protected float m_NextConsumablePossibleAt;

	protected int m_NextZombieToUse;

	protected int m_PreviousConsumable;

	protected int m_PrevoiusCollectible;

	protected int m_PreviousZombieTutorial;

	protected const int k_OBSTACLE_ZOMBIE_1 = 0;

	protected const int k_OBSTACLE_ZOMBIE_2 = 1;

	protected const int k_OBSTACLE_ZOMBIE_3 = 2;

	protected const int k_OBSTACLE_HANDS = 3;

	protected const int k_OBSTACLE_RAVENS = 4;

	protected const int k_COLLECTIBLE = 5;

	protected const int k_CONSUMABLE = 6;

	protected const int k_OBSTACLE_TWO_ZOMBIES = 7;

	protected bool m_JumpSlide;

	protected float m_BoostSpeed;

	protected bool m_Boosting;

	protected float m_BoostStartAt;

	protected float m_BoostSlowDownAt;

	protected float m_BoostSlowedDownAt;

	protected float m_SpeedBeforeBoosting;

	protected float m_BoostDistanceFactor;

	protected float m_BoostSpeedFactor;

	protected bool m_UsingPowerUp;

	protected float m_PowerUpEndAt;

	protected bool m_ParticlesStopped;

	protected ConsumableType m_CurrentPowerUpType;

	protected bool m_Tutorial;

	protected int m_CurrentMissionLevel;

	protected int m_CurrentMissionLevelIndex;

	protected const int c_TRACK_PART_NORMAL = 0;

	protected const int c_TRACK_PART_SPECIAL = 1;

	protected int m_PreviousTrackType;

	protected int m_PreviousPartSpecial;

	protected bool m_TutorialRunningBeforePaused;

	public bool isRunning
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool jumpSlide
	{
		set
		{
		}
	}

	private void Awake()
	{
	}

	public void SetPrefabs(TrackPart[] trackPrefabs, TrackPart[] specialTrackParts, Obstacle[] zombiePrefabs, Obstacle[] airPrefabs, Obstacle[] groundPrefabs, MissionLevels[] missionLevels, MissionLevels tutorialLevel)
	{
	}

	public void SetMissionLevel(MissionLevels[] missionLevels)
	{
	}

	public void InitTheTrack()
	{
	}

	public void PrepareNewTrack()
	{
	}

	public void HideFog()
	{
	}

	public void Boost()
	{
	}

	public void UsePowerUp(ConsumableType consumableType)
	{
	}

	public void StopPowerUp()
	{
	}

	protected void LoadLevels()
	{
	}

	public void EndTutorial()
	{
	}

	public void TutorialStartNewGame()
	{
	}

	public void TutorialPaused(bool paused)
	{
	}

	public void StartTutorial()
	{
	}

	public void TutorialSpawnFairy(int lane, bool stopRunning)
	{
	}

	public void TutorialResumeRunning()
	{
	}

	public void TutorialSpawnZombie(int lane, bool stopRunning)
	{
	}

	public void TutorialSpawnHands(bool stopRunning)
	{
	}

	public void TutorialSpawnRavens(bool stopRunning)
	{
	}

	public void TutorialRevive()
	{
	}

	private IEnumerator StopRunning(float inDistance)
	{
		return null;
	}

	protected void PlaceElement(float inDistance, int laneToUse, int type)
	{
	}

	public void StartNewGame()
	{
	}

	public void StopRunning(bool pauseFog, bool slowDownFog)
	{
	}

	public void CleanUpTheme()
	{
	}

	public void CleanUp()
	{
	}

	public void UnPauseFog()
	{
	}

	public void ResumeRunning()
	{
	}

	public void ReviveGame()
	{
	}

	protected void RemoveSpecialTrackPart(int position)
	{
	}

	protected void RemoveSpecialTrackParts(int numberOfParts)
	{
	}

	protected void RemoveElements(int withinDistance)
	{
	}

	protected void RemoveAllElements()
	{
	}

	protected void RemoveConsumables(int withinDistance)
	{
	}

	private void Update()
	{
	}

	protected TrackPart GetTrackPartToUse()
	{
		return null;
	}

	protected TrackPart GetNormalTrackPart()
	{
		return null;
	}

	protected TrackPart GetSpecialTrackPart()
	{
		return null;
	}

	public void SpawnNewTrackPart()
	{
	}

	protected void InitLevel()
	{
	}

	private void SpawnElements(TrackPart trackPart)
	{
	}

	private void SpawnSpecialElements(TrackPart trackPart)
	{
	}

	private void SpawnNormalElements(TrackPart trackPart)
	{
	}

	protected void SpawnConsumableElement(Transform parent, int laneToUse, int zPosition)
	{
	}

	protected void SpawnCollectibleElement(Transform parent, int laneToUse, int zPosition)
	{
	}

	protected void SpawnObstacleElement(Obstacle obstacleToUse, Transform parent, int laneToUse, int zPosition)
	{
	}

	protected int GetElementZPosition(int elementsOnPart, int elementNumber)
	{
		return 0;
	}

	protected int GetLaneToUse(int typeOfElement)
	{
		return 0;
	}

	protected int GetSpawnElementType(Transform trackPart, int zPosition)
	{
		return 0;
	}
}
