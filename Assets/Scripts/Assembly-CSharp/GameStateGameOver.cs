using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateGameOver : GameState
{
	public RectTransform panelTransform;

	public GameObject navPanel;

	public GameObject navNextBtns;

	public GameObject navPlayBtns;

	public GameObject navPLAYBtn;

	public GameObject navNEXTBtn;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public MissionPanelUI missionSetPanel;

	public GameObject missionSetUnlocked;

	public GameObject counterPanel;

	public Animation counterPanelAnim;

	public Text counterPanelScoreTitle;

	public Text counterPanelScore;

	public Text counterPanelLightsTitle;

	public Text counterPanelLights;

	public Button shareButton;

	public AudioSource audioProgress;

	public AudioSource audioHighscore;

	public GameObject highscoreStar;

	public string highscoreColorCode;

	public GameObject loadingPanel;

	public GameObject highscoreBackground;

	public GameObject highscoreTitle1;

	public GameObject highscoreTitle2;

	public Image highscoreBackgroundImage;

	public GameObject usernameInputPanel;

	public InputFieldUI inputField;

	public InputField usernameInputField;

	public ButtonUI usernameSubmitBtn;

	public GameObject usernameSumbitText;

	public GameObject usernameErrorText;

	public GameObject usernameCancelBtn;

	public GameObject usernameOKBtn;

	public GameObject usernameLoadingWheel;

	public UAP_BaseElement accesibleInputField;

	public GameObject congratsPanel;

	public Text congratsText;

	public GameObject congratsPanelButton;

	public RateAppDialog rateAppDialog;

	public GameObject missionSetBackground;

	public GameObject sharePopUpAndroid;

	public Dropdown shareAndroidDropDown;

	protected bool m_SharePopUpAndroidPopulated;

	protected bool m_WaitForServer = true;

	protected bool m_IsNewHighscore;

	protected int m_SelectedLeaderBoardPanel = -1;

	protected int m_UserRank = -1;

	protected const int c_USERNAME_PANEL = 0;

	protected const int c_CONGRATS_PANEL = 1;

	protected const int c_RANK_PANEL = 2;

	protected bool m_TickInputField;

	protected bool m_shouldShowRateApp;

	protected GameStateName switchToNextState;

	protected bool m_TickAccessibleFocus;

	protected bool m_CounterPanelActive;

	protected bool m_HighscorePanelActive;

	protected bool m_HighscoreAlreadyShown;

	protected bool m_LevelUp;

	protected bool m_WaitForMissionUpdate;

	protected bool m_Init;

	protected UAP_BaseElement m_UAPCounterPanelScore;

	protected UAP_BaseElement m_UAPCounterPanelLight;

	protected UAP_BaseElement m_UAPCongratsPanelText;

	protected string m_ttsYouScored;

	protected string m_ttsYouCollected;

	protected string m_ttsLightsThisRun;

	protected string m_ttsPointsThisRun;

	protected string m_ttsNewHighscore;

	protected string m_strCongrats;

	protected string m_strYouHaveTopped;

	protected string m_strRank1;

	protected string m_strRankGlobal;

	protected string m_strRankFriends;

	protected string m_strRankConcat;

	protected string m_shareSubjectNewHighscore;

	protected string m_shareSubject;

	protected string m_shareBody1NewHighscore;

	protected string m_shareBody1;

	protected string m_shareBody2;

	protected string m_shareGlobalRank1;

	protected string m_shareGlobalRank2;

	protected bool m_PlayerRegistered;

	protected int m_PlayerGlobalRank = -1;

	protected int m_PlayerFriendsRank = -1;

	protected int m_MinLeaderboardScore = -1;

	private static CustomGameManager CGM => CustomGameManager.Instance;

	public override void Enter(GameState from)
	{
		if (!m_Init)
		{
			Init();
		}
		sharePopUpAndroid.SetActive(false);
		DataManager dataManager = DataManager.Instance;
		dataManager.UpdatePlayerStatsOnGameOver();
		m_WaitForServer = true;
		m_IsNewHighscore = false;
		m_MinLeaderboardScore = -1;
		m_SelectedLeaderBoardPanel = -1;
		m_UserRank = -1;
		m_TickInputField = false;
		m_shouldShowRateApp = false;
		switchToNextState = GameStateName.None;
		m_TickAccessibleFocus = false;
		m_CounterPanelActive = false;
		m_HighscorePanelActive = false;
		m_HighscoreAlreadyShown = false;
		m_LevelUp = false;
		m_PlayerGlobalRank = -1;
		m_PlayerFriendsRank = -1;
		if (!dataManager.playerData.isFirstGame)
		{
			MissionManager.Instance.UpdateProgress();
		}
		else
		{
			MissionManager.Instance.ResetProgress();
		}
		if (UAP_AccessibilityManager.IsEnabled() && 0 < CGM.sumCollectedGhosts)
		{
			dataManager.SaveCoins(CGM.sumCollectedGhosts, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
		}
		// PORT: CustomAnalyticsTracker.GameOver removido (analytics).
		dataManager.GameOver();
		m_IsNewHighscore = IsNewHighscore();
		if (m_IsNewHighscore)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				dataManager.NewHighscore(CGM.score);
			}
			if (!m_PlayerRegistered)
			{
				m_SelectedLeaderBoardPanel = 0;
				dataManager.GetMinGlobalHighscore();
			}
			else
			{
				dataManager.UpdateAndGetPlayerServerRanks(CGM.score);
			}
		}
		if (!dataManager.playerData.isFirstGame)
		{
			missionSetPanel.SaveProgress();
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		gameObject.SetActive(true);
		counterPanel.SetActive(true);
		m_CounterPanelActive = true;
		navNextBtns.SetActive(true);
		navPanel.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, true, GameStateName.GameOver);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			counterPanelScore.text = NumberFormatter.FormatToLocale(CGM.score);
			m_UAPCounterPanelScore.m_Text = m_ttsYouScored + NumberFormatter.FormatToLocale(CGM.score) + m_ttsPointsThisRun;
			counterPanelLights.text = NumberFormatter.FormatToLocale(CGM.sumCollectedGhosts);
			m_UAPCounterPanelLight.m_Text = m_ttsYouCollected + NumberFormatter.FormatToLocale(CGM.sumCollectedGhosts) + m_ttsLightsThisRun;
			counterPanelScoreTitle.gameObject.SetActive(true);
			counterPanelScore.gameObject.SetActive(true);
			counterPanelLightsTitle.gameObject.SetActive(true);
			counterPanelLights.gameObject.SetActive(true);
			shareButton.gameObject.SetActive(true);
			if (gameObject.activeSelf)
			{
				StartCoroutine(StartCounterPanelScoreAnimation(true));
			}
		}
		else if (gameObject.activeSelf)
		{
			StartCoroutine(StartCounterPanelScoreAnimation(false));
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			return;
		}
		if (m_IsNewHighscore)
		{
			dataManager.NewHighscore(CGM.score);
		}
		if (0 < CGM.sumCollectedGhosts)
		{
			dataManager.SaveCoins(CGM.sumCollectedGhosts, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
		}
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override GameStateName GetName()
	{
		return GameStateName.GameOver;
	}

	public override void Tick()
	{
		if (m_TickInputField)
		{
			inputField.Tick();
		}
		if (!UAP_AccessibilityManager.IsEnabled() || !m_TickAccessibleFocus)
		{
			return;
		}
		if (Input.touchCount == 1)
		{
			if (Input.GetTouch(0).phase != TouchPhase.Ended)
			{
				return;
			}
		}
		else if (!Input.GetMouseButtonUp(0))
		{
			return;
		}
		if (m_CounterPanelActive)
		{
			StopAllCoroutines();
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				UAP_AccessibilityManager.StopSpeaking();
			}
			if (m_IsNewHighscore)
			{
				m_UAPCounterPanelScore.m_Text = m_ttsYouScored + NumberFormatter.FormatToLocale(CGM.score) + m_ttsPointsThisRun + ".\n " + m_ttsNewHighscore;
				highscoreStar.SetActive(true);
			}
			UAP_AccessibilityManager.BlockInput(false, true);
			m_TickAccessibleFocus = false;
			UAP_AccessibilityManager.SelectElement(navNEXTBtn, true);
			return;
		}
		if (m_HighscorePanelActive && congratsPanel.activeSelf)
		{
			StopAllCoroutines();
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				UAP_AccessibilityManager.StopSpeaking();
			}
			UAP_AccessibilityManager.BlockInput(false, true);
			m_TickAccessibleFocus = false;
			UAP_AccessibilityManager.SelectElement(congratsPanelButton, true);
			return;
		}
		if (!missionSetPanel.AccessibleFocusChanged() || m_LevelUp)
		{
			return;
		}
		if (m_IsNewHighscore)
		{
			m_shouldShowRateApp = true;
		}
		m_TickAccessibleFocus = false;
		UAP_AccessibilityManager.BlockInput(false, true);
		UAP_AccessibilityManager.SelectElement(navPLAYBtn.GetComponentInChildren<UAP_BaseElement>().gameObject, true);
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
		if (!m_IsNewHighscore)
		{
			switch (infoMessage)
			{
			case InfoMessage.PurchaseMade:
			case InfoMessage.MissionSetAnimationFinished:
			case InfoMessage.MissionSetLevelUp:
			case InfoMessage.MissionSetLevelUpFinished:
				break;
			default:
				return;
			}
		}
		switch (infoMessage)
		{
		case InfoMessage.PurchaseMade:
			scoreCoinsPanel.UpdateCoins(0.5f);
			return;
		case InfoMessage.MissionSetAnimationFinished:
			if (m_IsNewHighscore)
			{
				m_shouldShowRateApp = true;
			}
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.BlockInput(false, true);
				UAP_AccessibilityManager.SelectElement(navPLAYBtn.GetComponentInChildren<UAP_BaseElement>().gameObject, true);
			}
			break;
		case InfoMessage.MissionSetLevelUp:
			CGM.AllowSleepMode(false);
			m_LevelUp = true;
			m_TickAccessibleFocus = false;
			m_shouldShowRateApp = true;
			scoreCoinsPanel.SlideOut();
			navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
			Invoke("HideInteractivePanels", 0.5f);
			return;
		case InfoMessage.MissionSetLevelUpFinished:
			navNextBtns.SetActive(false);
			navPlayBtns.SetActive(true);
			navPanel.SetActive(true);
			scoreCoinsPanel.UpdateCoins(-1f);
			scoreCoinsPanel.SlideIn();
			m_LevelUp = false;
			break;
		case InfoMessage.MinLeaderboardSuccess:
			if (m_WaitForServer)
			{
				m_MinLeaderboardScore = JsonUtility.FromJson<JsonServerResponseGetMinGlobalHighscore>(additionalData).minHighscore;
				m_WaitForServer = false;
			}
			return;
		case InfoMessage.MinLeaderboardFailed:
		case InfoMessage.PlayerRanksFailed:
			m_SelectedLeaderBoardPanel = 1;
			m_WaitForServer = false;
			return;
		case InfoMessage.SetNicknameFailed:
			UsernameError();
			return;
		case InfoMessage.SetNicknameSuccess:
			UsernameSuccess();
			return;
		case InfoMessage.PlayerRanksSuccess:
		{
			if (!m_WaitForServer)
			{
				return;
			}
			JsonServerResponseUpdateAndGetPlayerServerRanks ranks = JsonUtility.FromJson<JsonServerResponseUpdateAndGetPlayerServerRanks>(additionalData);
			m_PlayerGlobalRank = (ranks.globalRank == -1) ? m_PlayerGlobalRank : ranks.globalRank;
			m_PlayerFriendsRank = (ranks.friendsRank == -1) ? m_PlayerFriendsRank : ranks.friendsRank;
			m_SelectedLeaderBoardPanel = 2;
			m_WaitForServer = false;
			return;
		}
		default:
			return;
		}
		CGM.AllowSleepMode(true);
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		missionSetUnlocked.SetActive(false);
		highscoreStar.SetActive(false);
		missionSetPanel.Hide();
		scoreCoinsPanel.Hide();
		navPanel.SetActive(false);
		navPlayBtns.SetActive(false);
		loadingPanel.SetActive(false);
		congratsPanel.SetActive(false);
		usernameInputPanel.SetActive(false);
		highscoreTitle1.SetActive(false);
		highscoreTitle2.SetActive(false);
		Color color = highscoreBackgroundImage.color;
		highscoreBackgroundImage.color = new Color(color.r, color.g, color.b, 0f);
		highscoreBackground.SetActive(false);
		counterPanelScore.gameObject.SetActive(false);
		counterPanelLights.gameObject.SetActive(false);
		counterPanelScoreTitle.gameObject.SetActive(false);
		counterPanelLightsTitle.gameObject.SetActive(false);
		shareButton.gameObject.SetActive(false);
		counterPanel.SetActive(false);
		gameObject.SetActive(false);
		missionSetBackground.SetActive(false);
		UAP_AccessibilityManager.BlockInput(false, true);
	}

	protected void Init()
	{
		m_UAPCounterPanelScore = counterPanelScore.gameObject.GetComponent<UAP_BaseElement>();
		m_UAPCounterPanelLight = counterPanelLights.gameObject.GetComponent<UAP_BaseElement>();
		m_UAPCongratsPanelText = congratsText.GetComponent<UAP_BaseElement>();
		LocalizationManager lm = LocalizationManager.Instance;
		m_ttsYouScored = lm.GetLocalizedValue("tts_you_have_scored") + " ";
		m_ttsYouCollected = lm.GetLocalizedValue("tts_you_have_collected") + " ";
		m_ttsLightsThisRun = " " + lm.GetLocalizedValue("tts_lights_this_run");
		m_ttsPointsThisRun = " " + lm.GetLocalizedValue("tts_points_this_run");
		m_ttsNewHighscore = lm.GetLocalizedValue("tts_new_highscore");
		m_strCongrats = lm.GetLocalizedValue("Congrats");
		m_strYouHaveTopped = lm.GetLocalizedValue("You have topped yourself!");
		m_strRank1 = lm.GetLocalizedValue("You rank on the");
		m_strRankGlobal = lm.GetLocalizedValue("place of the global leaderboard");
		m_strRankFriends = lm.GetLocalizedValue("place of the friends board");
		m_strRankConcat = lm.GetLocalizedValue("and on the");
		// PORT: textos de compartilhamento nao sao usados (compartilhamento removido).
		m_PlayerRegistered = DataManager.Instance.playerData.highscoreNickname != "" && !string.IsNullOrEmpty(DataManager.Instance.playerData.highscoreNickname);
		m_Init = true;
	}

	private IEnumerator TimeOutRequestWebUsername(float timeOut)
	{
		FeerSceneManager.Instance.StartListeningForURLSchemes();
		float timePassed = 0f;
		while (timePassed < timeOut)
		{
			timePassed += Time.deltaTime;
			yield return null;
		}
	}

	protected void HideInteractivePanels()
	{
		navPanel.SetActive(false);
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		audioProgress.gameObject.SetActive(false);
		m_WaitForMissionUpdate = true;
		UpdateMissionSet();
		missionSetPanel.missionSetAnim.Play("PanelSlideOut");
		counterPanelAnim.Play("CounterPanelSlideOut");
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		while (m_WaitForMissionUpdate)
		{
			yield return null;
		}
		if (!m_shouldShowRateApp)
		{
			CGM.SwitchState(toState);
			yield break;
		}
		m_shouldShowRateApp = false;
		switchToNextState = toState;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false, true);
		}
		rateAppDialog.Show();
	}

	private IEnumerator StartGameOverMissionAnimation()
	{
		yield return new WaitForSeconds(0.6f);
		if (DataManager.Instance.playerData.isFirstGame)
		{
			missionSetUnlocked.SetActive(true);
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			m_TickAccessibleFocus = true;
		}
		missionSetPanel.StartGameOverAnimation();
	}

	protected void UpdateMissionSet()
	{
		bool allCompleted = true;
		Mission[] missions = MissionManager.Instance.currentMissions;
		for (int i = 0; i < 3; i++)
		{
			if (!missions[i].finished)
			{
				allCompleted &= missions[i].completed;
			}
		}
		if (!allCompleted)
		{
			m_WaitForMissionUpdate = false;
			return;
		}
		MissionManager.Instance.LoadNextMissionSet(NextMissionSetReceived);
	}

	public void NextMissionSetReceived(bool nextMissionAvailable)
	{
		if (nextMissionAvailable)
		{
			DataManager dataManager = DataManager.Instance;
			int lastRewarded = dataManager.playerData.lastRewardedMissionNumber;
			int currentSet = MissionManager.Instance.currentMissionSet;
			if (lastRewarded != -1 && lastRewarded < currentSet - 1)
			{
				if (currentSet - 1 < dataManager.maxAvailableMissions)
				{
					dataManager.IncreaseScoreMultiplier();
				}
				else
				{
					dataManager.SaveCoins(5000, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
				}
				dataManager.SetLastRewardedMission(MissionManager.Instance.currentMissionSet - 1);
			}
		}
		m_WaitForMissionUpdate = false;
	}

	private IEnumerator StartCounterPanelScoreAnimation(bool accessible)
	{
		yield return new WaitForSeconds(0.6f);
		if (!accessible)
		{
			counterPanelScore.text = "0";
			counterPanelScoreTitle.gameObject.SetActive(true);
			counterPanelScore.gameObject.SetActive(true);
		}
		if (gameObject.activeSelf)
		{
			StartCoroutine(CounterPanelScoreAnimation(accessible));
		}
	}

	// Espera a fala comecar (ou timeout) e terminar; opcionalmente liga o foco acessivel quando comecar
	private IEnumerator WaitSpeech(float startTimeOut, bool setTickFocus)
	{
		bool startedSpeaking = false;
		float timeOut = 0f;
		while (!startedSpeaking)
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				startedSpeaking = true;
				if (setTickFocus)
				{
					m_TickAccessibleFocus = true;
				}
			}
			timeOut += Time.deltaTime;
			if (startTimeOut < timeOut)
			{
				startedSpeaking = true;
				if (setTickFocus)
				{
					m_TickAccessibleFocus = true;
				}
			}
			yield return null;
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
	}

	private IEnumerator CounterPanelScoreAnimation(bool accessible)
	{
		if (accessible)
		{
			bool startedSpeaking = false;
			float timeToWait = 0f;
			while (true)
			{
				if (UAP_AccessibilityManager.IsSpeaking())
				{
					startedSpeaking = true;
					m_TickAccessibleFocus = true;
				}
				else if (1f < timeToWait)
				{
					UAP_AccessibilityManager.Say(m_ttsYouScored + NumberFormatter.FormatToLocale(CGM.score) + m_ttsPointsThisRun, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
					timeToWait = 0f;
				}
				timeToWait += Time.deltaTime;
				yield return null;
				if (startedSpeaking)
				{
					break;
				}
			}
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
		}
		else
		{
			float elapsedTime = 0f;
			float duration = 1f;
			int startingValue = 0;
			audioProgress.gameObject.SetActive(true);
			float pitchStartValue = 0.8f;
			audioProgress.pitch = 0.8f;
			audioProgress.Play();
			float finalPitch = 1.2f;
			if (m_IsNewHighscore)
			{
				scoreCoinsPanel.SetScore(CGM.score, duration);
			}
			while (elapsedTime < duration)
			{
				counterPanelScore.text = NumberFormatter.FormatToLocale((int)Mathf.Lerp(startingValue, CGM.score, elapsedTime / duration));
				audioProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			audioProgress.Stop();
			counterPanelScore.text = NumberFormatter.FormatToLocale(CGM.score);
		}
		yield return new WaitForEndOfFrame();
		if (m_IsNewHighscore)
		{
			float wait = 0f;
			while (m_WaitForServer)
			{
				wait += Time.deltaTime;
				if (3f < wait)
				{
					m_WaitForServer = false;
				}
				yield return null;
			}
			bool showHighscorePanel;
			if (!m_PlayerRegistered)
			{
				showHighscorePanel = m_MinLeaderboardScore != -1 && m_MinLeaderboardScore < CGM.score && DataManager.Instance.playerData.timesSubmitHighscoreAsked < 3;
			}
			else
			{
				showHighscorePanel = m_PlayerGlobalRank != -1 || m_PlayerFriendsRank != -1;
			}
			if (showHighscorePanel)
			{
				m_TickAccessibleFocus = false;
				m_CounterPanelActive = false;
				m_HighscorePanelActive = true;
				counterPanelAnim.Play("CounterPanelNewHighscore");
				audioHighscore.Play();
				if (gameObject.activeSelf)
				{
					StartCoroutine(ShowHighscorePanel(false));
				}
				yield break;
			}
			counterPanelAnim.Play("CounterPanelSimpleHighscore");
			audioHighscore.Play();
			if (accessible)
			{
				UAP_AccessibilityManager.Say(m_ttsNewHighscore, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
				yield return WaitSpeech(1.5f, false);
			}
		}
		if (!accessible)
		{
			counterPanelLights.text = "0";
			counterPanelLightsTitle.gameObject.SetActive(true);
			counterPanelLights.gameObject.SetActive(true);
			if (0 < CGM.sumCollectedGhosts)
			{
				if (gameObject.activeSelf)
				{
					StartCoroutine(CounterPanelLightsAnimation());
				}
			}
			else
			{
				shareButton.gameObject.SetActive(true);
			}
			yield break;
		}
		UAP_AccessibilityManager.Say(m_ttsYouCollected + NumberFormatter.FormatToLocale(CGM.sumCollectedGhosts) + m_ttsLightsThisRun, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		yield return WaitSpeech(1.5f, false);
		yield return new WaitForEndOfFrame();
		if (m_IsNewHighscore)
		{
			m_UAPCounterPanelScore.m_Text = m_ttsYouScored + NumberFormatter.FormatToLocale(CGM.score) + m_ttsPointsThisRun + ".\n " + m_ttsNewHighscore;
		}
		m_TickAccessibleFocus = false;
		UAP_AccessibilityManager.BlockInput(false, true);
		UAP_AccessibilityManager.SelectElement(shareButton.gameObject, true);
	}

	private IEnumerator CounterPanelLightsAnimation()
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
		}
		else
		{
			float elapsedTime = 0f;
			float duration = 1f;
			int startingValue = 0;
			float pitchStartValue = 0.8f;
			audioProgress.pitch = 0.8f;
			audioProgress.Play();
			float finalPitch = 1.2f;
			scoreCoinsPanel.SetCoins(DataManager.Instance.playerData.coins, duration);
			while (elapsedTime < duration)
			{
				counterPanelLights.text = NumberFormatter.FormatToLocale((int)Mathf.Lerp(startingValue, CGM.sumCollectedGhosts, elapsedTime / duration));
				audioProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			audioProgress.Stop();
			counterPanelLights.text = NumberFormatter.FormatToLocale(CGM.sumCollectedGhosts);
		}
		yield return new WaitForEndOfFrame();
		shareButton.gameObject.SetActive(true);
		CGM.AllowSleepMode(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			m_TickAccessibleFocus = false;
			UAP_AccessibilityManager.SelectElement(shareButton.gameObject, true);
			UAP_AccessibilityManager.BlockInput(false, true);
		}
	}

	private IEnumerator StartLightAnimation()
	{
		yield return new WaitForSeconds(0.2f);
		if (!UAP_AccessibilityManager.IsEnabled())
		{
			counterPanelLights.text = "0";
			counterPanelLightsTitle.gameObject.SetActive(true);
			counterPanelLights.gameObject.SetActive(true);
		}
		if (0 < CGM.sumCollectedGhosts && gameObject.activeSelf && !UAP_AccessibilityManager.IsEnabled())
		{
			StartCoroutine(CounterPanelLightsAnimation());
			yield break;
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.Say(m_ttsYouCollected + NumberFormatter.FormatToLocale(CGM.sumCollectedGhosts) + m_ttsLightsThisRun, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
			yield return WaitSpeech(1.5f, true);
			yield return new WaitForEndOfFrame();
			UAP_AccessibilityManager.BlockInput(false, true);
			m_TickAccessibleFocus = false;
			UAP_AccessibilityManager.SelectElement(shareButton.gameObject, true);
		}
		else
		{
			shareButton.gameObject.SetActive(true);
		}
		CGM.AllowSleepMode(true);
	}

	private IEnumerator ShowHighscorePanel(bool fromUsernameSubmit)
	{
		if (!m_HighscoreAlreadyShown)
		{
			m_HighscoreAlreadyShown = true;
		}
		yield return new WaitForSeconds(0.6f);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (!fromUsernameSubmit)
			{
				bool startedSpeaking = false;
				float timeToWait = 0f;
				while (!startedSpeaking)
				{
					if (UAP_AccessibilityManager.IsSpeaking())
					{
						startedSpeaking = true;
					}
					else if (1f < timeToWait)
					{
						UAP_AccessibilityManager.Say(m_ttsNewHighscore, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
						timeToWait = 0f;
					}
					timeToWait += Time.deltaTime;
					yield return null;
				}
			}
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
		}
		if (loadingPanel.activeSelf)
		{
			loadingPanel.SetActive(false);
		}
		switch (m_SelectedLeaderBoardPanel)
		{
		case 2:
			ShowRankPanel(fromUsernameSubmit);
			break;
		case 1:
			ShowCongratsOnlyPanel();
			break;
		case 0:
			ShowUsernameInputPanel();
			break;
		}
	}

	protected bool IsNewHighscore()
	{
		return DataManager.Instance.playerData.highscore < CGM.score;
	}

	protected void ShowUsernameInputPanel()
	{
		usernameSumbitText.SetActive(true);
		usernameSubmitBtn.EnableButton(false);
		inputField.Reset();
		usernameCancelBtn.SetActive(true);
		usernameLoadingWheel.SetActive(false);
		usernameErrorText.SetActive(false);
		usernameOKBtn.SetActive(false);
		accesibleInputField.m_Text = LocalizationManager.Instance.GetLocalizedValue("Your Nickname");
		usernameSubmitBtn.gameObject.SetActive(true);
		inputField.gameObject.SetActive(true);
		usernameInputField.gameObject.SetActive(true);
		usernameInputPanel.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false, true);
			m_TickAccessibleFocus = false;
			m_TickInputField = false;
			UAP_AccessibilityManager.SelectElement(usernameSumbitText, true);
		}
		else
		{
			m_TickInputField = true;
		}
		CGM.AllowSleepMode(true);
	}

	public void OnInputFieldValueChanged()
	{
		string text = usernameInputField.text;
		if (text.Equals(""))
		{
			accesibleInputField.m_Text = LocalizationManager.Instance.GetLocalizedValue("Your Nickname");
			usernameSubmitBtn.EnableButton(false);
		}
		else if (text.Length >= 1)
		{
			accesibleInputField.m_Text = text;
			usernameSubmitBtn.EnableButton(true);
		}
	}

	public void UsernameSubmitBtnClicked()
	{
		string value = usernameInputField.text;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		CGM.AllowSleepMode(false);
		inputField.gameObject.SetActive(false);
		usernameSubmitBtn.gameObject.SetActive(false);
		usernameCancelBtn.SetActive(false);
		usernameSumbitText.SetActive(false);
		usernameLoadingWheel.SetActive(true);
		m_TickInputField = false;
		inputField.Reset();
		m_WaitForServer = true;
		DataManager.Instance.SetNickname(value);
	}

	protected void UsernameError()
	{
		m_WaitForServer = false;
		m_TickInputField = false;
		inputField.Reset();
		usernameLoadingWheel.SetActive(false);
		usernameErrorText.SetActive(true);
		usernameOKBtn.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.SelectElement(usernameErrorText, true);
		}
		CGM.AllowSleepMode(true);
	}

	protected void UsernameSuccess()
	{
		m_PlayerRegistered = true;
		// PORT: CustomAnalyticsTracker.UserSignedUp removido.
		DataManager.Instance.UpdateAndGetPlayerServerRanks(CGM.score);
		usernameInputPanel.SetActive(false);
		m_TickInputField = false;
		inputField.Reset();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		if (gameObject.activeSelf)
		{
			StartCoroutine(ShowHighscorePanel(true));
		}
	}

	public void CancelBtnClicked()
	{
		CGM.AllowSleepMode(false);
		m_TickInputField = false;
		usernameInputPanel.SetActive(false);
		DataManager.Instance.SubmitHighscoreAsked();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		ReturnToCounterPanel();
	}

	public void UsernameOKBtnClicked()
	{
		CGM.AllowSleepMode(false);
		m_TickInputField = false;
		usernameInputPanel.SetActive(false);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		ReturnToCounterPanel();
	}

	protected void ShowCongratsOnlyPanel()
	{
		string nickname = DataManager.Instance.playerData.highscoreNickname;
		if (!m_PlayerRegistered)
		{
			congratsText.text = "<color=#ffffffff>" + m_strCongrats + "! " + m_strYouHaveTopped + "</color>";
			if (UAP_AccessibilityManager.IsEnabled())
			{
				m_UAPCongratsPanelText.m_Text = m_strCongrats + "!\n " + m_strYouHaveTopped;
			}
		}
		else
		{
			congratsText.text = "<color=#ffffffff>" + m_strCongrats + " </color><color=" + highscoreColorCode + ">" + nickname + "</color><color=#ffffffff>! " + m_strYouHaveTopped + "</color>";
			if (UAP_AccessibilityManager.IsEnabled())
			{
				m_UAPCongratsPanelText.m_Text = m_strCongrats + " " + nickname + "!\n " + m_strYouHaveTopped;
			}
		}
		congratsPanel.SetActive(true);
		m_TickAccessibleFocus = true;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (gameObject.activeSelf)
			{
				StartCoroutine(SayCongratsText());
			}
			return;
		}
		CGM.AllowSleepMode(true);
	}

	private IEnumerator SayCongratsText()
	{
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		UAP_AccessibilityManager.Say(m_UAPCongratsPanelText.m_Text, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		yield return WaitSpeech(1.5f, true);
		yield return new WaitForEndOfFrame();
		UAP_AccessibilityManager.BlockInput(false, true);
		m_TickAccessibleFocus = false;
		UAP_AccessibilityManager.SelectElement(congratsPanelButton, true);
		CGM.AllowSleepMode(true);
	}

	protected void ShowRankPanel(bool fromUsernameSubmit)
	{
		// PORT: o painel de ranking so aparece com resposta do servidor online (ranking global/amigos),
		// que foi removido; este caminho nunca e alcancado offline.
		ShowCongratsOnlyPanel();
	}

	public void OKButtonClicked()
	{
		CGM.AllowSleepMode(false);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		congratsPanel.SetActive(false);
		ReturnToCounterPanel();
	}

	protected void ReturnToCounterPanel()
	{
		StopAllCoroutines();
		if (UAP_AccessibilityManager.IsEnabled() && UAP_AccessibilityManager.IsSpeaking())
		{
			UAP_AccessibilityManager.StopSpeaking();
		}
		counterPanelAnim.Play("CounterPanelCloseHighscore");
		if (UAP_AccessibilityManager.IsEnabled())
		{
			m_CounterPanelActive = true;
			m_HighscorePanelActive = false;
		}
		if (gameObject.activeSelf)
		{
			StartCoroutine(StartLightAnimation());
		}
	}

	public void ShareBtnClicked()
	{
		// PORT: compartilhamento removido (pedido do usuario).
	}

	protected void PopulateSharePopUpAndroid()
	{
	}

	public void ShareAndroidCancelBtnClicked()
	{
		sharePopUpAndroid.SetActive(false);
	}

	public void ShareAndroidShareBtnClicked()
	{
		sharePopUpAndroid.SetActive(false);
	}

	private IEnumerator StartSharingAndroid(string selectedOption, string subject, string text, string sharePhotoPath)
	{
		yield break;
	}

	protected void OpenNativeShareDialog()
	{
	}

	public void PlayAgainBtnClicked()
	{
		CGM.QuitGame();
		missionSetPanel.StopEverything();
		MissionManager.Instance.ResetProgress();
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.SelectBoost));
		}
	}

	// PORT: Esc na tela final (Jogar de novo / Menu principal) volta ao menu principal.
	public override bool PortBack()
	{
		if (!gameObject.activeSelf)
		{
			return false;
		}
		if (sharePopUpAndroid.activeSelf)
		{
			ShareAndroidCancelBtnClicked();
			return true;
		}
		if (usernameInputPanel.activeSelf || !navPanel.activeSelf || !navPlayBtns.activeSelf)
		{
			return false;
		}
		BackToMainMenuBtnClicked();
		return true;
	}

	public void BackToMainMenuBtnClicked()
	{
		MissionManager.Instance.ResetProgress();
		missionSetPanel.StopEverything();
		CGM.QuitGame();
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.Menu));
		}
	}

	public void RateAppDialogClosed()
	{
		CGM.SwitchState(switchToNextState);
	}

	public void NavNextBtnClicked()
	{
		CGM.AllowSleepMode(false);
		StopAllCoroutines();
		audioProgress.Stop();
		m_CounterPanelActive = false;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		else
		{
			scoreCoinsPanel.UpdateScoreCoins();
		}
		scoreCoinsPanel.SetDefaultColor();
		missionSetPanel.Show(GetName());
		navNextBtns.SetActive(false);
		navPlayBtns.SetActive(true);
		if (gameObject.activeSelf)
		{
			StartCoroutine(StartGameOverMissionAnimation());
		}
	}
}
