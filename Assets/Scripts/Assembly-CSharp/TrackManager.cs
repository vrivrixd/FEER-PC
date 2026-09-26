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

	public float maxSpeed = 10f;

	protected TrackPart[] m_TrackPrefabs;

	protected TrackPart[] m_SpecialTrackPrefabs;

	protected Obstacle[] m_ZombiePrefabs;

	protected Obstacle[] m_AirPrefabs;

	protected Obstacle[] m_GroundPrefabs;

	protected float m_DarkenAt;

	protected float m_TimePassedSinceSpeedIncreased;

	protected float m_Speed = 5f;

	protected float m_SumScaledSpeed;

	protected float m_TotalWorldDistance;

	protected List<TrackPart> m_TrackParts = new List<TrackPart>();

	protected int m_PreviousPart = -1;

	protected int m_TrackSpawnCount;

	protected const int k_DesiredTrackPartsCount = 4;

	protected const float k_TrackPartRemovalDistance = -10f;

	protected const float k_LaneOffset = 1f;

	protected int m_CurrentLevelSpawned;

	protected int m_CurrentLevel;

	protected List<float> m_NextLevelStarts = new List<float>();

	protected float m_LevelLength;

	protected bool m_IsRunning;

	protected int m_ObstaclesCount;

	protected int m_NextCollectibleAt = -1;

	protected int m_SumGewichtung = -1;

	protected int m_SumGewichtungTrackParts = -1;

	protected List<int> m_LastSpawnedTypes = new List<int>();

	protected int m_MaxListSpawnedTypesSize = -1;

	protected List<int> m_LastLanesUsed = new List<int>();

	protected int m_MaxListLanesSize = -1;

	protected float m_LastConsumableSpawnedAt;

	protected float m_NextConsumablePossibleAt;

	protected int m_NextZombieToUse;

	protected int m_PreviousConsumable = -1;

	protected int m_PrevoiusCollectible = -1;

	protected int m_PreviousZombieTutorial = -1;

	protected const int k_OBSTACLE_ZOMBIE_1 = 0;

	protected const int k_OBSTACLE_ZOMBIE_2 = 1;

	protected const int k_OBSTACLE_ZOMBIE_3 = 2;

	protected const int k_OBSTACLE_HANDS = 3;

	protected const int k_OBSTACLE_RAVENS = 4;

	protected const int k_COLLECTIBLE = 5;

	protected const int k_CONSUMABLE = 6;

	protected const int k_OBSTACLE_TWO_ZOMBIES = 7;

	protected bool m_JumpSlide;

	protected float m_BoostSpeed = 20f;

	protected bool m_Boosting;

	protected float m_BoostStartAt;

	protected float m_BoostSlowDownAt;

	protected float m_BoostSlowedDownAt;

	protected float m_SpeedBeforeBoosting;

	protected float m_BoostDistanceFactor = 10f;

	protected float m_BoostSpeedFactor;

	protected bool m_UsingPowerUp;

	protected float m_PowerUpEndAt;

	protected bool m_ParticlesStopped;

	protected ConsumableType m_CurrentPowerUpType;

	protected bool m_Tutorial;

	protected int m_CurrentMissionLevel = -1;

	protected int m_CurrentMissionLevelIndex = -1;

	protected const int c_TRACK_PART_NORMAL = 0;

	protected const int c_TRACK_PART_SPECIAL = 1;

	protected int m_PreviousTrackType = -1;

	protected int m_PreviousPartSpecial = -1;

	protected bool m_TutorialRunningBeforePaused;

	public bool isRunning
	{
		get
		{
			return m_IsRunning;
		}
		set
		{
			m_IsRunning = value;
		}
	}

	public bool jumpSlide
	{
		set
		{
			m_JumpSlide = value;
		}
	}

	private void Awake()
	{
		trackPartPooler.ResetPools();
		obstaclePooler.ResetPools();
		collectiblePooler.ResetPools();
		consumablePooler.ResetPools();
		boostParticles.Stop();
		boostParticles.Clear();
	}

	public void SetPrefabs(TrackPart[] trackPrefabs, TrackPart[] specialTrackParts, Obstacle[] zombiePrefabs, Obstacle[] airPrefabs, Obstacle[] groundPrefabs, MissionLevels[] missionLevels, MissionLevels tutorialLevel)
	{
		m_TrackPrefabs = trackPrefabs;
		m_SpecialTrackPrefabs = specialTrackParts;
		m_ZombiePrefabs = zombiePrefabs;
		m_AirPrefabs = airPrefabs;
		m_GroundPrefabs = groundPrefabs;
		m_CurrentMissionLevel = -1;
		m_MissionLevels = missionLevels;
		m_TutorialLevel = tutorialLevel;
	}

	public void SetMissionLevel(MissionLevels[] missionLevels)
	{
		m_MissionLevels = missionLevels;
		m_CurrentMissionLevel = -1;
	}

	public void InitTheTrack()
	{
		PrepareNewTrack();
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.TrackManagerInitFinished, null);
	}

	private void ResetSpawnState()
	{
		m_NextLevelStarts = new List<float>();
		m_LevelLength = 0f;
		m_SumGewichtung = -1;
		m_SumGewichtungTrackParts = -1;
		m_ObstaclesCount = 0;
		m_NextCollectibleAt = -1;
		m_LastSpawnedTypes = new List<int>();
		m_MaxListSpawnedTypesSize = -1;
		m_LastLanesUsed = new List<int>();
		m_MaxListLanesSize = -1;
		m_NextConsumablePossibleAt = 0f;
		m_LastConsumableSpawnedAt = 0f;
		m_UsingPowerUp = false;
		m_Boosting = false;
	}

	public void PrepareNewTrack()
	{
		LoadLevels();
		if (m_Boosting)
		{
			boostParticles.Stop();
			boostParticles.Clear();
		}
		m_CurrentPowerUpType = ConsumableType.None;
		fog.ResetFog();
		m_SumScaledSpeed = 0f;
		m_TimePassedSinceSpeedIncreased = 0f;
		MissionLevels missionLevels = m_Tutorial ? m_TutorialLevel : m_MissionLevels[m_CurrentMissionLevelIndex];
		m_Speed = missionLevels.startSpeed;
		CustomGameManager.Instance.SetSpeed(m_Speed);
		m_TotalWorldDistance = 0f;
		CustomGameManager.Instance.worldDistance = m_TotalWorldDistance;
		m_PreviousTrackType = -1;
		m_PreviousPartSpecial = -1;
		m_PreviousConsumable = -1;
		m_PrevoiusCollectible = -1;
		m_PreviousZombieTutorial = -1;
		m_CurrentLevelSpawned = 0;
		m_CurrentLevel = 0;
		m_PreviousPart = -1;
		m_TrackSpawnCount = 0;
		m_NextLevelStarts = new List<float>();
		m_LevelLength = 0f;
		m_TrackParts = new List<TrackPart>();
		ResetSpawnState();
		InitLevel();
		m_IsRunning = false;
		while (m_TrackParts.Count <= 3)
		{
			SpawnNewTrackPart();
		}
	}

	public void HideFog()
	{
		fog.HideFog();
	}

	public void Boost()
	{
		m_BoostStartAt = m_TotalWorldDistance;
		m_SpeedBeforeBoosting = m_Speed;
		m_BoostSlowDownAt = m_TotalWorldDistance + (float)DataManager.Instance.playerData.boostDistance;
		m_BoostSpeedFactor = m_BoostSpeed - m_Speed;
		m_BoostSlowedDownAt = m_BoostSlowDownAt + m_BoostDistanceFactor;
		m_Boosting = true;
		fog.UnpauseFog(true);
		boostParticles.Clear();
		boostParticles.Play();
		m_ParticlesStopped = false;
		fog.ColorizeFog(ConsumableType.Boost);
		m_CurrentPowerUpType = ConsumableType.Boost;
		m_IsRunning = true;
	}

	public void UsePowerUp(ConsumableType consumableType)
	{
		float distance = 0f;
		switch (consumableType)
		{
		case ConsumableType.Weapon:
			m_CurrentPowerUpType = ConsumableType.Weapon;
			distance = DataManager.Instance.playerData.weaponDistance;
			break;
		case ConsumableType.Shield:
			m_CurrentPowerUpType = ConsumableType.Shield;
			distance = DataManager.Instance.playerData.shieldDistance;
			break;
		case ConsumableType.CoinMultiplier:
			m_CurrentPowerUpType = ConsumableType.CoinMultiplier;
			distance = DataManager.Instance.playerData.coinMultiplierDistance;
			break;
		}
		m_UsingPowerUp = true;
		m_PowerUpEndAt = m_TotalWorldDistance + distance;
		fog.UnpauseFog(true);
		m_IsRunning = true;
	}

	public void StopPowerUp()
	{
		m_UsingPowerUp = false;
	}

	protected void LoadLevels()
	{
		int currentMissionLevel = m_CurrentMissionLevel;
		int missionSetLevel = MissionManager.Instance.currentMissionSet + 1;
		if (!m_Tutorial && currentMissionLevel != -1 && missionSetLevel <= currentMissionLevel)
		{
			m_CurrentMissionLevel = missionSetLevel;
			return;
		}
		if (currentMissionLevel == -1 && DataManager.Instance.playerData.playTutorial)
		{
			m_Tutorial = true;
			m_Levels = m_TutorialLevel.levels;
			return;
		}
		for (int i = 0; i < m_MissionLevels.Length && m_MissionLevels[i].missionLevel <= missionSetLevel; i++)
		{
			m_CurrentMissionLevelIndex = i;
		}
		m_Levels = m_MissionLevels[m_CurrentMissionLevelIndex].levels;
		if (!m_Tutorial)
		{
			m_CurrentMissionLevel = missionSetLevel;
		}
	}

	public void EndTutorial()
	{
		m_Tutorial = false;
		m_CurrentMissionLevel = -1;
	}

	public void TutorialStartNewGame()
	{
		RemoveAllElements();
		m_Tutorial = false;
		LoadLevels();
		if (m_Boosting)
		{
			boostParticles.Stop();
			boostParticles.Clear();
		}
		m_CurrentPowerUpType = ConsumableType.None;
		m_SumScaledSpeed = 0f;
		m_DarkenAt = CustomGameManager.Instance.metersTillDarken;
		m_Speed = m_MissionLevels[m_CurrentMissionLevelIndex].startSpeed;
		CustomGameManager.Instance.SetSpeed(m_Speed);
		m_TotalWorldDistance = 0f;
		CustomGameManager.Instance.worldDistance = m_TotalWorldDistance;
		m_TrackSpawnCount = 0;
		m_CurrentLevelSpawned = 0;
		m_CurrentLevel = 1;
		m_IsRunning = false;
		ResetSpawnState();
		InitLevel();
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			m_TrackSpawnCount++;
			m_LevelLength += m_TrackParts[i].trackLength;
			if (m_TrackParts[i].exitPoint.position.z <= 20f)
			{
				m_TrackSpawnCount = 0;
				m_LevelLength = 0f;
				continue;
			}
			SpawnElements(m_TrackParts[i]);
			if (m_CurrentLevelSpawned < m_Levels.Length - 1 && m_TrackSpawnCount == m_Levels[m_CurrentLevelSpawned].trackPartsTillNextLevel)
			{
				m_CurrentLevelSpawned++;
				float start = (m_NextLevelStarts.Count < 1) ? m_TotalWorldDistance : m_NextLevelStarts[m_NextLevelStarts.Count - 1];
				m_NextLevelStarts.Add(start + m_LevelLength);
				m_LevelLength = 0f;
				m_TrackSpawnCount = 0;
				InitLevel();
			}
		}
	}

	public void TutorialPaused(bool paused)
	{
		if (paused)
		{
			m_TutorialRunningBeforePaused = m_IsRunning;
			m_IsRunning = false;
			fog.PauseFog();
		}
		else
		{
			m_IsRunning = m_TutorialRunningBeforePaused;
			fog.UnpauseFog(false);
		}
	}

	public void StartTutorial()
	{
		m_Speed = 4f;
		CustomGameManager.Instance.SetSpeed(m_Speed);
		m_Tutorial = true;
		if (m_CurrentMissionLevel != -1)
		{
			CleanUp();
		}
		fog.UnpauseFog(true);
		m_IsRunning = true;
	}

	public void TutorialSpawnFairy(int lane, bool stopRunning)
	{
		PlaceElement(17f, lane, 1);
		if (stopRunning)
		{
			StartCoroutine(StopRunning(8f));
		}
	}

	public void TutorialResumeRunning()
	{
		m_IsRunning = true;
	}

	public void TutorialSpawnZombie(int lane, bool stopRunning)
	{
		PlaceElement(17f, lane, 2);
		if (stopRunning)
		{
			StartCoroutine(StopRunning(8f));
		}
	}

	public void TutorialSpawnHands(bool stopRunning)
	{
		PlaceElement(23f, 0, 3);
		if (stopRunning)
		{
			StartCoroutine(StopRunning(8f));
		}
	}

	public void TutorialSpawnRavens(bool stopRunning)
	{
		PlaceElement(23f, 0, 4);
		if (stopRunning)
		{
			StartCoroutine(StopRunning(8f));
		}
	}

	public void TutorialRevive()
	{
		fog.SpeedUpFog();
		RemoveAllElements();
		m_IsRunning = true;
	}

	private IEnumerator StopRunning(float inDistance)
	{
		float endPoint = m_TotalWorldDistance + inDistance;
		while (m_TotalWorldDistance < endPoint)
		{
			yield return null;
		}
		m_IsRunning = false;
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.TutorialStoppedRunning, null);
	}

	protected internal void PlaceElement(float inDistance, int laneToUse, int type)
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			if (!(inDistance < m_TrackParts[i].exitPoint.position.z))
			{
				continue;
			}
			int zPosition;
			if (m_TrackParts[i].entryPoint.position.z >= 0f)
			{
				zPosition = Mathf.FloorToInt(inDistance) - Mathf.FloorToInt(m_TrackParts[i].entryPoint.position.z);
			}
			else
			{
				zPosition = Mathf.FloorToInt(0f - m_TrackParts[i].entryPoint.position.z) + Mathf.FloorToInt(inDistance);
			}
			switch (type)
			{
			case 1:
				SpawnCollectibleElement(m_TrackParts[i].collectiblesTransform, laneToUse, zPosition);
				break;
			case 2:
			{
				int zombie = Random.Range(0, m_ZombiePrefabs.Length);
				if (m_PreviousZombieTutorial == zombie)
				{
					zombie = (zombie + 1) % m_ZombiePrefabs.Length;
				}
				m_PreviousZombieTutorial = zombie;
				SpawnObstacleElement(m_ZombiePrefabs[zombie], m_TrackParts[i].obstaclesTransform, laneToUse, zPosition);
				break;
			}
			case 3:
				SpawnObstacleElement(m_GroundPrefabs[0], m_TrackParts[i].obstaclesTransform, laneToUse, zPosition);
				break;
			case 4:
				SpawnObstacleElement(m_AirPrefabs[0], m_TrackParts[i].obstaclesTransform, laneToUse, zPosition);
				break;
			}
			return;
		}
	}

	public void StartNewGame()
	{
		m_Tutorial = false;
		m_DarkenAt = CustomGameManager.Instance.metersTillDarken;
		fog.UnpauseFog(true);
		m_IsRunning = true;
	}

	public void StopRunning(bool pauseFog, bool slowDownFog)
	{
		m_IsRunning = false;
		if (pauseFog)
		{
			fog.PauseFog();
		}
		else if (slowDownFog)
		{
			fog.SlowDownFog();
		}
		if (m_Boosting)
		{
			boostParticles.Pause();
		}
	}

	public void CleanUpTheme()
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			m_TrackParts[i].CleanUp();
		}
		trackPartPooler.ClearPools();
		obstaclePooler.ClearPools();
	}

	public void CleanUp()
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			m_TrackParts[i].CleanUp();
		}
		PrepareNewTrack();
	}

	public void UnPauseFog()
	{
		fog.UnpauseFog(true);
	}

	public void ResumeRunning()
	{
		fog.UnpauseFog(true);
		if (m_Boosting)
		{
			boostParticles.Play();
		}
		m_IsRunning = true;
	}

	public void ReviveGame()
	{
		RemoveSpecialTrackParts(2);
		RemoveElements(32);
		ResumeRunning();
	}

	protected void RemoveSpecialTrackPart(int position)
	{
		if (position >= m_TrackParts.Count || m_TrackParts[position].trackPartType != TrackPartType.Bridge)
		{
			return;
		}
		float offset = 0f;
		for (int i = position + 1; i < m_TrackParts.Count; i++)
		{
			if (i == position + 1)
			{
				offset = 0f - m_TrackParts[position + 1].entryPoint.position.z;
			}
			m_TrackParts[i].TranslatePart(new Vector3(0f, 0f, offset));
		}
		m_TrackParts[position].CleanUp();
		m_TrackParts.RemoveAt(position);
	}

	protected void RemoveSpecialTrackParts(int numberOfParts)
	{
		for (int position = numberOfParts - 1; position >= 0; position--)
		{
			RemoveSpecialTrackPart(position);
		}
		while (m_TrackParts.Count <= 3)
		{
			SpawnNewTrackPart();
		}
	}

	protected void RemoveElements(int withinDistance)
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			if (!(0f < m_TrackParts[i].exitPoint.position.z))
			{
				continue;
			}
			bool allRemoved = true;
			Obstacle[] obstacles = m_TrackParts[i].obstaclesTransform.GetComponentsInChildren<Obstacle>();
			for (int j = 0; j < obstacles.Length; j++)
			{
				if ((float)withinDistance <= obstacles[j].transform.position.z)
				{
					allRemoved = false;
				}
				else
				{
					obstacles[j].FreeObstacle();
				}
			}
			Collectible[] collectibles = m_TrackParts[i].collectiblesTransform.GetComponentsInChildren<Collectible>();
			for (int k = 0; k < collectibles.Length; k++)
			{
				if ((float)withinDistance <= collectibles[k].transform.position.z)
				{
					allRemoved = false;
				}
				else
				{
					collectibles[k].FreeCollectible();
				}
			}
			Consumable[] consumables = m_TrackParts[i].consumablesTransform.GetComponentsInChildren<Consumable>();
			for (int l = 0; l < consumables.Length; l++)
			{
				if ((float)withinDistance <= consumables[l].transform.position.z)
				{
					allRemoved = false;
				}
				else
				{
					consumables[l].FreeConsumable();
				}
			}
			if (!allRemoved)
			{
				break;
			}
		}
	}

	protected void RemoveAllElements()
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			Obstacle[] obstacles = m_TrackParts[i].obstaclesTransform.GetComponentsInChildren<Obstacle>();
			for (int j = 0; j < obstacles.Length; j++)
			{
				obstacles[j].FreeObstacle();
			}
			Collectible[] collectibles = m_TrackParts[i].collectiblesTransform.GetComponentsInChildren<Collectible>();
			for (int k = 0; k < collectibles.Length; k++)
			{
				collectibles[k].FreeCollectible();
			}
			Consumable[] consumables = m_TrackParts[i].consumablesTransform.GetComponentsInChildren<Consumable>();
			for (int l = 0; l < consumables.Length; l++)
			{
				consumables[l].FreeConsumable();
			}
		}
	}

	protected void RemoveConsumables(int withinDistance)
	{
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			if (!(0f < m_TrackParts[i].exitPoint.position.z))
			{
				continue;
			}
			Consumable[] consumables = m_TrackParts[i].consumablesTransform.GetComponentsInChildren<Consumable>();
			if (consumables.Length <= 0)
			{
				continue;
			}
			bool allRemoved = true;
			for (int j = 0; j < consumables.Length; j++)
			{
				if ((float)withinDistance <= consumables[j].transform.position.z)
				{
					allRemoved = false;
				}
				else
				{
					consumables[j].FreeConsumable();
				}
			}
			if (!allRemoved)
			{
				break;
			}
		}
	}

	private void Update()
	{
		if (FeerSceneManager.Instance.debugMode && FeerSceneManager.Instance.stopTrack)
		{
			return;
		}
		if (!m_IsRunning)
		{
			return;
		}
		float distance = m_Speed * Time.deltaTime;
		m_TotalWorldDistance += distance;
		CustomGameManager.Instance.worldDistance = m_TotalWorldDistance;
		m_SumScaledSpeed += distance;
		if (m_SumScaledSpeed >= 1f)
		{
			int scoreToAdd = 0;
			while (m_SumScaledSpeed >= 1f)
			{
				m_SumScaledSpeed -= 1f;
				scoreToAdd++;
			}
			if (scoreToAdd > 0)
			{
				CustomGameManager.Instance.IncreaseScore(scoreToAdd);
			}
		}
		if (!m_Boosting)
		{
			if (m_UsingPowerUp && m_PowerUpEndAt <= m_TotalWorldDistance)
			{
				m_UsingPowerUp = false;
				CustomGameManager.Instance.StopPowerUp();
			}
			if (!m_Tutorial)
			{
				bool skipSpeedUp = false;
				if (m_DarkenAt <= m_TotalWorldDistance)
				{
					m_DarkenAt = m_TotalWorldDistance + (float)CustomGameManager.Instance.metersTillDarken;
					CustomGameManager.Instance.DarkenTheScreen();
					skipSpeedUp = m_Tutorial;
				}
				if (!skipSpeedUp && m_Speed < maxSpeed)
				{
					m_TimePassedSinceSpeedIncreased += Time.deltaTime;
					MissionLevels missionLevels = m_MissionLevels[m_CurrentMissionLevelIndex];
					if ((float)missionLevels.secondsTillSpeedUp <= m_TimePassedSinceSpeedIncreased)
					{
						m_Speed += missionLevels.speedStep;
						CustomGameManager.Instance.SetSpeed(m_Speed);
						m_TimePassedSinceSpeedIncreased = 0f;
					}
				}
			}
		}
		else
		{
			if (m_BoostSlowDownAt - 10f <= m_TotalWorldDistance && !m_ParticlesStopped)
			{
				boostParticles.Stop();
				fog.ColorizeFog(ConsumableType.None);
				m_ParticlesStopped = true;
			}
			if (m_Speed < m_BoostSpeed && m_TotalWorldDistance < m_BoostSlowDownAt)
			{
				m_Speed = m_SpeedBeforeBoosting + (m_TotalWorldDistance - m_BoostStartAt) / m_BoostDistanceFactor * m_BoostSpeedFactor;
				CustomGameManager.Instance.SetSpeed(m_Speed);
			}
			else if (m_BoostSlowDownAt <= m_TotalWorldDistance)
			{
				if (m_SpeedBeforeBoosting < m_Speed && m_TotalWorldDistance < m_BoostSlowedDownAt)
				{
					m_Speed = m_BoostSpeed - (m_TotalWorldDistance - m_BoostSlowDownAt) / m_BoostDistanceFactor * m_BoostSpeedFactor;
					CustomGameManager.Instance.SetSpeed(m_Speed);
				}
				else if (m_BoostSlowedDownAt <= m_TotalWorldDistance)
				{
					TrackPart trackPart = (m_TrackParts[0].exitPoint.position.z >= 0f) ? m_TrackParts[0] : m_TrackParts[1];
					if (trackPart.trackPartType != TrackPartType.Bridge)
					{
						m_Boosting = false;
						m_Speed = m_SpeedBeforeBoosting;
						CustomGameManager.Instance.SetSpeed(m_Speed);
						m_DarkenAt = m_DarkenAt - m_BoostStartAt + m_TotalWorldDistance;
						CustomGameManager.Instance.StopPowerUp();
					}
				}
			}
		}
		if (!m_Tutorial && m_NextLevelStarts.Count > 0 && m_NextLevelStarts[0] <= m_TotalWorldDistance)
		{
			m_CurrentLevel++;
			m_NextLevelStarts.RemoveAt(0);
		}
		while (m_TrackParts.Count <= 3)
		{
			SpawnNewTrackPart();
		}
		for (int i = 0; i < m_TrackParts.Count; i++)
		{
			m_TrackParts[i].TranslatePart(new Vector3(0f, 0f, 0f - distance));
		}
		if (!(-10f < m_TrackParts[0].exitPoint.position.z))
		{
			m_TrackParts[0].CleanUp();
			m_TrackParts.RemoveAt(0);
		}
	}

	protected TrackPart GetTrackPartToUse()
	{
		if (m_SpecialTrackPrefabs.Length == 0 || m_Tutorial)
		{
			return GetNormalTrackPart();
		}
		if (m_PreviousTrackType == c_TRACK_PART_SPECIAL && !m_Levels[m_CurrentLevelSpawned].specialPartsAfterEachOther)
		{
			return GetNormalTrackPart();
		}
		int random = Random.Range(0, m_SumGewichtungTrackParts);
		if (m_Levels[m_CurrentLevelSpawned].gewichtungNormalParts <= random)
		{
			return GetSpecialTrackPart();
		}
		return GetNormalTrackPart();
	}

	protected TrackPart GetNormalTrackPart()
	{
		int part = Random.Range(0, m_TrackPrefabs.Length);
		if (part == m_PreviousPart)
		{
			part = (part + 1) % m_TrackPrefabs.Length;
		}
		m_PreviousPart = part;
		m_PreviousTrackType = c_TRACK_PART_NORMAL;
		return m_TrackPrefabs[part];
	}

	protected TrackPart GetSpecialTrackPart()
	{
		int part = Random.Range(0, m_SpecialTrackPrefabs.Length);
		if (part == m_PreviousPartSpecial)
		{
			part = (part + 1) % m_SpecialTrackPrefabs.Length;
		}
		m_PreviousPartSpecial = part;
		m_PreviousTrackType = c_TRACK_PART_SPECIAL;
		return m_SpecialTrackPrefabs[part];
	}

	public void SpawnNewTrackPart()
	{
		TrackPart prefab = GetTrackPartToUse();
		TrackPartPool pool = trackPartPooler.GetPool(prefab);
		TrackPart trackPart = pool.Get(Vector3.zero, Quaternion.identity);
		Vector3 position;
		Quaternion rotation;
		if (m_TrackParts.Count < 1)
		{
			position = transform.position;
			rotation = transform.rotation;
		}
		else
		{
			Transform exitPoint = m_TrackParts[m_TrackParts.Count - 1].exitPoint;
			position = exitPoint.position;
			rotation = exitPoint.rotation;
		}
		trackPart.transform.rotation = rotation;
		Vector3 entryPosition = trackPart.entryPoint.position;
		trackPart.transform.position = position + (trackPart.transform.position - entryPosition);
		trackPart.pool = pool;
		trackPart.transform.SetParent(m_TrackTransform, true);
		if (!m_Tutorial)
		{
			SpawnElements(trackPart);
		}
		m_TrackParts.Add(trackPart);
		m_TrackSpawnCount++;
		m_LevelLength += trackPart.trackLength;
		if (!m_Tutorial && m_CurrentLevelSpawned < m_Levels.Length - 1 && m_TrackSpawnCount == m_Levels[m_CurrentLevelSpawned].trackPartsTillNextLevel)
		{
			m_CurrentLevelSpawned++;
			float start = (m_NextLevelStarts.Count < 1) ? m_TotalWorldDistance : m_NextLevelStarts[m_NextLevelStarts.Count - 1];
			m_NextLevelStarts.Add(start + m_LevelLength);
			m_LevelLength = 0f;
			m_TrackSpawnCount = 0;
			InitLevel();
		}
	}

	protected void InitLevel()
	{
		Level level = m_Levels[m_CurrentLevelSpawned];
		m_SumGewichtung = level.gewichtungHaende + level.gewichtungRaben + level.gewichtungZombies + level.gewichtungGhosts + level.gewichtungPowerUps;
		if (m_SpecialTrackPrefabs.Length != 0)
		{
			m_SumGewichtungTrackParts = level.gewichtungSpecialParts + level.gewichtungNormalParts;
		}
		int maxAfterEachOther = level.maxZombiesAfterEachOther;
		if (maxAfterEachOther < level.maxRabenAfterEachOther)
		{
			maxAfterEachOther = level.maxRabenAfterEachOther;
		}
		if (maxAfterEachOther < level.maxHaendeAfterEachOther)
		{
			maxAfterEachOther = level.maxHaendeAfterEachOther;
		}
		if (maxAfterEachOther < level.maxGhostsAfterEachOther)
		{
			maxAfterEachOther = level.maxGhostsAfterEachOther;
		}
		m_MaxListSpawnedTypesSize = maxAfterEachOther + 1;
		m_MaxListLanesSize = level.maxOnSameLaneAfterEachOther + 1;
	}

	private void SpawnElements(TrackPart trackPart)
	{
		if (m_PreviousTrackType != c_TRACK_PART_SPECIAL)
		{
			SpawnNormalElements(trackPart);
		}
		else
		{
			trackPart.SpawnElements();
		}
	}

	private void SpawnSpecialElements(TrackPart trackPart)
	{
		trackPart.SpawnElements();
	}

	private void SpawnNormalElements(TrackPart trackPart)
	{
		Level level = m_Levels[m_CurrentLevelSpawned];
		int elementsOnPart = Random.Range(level.minElementsPerPart, level.maxElementsPerPart + 1);
		for (int elementNumber = 0; elementNumber < elementsOnPart; elementNumber++)
		{
			int zPosition = GetElementZPosition(elementsOnPart, elementNumber);
			int typeOfElement;
			do
			{
				typeOfElement = GetSpawnElementType(trackPart.transform, zPosition);
			}
			while (typeOfElement == -1);
			int laneToUse = GetLaneToUse(typeOfElement);
			switch (typeOfElement)
			{
			case k_OBSTACLE_ZOMBIE_1:
			case k_OBSTACLE_ZOMBIE_2:
			case k_OBSTACLE_ZOMBIE_3:
				SpawnObstacleElement(m_ZombiePrefabs[typeOfElement], trackPart.obstaclesTransform, laneToUse, zPosition);
				if (laneToUse != 0)
				{
					Level currentLevel = m_Levels[m_CurrentLevelSpawned];
					if (currentLevel.twoZombiesOnRow && (m_LastSpawnedTypes.Count < 1 || m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1] != k_OBSTACLE_TWO_ZOMBIES) && Random.value < m_Levels[m_CurrentLevelSpawned].twoZombiesOnRowProbability)
					{
						int secondZombie = Random.Range(0, 3);
						if (secondZombie == typeOfElement)
						{
							secondZombie = (typeOfElement + 1) % 3;
						}
						SpawnObstacleElement(m_ZombiePrefabs[secondZombie], trackPart.obstaclesTransform, -laneToUse, zPosition);
						if (typeOfElement == 0 || secondZombie == 0)
						{
							m_NextZombieToUse = (typeOfElement == 1 || secondZombie == 1) ? 2 : 1;
						}
						else
						{
							m_NextZombieToUse = 0;
						}
						typeOfElement = k_OBSTACLE_TWO_ZOMBIES;
					}
				}
				break;
			case k_OBSTACLE_HANDS:
				SpawnObstacleElement(m_GroundPrefabs[0], trackPart.obstaclesTransform, laneToUse, zPosition);
				break;
			case k_OBSTACLE_RAVENS:
				SpawnObstacleElement(m_AirPrefabs[0], trackPart.obstaclesTransform, laneToUse, zPosition);
				break;
			case k_COLLECTIBLE:
				SpawnCollectibleElement(trackPart.collectiblesTransform, laneToUse, zPosition);
				break;
			case k_CONSUMABLE:
				SpawnConsumableElement(trackPart.consumablesTransform, laneToUse, zPosition);
				break;
			}
			m_LastSpawnedTypes.Add(typeOfElement);
			if (m_LastSpawnedTypes.Count > m_MaxListSpawnedTypesSize)
			{
				m_LastSpawnedTypes.RemoveAt(0);
			}
			m_ObstaclesCount++;
			if (m_LastLanesUsed.Count > m_MaxListLanesSize)
			{
				m_LastLanesUsed.RemoveAt(0);
			}
		}
	}

	protected void SpawnConsumableElement(Transform parent, int laneToUse, int zPosition)
	{
		int index = Random.Range(0, consumablePrefabs.Length);
		if (index == m_PreviousConsumable)
		{
			index = (index + 1) % consumablePrefabs.Length;
		}
		m_PreviousConsumable = index;
		ConsumablePool pool = consumablePooler.GetPool(consumablePrefabs[index]);
		Consumable consumable = pool.Get(Vector3.zero, Quaternion.identity);
		consumable.transform.SetParent(parent, true);
		consumable.transform.localPosition = new Vector3(laneToUse, 0f, zPosition);
		consumable.pool = pool;
		consumable.Spawn(this);
		m_LastConsumableSpawnedAt = m_TotalWorldDistance;
		int distance;
		switch (consumable.consumableType)
		{
		case ConsumableType.Boost:
			distance = DataManager.Instance.playerData.boostDistance + (int)m_BoostDistanceFactor + 30;
			break;
		case ConsumableType.CoinMultiplier:
			distance = DataManager.Instance.playerData.coinMultiplierDistance;
			break;
		case ConsumableType.Shield:
			distance = DataManager.Instance.playerData.shieldDistance;
			break;
		case ConsumableType.Weapon:
			distance = DataManager.Instance.playerData.weaponDistance;
			break;
		default:
			distance = 0;
			break;
		}
		m_NextConsumablePossibleAt = consumable.transform.position.z + (float)distance + 50f;
	}

	protected void SpawnCollectibleElement(Transform parent, int laneToUse, int zPosition)
	{
		int index = Random.Range(0, collectiblePrefabs.Length);
		if (index == m_PrevoiusCollectible)
		{
			index = (index + 1) % collectiblePrefabs.Length;
		}
		m_PrevoiusCollectible = index;
		CollectiblePool pool = collectiblePooler.GetPool(collectiblePrefabs[index]);
		Collectible collectible = pool.Get(Vector3.zero, Quaternion.identity);
		collectible.transform.SetParent(parent, true);
		collectible.transform.localPosition = new Vector3(laneToUse, 0f, zPosition);
		collectible.pool = pool;
		collectible.Spawn(this);
	}

	protected void SpawnObstacleElement(Obstacle obstacleToUse, Transform parent, int laneToUse, int zPosition)
	{
		ObstaclePool pool = obstaclePooler.GetPool(obstacleToUse);
		Obstacle obstacle = pool.Get(Vector3.zero, Quaternion.identity);
		obstacle.transform.SetParent(parent, true);
		obstacle.transform.localPosition = new Vector3(laneToUse, 0f, zPosition);
		obstacle.pool = pool;
		obstacle.Spawn(this);
	}

	protected int GetElementZPosition(int elementsOnPart, int elementNumber)
	{
		switch (elementsOnPart)
		{
		case 3:
			if (elementNumber == 1)
			{
				return 16;
			}
			if (elementNumber == 0)
			{
				return Random.Range(5, 6);
			}
			return Random.Range(26, 27);
		case 2:
			if (elementNumber != 0)
			{
				return Random.Range(22, 26);
			}
			return Random.Range(6, 10);
		case 1:
			return Random.Range(9, 22);
		default:
			return 0;
		}
	}

	protected int GetLaneToUse(int typeOfElement)
	{
		if (typeOfElement < 3 || typeOfElement == k_COLLECTIBLE || typeOfElement == k_CONSUMABLE)
		{
			int lane = Random.Range(-1, 2);
			int i = 0;
			while (true)
			{
				if (m_Levels[m_CurrentLevelSpawned].maxOnSameLaneAfterEachOther + 1 <= i)
				{
					int otherLane = (lane + 1 < 2) ? (lane + 1) : (-1);
					m_LastLanesUsed.Add(otherLane);
					return otherLane;
				}
				if (m_LastLanesUsed.Count <= i || m_LastLanesUsed[m_LastLanesUsed.Count - 1 - i] != lane)
				{
					break;
				}
				i++;
			}
			m_LastLanesUsed.Add(lane);
			return lane;
		}
		m_LastLanesUsed.Add(10);
		return 0;
	}

	private int ChooseZombie()
	{
		if (m_LastSpawnedTypes.Count > 0 && m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1] == k_OBSTACLE_TWO_ZOMBIES)
		{
			return m_NextZombieToUse;
		}
		int zombie = Random.Range(0, 3);
		if (m_LastSpawnedTypes.Count < 1)
		{
			return zombie;
		}
		if (m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1] == zombie)
		{
			return (zombie + 1) % 3;
		}
		return zombie;
	}

	private int CountLastTypes(int maxAfterEachOther, int type, out bool tooMany)
	{
		int i = 0;
		while (true)
		{
			if (maxAfterEachOther + 1 <= i)
			{
				tooMany = true;
				return i;
			}
			if (m_LastSpawnedTypes.Count <= i || m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1 - i] != type)
			{
				tooMany = false;
				return i;
			}
			i++;
		}
	}

	protected int GetSpawnElementType(Transform trackPart, int zPosition)
	{
		int random = Random.Range(0, m_SumGewichtung);
		Level level = m_Levels[m_CurrentLevelSpawned];
		int limit = level.gewichtungZombies;
		if (random < limit)
		{
			int i = 0;
			while (true)
			{
				if (level.maxZombiesAfterEachOther + 1 <= i)
				{
					return -1;
				}
				if (m_LastSpawnedTypes.Count <= i)
				{
					break;
				}
				int type = m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1 - i];
				if (type != k_OBSTACLE_ZOMBIE_1 && type != k_OBSTACLE_ZOMBIE_2 && type != k_OBSTACLE_ZOMBIE_3 && type != k_OBSTACLE_TWO_ZOMBIES)
				{
					break;
				}
				i++;
			}
			return ChooseZombie();
		}
		limit += level.gewichtungRaben;
		if (random < limit)
		{
			bool tooMany;
			CountLastTypes(level.maxRabenAfterEachOther, k_OBSTACLE_RAVENS, out tooMany);
			if (!level.rabenHaendeAfterEachOther && m_LastSpawnedTypes.Count > 0)
			{
				if (!tooMany && m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1] != k_OBSTACLE_HANDS)
				{
					return k_OBSTACLE_RAVENS;
				}
				return -1;
			}
			return tooMany ? (-1) : k_OBSTACLE_RAVENS;
		}
		limit += level.gewichtungHaende;
		if (random < limit)
		{
			bool tooMany2;
			CountLastTypes(level.maxHaendeAfterEachOther, k_OBSTACLE_HANDS, out tooMany2);
			if (!level.rabenHaendeAfterEachOther && m_LastSpawnedTypes.Count > 0)
			{
				if (!tooMany2 && m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1] != k_OBSTACLE_RAVENS)
				{
					return k_OBSTACLE_HANDS;
				}
				return -1;
			}
			return tooMany2 ? (-1) : k_OBSTACLE_HANDS;
		}
		if (level.gewichtungGhosts + limit <= random)
		{
			if (trackPart.position.z + (float)zPosition < m_NextConsumablePossibleAt - (m_TotalWorldDistance - m_LastConsumableSpawnedAt))
			{
				return -1;
			}
			return k_CONSUMABLE;
		}
		int j = 0;
		while (true)
		{
			if (level.maxGhostsAfterEachOther + 1 <= j)
			{
				return -1;
			}
			if (m_LastSpawnedTypes.Count <= j || m_LastSpawnedTypes[m_LastSpawnedTypes.Count - 1 - j] != k_COLLECTIBLE)
			{
				return k_COLLECTIBLE;
			}
			j++;
		}
	}
}
