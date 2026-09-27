using System;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	protected int m_CurrentMissionSet;

	protected Mission[] m_CurrentMissions;

	protected MissionSetData missionSetData;

	protected int m_NextMissionNumber;

	protected Action<bool> m_NextMissionCallbackFunction;

	protected const string c_MissionSetFileName = "missionset_";

	protected const string c_MissionSetFileType = ".json";

	private static MissionManager instance;

	public Mission[] currentMissions => m_CurrentMissions;

	public int currentMissionSet => m_CurrentMissionSet;

	public static MissionManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		instance = this;
	}

	public void Init()
	{
		missionSetData = DataManager.Instance.LoadMissionSetData();
		if (missionSetData != null)
		{
			CreateMissions();
			InitCheckMissionComplete();
			return;
		}
		m_NextMissionNumber = 0;
		TextAsset textAsset = Resources.Load<TextAsset>("missionset_" + m_NextMissionNumber.ToString());
		FirstMissionSetReceived(true, textAsset.text);
	}

	public void FirstMissionSetReceived(bool success, string fileContent)
	{
		if (success)
		{
			ExtractFileData(fileContent);
		}
		else
		{
			Debug.LogError("Mission Manager: can't load mission file!");
		}
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionManagerInitFinished, null);
	}

	protected void CreateMissions()
	{
		m_CurrentMissionSet = missionSetData.missionSet;
		m_CurrentMissions = new Mission[missionSetData.missions.Length];
		for (int i = 0; i < missionSetData.missions.Length; i++)
		{
			MissionData d = missionSetData.missions[i];
			Mission mission;
			switch (d.type)
			{
			case MissionType.JumpOver:
				mission = new MissionJumpOver(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideUnder:
				mission = new MissionSlideUnder(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.DodgeZombies:
				mission = new MissionDodgeZombies(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PlayGame:
				mission = new MissionPlayGame(d.goal, d.progress, d.completed);
				break;
			case MissionType.DieZombie:
				mission = new MissionDieZombie(d.goal, d.progress, d.completed);
				break;
			case MissionType.DieAir:
				mission = new MissionDieAir(d.goal, d.progress, d.completed);
				break;
			case MissionType.DieGround:
				mission = new MissionDieGround(d.goal, d.progress, d.completed);
				break;
			case MissionType.BeatScore:
				mission = new MissionBeatHighscore(d.goal, d.progress, d.completed);
				break;
			case MissionType.ChangeLane:
				mission = new MissionChangeLane(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.CollectGhosts:
				mission = new MissionCollectGhosts(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.Slide:
				mission = new MissionSlide(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.Jump:
				mission = new MissionJump(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideUnderExact:
				mission = new MissionSlideUnderExact(d.goal, d.progress, d.completed);
				break;
			case MissionType.JumpOverExact:
				mission = new MissionJumpOverExact(d.goal, d.progress, d.completed);
				break;
			case MissionType.CollectGhostsExact:
				mission = new MissionCollectGhostsExact(d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideExact:
				mission = new MissionSlideExact(d.goal, d.progress, d.completed);
				break;
			case MissionType.JumpExact:
				mission = new MissionJumpExact(d.goal, d.progress, d.completed);
				break;
			case MissionType.SkipMission:
				mission = new MissionSkipMission(d.goal, d.progress, d.completed);
				break;
			case MissionType.SaveMe:
				mission = new MissionSaveMe(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.Score:
				mission = new MissionScore(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.ScoreWithoutCollectingGhosts:
				mission = new MissionScoreWithoutCollectingGhosts(d.goal, d.progress, d.completed);
				break;
			case MissionType.ScoreWithoutPowerUps:
				mission = new MissionScoreWithoutPowerUps(d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideLeft:
				mission = new MissionSlideLeft(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideCenter:
				mission = new MissionSlideCenter(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.SlideRight:
				mission = new MissionSlideRight(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.JumpLeft:
				mission = new MissionJumpLeft(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.JumpCenter:
				mission = new MissionJumpCenter(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.JumpRight:
				mission = new MissionJumpRight(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PowerUp:
				mission = new MissionPowerUp(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PowerUpShield:
				mission = new MissionPowerUpShield(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PowerUpBoost:
				mission = new MissionPowerUpBoost(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PowerUpCoinDoubler:
				mission = new MissionPowerUpCoinDoubler(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.PowerUpWeapon:
				mission = new MissionPowerUpWeapon(d.scope, d.goal, d.progress, d.completed);
				break;
			case MissionType.Headstart:
				mission = new MissionHeadStart(d.goal, d.progress, d.completed);
				break;
			case MissionType.CoinIncreaser:
				mission = new MissionCoinIncreaser(d.goal, d.progress, d.completed);
				break;
			case MissionType.ScoreIncreaser:
				mission = new MissionScoreIncreaser(d.goal, d.progress, d.completed);
				break;
			case MissionType.CollectGhostsWithoutPowerUps:
				mission = new MissionCollectGhostsWithoutPowerUps(d.goal, d.progress, d.completed);
				break;
			case MissionType.KillZombies:
				mission = new MissionKillZombies(d.goal, d.progress, d.completed);
				break;
			default:
				continue;
			}
			m_CurrentMissions[i] = mission;
		}
	}

	protected void InitCheckMissionComplete()
	{
		bool allCompleted = true;
		for (int i = 0; i < 3; i++)
		{
			Mission mission = m_CurrentMissions[i];
			if (mission.finished)
			{
				mission.completed = true;
			}
			else
			{
				allCompleted &= mission.completed;
			}
		}
		if (allCompleted)
		{
			LoadNextMissionSet(InitLatestMissionSetReceived);
			return;
		}
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionManagerInitFinished, null);
	}

	public void UpdateMissionThemeText()
	{
		for (int i = 0; i < m_CurrentMissions.Length; i++)
		{
			m_CurrentMissions[i].UpdateMissionThemeText();
		}
	}

	public void LoadNextMissionSet(Action<bool> callbackFunction)
	{
		m_NextMissionNumber = missionSetData.missionSet + 1;
		m_NextMissionCallbackFunction = callbackFunction;
		if (DataManager.Instance.maxAvailableMissions <= m_NextMissionNumber)
		{
			GenerateRandomMissionSet();
			return;
		}
		TextAsset textAsset = Resources.Load<TextAsset>("missionset_" + m_NextMissionNumber.ToString());
		NextMissionSetReceived(true, textAsset.text);
	}

	public void InitLatestMissionSetReceived(bool success)
	{
		if (success)
		{
			DataManager dataManager = DataManager.Instance;
			int lastSet = m_CurrentMissionSet - 1;
			if (dataManager.playerData.lastRewardedMissionNumber < lastSet)
			{
				if (lastSet < dataManager.maxAvailableMissions)
				{
					dataManager.IncreaseScoreMultiplier();
				}
				else
				{
					dataManager.SaveCoins(5000, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
				}
				dataManager.SetLastRewardedMission(m_CurrentMissionSet - 1);
			}
		}
		else
		{
			Debug.LogError("Mission set loading failed!");
		}
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionManagerInitFinished, null);
	}

	protected void ExtractFileData(string fileContent)
	{
		missionSetData = new MissionSetData();
		missionSetData.missions = new MissionData[3];
		missionSetData.missionSet = m_NextMissionNumber;
		JsonMissionSet jsonMissionSet = JsonUtility.FromJson<JsonMissionSet>(fileContent);
		for (int i = 0; i < jsonMissionSet.missions.Length; i++)
		{
			MissionData missionData = new MissionData();
			missionData.completed = false;
			missionData.progress = 0f;
			missionData.goal = jsonMissionSet.missions[i].goal;
			string scope = jsonMissionSet.missions[i].scope;
			if (scope == "MultiRun")
			{
				missionData.scope = MissionScope.MultiRun;
			}
			else if (scope == "SingleRun")
			{
				missionData.scope = MissionScope.SingleRun;
			}
			// in the original: a string switch over the MissionType enum names
			string type = jsonMissionSet.missions[i].type;
			if (type != null && Enum.IsDefined(typeof(MissionType), type))
			{
				missionData.type = (MissionType)Enum.Parse(typeof(MissionType), type);
			}
			missionSetData.missions[i] = missionData;
		}
		DataManager.Instance.SaveMissionSetData(missionSetData);
		CreateMissions();
		string nickname = DataManager.Instance.playerData.highscoreNickname;
		if (nickname != "" && !string.IsNullOrEmpty(nickname))
		{
			DataManager.Instance.UpdatePlayerServerData(false);
		}
		// PORT: CustomAnalyticsTracker.MissionUnlocked removed (analytics).
	}

	public void UpdateProgress()
	{
		for (int i = 0; i < m_CurrentMissions.Length; i++)
		{
			m_CurrentMissions[i].UpdateProgress();
		}
	}

	private bool AllMissionsCompleted()
	{
		bool result = true;
		for (int i = 0; i < m_CurrentMissions.Length; i++)
		{
			result &= m_CurrentMissions[i].completed;
		}
		return result;
	}

	public bool SkipMission(int missionNumber)
	{
		m_CurrentMissions[missionNumber].progress = m_CurrentMissions[missionNumber].goal;
		m_CurrentMissions[missionNumber].completed = true;
		missionSetData.missions[missionNumber].progress = missionSetData.missions[missionNumber].goal;
		missionSetData.missions[missionNumber].completed = true;
		DataManager.Instance.SaveMissionSetData(missionSetData);
		DataManager.Instance.UpdatePlayerStatsQuestSkipped();
		return AllMissionsCompleted();
	}

	public bool CompleteMission(int missionNumber)
	{
		m_CurrentMissions[missionNumber].progress = m_CurrentMissions[missionNumber].goal;
		m_CurrentMissions[missionNumber].completed = true;
		missionSetData.missions[missionNumber].progress = missionSetData.missions[missionNumber].goal;
		missionSetData.missions[missionNumber].completed = true;
		DataManager.Instance.SaveMissionSetData(missionSetData);
		return AllMissionsCompleted();
	}

	public bool SaveProgress()
	{
		bool allFinished = true;
		for (int i = 0; i < m_CurrentMissions.Length; i++)
		{
			Mission mission = m_CurrentMissions[i];
			if (!mission.completed && !mission.finished)
			{
				missionSetData.missions[i].progress = mission.GetResultToSave();
				allFinished = false;
			}
			else
			{
				missionSetData.missions[i].completed = true;
				missionSetData.missions[i].progress = missionSetData.missions[i].goal;
			}
		}
		DataManager.Instance.SaveMissionSetData(missionSetData);
		return allFinished;
	}

	public void ResetProgress()
	{
		CreateMissions();
		InitCheckMissionComplete();
	}

	public void NextMissionSetReceived(bool success, string fileContent)
	{
		if (success)
		{
			ExtractFileData(fileContent);
		}
		if (m_NextMissionCallbackFunction != null)
		{
			m_NextMissionCallbackFunction(success);
			m_NextMissionCallbackFunction = null;
		}
	}

	protected void GenerateRandomMissionSet()
	{
		m_CurrentMissionSet = m_NextMissionNumber;
		m_CurrentMissions = RandomMissionGenerator.GetRandomMissionSet(DataManager.Instance.playerData.scoreMultiplier);
		missionSetData = new MissionSetData();
		missionSetData.missions = new MissionData[3];
		missionSetData.missionSet = m_NextMissionNumber;
		for (int i = 0; i < m_CurrentMissions.Length; i++)
		{
			MissionData missionData = new MissionData();
			missionData.completed = false;
			missionData.progress = 0f;
			missionData.goal = m_CurrentMissions[i].goal;
			missionData.scope = m_CurrentMissions[i].missionScope;
			missionData.type = m_CurrentMissions[i].missionType;
			missionSetData.missions[i] = missionData;
		}
		DataManager.Instance.SaveMissionSetData(missionSetData);
		string nickname = DataManager.Instance.playerData.highscoreNickname;
		if (nickname != "" && !string.IsNullOrEmpty(nickname))
		{
			DataManager.Instance.UpdatePlayerServerData(false);
		}
		// PORT: CustomAnalyticsTracker.MissionUnlocked removed (analytics).
		if (m_NextMissionCallbackFunction != null)
		{
			m_NextMissionCallbackFunction(true);
			m_NextMissionCallbackFunction = null;
		}
	}
}
