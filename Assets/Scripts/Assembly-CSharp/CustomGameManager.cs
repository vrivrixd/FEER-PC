using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CustomGameManager : MonoBehaviour
{
	public GameState[] states;

	protected List<GameState> m_StateStack;

	protected Dictionary<GameStateName, GameState> m_StateDict;

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

	protected int m_CurrentLane;

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

	public GameState topState => null;

	public ThemeTutorial tutorialScript => null;

	public ConsumableType activePowerUpType => ConsumableType.None;

	public bool isSliding
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool isJumping
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool isTutorial
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public int currentLane
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public int laneChangeCount => 0;

	public int jumpCount => 0;

	public int jumpCountLeft => 0;

	public int jumpCountCenter => 0;

	public int jumpCountRight => 0;

	public int slideCount => 0;

	public int slideCountLeft => 0;

	public int slideCountCenter => 0;

	public int slideCountRight => 0;

	public int killedByHole => 0;

	public int holesSurvived => 0;

	public int killedByZombie => 0;

	public int killedByAir => 0;

	public int killedByGround => 0;

	public int zombiesDodged => 0;

	public int zombiesKilled => 0;

	public int jumpedOver => 0;

	public int slidedUnder => 0;

	public int ghostsCollected => 0;

	public int ghostsReallyCollected => 0;

	public int powerUpCollected => 0;

	public int powerUpShieldCollected => 0;

	public int powerUpBoostCollected => 0;

	public int powerUpCoinDoublerCollected => 0;

	public int powerUpWeaponCollected => 0;

	public int coinMultiplier => 0;

	public float worldDistance
	{
		get
		{
			return 0f;
		}
		set
		{
		}
	}

	public float speed => 0f;

	public int score => 0;

	public int sumCollectedGhosts => 0;

	public int scoreMultiplier => 0;

	public int sumGamesPlayed => 0;

	public int currentGameNumber
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public string isKilledBy => null;

	public int ghostsSpawned => 0;

	public int powerUpsSpawned => 0;

	public int powerUpBoostSpawned => 0;

	public int powerUpShieldSpawned => 0;

	public int powerUpWeaponSpawned => 0;

	public int powerUpCoinDoublerSpawned => 0;

	public int saveMeUsed
	{
		get
		{
			return 0;
		}
		set
		{
		}
	}

	public static CustomGameManager Instance => null;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void InitThemeData()
	{
	}

	public void StartTutorialTheme()
	{
	}

	public void FinishTutorialTheme()
	{
	}

	protected void ChangeInGameBackgroundSound(AudioClip clip)
	{
	}

	public void PlaySpeechMuted(bool playMuted, float transitionTime)
	{
	}

	public void SwitchState(GameStateName newState)
	{
	}

	public GameState FindState(GameStateName stateName)
	{
		return null;
	}

	public void PopState()
	{
	}

	public void PushState(GameStateName name)
	{
	}

	public void RemoveState(GameStateName name)
	{
	}

	protected void SetOptionsForState(GameStateName stateName, GameStateName fromStateName)
	{
	}

	public void AllowSleepMode(bool allow)
	{
	}

	public void EnableMenuMusic(bool enable)
	{
	}

	public void EnableGameMusic(bool enable)
	{
	}

	public void SetSpeed(float speed)
	{
	}

	public void IncreaseGhostSpawned()
	{
	}

	public void IncreasePowerUpBoostSpawned()
	{
	}

	public void IncreasePowerUpShieldSpawned()
	{
	}

	public void IncreasePowerUpWeaponSpawned()
	{
	}

	public void IncreasePowerUpCoinDoublerSpawned()
	{
	}

	public void JumpedOverHands()
	{
	}

	public void SlidedUnderRavens()
	{
	}

	public void CollectedGhost()
	{
	}

	public void ShieldCollected()
	{
	}

	public void WeaponCollected()
	{
	}

	public void BoostCollected()
	{
	}

	public void CoinDoublerCollected()
	{
	}

	public void LaneChangedTo(int lane)
	{
	}

	public void Jumped()
	{
	}

	public void Slided()
	{
	}

	public void IsKilledByHole()
	{
	}

	public void IsKilledByZombie()
	{
	}

	public void IsKilledByAir()
	{
	}

	public void IsKilledByGround()
	{
	}

	public void IsZombieDodged()
	{
	}

	public void IsHoleSurvived()
	{
	}

	public void IsZombieKilled()
	{
	}

	public void DoubleCoinMultiplier(bool doubleCoins)
	{
	}

	public void IncreaseScore(int scoreToAdd)
	{
	}

	private void OnApplicationPause(bool pause)
	{
	}

	private void OnApplicationQuit()
	{
	}

	private void OnApplicationFocus(bool focus)
	{
	}

	protected void PauseTheApplication()
	{
	}

	protected void UnpauseTheApplication()
	{
	}

	public void StartTutorial()
	{
	}

	public void StartNewGame()
	{
	}

	protected void StartTheGame()
	{
	}

	public void StopGame(bool stopFog, bool slowDownFog)
	{
	}

	public void ReviveGame()
	{
	}

	public void QuitGame()
	{
	}

	public void TutorialStartNewGame()
	{
	}

	public void PauseGame()
	{
	}

	public void ResumeMoonAnimation()
	{
	}

	public void ResumeGame(bool stoppedForPowerUp = false)
	{
	}

	public void PowerUpCollected(Consumable consumable)
	{
	}

	public void FirstTimeAccessiblePowerUp(ConsumableType consumableType)
	{
	}

	public void FirstTimeAccessiblePowerUpFinished(ConsumableType consumableType)
	{
	}

	public void StartPowerUpAnim(ConsumableType consumableType)
	{
	}

	public void UsePowerUp(bool animAlreadyStarted = false)
	{
	}

	public void StopPowerUp()
	{
	}

	protected IEnumerator StopInvincibility(float t)
	{
		return null;
	}

	public void DarkenTheScreen()
	{
	}

	protected void EnlightenTheScreen()
	{
	}
}
