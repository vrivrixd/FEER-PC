using System.Collections;
using UnityEngine;

public class GameStatePlayGame : GameState
{
	public GameObject resumePanel;

	public Animator clockAnimator;

	public AudioSource speechAudio;

	public GameObject skipTutorialPanel;

	public GameObject tutorialLoadingPanel;

	public GameObject ConfirmSkipTutorial;

	public RectTransform panelTransform;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject topPanel;

	protected GameStateStatus m_Status;

	protected bool m_Tutorial;

	protected GameStateName m_GameStateFrom;

	protected string m_TutorialType;

	protected AudioClip[] m_TutorialClips;

	protected bool m_AudioClipsLoaded;

	protected ThemeTutorial m_TutorialScript;

	protected bool m_FirstTimePowerUpSpeaking;

	protected ConsumableType m_FirstTimePowerUpSpeakingType;

	public override void Enter(GameState from)
	{
		m_GameStateFrom = from.GetName();
		switch (m_GameStateFrom)
		{
		case GameStateName.Init:
		case GameStateName.MenuOptions:
			m_Tutorial = true;
			m_TutorialType = "ReplayTutorial";
			break;
		case GameStateName.StartTutorial:
			m_Tutorial = true;
			m_TutorialType = "FirstStartTutorial";
			break;
		default:
			m_Tutorial = false;
			break;
		}
		UAP_AccessibilityManager.PauseAccessibility(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.RegisterOnPauseToggledCallback(m_Tutorial ? new UAP_AccessibilityManager.OnPauseToggleCallbackFunc(SkipTutorialBtnClicked) : new UAP_AccessibilityManager.OnPauseToggleCallbackFunc(PauseBtnClicked));
		}
		topPanel.SetActive(false);
		resumePanel.SetActive(false);
		tutorialLoadingPanel.SetActive(false);
		skipTutorialPanel.SetActive(false);
		gameObject.SetActive(true);
		switch (m_GameStateFrom)
		{
		case GameStateName.Init:
		case GameStateName.StartTutorial:
			m_Status = GameStateStatus.Tutorial;
			StartTutorial();
			break;
		case GameStateName.IsDead:
			m_Status = GameStateStatus.Running;
			CustomGameManager.Instance.ReviveGame();
			topPanel.SetActive(true);
			ShowScoreCoinsPanel();
			break;
		case GameStateName.MenuOptions:
			m_Status = GameStateStatus.Tutorial;
			PrepareTutorial();
			break;
		case GameStateName.Pause:
			if (m_FirstTimePowerUpSpeaking)
			{
				CustomGameManager.Instance.PlaySpeechMuted(true, 0f);
			}
			m_Status = GameStateStatus.Resume;
			CustomGameManager.Instance.ResumeMoonAnimation();
			resumePanel.SetActive(true);
			clockAnimator.Play("ClockAnim1");
			StartCoroutine(ClockResume());
			break;
		case GameStateName.SelectBoost:
			m_Status = GameStateStatus.Running;
			if (DataManager.Instance.playerData.isFirstAccesibleGameEver && UAP_AccessibilityManager.IsEnabled())
			{
				StartCoroutine(FirstGameInformation());
			}
			else
			{
				StartNewGame();
			}
			break;
		}
	}

	private IEnumerator ClockResume()
	{
		yield return new WaitForSeconds(3.083f);
		resumePanel.SetActive(false);
		if (m_Status == GameStateStatus.TutorialStartGame)
		{
			CustomGameManager.Instance.StartNewGame();
			m_Status = GameStateStatus.Running;
		}
		else if (m_Status == GameStateStatus.Resume)
		{
			if (m_FirstTimePowerUpSpeaking)
			{
				m_FirstTimePowerUpSpeaking = false;
				CustomGameManager.Instance.ResumeGame(true);
			}
			else
			{
				CustomGameManager.Instance.ResumeGame();
			}
			m_Status = GameStateStatus.Running;
		}
		topPanel.SetActive(true);
		ShowScoreCoinsPanel();
	}

	public override void Tick()
	{
	}

	public override GameStateName GetName()
	{
		return GameStateName.PlayGame;
	}

	public override GameStateStatus GetStatus()
	{
		return m_Status;
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
		switch (infoMessage)
		{
		case InfoMessage.PlayGameUpdateLights:
			if (m_Status == GameStateStatus.Running)
			{
				UpdateLights();
			}
			break;
		case InfoMessage.PlayGameUpdateScore:
			if (m_Status == GameStateStatus.Running)
			{
				UpdateScore();
			}
			break;
		case InfoMessage.TutorialIsDead:
			if (m_Status == GameStateStatus.Tutorial)
			{
				m_TutorialScript.PlayerDied();
			}
			break;
		case InfoMessage.TutorialPaused:
			if (m_Status == GameStateStatus.Tutorial)
			{
				SkipTutorialBtnClicked();
			}
			break;
		case InfoMessage.TutorialStoppedRunning:
			if (m_Status == GameStateStatus.Tutorial)
			{
				m_TutorialScript.TutorialStoppedRunning();
			}
			break;
		case InfoMessage.IsDeadAnimationFinished:
			if (m_Status == GameStateStatus.TutorialPaused)
			{
				m_TutorialScript.IsDeadAnimationFinishedWhilePaused();
			}
			else if (m_Status == GameStateStatus.Tutorial)
			{
				m_TutorialScript.IsDeadAnimationFinished();
			}
			break;
		}
	}

	public override void Exit(GameState to)
	{
		MissionManager.Instance.UpdateProgress();
		scoreCoinsPanel.Hide();
		ConfirmSkipTutorial.SetActive(false);
		gameObject.SetActive(false);
		if (to.GetName() == GameStateName.IsDead)
		{
			CustomGameManager.Instance.StopGame(false, true);
		}
		else
		{
			CustomGameManager.Instance.StopGame(true, false);
		}
		StopAllCoroutines();
		if (speechAudio.isPlaying)
		{
			speechAudio.Stop();
		}
		if (speechAudio.gameObject.activeSelf)
		{
			speechAudio.gameObject.SetActive(false);
		}
		if (m_Status == GameStateStatus.TutorialPaused || m_Status == GameStateStatus.Tutorial)
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				UAP_AccessibilityManager.StopSpeaking();
			}
			CustomGameManager.Instance.QuitGame();
		}
		else
		{
			CustomGameManager.Instance.PlaySpeechMuted(false, 0f);
		}
		if (UAP_AccessibilityManager.IsEnabled() && m_Status != GameStateStatus.Tutorial)
		{
			UAP_AccessibilityManager.UnregisterOnPauseToggledCallback(PauseBtnClicked);
		}
		UAP_AccessibilityManager.PauseAccessibility(false);
	}

	protected void ShowScoreCoinsPanel()
	{
		scoreCoinsPanel.RenderPlayGameCoinsUI();
		scoreCoinsPanel.RenderPlayGameScoreUI();
		scoreCoinsPanel.Show(panelTransform, false, false, 0, false);
	}

	protected void UpdateScore()
	{
		scoreCoinsPanel.RenderPlayGameScoreUI();
	}

	protected void UpdateLights()
	{
		scoreCoinsPanel.RenderPlayGameCoinsUI();
	}

	protected void StartNewGame()
	{
		CustomGameManager.Instance.StartNewGame();
		topPanel.SetActive(true);
		ShowScoreCoinsPanel();
	}

	public void TutorialStartNewGame()
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.RegisterOnPauseToggledCallback(PauseBtnClicked);
		}
		resumePanel.SetActive(true);
		clockAnimator.Play("ClockAnim1");
		StartCoroutine(ClockResume());
	}

	private IEnumerator FirstGameInformation()
	{
		yield return new WaitForSeconds(0.5f);
		bool startedSpeaking = false;
		float timeOut = 0f;
		UAP_AccessibilityManager.Say(LocalizationManager.Instance.GetLocalizedValue("You can pause the game by tapping in the upper left corner or by double-tapping with two fingers on the screen."));
		while (!startedSpeaking)
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				startedSpeaking = true;
			}
			timeOut += Time.deltaTime;
			if (1f < timeOut)
			{
				startedSpeaking = true;
			}
			yield return null;
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		DataManager.Instance.FirstGameInformationTold();
		if (m_Status == GameStateStatus.TutorialStartGame)
		{
			TutorialStartNewGame();
		}
		else
		{
			StartNewGame();
		}
	}

	public void PauseBtnClicked()
	{
		CustomGameManager.Instance.SwitchState(GameStateName.Pause);
	}

	public void PowerUpCollected(Consumable powerUp)
	{
		PlayerData_v_1_1_3 playerData = DataManager.Instance.playerData;
		switch (powerUp.consumableType)
		{
		case ConsumableType.Boost:
			if (playerData.isFirstAccessiblePowerUpBoost)
			{
				DataManager.Instance.FirstAccessiblePowerUpBoostTold();
				CustomGameManager.Instance.StartPowerUpAnim(ConsumableType.Boost);
				StartCoroutine(FirstAccessiblePowerUpInfo(ConsumableType.Boost));
				return;
			}
			break;
		case ConsumableType.CoinMultiplier:
			if (playerData.isFirstAccessiblePowerUpCoinDoubler)
			{
				DataManager.Instance.FirstAccessiblePowerUpCoinDoublerTold();
				CustomGameManager.Instance.StartPowerUpAnim(ConsumableType.CoinMultiplier);
				StartCoroutine(FirstAccessiblePowerUpInfo(ConsumableType.CoinMultiplier));
				return;
			}
			break;
		case ConsumableType.Shield:
			if (playerData.isFirstAccessiblePowerUpShield)
			{
				DataManager.Instance.FirstAccessiblePowerUpShieldTold();
				CustomGameManager.Instance.StartPowerUpAnim(ConsumableType.Shield);
				StartCoroutine(FirstAccessiblePowerUpInfo(ConsumableType.Shield));
				return;
			}
			break;
		case ConsumableType.Weapon:
			if (playerData.isFirstAccessiblePowerUpWeapon)
			{
				DataManager.Instance.FirstAccessiblePowerUpWeaponTold();
				CustomGameManager.Instance.StartPowerUpAnim(ConsumableType.Weapon);
				StartCoroutine(FirstAccessiblePowerUpInfo(ConsumableType.Weapon));
				return;
			}
			break;
		}
		CustomGameManager.Instance.UsePowerUp();
		UpdateLights();
	}

	public void FirstAccessiblePowerUp(ConsumableType consumableType)
	{
		StartCoroutine(FirstAccessiblePowerUpInfo(consumableType));
	}

	protected IEnumerator FirstAccessiblePowerUpInfo(ConsumableType consumableType)
	{
		m_FirstTimePowerUpSpeaking = true;
		m_FirstTimePowerUpSpeakingType = consumableType;
		CustomGameManager.Instance.PlaySpeechMuted(true, 0.25f);
		AudioClip clip;
		switch (consumableType)
		{
		case ConsumableType.Boost:
			clip = DataManager.Instance.firstPowerUpBoostInfoClip;
			break;
		case ConsumableType.CoinMultiplier:
			clip = DataManager.Instance.firstPowerUpCoinDoublerInfoClip;
			break;
		case ConsumableType.Shield:
			clip = DataManager.Instance.firstPowerUpShieldInfoClip;
			break;
		case ConsumableType.Weapon:
			clip = DataManager.Instance.firstPowerUpWeaponInfoClip;
			break;
		default:
			clip = DataManager.Instance.firstPowerUpInfoClip;
			break;
		}
		speechAudio.gameObject.SetActive(true);
		speechAudio.clip = clip;
		speechAudio.Play();
		while (speechAudio.isPlaying)
		{
			yield return null;
		}
		yield return new WaitForSeconds(0.2f);
		speechAudio.clip = null;
		speechAudio.gameObject.SetActive(false);
		CustomGameManager.Instance.PlaySpeechMuted(false, 0.25f);
		m_FirstTimePowerUpSpeaking = false;
		CustomGameManager.Instance.FirstTimeAccessiblePowerUpFinished(consumableType);
		UpdateLights();
	}

	public void StartTutorial()
	{
		if (m_TutorialClips == null)
		{
			m_TutorialClips = DataManager.Instance.GetTutorialClips();
		}
		speechAudio.gameObject.SetActive(true);
		m_TutorialScript = Object.Instantiate(CustomGameManager.Instance.tutorialScript);
		m_TutorialScript.gameObject.SetActive(true);
		m_TutorialScript.StartTutorial(this, speechAudio, m_TutorialClips, m_TutorialType);
	}

	protected void PrepareTutorial()
	{
		tutorialLoadingPanel.SetActive(true);
		StartCoroutine(LoadTutorialClips());
	}

	private IEnumerator LoadTutorialClips()
	{
		float timePassed = 0f;
		DataManager.Instance.LoadTutorialClips(TutorialAudioClipsLoaded);
		CustomGameManager.Instance.StartTutorialTheme();
		while (!m_AudioClipsLoaded)
		{
			timePassed += Time.deltaTime;
			yield return null;
		}
		float remaining = 2f - timePassed;
		if (0f <= remaining)
		{
			yield return new WaitForSeconds(remaining);
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		tutorialLoadingPanel.SetActive(false);
		StartTutorial();
	}

	public void TutorialAudioClipsLoaded(bool success)
	{
		if (success)
		{
			m_AudioClipsLoaded = true;
		}
		else
		{
			CustomGameManager.Instance.SwitchState(GameStateName.MenuOptions);
		}
	}

	public void ShowTutorialSkipPanel()
	{
		skipTutorialPanel.SetActive(true);
	}

	public void TutorialEnd()
	{
		StopAllCoroutines();
		m_TutorialScript.TutorialEnded();
		speechAudio.gameObject.SetActive(false);
		m_TutorialClips = null;
		Object.Destroy(m_TutorialScript.gameObject);
		m_TutorialScript = null;
		DataManager.Instance.TutorialFinished();
		m_Tutorial = false;
		CustomGameManager.Instance.isTutorial = false;
		CustomGameManager.Instance.trackManager.EndTutorial();
		CustomGameManager.Instance.playerController.TutorialEnd();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.UnregisterOnPauseToggledCallback(SkipTutorialBtnClicked);
		}
		if (m_GameStateFrom == GameStateName.MenuOptions)
		{
			CustomGameManager.Instance.QuitGame();
			MissionManager.Instance.ResetProgress();
			tutorialLoadingPanel.SetActive(true);
			CustomGameManager.Instance.FinishTutorialTheme();
			tutorialLoadingPanel.SetActive(false);
			CustomGameManager.Instance.SwitchState(GameStateName.MenuOptions);
			return;
		}
		CustomGameManager.Instance.StopGame(true, false);
		skipTutorialPanel.SetActive(false);
		CustomGameManager.Instance.FinishTutorialTheme();
		CustomGameManager.Instance.TutorialStartNewGame();
		CustomAnalyticsTracker.Instance.NewGameStarted(GameStateName.PlayGame);
		m_Status = GameStateStatus.TutorialStartGame;
		UAP_AccessibilityManager.PauseAccessibility(true);
		if (UAP_AccessibilityManager.IsEnabled() && DataManager.Instance.playerData.isFirstAccesibleGameEver)
		{
			StartCoroutine(FirstGameInformation());
		}
		else
		{
			TutorialStartNewGame();
		}
	}

	private IEnumerator InformAnalytics()
	{
		yield return new WaitForSeconds(2f);
		CustomAnalyticsTracker.Instance.NewGameStarted(GameStateName.PlayGame);
	}

	public void SkipTutorialBtnClicked()
	{
		PauseTutorial();
		ConfirmSkipTutorial.SetActive(true);
	}

	protected void PauseTutorial()
	{
		m_Status = GameStateStatus.TutorialPaused;
		m_TutorialScript.TutorialPaused();
	}

	public void ConfirmTutorialQuitClicked()
	{
		ConfirmSkipTutorial.SetActive(false);
		CustomGameManager.Instance.EnableMenuMusic(false);
		CustomGameManager.Instance.EnableGameMusic(true);
		m_TutorialScript.TutorialSkipped();
		TutorialEnd();
	}

	public void ConfirmTutorialResumeClicked()
	{
		ConfirmSkipTutorial.SetActive(false);
		CustomGameManager.Instance.EnableMenuMusic(false);
		CustomGameManager.Instance.EnableGameMusic(true);
		m_Status = GameStateStatus.Tutorial;
		m_TutorialScript.ResumeTutorial();
	}

	public void TutorialResume()
	{
		m_Status = GameStateStatus.Tutorial;
		m_TutorialScript.ResumeTutorial();
	}
}
