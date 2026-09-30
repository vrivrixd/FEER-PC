using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateProfile : GameState
{
	public RectTransform panelTransform;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public Color selectedColor;

	public GameObject navPanel;

	public Button profileBtn;

	public Button statsBtn;

	protected Text m_profileBtnTxt;

	protected Text m_statsBtnTxt;

	public GameObject profilePanel;

	public GameObject accessibleProfilePanel;

	public Text profileNickname;

	public Text profileMissionLevel;

	public Text profileTopScore;

	public Text changeNicknameTxt;

	protected bool m_PlayerRegistered;

	public GameObject sharePopUpAndroid;

	public Dropdown shareAndroidDropDown;

	protected bool m_SharePopUpAndroidPopulated;

	public InputField usernameInputField;

	public ButtonUI usernameSubmitBtn;

	public InputFieldUI inputField;

	public GameObject usernameCancelBtn;

	public GameObject usernameLoadingWheel;

	public GameObject usernameText;

	protected Text m_usernameTxtMsg;

	public GameObject usernameOkBtn;

	public GameObject usernameErrorText;

	public UAP_BaseElement accesibleInputField;

	public GameObject enterUserNamePanel;

	protected bool m_TickInputField;

	protected bool m_TimeOutRequestUsername;

	protected bool m_UsernameFinished;

	protected bool m_SubmittedUsername;

	public GameObject statsPanel;

	public GameObject accessibleStatsPanel;

	public Text bestScore;

	public Text mostLights;

	public Text longestRun;

	public Text mostZombiesSurvived;

	public Text mostRobotsSurvived;

	public Text mostHandsSurvived;

	public Text mostBladesSurvived;

	public Text mostRavensSurvived;

	public Text mostCranesSurvived;

	public Text mostPowerUps;

	public Text mostZombiesKilled;

	public Text mostRobotsKilled;

	public Text runDistance;

	public Text lightsCollected;

	public Text powerUpsCollected;

	public Text zombiesKilled;

	public Text robotsKilled;

	public Text gamesPlayed;

	public Text lightsSpent;

	public Text zombiesSurvived;

	public Text robotsSurvived;

	public Text handsSurvived;

	public Text bladesSurvived;

	public Text ravensSurvived;

	public Text cranesSurvived;

	public Text deathByZombies;

	public Text deathByRobots;

	public Text deathByRavens;

	public Text deathByCranes;

	public Text deathByHands;

	public Text deathByBlades;

	public Text boostsCollected;

	public Text shieldsCollected;

	public Text weaponsCollected;

	public Text lightDoublerCollected;

	public Text missionsCompleted;

	public Text questsCompleted;

	public Text questsSkipped;

	protected bool m_Init;

	protected int m_currentPanel = 1;

	protected const int c_PROFILE_PANEL = 0;

	protected const int c_STATS_PANEL = 1;

	protected string m_strYourNickname;

	protected string m_strNewNickname;

	protected string m_strSubmitNickname;

	protected string m_strSubmit;

	protected string m_strChange;

	protected string m_strPanelAnnouncement;

	public override void Enter(GameState from)
	{
		if (!m_Init)
		{
			InitUI();
		}
		UpdateUI();
		gameObject.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (m_strPanelAnnouncement == null)
			{
				m_strPanelAnnouncement = LocalizationManager.Instance.GetLocalizedValue("PROFILE");
			}
			UAP_AccessibilityManager.Say(m_strPanelAnnouncement, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		}
		navPanel.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, true, GameStateName.MenuProfile);
		if (m_currentPanel == 1)
		{
			statsPanel.SetActive(true);
			m_statsBtnTxt.color = selectedColor;
			m_profileBtnTxt.color = Color.white;
		}
		else if (m_currentPanel == 0)
		{
			profilePanel.SetActive(true);
			m_profileBtnTxt.color = selectedColor;
			m_statsBtnTxt.color = Color.white;
		}
	}

	protected void InitUI()
	{
		m_statsBtnTxt = statsBtn.GetComponentInChildren<Text>();
		m_profileBtnTxt = profileBtn.GetComponentInChildren<Text>();
		m_usernameTxtMsg = usernameText.GetComponent<Text>();
		LocalizationManager lm = LocalizationManager.Instance;
		m_strNewNickname = lm.GetLocalizedValue("Enter a new nickname:");
		m_strYourNickname = lm.GetLocalizedValue("Your Nickname");
		m_strSubmitNickname = lm.GetLocalizedValue("Enter a nickname to post your score to the global leaderboard and connect with your friends.");
		m_strSubmit = lm.GetLocalizedValue("SUBMIT");
		m_strChange = lm.GetLocalizedValue("CHANGE");
		PortCreateCopyButton();
		m_Init = true;
	}

	// PORT: "Copy to clipboard" button on the statistics tab (the tab the profile opens on), a copy of the
	// profile tab's share button placed below the statistics list. It is ordered by its own position, so the
	// accessibility plugin reads it after the list, right before "Back to Main Menu" of the navigation panel.
	// The profile tab's share button does the same and gets the same name.
	private void PortCreateCopyButton()
	{
		AccessibleUIGroupRoot statsRoot = PortGroupRoot(bestScore.transform);
		AccessibleUIGroupRoot profileRoot = PortGroupRoot(profileTopScore.transform);
		Transform source = profileRoot.transform.Find("ShareBtn");
		PortRenameCopyButton(source.gameObject);
		GameObject go = Object.Instantiate(source.gameObject, statsRoot.transform, false);
		go.name = "PortCopyBtn";
		RectTransform rect = (RectTransform)go.transform;
		rect.anchorMin = new Vector2(0.64f, 0.03f);
		rect.anchorMax = new Vector2(0.94f, 0.13f);
		PortRenameCopyButton(go);
		Button button = go.GetComponent<Button>();
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(ShareBtnClicked);
		AccessibleButton accessible = go.GetComponent<AccessibleButton>();
		accessible.m_ManualPositionParent = null;
		accessible.m_ManualPositionOrder = -1;
	}

	private static void PortRenameCopyButton(GameObject button)
	{
		// LocalizedButtonUI translates the label when first shown; a translated text stays the same
		Transform label = button.transform.Find("Text");
		label.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue("port_copy_to_clipboard");
		AccessibleButton accessible = button.GetComponent<AccessibleButton>();
		accessible.m_Text = "port_copy_to_clipboard";
		accessible.m_IsLocalizationKey = true;
		accessible.m_NameLabel = label.gameObject;
	}

	private static AccessibleUIGroupRoot PortGroupRoot(Transform t)
	{
		while (t != null && t.GetComponent<AccessibleUIGroupRoot>() == null)
		{
			t = t.parent;
		}
		return (t != null) ? t.GetComponent<AccessibleUIGroupRoot>() : null;
	}

	protected void UpdateUI()
	{
		DataManager dm = DataManager.Instance;
		PlayerData_v_1_1_3 playerData = dm.playerData;
		PlayerStats_v_1_1_6 stats = dm.playerStats;
		if (string.IsNullOrEmpty(playerData.highscoreNickname))
		{
			m_PlayerRegistered = false;
			profileNickname.text = LocalizationManager.Instance.GetLocalizedValue("not set");
			changeNicknameTxt.text = m_strSubmit;
		}
		else
		{
			m_PlayerRegistered = true;
			profileNickname.text = playerData.highscoreNickname;
			changeNicknameTxt.text = m_strChange;
		}
		profileMissionLevel.text = NumberFormatter.FormatToLocale(playerData.isFirstGame ? 0 : (MissionManager.Instance.currentMissionSet + 1));
		profileTopScore.text = NumberFormatter.FormatToLocale(playerData.highscore);
		bestScore.text = NumberFormatter.FormatToLocale(playerData.highscore);
		mostLights.text = NumberFormatter.FormatToLocale(stats.mostLights);
		longestRun.text = NumberFormatter.FormatToLocale(stats.longestRun);
		mostZombiesSurvived.text = NumberFormatter.FormatToLocale(stats.mostZombiesSurvived);
		mostRobotsSurvived.text = NumberFormatter.FormatToLocale(stats.mostRobotsSurvived);
		mostHandsSurvived.text = NumberFormatter.FormatToLocale(stats.mostHandsSurvived);
		mostBladesSurvived.text = NumberFormatter.FormatToLocale(stats.mostSawBladesSurvived);
		mostRavensSurvived.text = NumberFormatter.FormatToLocale(stats.mostRavensSurvived);
		mostCranesSurvived.text = NumberFormatter.FormatToLocale(stats.mostCranesSurvived);
		mostPowerUps.text = NumberFormatter.FormatToLocale(stats.mostPowerUps);
		mostZombiesKilled.text = NumberFormatter.FormatToLocale(stats.mostZombiesKilled);
		mostRobotsKilled.text = NumberFormatter.FormatToLocale(stats.mostRobotsKilled);
		runDistance.text = NumberFormatter.FormatToLocale(stats.runDistance);
		lightsCollected.text = NumberFormatter.FormatToLocale(stats.lightsCollected);
		powerUpsCollected.text = NumberFormatter.FormatToLocale(stats.powerUpsCollected);
		zombiesKilled.text = NumberFormatter.FormatToLocale(stats.zombiesKilled);
		robotsKilled.text = NumberFormatter.FormatToLocale(stats.robotsKilled);
		gamesPlayed.text = NumberFormatter.FormatToLocale(stats.gamesPlayed);
		lightsSpent.text = NumberFormatter.FormatToLocale(stats.lightsSpent);
		zombiesSurvived.text = NumberFormatter.FormatToLocale(stats.zombiesSurvived);
		handsSurvived.text = NumberFormatter.FormatToLocale(stats.handsSurvived);
		ravensSurvived.text = NumberFormatter.FormatToLocale(stats.ravensSurvived);
		robotsSurvived.text = NumberFormatter.FormatToLocale(stats.robotsSurvived);
		bladesSurvived.text = NumberFormatter.FormatToLocale(stats.sawBladesSurvived);
		cranesSurvived.text = NumberFormatter.FormatToLocale(stats.cranesSurvived);
		deathByZombies.text = NumberFormatter.FormatToLocale(stats.deathByZombies);
		deathByRavens.text = NumberFormatter.FormatToLocale(stats.deathByRavens);
		deathByHands.text = NumberFormatter.FormatToLocale(stats.deathByHands);
		deathByRobots.text = NumberFormatter.FormatToLocale(stats.deathByRobot);
		deathByCranes.text = NumberFormatter.FormatToLocale(stats.deathByCrane);
		deathByBlades.text = NumberFormatter.FormatToLocale(stats.deathBySawBlade);
		boostsCollected.text = NumberFormatter.FormatToLocale(stats.boostsCollected);
		shieldsCollected.text = NumberFormatter.FormatToLocale(stats.shieldsCollected);
		weaponsCollected.text = NumberFormatter.FormatToLocale(stats.weaponsCollected);
		lightDoublerCollected.text = NumberFormatter.FormatToLocale(stats.lightDoublerCollected);
		long missionSets = MissionManager.Instance.currentMissionSet;
		long quests = missionSets * 3;
		Mission[] missions = MissionManager.Instance.currentMissions;
		for (int i = 0; i < 3; i++)
		{
			quests += missions[i].completed ? 1 : 0;
		}
		missionsCompleted.text = NumberFormatter.FormatToLocale(missionSets);
		questsCompleted.text = NumberFormatter.FormatToLocale(quests);
		questsSkipped.text = NumberFormatter.FormatToLocale(stats.questsSkipped);
	}

	public override GameStateName GetName()
	{
		return GameStateName.MenuProfile;
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override void Tick()
	{
		if (m_TickInputField)
		{
			inputField.Tick();
		}
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
		switch (infoMessage)
		{
		case InfoMessage.SetNicknameSuccess:
			if (!m_TimeOutRequestUsername)
			{
				StopAllCoroutines();
				UsernameSuccess();
			}
			break;
		case InfoMessage.ChangeNicknameSuccess:
			if (!m_TimeOutRequestUsername)
			{
				StopAllCoroutines();
				ChangeUsernameSuccess();
			}
			break;
		case InfoMessage.SetNicknameFailed:
		case InfoMessage.ChangeNicknameFailed:
			if (!m_TimeOutRequestUsername)
			{
				StopAllCoroutines();
				UsernameError();
			}
			break;
		}
	}

	protected void UsernameError()
	{
		usernameLoadingWheel.SetActive(false);
		usernameErrorText.SetActive(true);
		usernameOkBtn.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.SelectElement(usernameErrorText, true);
		}
	}

	// PORT: success only happens with the online server (removed); kept in a simplified form.
	protected void UsernameSuccess()
	{
		m_UsernameFinished = true;
		m_PlayerRegistered = true;
		enterUserNamePanel.SetActive(false);
		usernameLoadingWheel.SetActive(false);
		UpdateUI();
	}

	protected void ChangeUsernameSuccess()
	{
		UsernameSuccess();
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
		scoreCoinsPanel.Hide();
		navPanel.SetActive(false);
		statsPanel.SetActive(false);
		profilePanel.SetActive(false);
		sharePopUpAndroid.SetActive(false);
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		if (m_currentPanel == 1)
		{
			statsPanel.GetComponent<Animation>().Play("PanelSlideOut");
		}
		else if (m_currentPanel == 0)
		{
			profilePanel.GetComponent<Animation>().Play("PanelSlideOut");
		}
		statsPanel.GetComponent<Animation>().Play("PanelSlideOut");
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		CustomGameManager.Instance.SwitchState(toState);
	}

	public override bool PortBack()
	{
		if (!gameObject.activeSelf)
		{
			return false;
		}
		if (sharePopUpAndroid.activeInHierarchy)
		{
			ShareAndroidCancelBtnClicked();
		}
		else if (enterUserNamePanel.activeInHierarchy)
		{
			if (usernameOkBtn.activeInHierarchy)
			{
				UsernameErrorOKClicked();
			}
			else
			{
				UsernameCancelBtnClicked();
			}
		}
		else
		{
			MainMenuBtnClicked();
		}
		return true;
	}

	public void MainMenuBtnClicked()
	{
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.Menu));
		}
	}

	public void NavProfileBtnClicked()
	{
		if (m_currentPanel == 0)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessibleProfilePanel, true);
			}
			return;
		}
		StartCoroutine(SwitchPanel(0));
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		if (m_currentPanel == 1)
		{
			statsPanel.GetComponent<Animation>().Play("PanelSlideOut");
		}
		else if (m_currentPanel == 0)
		{
			profilePanel.GetComponent<Animation>().Play("PanelSlideOut");
		}
		if (toPanel == 1)
		{
			statsPanel.SetActive(true);
			m_statsBtnTxt.color = selectedColor;
		}
		else if (toPanel == 0)
		{
			profilePanel.SetActive(true);
			m_profileBtnTxt.color = selectedColor;
		}
		if (m_currentPanel == 1)
		{
			m_statsBtnTxt.color = Color.white;
		}
		else if (m_currentPanel == 0)
		{
			m_profileBtnTxt.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		if (m_currentPanel == 1)
		{
			statsPanel.SetActive(false);
		}
		else if (m_currentPanel == 0)
		{
			profilePanel.SetActive(false);
		}
		m_currentPanel = toPanel;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false, true);
		}
	}

	public void NavStatsBtnClicked()
	{
		if (m_currentPanel == 1)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessibleStatsPanel, true);
			}
			return;
		}
		StartCoroutine(SwitchPanel(1));
	}

	public void ChangeUsernameBtnClicked()
	{
		m_usernameTxtMsg.text = m_PlayerRegistered ? m_strNewNickname : m_strSubmitNickname;
		usernameInputField.text = "";
		accesibleInputField.m_Text = m_strYourNickname;
		usernameSubmitBtn.gameObject.SetActive(true);
		inputField.gameObject.SetActive(true);
		enterUserNamePanel.SetActive(true);
		m_TickInputField = !UAP_AccessibilityManager.IsEnabled();
	}

	public void ShareBtnClicked()
	{
		// PORT: the Android share dialog is replaced by copying the results to the clipboard.
		// Rows of the profile tab (TopScore/Text (1)) and of the statistics list (Highest Score/Text (1))
		PortShare.CopyProfile(profileTopScore.transform.parent.parent, bestScore.transform.parent.parent);
	}

	protected void PopulateSharePopUpAndroid()
	{
	}

	protected void OpenNativeShareDialog()
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

	public void ChangeUsernameSubmitBtnClicked()
	{
		string value = usernameInputField.text;
		if (string.IsNullOrEmpty(value))
		{
			return;
		}
		StopAllCoroutines();
		usernameSubmitBtn.EnableButton(false);
		inputField.gameObject.SetActive(false);
		usernameSubmitBtn.gameObject.SetActive(false);
		usernameCancelBtn.SetActive(false);
		usernameText.SetActive(false);
		usernameLoadingWheel.SetActive(true);
		m_TickInputField = false;
		inputField.Reset();
		m_TimeOutRequestUsername = false;
		m_UsernameFinished = false;
		StartCoroutine(TimeOutRequestUsername(15f));
		if (!m_PlayerRegistered)
		{
			DataManager.Instance.SetNickname(value);
		}
		else
		{
			DataManager.Instance.ChangeNickname(value);
		}
	}

	private IEnumerator TimeOutRequestUsername(float timeOut)
	{
		float timePassed = 0f;
		while (timePassed < timeOut)
		{
			timePassed += Time.deltaTime;
			yield return null;
		}
		if (!m_UsernameFinished)
		{
			m_TimeOutRequestUsername = true;
			UsernameError();
		}
	}

	public void UsernameErrorOKClicked()
	{
		enterUserNamePanel.SetActive(false);
		usernameErrorText.SetActive(false);
		usernameOkBtn.SetActive(false);
		inputField.gameObject.SetActive(true);
		usernameSubmitBtn.gameObject.SetActive(true);
		usernameCancelBtn.SetActive(true);
		usernameText.SetActive(true);
	}

	public void UsernameCancelBtnClicked()
	{
		m_TickInputField = false;
		inputField.Reset();
		enterUserNamePanel.SetActive(false);
		usernameSubmitBtn.EnableButton(false);
	}

	public void ReshowUsernameInputPanel()
	{
		usernameLoadingWheel.SetActive(false);
		accesibleInputField.m_Text = m_strYourNickname;
		usernameSubmitBtn.gameObject.SetActive(true);
		inputField.gameObject.SetActive(true);
		usernameInputField.text = "";
		usernameText.SetActive(true);
		usernameCancelBtn.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.SelectElement(usernameText, true);
		}
	}

	public void OnInputFieldValueChanged()
	{
		string text = usernameInputField.text;
		if (text.Equals(""))
		{
			accesibleInputField.m_Text = m_strYourNickname;
			usernameSubmitBtn.EnableButton(false);
		}
		else if (text.Length >= 1)
		{
			accesibleInputField.m_Text = text;
			usernameSubmitBtn.EnableButton(true);
		}
	}
}
