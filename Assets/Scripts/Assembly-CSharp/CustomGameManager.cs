using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CustomGameManager : MonoBehaviour
{
	public GameState[] states;

	protected List<GameState> m_StateStack = new List<GameState>();

	protected Dictionary<GameStateName, GameState> m_StateDict = new Dictionary<GameStateName, GameState>();

	protected bool m_ApplicationPaused;

	public bool debugMode;

	public bool invincible;

	public PlayerInputController playerController;

	public TrackManager trackManager;

	public AudioSource menuBackgroundAudio;

	public AudioSource inGameBackgroundAudio;

	protected AudioMixerSnapshot m_ThemeAudioSnapshot;

	protected AudioMixerSnapshot m_ThemeAudioSnapshotMuted;

	protected AudioClip m_InGameBackgroundSound;

	protected AudioClip m_TutorialBackgroundSound;

	protected ThemeTutorial m_TutorialScript;

	public Animator moonAnimator;

	public MainCamera mainCamera;

	public BloodOnLens bloodOnLens;

	public HitCollider hitCollider;

	public HitColliderAllLane allLaneHitCollider;

	public PlayerCollider playerCollider;

	public GameStateIsDead gameStateIsDead;

	public GameStatePlayGame gameStatePlayGame;

	public GameObject runningBlindCanvas;

	public CanvasGroup darkScreen;

	public int metersTillDarken;

	public float darkenStep;

	public float enlightenStep;

	public float maxDarkenValue;

	protected bool m_ScreenFullyDarken;

	protected bool m_ScreenFullyEnlighted;

	protected ConsumableType m_ActivePowerUpType;

	protected Consumable m_ActivePowerUp;

	protected bool m_isSliding;

	protected bool m_isJumping;

	protected bool m_Tutorial;

	protected int m_CurrentLane = 1;

	protected int m_LaneChangeCount;

	protected int m_JumpCount;

	protected int m_JumpCountLeft;

	protected int m_JumpCountCenter;

	protected int m_JumpCountRight;

	protected int m_SlideCount;

	protected int m_SlideCountLeft;

	protected int m_SlideCountCenter;

	protected int m_SlideCountRight;

	protected int m_KilledByHole;

	protected int m_HolesSurvived;

	protected int m_KilledByZombie;

	protected int m_KilledByAir;

	protected int m_KilledByGround;

	protected int m_ZombiesDodged;

	protected int m_ZombiesKilled;

	protected int m_JumpedOver;

	protected int m_SlidedUnder;

	protected int m_GhostsCollected;

	protected int m_GhostsReallyCollected;

	protected int m_PowerUpsCollected;

	protected int m_PowerUpShieldCollected;

	protected int m_PowerUpBoostCollected;

	protected int m_PowerUpCoinDoublerCollected;

	protected int m_PowerUpWeaponCollected;

	protected int m_CoinMultiplier;

	protected float m_TotalWorldDistance;

	protected float m_Speed;

	protected int m_Score;

	protected int m_SumCollectedGhosts;

	protected int m_ScoreMultiplier;

	protected int m_SumGamesPlayed;

	protected int m_CurrentGameNumber;

	protected string m_IsKilledBy;

	protected int m_GhostsSpawned;

	protected int m_PowerUpBoostSpawned;

	protected int m_PowerUpShieldSpawned;

	protected int m_PowerUpWeaponSpawned;

	protected int m_PowerUpCoinDoublerSpawned;

	protected int m_SaveMeUsed;

	protected float m_DarkenScreenCalculatedAlpha;

	[HideInInspector]
	public bool isReady;

	private static CustomGameManager instance;

	public GameState topState
	{
		get
		{
			if (m_StateStack.Count == 0)
			{
				return null;
			}
			return m_StateStack[m_StateStack.Count - 1];
		}
	}

	public ThemeTutorial tutorialScript => m_TutorialScript;

	public ConsumableType activePowerUpType => m_ActivePowerUpType;

	public bool isSliding
	{
		get
		{
			return m_isSliding;
		}
		set
		{
			m_isSliding = value;
		}
	}

	public bool isJumping
	{
		get
		{
			return m_isJumping;
		}
		set
		{
			m_isJumping = value;
		}
	}

	public bool isTutorial
	{
		get
		{
			return m_Tutorial;
		}
		set
		{
			m_Tutorial = value;
		}
	}

	public int currentLane
	{
		get
		{
			return m_CurrentLane;
		}
		set
		{
			m_CurrentLane = value;
		}
	}

	public int laneChangeCount => m_LaneChangeCount;

	public int jumpCount => m_JumpCount;

	public int jumpCountLeft => m_JumpCountLeft;

	public int jumpCountCenter => m_JumpCountCenter;

	public int jumpCountRight => m_JumpCountRight;

	public int slideCount => m_SlideCount;

	public int slideCountLeft => m_SlideCountLeft;

	public int slideCountCenter => m_SlideCountCenter;

	public int slideCountRight => m_SlideCountRight;

	public int killedByHole => m_KilledByHole;

	public int holesSurvived => m_HolesSurvived;

	public int killedByZombie => m_KilledByZombie;

	public int killedByAir => m_KilledByAir;

	public int killedByGround => m_KilledByGround;

	public int zombiesDodged => m_ZombiesDodged;

	public int zombiesKilled => m_ZombiesKilled;

	public int jumpedOver => m_JumpedOver;

	public int slidedUnder => m_SlidedUnder;

	public int ghostsCollected => m_GhostsCollected;

	public int ghostsReallyCollected => m_GhostsReallyCollected;

	public int powerUpCollected => m_PowerUpsCollected;

	public int powerUpShieldCollected => m_PowerUpShieldCollected;

	public int powerUpBoostCollected => m_PowerUpBoostCollected;

	public int powerUpCoinDoublerCollected => m_PowerUpCoinDoublerCollected;

	public int powerUpWeaponCollected => m_PowerUpWeaponCollected;

	public int coinMultiplier => m_CoinMultiplier;

	public float worldDistance
	{
		get
		{
			return m_TotalWorldDistance;
		}
		set
		{
			m_TotalWorldDistance = value;
		}
	}

	public float speed => m_Speed;

	public int score => m_Score;

	public int sumCollectedGhosts => m_SumCollectedGhosts;

	public int scoreMultiplier => m_ScoreMultiplier;

	public int sumGamesPlayed => m_SumGamesPlayed;

	public int currentGameNumber
	{
		get
		{
			return m_CurrentGameNumber;
		}
		set
		{
			m_CurrentGameNumber = value;
		}
	}

	public string isKilledBy => m_IsKilledBy;

	public int ghostsSpawned => m_GhostsSpawned;

	public int powerUpsSpawned => m_PowerUpShieldSpawned + m_PowerUpBoostSpawned + m_PowerUpCoinDoublerSpawned;

	public int powerUpBoostSpawned => m_PowerUpBoostSpawned;

	public int powerUpShieldSpawned => m_PowerUpShieldSpawned;

	public int powerUpWeaponSpawned => m_PowerUpWeaponSpawned;

	public int powerUpCoinDoublerSpawned => m_PowerUpCoinDoublerSpawned;

	public int saveMeUsed
	{
		get
		{
			return m_SaveMeUsed;
		}
		set
		{
			m_SaveMeUsed = value;
		}
	}

	public static CustomGameManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Object.Destroy(gameObject);
			return;
		}
		instance = this;
		m_StateDict.Clear();
		if (states.Length == 0)
		{
			return;
		}
		for (int i = 0; i < states.Length; i++)
		{
			m_StateDict.Add(states[i].GetName(), states[i]);
		}
		m_StateStack.Clear();
	}

	private void Start()
	{
		PushState(states[0].GetName());
		isReady = true;
	}

	private void Update()
	{
		if (m_StateStack.Count > 0)
		{
			m_StateStack[m_StateStack.Count - 1].Tick();
		}
	}

	public void InitThemeData()
	{
		ThemeDatabase themeDatabase = Resources.Load<ThemeDatabase>((DataManager.Instance.selectedTheme == Theme.Factory) ? "theme_factory" : "theme_forest");
		mainCamera.SetBackgroundMaterial(themeDatabase.cameraBackground);
		playerController.SetPlayerSound(themeDatabase.runClip);
		trackManager.SetPrefabs(themeDatabase.trackParts, themeDatabase.trackPartsSpecial, themeDatabase.zombiePrefabs, themeDatabase.airPrefabs, themeDatabase.groundPrefabs, themeDatabase.themeLevels, themeDatabase.tutorialLevel);
		m_ThemeAudioSnapshot = themeDatabase.defaultSnapshot;
		m_ThemeAudioSnapshotMuted = themeDatabase.mutedSnapshot;
		m_InGameBackgroundSound = themeDatabase.backgroundSound;
		m_TutorialBackgroundSound = themeDatabase.tutorialSound;
		inGameBackgroundAudio.clip = DataManager.Instance.playerData.playTutorial ? m_TutorialBackgroundSound : m_InGameBackgroundSound;
		m_ThemeAudioSnapshot.TransitionTo(0f);
		m_TutorialScript = themeDatabase.tutorialScript;
		topState.ReceiveInfoMessage(InfoMessage.GameManagerInitFinished, null);
	}

	public void StartTutorialTheme()
	{
		if (m_TutorialBackgroundSound != null)
		{
			ChangeInGameBackgroundSound(m_TutorialBackgroundSound);
		}
	}

	public void FinishTutorialTheme()
	{
		if (m_TutorialBackgroundSound != null)
		{
			ChangeInGameBackgroundSound(m_InGameBackgroundSound);
		}
	}

	protected void ChangeInGameBackgroundSound(AudioClip clip)
	{
		bool wasPlaying = false;
		if (inGameBackgroundAudio.isPlaying)
		{
			inGameBackgroundAudio.Stop();
			wasPlaying = true;
		}
		inGameBackgroundAudio.clip = clip;
		if (wasPlaying)
		{
			inGameBackgroundAudio.Play();
		}
	}

	public void PlaySpeechMuted(bool playMuted, float transitionTime)
	{
		(playMuted ? m_ThemeAudioSnapshotMuted : m_ThemeAudioSnapshot).TransitionTo(transitionTime);
	}

	public void SwitchState(GameStateName newState)
	{
		GameState state = FindState(newState);
		if (state == null)
		{
			Debug.LogError("Can't find the state named " + newState);
			return;
		}
		GameState currentState = m_StateStack[m_StateStack.Count - 1];
		GameStateName fromStateName = currentState.GetName();
		currentState.Exit(state);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.PauseAccessibility(false);
			UAP_AccessibilityManager.BlockInput(false, true);
		}
		GameStateName stateName = state.GetName();
		m_StateStack[m_StateStack.Count - 1].GetName();
		SetOptionsForState(stateName, fromStateName);
		state.Enter(m_StateStack[m_StateStack.Count - 1]);
		m_StateStack.RemoveAt(m_StateStack.Count - 1);
		m_StateStack.Add(state);
	}

	public GameState FindState(GameStateName stateName)
	{
		GameState state;
		if (!m_StateDict.TryGetValue(stateName, out state))
		{
			return null;
		}
		return state;
	}

	public void PopState()
	{
		if (m_StateStack.Count < 2)
		{
			Debug.LogError("Can't pop states, only one in stack.");
			return;
		}
		GameState currentState = m_StateStack[m_StateStack.Count - 1];
		GameStateName fromStateName = currentState.GetName();
		currentState.Exit(m_StateStack[m_StateStack.Count - 2]);
		GameStateName stateName = m_StateStack[m_StateStack.Count - 2].GetName();
		m_StateStack[m_StateStack.Count - 2].GetName();
		SetOptionsForState(stateName, fromStateName);
		// Original: o estado anterior recebe ele mesmo como "from".
		m_StateStack[m_StateStack.Count - 2].Enter(m_StateStack[m_StateStack.Count - 2]);
		m_StateStack.RemoveAt(m_StateStack.Count - 1);
	}

	public void PushState(GameStateName name)
	{
		GameState state;
		if (!m_StateDict.TryGetValue(name, out state))
		{
			Debug.LogError("Can't find the state named " + name);
			return;
		}
		if (m_StateStack.Count < 1)
		{
			m_StateStack.Add(state);
			SetOptionsForState(state.GetName(), GameStateName.None);
			state.Enter(null);
			return;
		}
		GameState currentState = m_StateStack[m_StateStack.Count - 1];
		GameStateName fromStateName = currentState.GetName();
		currentState.Exit(state);
		m_StateStack.Add(state);
		GameStateName stateName = state.GetName();
		m_StateStack[m_StateStack.Count - 1].GetName();
		SetOptionsForState(stateName, fromStateName);
		// Original: Enter recebe o topo da pilha (o proprio estado recem-adicionado).
		state.Enter(m_StateStack[m_StateStack.Count - 1]);
	}

	public void RemoveState(GameStateName name)
	{
		GameState state;
		if (!m_StateDict.TryGetValue(name, out state))
		{
			Debug.LogError("Can't find the state named " + name);
			return;
		}
		m_StateDict.Remove(name);
		Object.DestroyImmediate(state.gameObject);
	}

	protected void SetOptionsForState(GameStateName stateName, GameStateName fromStateName)
	{
		switch (stateName)
		{
		case GameStateName.GameOver:
		case GameStateName.StartTutorial:
			if (Screen.sleepTimeout != -1)
			{
				Screen.sleepTimeout = -1;
			}
			EnableMenuMusic(true);
			EnableGameMusic(false);
			break;
		case GameStateName.Init:
			if (Screen.sleepTimeout != -1)
			{
				Screen.sleepTimeout = -1;
			}
			EnableMenuMusic(false);
			EnableGameMusic(false);
			break;
		case GameStateName.IsDead:
		case GameStateName.PlayGame:
			if (Screen.sleepTimeout != -1)
			{
				Screen.sleepTimeout = -1;
			}
			EnableMenuMusic(false);
			EnableGameMusic(true);
			break;
		case GameStateName.Menu:
		case GameStateName.MenuLeaderboard:
		case GameStateName.MenuOptions:
		case GameStateName.MenuUpgrades:
		case GameStateName.Pause:
		case GameStateName.SelectBoost:
		case GameStateName.MenuProfile:
			if (Screen.sleepTimeout == -1)
			{
				Screen.sleepTimeout = -2;
			}
			EnableMenuMusic(true);
			EnableGameMusic(false);
			break;
		case GameStateName.InitAnnouncement:
			if (Screen.sleepTimeout == -1)
			{
				Screen.sleepTimeout = -2;
			}
			EnableMenuMusic(false);
			EnableGameMusic(false);
			break;
		}
	}

	public void AllowSleepMode(bool allow)
	{
		int sleepTimeout = Screen.sleepTimeout;
		if (allow)
		{
			if (sleepTimeout == -1)
			{
				Screen.sleepTimeout = -2;
			}
		}
		else if (sleepTimeout != -1)
		{
			Screen.sleepTimeout = -1;
		}
	}

	public void EnableMenuMusic(bool enable)
	{
		bool activeSelf = menuBackgroundAudio.gameObject.activeSelf;
		if (enable)
		{
			if (activeSelf)
			{
				if (menuBackgroundAudio.isPlaying)
				{
					return;
				}
			}
			else
			{
				menuBackgroundAudio.gameObject.SetActive(true);
			}
			menuBackgroundAudio.Play();
		}
		else if (activeSelf)
		{
			menuBackgroundAudio.Stop();
			menuBackgroundAudio.gameObject.SetActive(false);
		}
	}

	public void EnableGameMusic(bool enable)
	{
		bool activeSelf = inGameBackgroundAudio.gameObject.activeSelf;
		if (enable)
		{
			if (activeSelf)
			{
				if (inGameBackgroundAudio.isPlaying)
				{
					return;
				}
			}
			else
			{
				inGameBackgroundAudio.gameObject.SetActive(true);
			}
			inGameBackgroundAudio.Play();
		}
		else if (activeSelf)
		{
			inGameBackgroundAudio.Stop();
			inGameBackgroundAudio.gameObject.SetActive(false);
		}
	}

	public void SetSpeed(float speed)
	{
		m_Speed = speed;
		playerController.SetRunSpeed(speed);
	}

	public void IncreaseGhostSpawned()
	{
		m_GhostsSpawned++;
	}

	public void IncreasePowerUpBoostSpawned()
	{
		m_PowerUpBoostSpawned++;
	}

	public void IncreasePowerUpShieldSpawned()
	{
		m_PowerUpShieldSpawned++;
	}

	public void IncreasePowerUpWeaponSpawned()
	{
		m_PowerUpWeaponSpawned++;
	}

	public void IncreasePowerUpCoinDoublerSpawned()
	{
		m_PowerUpCoinDoublerSpawned++;
	}

	public void JumpedOverHands()
	{
		m_JumpedOver++;
	}

	public void SlidedUnderRavens()
	{
		m_SlidedUnder++;
	}

	public void CollectedGhost()
	{
		m_GhostsReallyCollected++;
		int ghostValue = m_CoinMultiplier * 10;
		if (m_SumCollectedGhosts < DataManager.Instance.maxScoreCoinsValue)
		{
			m_SumCollectedGhosts += ghostValue;
			if (DataManager.Instance.maxScoreCoinsValue < m_SumCollectedGhosts)
			{
				m_SumCollectedGhosts = DataManager.Instance.maxScoreCoinsValue;
			}
			m_GhostsCollected = m_SumCollectedGhosts;
		}
		topState.ReceiveInfoMessage(InfoMessage.PlayGameUpdateLights, null);
		IncreaseScore(ghostValue);
		EnlightenTheScreen();
	}

	public void ShieldCollected()
	{
		m_PowerUpsCollected++;
		m_PowerUpShieldCollected++;
	}

	public void WeaponCollected()
	{
		m_PowerUpWeaponCollected++;
		m_PowerUpsCollected++;
	}

	public void BoostCollected()
	{
		m_PowerUpBoostCollected++;
		m_PowerUpsCollected++;
	}

	public void CoinDoublerCollected()
	{
		m_PowerUpCoinDoublerCollected++;
		m_PowerUpsCollected++;
	}

	public void LaneChangedTo(int lane)
	{
		m_CurrentLane = lane;
		m_LaneChangeCount++;
	}

	public void Jumped()
	{
		m_JumpCount++;
		switch (m_CurrentLane)
		{
		case 2:
			m_JumpCountRight++;
			break;
		case 1:
			m_JumpCountCenter++;
			break;
		case 0:
			m_JumpCountLeft++;
			break;
		}
	}

	public void Slided()
	{
		m_SlideCount++;
		switch (m_CurrentLane)
		{
		case 2:
			m_SlideCountRight++;
			break;
		case 1:
			m_SlideCountCenter++;
			break;
		case 0:
			m_SlideCountLeft++;
			break;
		}
	}

	private void PlayerKilled()
	{
		if (m_Tutorial)
		{
			topState.ReceiveInfoMessage(InfoMessage.TutorialIsDead, null);
		}
		else
		{
			SwitchState(GameStateName.IsDead);
		}
	}

	public void IsKilledByHole()
	{
		m_KilledByHole++;
		m_HolesSurvived--;
		m_IsKilledBy = "Hole";
		PlayerKilled();
	}

	public void IsKilledByZombie()
	{
		m_KilledByZombie++;
		m_ZombiesDodged--;
		m_IsKilledBy = "Zombie";
		PlayerKilled();
	}

	public void IsKilledByAir()
	{
		m_KilledByAir++;
		m_SlidedUnder--;
		m_IsKilledBy = "Ravens";
		PlayerKilled();
	}

	public void IsKilledByGround()
	{
		m_KilledByGround++;
		m_JumpedOver--;
		m_IsKilledBy = "Hands";
		PlayerKilled();
	}

	public void IsZombieDodged()
	{
		m_ZombiesDodged++;
	}

	public void IsHoleSurvived()
	{
		m_HolesSurvived++;
	}

	public void IsZombieKilled()
	{
		m_ZombiesKilled++;
	}

	public void DoubleCoinMultiplier(bool doubleCoins)
	{
		m_CoinMultiplier = doubleCoins ? (m_CoinMultiplier * 2) : (m_CoinMultiplier / 2);
	}

	public void IncreaseScore(int scoreToAdd)
	{
		if (m_Score >= DataManager.Instance.maxScoreCoinsValue)
		{
			return;
		}
		m_Score += m_ScoreMultiplier * scoreToAdd;
		if (DataManager.Instance.maxScoreCoinsValue < m_Score)
		{
			m_Score = DataManager.Instance.maxScoreCoinsValue;
		}
		topState.ReceiveInfoMessage(InfoMessage.PlayGameUpdateScore, null);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!m_ApplicationPaused && pause)
		{
			PauseTheApplication();
		}
		else if (m_ApplicationPaused && !pause)
		{
			UnpauseTheApplication();
		}
	}

	private void OnApplicationQuit()
	{
	}

	private void OnApplicationFocus(bool focus)
	{
		if (focus)
		{
			if (m_ApplicationPaused)
			{
				UnpauseTheApplication();
			}
		}
		else if (!m_ApplicationPaused)
		{
			PauseTheApplication();
		}
	}

	protected void PauseTheApplication()
	{
		m_ApplicationPaused = true;
		if ((FeerSceneManager.Instance.debugMode && FeerSceneManager.Instance.ignorePause) || topState == null || topState.GetName() != GameStateName.PlayGame)
		{
			return;
		}
		switch (topState.GetStatus())
		{
		case GameStateStatus.Tutorial:
			topState.ReceiveInfoMessage(InfoMessage.TutorialPaused, null);
			break;
		case GameStateStatus.Running:
			SwitchState(GameStateName.Pause);
			break;
		}
	}

	protected void UnpauseTheApplication()
	{
		m_ApplicationPaused = false;
		FeerSceneManager.Instance.StartListeningForURLSchemes();
	}

	public void StartTutorial()
	{
		m_ActivePowerUpType = ConsumableType.None;
		m_Tutorial = true;
		m_Score = 0;
		m_SumCollectedGhosts = 0;
		m_CurrentLane = 1;
		m_ScoreMultiplier = DataManager.Instance.playerData.scoreMultiplier;
		m_CoinMultiplier = 1;
		trackManager.StartTutorial();
		playerController.gameObject.SetActive(true);
		playerController.TutorialStarted();
		mainCamera.StartNewGame();
	}

	public void StartNewGame()
	{
		m_SumGamesPlayed++;
		StartTheGame();
	}

	protected void StartTheGame()
	{
		m_ActivePowerUpType = ConsumableType.None;
		Mission[] currentMissions = MissionManager.Instance.currentMissions;
		for (int i = 0; i < MissionManager.Instance.currentMissions.Length; i++)
		{
			if (!currentMissions[i].completed && MissionManager.Instance.currentMissions[i].missionType == MissionType.PlayGame)
			{
				MissionManager.Instance.currentMissions[i].IncreaseProgress();
			}
		}
		m_Score = 0;
		m_SumCollectedGhosts = 0;
		m_ScoreMultiplier = DataManager.Instance.playerData.scoreMultiplier;
		m_CurrentLane = 1;
		darkScreen.alpha = 0f;
		m_DarkenScreenCalculatedAlpha = 0f;
		m_ScreenFullyDarken = false;
		m_ScreenFullyEnlighted = true;
		runningBlindCanvas.SetActive(true);
		playerController.gameObject.SetActive(true);
		playerController.NewGameStarted();
		mainCamera.StartNewGame();
		m_CoinMultiplier = 1;
		trackManager.StartNewGame();
	}

	public void StopGame(bool stopFog, bool slowDownFog)
	{
		playerController.GameStopped();
		playerCollider.StopPlaying();
		trackManager.StopRunning(stopFog, slowDownFog);
	}

	public void ReviveGame()
	{
		m_CurrentLane = 1;
		mainCamera.Reset(true);
		bloodOnLens.MoveBloodOnLens();
		playerCollider.ReviveGame();
		trackManager.ReviveGame();
		playerController.GameRevived();
	}

	private void ResetRunStatistics()
	{
		gameStateIsDead.ResetSaveMeCosts();
		m_TotalWorldDistance = 0f;
		m_Score = 0;
		m_SumCollectedGhosts = 0;
		m_LaneChangeCount = 0;
		m_JumpCount = 0;
		m_JumpCountLeft = 0;
		m_JumpCountCenter = 0;
		m_JumpCountRight = 0;
		m_SlideCount = 0;
		m_SlideCountLeft = 0;
		m_SlideCountCenter = 0;
		m_SlideCountRight = 0;
		m_KilledByHole = 0;
		m_HolesSurvived = 0;
		m_KilledByZombie = 0;
		m_KilledByAir = 0;
		m_KilledByGround = 0;
		m_ZombiesDodged = 0;
		m_ZombiesKilled = 0;
		m_JumpedOver = 0;
		m_SlidedUnder = 0;
		m_GhostsCollected = 0;
		m_GhostsReallyCollected = 0;
		m_PowerUpsCollected = 0;
		m_PowerUpShieldCollected = 0;
		m_PowerUpBoostCollected = 0;
		m_PowerUpCoinDoublerCollected = 0;
		m_PowerUpWeaponCollected = 0;
		m_PowerUpShieldSpawned = 0;
		m_PowerUpWeaponSpawned = 0;
		m_PowerUpCoinDoublerSpawned = 0;
		m_SaveMeUsed = 0;
		m_GhostsSpawned = 0;
		m_PowerUpBoostSpawned = 0;
	}

	public void QuitGame()
	{
		string animatorBool = null;
		switch (m_ActivePowerUpType)
		{
		case ConsumableType.Boost:
		case ConsumableType.SingleUseHeadstart:
			animatorBool = "boosting";
			break;
		case ConsumableType.CoinMultiplier:
			animatorBool = "coinDoubler";
			break;
		case ConsumableType.Shield:
			animatorBool = "shield";
			break;
		case ConsumableType.Weapon:
			animatorBool = "weapon";
			break;
		}
		if (animatorBool != null)
		{
			moonAnimator.speed = 1f;
			moonAnimator.SetBool(animatorBool, false);
		}
		playerController.GameQuit(true);
		m_ActivePowerUpType = ConsumableType.None;
		ResetRunStatistics();
		playerCollider.QuitPlaying();
		m_CoinMultiplier = 1;
		bloodOnLens.StopBloodOnLens();
		trackManager.CleanUp();
		mainCamera.StopGame();
		runningBlindCanvas.SetActive(false);
		playerController.gameObject.SetActive(false);
	}

	public void TutorialStartNewGame()
	{
		bloodOnLens.StopBloodOnLens();
		mainCamera.Reset(false);
		playerController.GameQuit(true);
		ResetRunStatistics();
		playerCollider.QuitPlaying();
		m_CoinMultiplier = 1;
		trackManager.TutorialStartNewGame();
	}

	private bool IsPowerUpActive()
	{
		return (uint)(m_ActivePowerUpType - 1) <= 4u;
	}

	public void PauseGame()
	{
		mainCamera.Pause(true);
		if (IsPowerUpActive())
		{
			moonAnimator.speed = 0f;
		}
	}

	public void ResumeMoonAnimation()
	{
		if (IsPowerUpActive())
		{
			moonAnimator.speed = 1f;
			playerCollider.StartPowerUpSound(m_ActivePowerUpType);
		}
	}

	public void ResumeGame(bool stoppedForPowerUp = false)
	{
		playerCollider.ResumePlaying();
		if (stoppedForPowerUp)
		{
			trackManager.UnPauseFog();
			FirstTimeAccessiblePowerUp(m_ActivePowerUpType);
		}
		else
		{
			mainCamera.Pause(false);
			trackManager.ResumeRunning();
			playerController.GameResumed();
		}
	}

	public void PowerUpCollected(Consumable consumable)
	{
		hitCollider.ConsumableCollected(consumable.consumableType);
		if (m_ActivePowerUpType == ConsumableType.None)
		{
			m_ActivePowerUp = consumable;
			m_ActivePowerUpType = consumable.consumableType;
			playerController.GameStopped();
			trackManager.StopRunning(false, false);
			mainCamera.Pause(true);
			gameStatePlayGame.PowerUpCollected(consumable);
		}
	}

	public void FirstTimeAccessiblePowerUp(ConsumableType consumableType)
	{
		playerController.GameStopped();
		trackManager.StopRunning(false, false);
		mainCamera.Pause(true);
		StartPowerUpAnim(consumableType);
		gameStatePlayGame.FirstAccessiblePowerUp(consumableType);
	}

	public void FirstTimeAccessiblePowerUpFinished(ConsumableType consumableType)
	{
		mainCamera.Pause(false);
		if (consumableType != ConsumableType.None)
		{
			if ((uint)(consumableType - 1) <= 3u)
			{
				UsePowerUp(true);
			}
		}
		else
		{
			playerController.GameResumed();
			trackManager.ResumeRunning();
		}
	}

	public void StartPowerUpAnim(ConsumableType consumableType)
	{
		switch (consumableType)
		{
		case ConsumableType.Boost:
			moonAnimator.SetBool("boosting", true);
			playerCollider.SetBoost(true);
			break;
		case ConsumableType.CoinMultiplier:
			moonAnimator.SetBool("coinDoubler", true);
			playerCollider.SetCoinMultiplier(true);
			topState.ReceiveInfoMessage(InfoMessage.PlayGameUpdateLights, null);
			break;
		case ConsumableType.Shield:
			moonAnimator.SetBool("shield", true);
			playerCollider.SetShield(true);
			break;
		case ConsumableType.Weapon:
			moonAnimator.SetBool("weapon", true);
			playerController.SetWeapon(true);
			break;
		}
	}

	public void UsePowerUp(bool animAlreadyStarted = false)
	{
		switch (m_ActivePowerUpType)
		{
		case ConsumableType.Boost:
			if (!animAlreadyStarted)
			{
				moonAnimator.SetBool("boosting", true);
				playerCollider.SetBoost(true);
			}
			mainCamera.Pause(false);
			playerController.GameResumed();
			playerController.GiveControlAway();
			trackManager.Boost();
			return;
		case ConsumableType.CoinMultiplier:
			if (!animAlreadyStarted)
			{
				moonAnimator.SetBool("coinDoubler", true);
				playerCollider.SetCoinMultiplier(true);
			}
			break;
		case ConsumableType.Shield:
			if (!animAlreadyStarted)
			{
				moonAnimator.SetBool("shield", true);
				playerCollider.SetShield(true);
			}
			break;
		case ConsumableType.Weapon:
			if (!animAlreadyStarted)
			{
				moonAnimator.SetBool("weapon", true);
				playerController.SetWeapon(true);
			}
			break;
		default:
			return;
		}
		mainCamera.Pause(false);
		playerController.GameResumed();
		trackManager.UsePowerUp(m_ActivePowerUpType);
	}

	public void StopPowerUp()
	{
		switch (m_ActivePowerUpType)
		{
		case ConsumableType.Boost:
		case ConsumableType.SingleUseHeadstart:
			playerController.GiveControlBack();
			playerCollider.SetBoost(false);
			StartCoroutine(StopInvincibility(3f));
			break;
		case ConsumableType.CoinMultiplier:
			moonAnimator.SetBool("coinDoubler", false);
			playerCollider.SetCoinMultiplier(false);
			m_ActivePowerUpType = ConsumableType.None;
			topState.ReceiveInfoMessage(InfoMessage.PlayGameUpdateLights, null);
			break;
		case ConsumableType.Shield:
			moonAnimator.SetBool("shield", false);
			playerCollider.SetShield(false);
			trackManager.StopPowerUp();
			m_ActivePowerUpType = ConsumableType.None;
			break;
		case ConsumableType.Weapon:
			moonAnimator.SetBool("weapon", false);
			playerController.SetWeapon(false);
			trackManager.StopPowerUp();
			m_ActivePowerUpType = ConsumableType.None;
			break;
		}
	}

	protected IEnumerator StopInvincibility(float t)
	{
		yield return new WaitForSeconds(t);
		playerCollider.StopInvincible();
		moonAnimator.SetBool("boosting", false);
		m_ActivePowerUpType = ConsumableType.None;
	}

	public void DarkenTheScreen()
	{
		if (m_ScreenFullyDarken)
		{
			return;
		}
		m_DarkenScreenCalculatedAlpha += darkenStep;
		float value = m_DarkenScreenCalculatedAlpha;
		if (value <= 0f)
		{
			if (!m_ScreenFullyEnlighted)
			{
				darkScreen.alpha = 0f;
				m_ScreenFullyEnlighted = true;
			}
			return;
		}
		if (maxDarkenValue <= value)
		{
			m_DarkenScreenCalculatedAlpha = maxDarkenValue;
			m_ScreenFullyDarken = true;
			value = maxDarkenValue;
		}
		darkScreen.alpha = value;
		if (m_ScreenFullyEnlighted)
		{
			m_ScreenFullyEnlighted = false;
		}
	}

	protected void EnlightenTheScreen()
	{
		float value = m_DarkenScreenCalculatedAlpha - enlightenStep * (float)m_CoinMultiplier;
		if (value <= -0.15f)
		{
			value = -0.15f;
		}
		m_DarkenScreenCalculatedAlpha = value;
		if (m_ScreenFullyEnlighted)
		{
			return;
		}
		if (value <= 0f)
		{
			m_ScreenFullyEnlighted = true;
			value = 0f;
		}
		darkScreen.alpha = value;
		if (m_ScreenFullyDarken)
		{
			m_ScreenFullyDarken = false;
		}
	}
}
