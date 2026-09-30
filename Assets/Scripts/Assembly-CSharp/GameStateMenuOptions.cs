using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameStateMenuOptions : GameState
{
	public RectTransform panelTransform;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject navPanel;

	public Button settingsBtn;

	public Button helpBtn;

	public Button contactBtn;

	public Button privacyBtn;

	protected Text m_SettingsBtnText;

	protected Text m_HelpBtnText;

	protected Text m_ContactBtnText;

	protected Text m_PrivacyBtnText;

	public GameObject infoPanel;

	public Text infoDescription;

	public GameObject settingsPanel;

	public GameObject accessibleSettingsPanelRoot;

	public Color selectedColor;

	public Color notSelectedColor;

	public AudioSource singleClickUI;

	public Dropdown langDropdown;

	public GameObject changeLanguagePopUp;

	public Toggle leftRightReverseToggle;

	public Image leftRightReverseImage;

	public Toggle upDownReverseToggle;

	public Image upDownReverseImage;

	public Toggle centerLaneToggle;

	public Image centerLaneImage;

	public Toggle vibrationToggle;

	public Image vibrationImage;

	public Toggle voiceOverToggle;

	public Image voiceOverImage;

	public GameObject helpPanel;

	public GameObject accessibleHelpPanelRoot;

	public GameObject confirmResetGamePanel;

	public GameObject contactPanel;

	public GameObject accessibleContactPanelRoot;

	public GameObject privacyPanel;

	public GameObject accessiblePrivacyPanelRoot;

	public string unityPrivacyURL;

	public GameObject sharePopUpAndroid;

	public Dropdown shareAndroidDropDown;

	protected bool m_SharePopUpAndroidPopulated;

	public GameObject powerUpSoundsPanel;

	public Button boostBtn;

	public Button shieldBtn;

	public Button lightDoublerBtn;

	public Button weaponBtn;

	public AudioClip boostSound;

	public AudioClip lightDoublerSound;

	public AudioClip shieldSound;

	public AudioClip weaponSound;

	public AudioMixerGroup boostMixerOutput;

	public AudioMixerGroup shieldMixerOutput;

	public AudioMixerGroup lightDoublerMixerOutput;

	public AudioMixerGroup weaponMixerOutput;

	public AudioSource powerUpAudioSource;

	protected int m_currentPanel = -1;

	protected bool m_Init;

	protected const int c_SETTINGS_PANEL = 0;

	protected const int c_HELP_PANEL = 1;

	protected const int c_CONTACT_PANEL = 2;

	protected const int c_PRIVACY_PANEL = 3;

	protected const int c_POWERUP_BOOST = 1;

	protected const int c_POWERUP_SHIELD = 2;

	protected const int c_POWERUP_LIGHTDOUBLER = 3;

	protected const int c_POWERUP_WEAPON = 4;

	protected string m_shareSubject;

	protected string m_shareBody;

	protected int m_powerUpPlaying;

	protected string m_strPanelAnnouncement;

	private Text PanelBtnText(int panel)
	{
		switch (panel)
		{
		case 0:
			return m_SettingsBtnText;
		case 1:
			return m_HelpBtnText;
		case 2:
			return m_ContactBtnText;
		case 3:
			return m_PrivacyBtnText;
		default:
			return null;
		}
	}

	private GameObject PanelObject(int panel)
	{
		switch (panel)
		{
		case 0:
			return settingsPanel;
		case 1:
			return helpPanel;
		case 2:
			return contactPanel;
		case 3:
			return privacyPanel;
		default:
			return null;
		}
	}

	public override void Enter(GameState from)
	{
		if (!m_Init)
		{
			InitUI();
		}
		gameObject.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (m_strPanelAnnouncement == null)
			{
				m_strPanelAnnouncement = LocalizationManager.Instance.GetLocalizedValue("OPTIONS");
			}
			UAP_AccessibilityManager.Say(m_strPanelAnnouncement, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		}
		navPanel.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, true, GameStateName.None);
		if (m_currentPanel == -1)
		{
			m_currentPanel = (from.GetName() != GameStateName.Init) ? 2 : 0;
		}
		else if (m_currentPanel < 0 || m_currentPanel > 3)
		{
			return;
		}
		PanelObject(m_currentPanel).SetActive(true);
		for (int i = 0; i < 4; i++)
		{
			PanelBtnText(i).color = (i == m_currentPanel) ? selectedColor : Color.white;
		}
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
		scoreCoinsPanel.Hide();
		navPanel.SetActive(false);
		settingsPanel.SetActive(false);
		helpPanel.SetActive(false);
		contactPanel.SetActive(false);
		privacyPanel.SetActive(false);
	}

	public override GameStateName GetName()
	{
		return GameStateName.MenuOptions;
	}

	public override void Tick()
	{
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		GameObject panel = PanelObject(m_currentPanel);
		if (panel != null)
		{
			panel.GetComponent<Animation>().Play("SmallPanelSlideOut");
		}
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		CustomGameManager.Instance.SwitchState(toState);
	}

	private void SetToggleImage(Image image, bool selected)
	{
		image.color = selected ? selectedColor : notSelectedColor;
		image.fillCenter = selected;
	}

	protected void InitUI()
	{
		langDropdown.ClearOptions();
		langDropdown.AddOptions(LocalizationManager.Instance.GetLanguageDropdownList());
		PlayerData_v_1_1_3 playerData = DataManager.Instance.playerData;
		if (playerData.useReverseLeftRight)
		{
			leftRightReverseToggle.isOn = true;
			SetToggleImage(leftRightReverseImage, true);
		}
		if (playerData.useReverseUpDown)
		{
			upDownReverseToggle.isOn = true;
			SetToggleImage(upDownReverseImage, true);
		}
		if (playerData.useCenterLaneOrientation)
		{
			centerLaneToggle.isOn = true;
			SetToggleImage(centerLaneImage, true);
		}
		if (playerData.useVibration)
		{
			vibrationToggle.isOn = true;
			SetToggleImage(vibrationImage, true);
		}
		if (voiceOverToggle.isOn)
		{
			SetToggleImage(voiceOverImage, true);
		}
		PortCreatePowerUpCountdownToggle(playerData);
		PortCreateControlsButton();
		m_SettingsBtnText = settingsBtn.GetComponentInChildren<Text>();
		m_HelpBtnText = helpBtn.GetComponentInChildren<Text>();
		m_ContactBtnText = contactBtn.GetComponentInChildren<Text>();
		m_PrivacyBtnText = privacyBtn.GetComponentInChildren<Text>();
		m_Init = true;
	}

	public void ChangeLanguageBtnClicked()
	{
		langDropdown.value = LocalizationManager.Instance.GetUserLanguageIndex();
		changeLanguagePopUp.SetActive(true);
	}

	public void ChangeLanguageCancelClicked()
	{
		changeLanguagePopUp.SetActive(false);
	}

	public void ChangeLanguageOKClicked()
	{
		int value = langDropdown.value;
		int userLanguageIndex = LocalizationManager.Instance.GetUserLanguageIndex();
		changeLanguagePopUp.SetActive(false);
		if (value != userLanguageIndex)
		{
			FeerSceneManager.Instance.ChangeLanguage(LocalizationManager.Instance.GetSystemLanguageForIndex(langDropdown.value));
		}
	}

	private void ToggleClicked(Image image, bool selected)
	{
		if (m_Init && m_currentPanel != -1)
		{
			singleClickUI.Play();
		}
		SetToggleImage(image, selected);
	}

	public void ValueChangedReverseLeftRight(bool selected)
	{
		DataManager.Instance.UseReverseLeftRight(selected);
		ToggleClicked(leftRightReverseImage, selected);
	}

	public void ValueChangedReverseUpDown(bool selected)
	{
		DataManager.Instance.UseReverseUpDown(selected);
		ToggleClicked(upDownReverseImage, selected);
	}

	public void ValueChangedCenterLaneOrientation(bool selected)
	{
		DataManager.Instance.UseCenterLaneOrientation(selected);
		ToggleClicked(centerLaneImage, selected);
	}

	public void ValueChangedVibration(bool selected)
	{
		DataManager.Instance.UseVibration(selected);
		ToggleClicked(vibrationImage, selected);
	}

	// PORT: "power-up countdown" option, a copy of the vibration toggle placed below it
	// (the scene has no room for it; the accessibility plugin reads it after VIBRATION).
	protected Image m_PortPowerUpCountdownImage;

	private void PortCreatePowerUpCountdownToggle(PlayerData_v_1_1_3 playerData)
	{
		GameObject go = Object.Instantiate(vibrationToggle.gameObject, vibrationToggle.transform.parent, false);
		go.name = "PortPowerUpCountdownToggle";
		RectTransform rect = (RectTransform)go.transform;
		RectTransform source = (RectTransform)vibrationToggle.transform;
		float height = source.anchorMax.y - source.anchorMin.y;
		rect.anchorMin = new Vector2(source.anchorMin.x, source.anchorMin.y - height);
		rect.anchorMax = new Vector2(source.anchorMax.x, source.anchorMin.y);
		Toggle toggle = go.GetComponent<Toggle>();
		// The copy also carries the vibration callback set in the scene
		toggle.onValueChanged = new Toggle.ToggleEvent();
		toggle.isOn = playerData.portPowerUpCountdown;
		Transform label = go.transform.Find("Label");
		LocalizedTextUI localized = label.GetComponent<LocalizedTextUI>();
		if (localized != null)
		{
			Object.DestroyImmediate(localized);
		}
		label.GetComponent<Text>().text = LocalizationManager.Instance.GetLocalizedValue("port_powerup_countdown");
		m_PortPowerUpCountdownImage = go.transform.Find("Background").GetComponent<Image>();
		SetToggleImage(m_PortPowerUpCountdownImage, toggle.isOn);
		AccessibleToggle accessible = go.GetComponent<AccessibleToggle>();
		accessible.m_Text = "port_powerup_countdown";
		accessible.m_IsLocalizationKey = true;
		accessible.m_ManualPositionOrder = vibrationToggle.GetComponent<AccessibleToggle>().m_ManualPositionOrder + 1;
		toggle.onValueChanged.AddListener(ValueChangedPowerUpCountdown);
	}

	// PORT: "Controls" button (PortControlsMenu), a copy of the change-language button placed below the
	// power-up countdown option and read after it.
	private void PortCreateControlsButton()
	{
		Transform source = vibrationToggle.transform.parent.parent.Find("LanguagePanel/ChangeLanguageButton");
		GameObject go = Object.Instantiate(source.gameObject, vibrationToggle.transform.parent, false);
		go.name = "PortControlsButton";
		RectTransform rect = (RectTransform)go.transform;
		RectTransform toggle = (RectTransform)vibrationToggle.transform;
		float height = toggle.anchorMax.y - toggle.anchorMin.y;
		rect.anchorMin = new Vector2(toggle.anchorMin.x, toggle.anchorMin.y - 2f * height);
		rect.anchorMax = new Vector2(toggle.anchorMax.x, toggle.anchorMin.y - height);
		rect.offsetMin = Vector2.zero;
		rect.offsetMax = Vector2.zero;
		Text label = go.GetComponentInChildren<Text>();
		label.text = LocalizationManager.Instance.GetLocalizedValue("port_controls");
		Button button = go.GetComponent<Button>();
		button.onClick = new Button.ButtonClickedEvent();
		button.onClick.AddListener(() => PortControlsMenu.Open(label.font, go));
		AccessibleButton accessible = go.GetComponent<AccessibleButton>();
		accessible.m_Text = "port_controls";
		accessible.m_IsLocalizationKey = true;
		accessible.m_NameLabel = label.gameObject;
		accessible.m_ManualPositionParent = vibrationToggle.GetComponent<AccessibleToggle>().m_ManualPositionParent;
		accessible.m_ManualPositionOrder = vibrationToggle.GetComponent<AccessibleToggle>().m_ManualPositionOrder + 2;
	}

	public void ValueChangedPowerUpCountdown(bool selected)
	{
		DataManager.Instance.UsePowerUpCountdown(selected);
		ToggleClicked(m_PortPowerUpCountdownImage, selected);
	}

	public void ValueChangedVoiceOver(bool selected)
	{
		ToggleClicked(voiceOverImage, selected);
		// PORT: CustomAnalyticsTracker.SettingsChanged removed.
	}

	public void LeftRightReverseInfoClicked()
	{
		ShowInfoPanel(LocalizationManager.Instance.GetLocalizedValue("info_left_right_reverse"));
	}

	public void UpDownReverseInfoClicked()
	{
		ShowInfoPanel(LocalizationManager.Instance.GetLocalizedValue("info_up_down_reverse"));
	}

	public void CenterLaneOrientationInfoClicked()
	{
		ShowInfoPanel(LocalizationManager.Instance.GetLocalizedValue("info_center_lane"));
	}

	public void VoiceOverInfoClicked()
	{
		ShowInfoPanel(LocalizationManager.Instance.GetLocalizedValue("info_voice_over"));
	}

	protected void ShowInfoPanel(string text)
	{
		infoDescription.text = text;
		infoPanel.SetActive(true);
	}

	public void InfoPanelOKClicked()
	{
		infoPanel.SetActive(false);
	}

	public void RePlayTutorial()
	{
		// PORT: CustomAnalyticsTracker.HelpOptionSelected removed.
		CustomGameManager.Instance.SwitchState(GameStateName.PlayGame);
	}

	public void UserGuideBtnClicked()
	{
	}

	public void PowerUpSoundsBtnClicked()
	{
		powerUpSoundsPanel.SetActive(true);
	}

	public void PowerUpSoundsCloseBtnClicked()
	{
		if (m_powerUpPlaying != 0)
		{
			StopPlayingPowerUpSound();
		}
		powerUpSoundsPanel.SetActive(false);
	}

	private Button PowerUpButton(int powerUp)
	{
		switch (powerUp)
		{
		case 1:
			return boostBtn;
		case 2:
			return shieldBtn;
		case 3:
			return lightDoublerBtn;
		case 4:
			return weaponBtn;
		default:
			return null;
		}
	}

	private void StartPowerUpSound(int powerUp)
	{
		if (m_powerUpPlaying != 0)
		{
			StopPlayingPowerUpSound();
		}
		m_powerUpPlaying = powerUp;
		StartCoroutine(PlayPowerUpSound());
	}

	public void PowerUpBoostPlay()
	{
		StartPowerUpSound(1);
	}

	public void PowerUpShieldPlay()
	{
		StartPowerUpSound(2);
	}

	public void PowerUpWeaponPlay()
	{
		StartPowerUpSound(4);
	}

	public void PowerUpLightDoublerPlay()
	{
		StartPowerUpSound(3);
	}

	protected void StopPlayingPowerUpSound()
	{
		StopAllCoroutines();
		Button btn = PowerUpButton(m_powerUpPlaying);
		if (btn != null)
		{
			btn.image.fillCenter = false;
			btn.image.color = notSelectedColor;
		}
		powerUpAudioSource.Stop();
	}

	private IEnumerator PlayPowerUpSound()
	{
		GameObject selectedObject = boostBtn.gameObject;
		switch (m_powerUpPlaying)
		{
		case 1:
			powerUpAudioSource.clip = boostSound;
			powerUpAudioSource.outputAudioMixerGroup = boostMixerOutput;
			break;
		case 2:
			powerUpAudioSource.clip = shieldSound;
			powerUpAudioSource.outputAudioMixerGroup = shieldMixerOutput;
			break;
		case 3:
			powerUpAudioSource.clip = lightDoublerSound;
			powerUpAudioSource.outputAudioMixerGroup = lightDoublerMixerOutput;
			break;
		case 4:
			powerUpAudioSource.clip = weaponSound;
			powerUpAudioSource.outputAudioMixerGroup = weaponMixerOutput;
			break;
		}
		Button btn = PowerUpButton(m_powerUpPlaying);
		if (btn != null)
		{
			btn.image.fillCenter = true;
			btn.image.color = selectedColor;
			selectedObject = btn.gameObject;
		}
		powerUpAudioSource.Play();
		bool isStopped = false;
		while (true)
		{
			if (UAP_AccessibilityManager.IsEnabled() && UAP_AccessibilityManager.GetCurrentFocusObject() != selectedObject)
			{
				isStopped = true;
			}
			yield return null;
			if (isStopped)
			{
				break;
			}
		}
		StopPlayingPowerUpSound();
	}

	// PORT: external links (website, FAQ, support, social media, rating, sharing) removed
	// at the user's request (no online features). The buttons stay on screen but do nothing.
	public void FAQBtnClicked()
	{
	}

	public void ContactSupportBtnClicked()
	{
	}

	public string GetSupportMail()
	{
		return "";
	}

	public static string MyEscapeURL(string url)
	{
		return UnityEngine.Networking.UnityWebRequest.EscapeURL(url).Replace("+", "%20");
	}

	public void OpenExternalURL(string url)
	{
	}

	protected IEnumerator AccessibleOpenURLNotification(string url)
	{
		yield break;
	}

	public void ResetGameBtnClicked()
	{
		confirmResetGamePanel.SetActive(true);
	}

	public void ConfirmResetYesClicked()
	{
		// PORT: CustomAnalyticsTracker.ResetGameClicked removed.
		FeerSceneManager.Instance.ResetAllData();
	}

	public void ConfirmResetNoClicked()
	{
		confirmResetGamePanel.SetActive(false);
	}

	public void PromoCodeBtnClicked()
	{
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
		else if (confirmResetGamePanel.activeInHierarchy)
		{
			ConfirmResetNoClicked();
		}
		else if (changeLanguagePopUp.activeInHierarchy)
		{
			ChangeLanguageCancelClicked();
		}
		else if (infoPanel.activeInHierarchy)
		{
			InfoPanelOKClicked();
		}
		else if (powerUpSoundsPanel.activeInHierarchy)
		{
			PowerUpSoundsCloseBtnClicked();
		}
		else
		{
			BackBtnClicked();
		}
		return true;
	}

	public void BackBtnClicked()
	{
		StartCoroutine(SwitchState(GameStateName.Menu));
	}

	private void PanelBtnClicked(int panel, GameObject accessibleRoot)
	{
		if (m_currentPanel != panel)
		{
			StartCoroutine(SwitchPanel(panel));
		}
		else if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.SelectElement(accessibleRoot, true);
		}
	}

	public void SettingsBtnClicked()
	{
		PanelBtnClicked(0, accessibleSettingsPanelRoot);
	}

	public void HelpBtnClicked()
	{
		PanelBtnClicked(1, accessibleHelpPanelRoot);
	}

	public void ContactBtnClicked()
	{
		PanelBtnClicked(2, accessibleContactPanelRoot);
	}

	public void PrivacyBtnClicked()
	{
		PanelBtnClicked(3, accessiblePrivacyPanelRoot);
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		GameObject current = PanelObject(m_currentPanel);
		if (current != null)
		{
			current.GetComponent<Animation>().Play("SmallPanelSlideOut");
		}
		GameObject next = PanelObject(toPanel);
		if (next != null)
		{
			next.SetActive(true);
			PanelBtnText(toPanel).color = selectedColor;
		}
		Text currentText = PanelBtnText(m_currentPanel);
		if (currentText != null)
		{
			currentText.color = Color.white;
		}
		yield return new WaitForSeconds(0.5f);
		current = PanelObject(m_currentPanel);
		if (current != null)
		{
			current.SetActive(false);
		}
		m_currentPanel = toPanel;
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false, true);
		}
	}

	public void FollowUsOnFacebookClicked()
	{
	}

	public void FollowUsOnTwitterClicked()
	{
	}

	public void FollowUsOnInstagramClicked()
	{
	}

	protected void OpenURLInApp(string appURL, string httpURL)
	{
	}

	public void RateAppBtnClicked()
	{
	}

	public void TellAFriendBtnClicked()
	{
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

	public void VisitOurWebsiteClicked()
	{
	}

	public void FindUsOnYoutubeClicked()
	{
	}

	public void ContactUsBtnClicked()
	{
	}

	public void PrivacyPolicyBtnClicked()
	{
	}

	public void UnityPrivacyPolicyBtnClicked()
	{
	}

	public void AGBButtonClicked()
	{
	}
}
