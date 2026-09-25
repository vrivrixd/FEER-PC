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

	protected int m_currentPanel;

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

	public override void Enter(GameState from)
	{
	}

	public override void Exit(GameState to)
	{
	}

	public override GameStateName GetName()
	{
		return GameStateName.None;
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
		return null;
	}

	protected void InitUI()
	{
	}

	public void ChangeLanguageBtnClicked()
	{
	}

	public void ChangeLanguageCancelClicked()
	{
	}

	public void ChangeLanguageOKClicked()
	{
	}

	public void ValueChangedReverseLeftRight(bool selected)
	{
	}

	public void ValueChangedReverseUpDown(bool selected)
	{
	}

	public void ValueChangedCenterLaneOrientation(bool selected)
	{
	}

	public void ValueChangedVibration(bool selected)
	{
	}

	public void ValueChangedVoiceOver(bool selected)
	{
	}

	public void LeftRightReverseInfoClicked()
	{
	}

	public void UpDownReverseInfoClicked()
	{
	}

	public void CenterLaneOrientationInfoClicked()
	{
	}

	public void VoiceOverInfoClicked()
	{
	}

	protected void ShowInfoPanel(string text)
	{
	}

	public void InfoPanelOKClicked()
	{
	}

	public void RePlayTutorial()
	{
	}

	public void UserGuideBtnClicked()
	{
	}

	public void PowerUpSoundsBtnClicked()
	{
	}

	public void PowerUpSoundsCloseBtnClicked()
	{
	}

	public void PowerUpBoostPlay()
	{
	}

	public void PowerUpShieldPlay()
	{
	}

	public void PowerUpWeaponPlay()
	{
	}

	public void PowerUpLightDoublerPlay()
	{
	}

	protected void StopPlayingPowerUpSound()
	{
	}

	private IEnumerator PlayPowerUpSound()
	{
		return null;
	}

	public void FAQBtnClicked()
	{
	}

	public void ContactSupportBtnClicked()
	{
	}

	public string GetSupportMail()
	{
		return null;
	}

	public static string MyEscapeURL(string url)
	{
		return null;
	}

	public void OpenExternalURL(string url)
	{
	}

	protected IEnumerator AccessibleOpenURLNotification(string url)
	{
		return null;
	}

	public void ResetGameBtnClicked()
	{
	}

	public void ConfirmResetYesClicked()
	{
	}

	public void ConfirmResetNoClicked()
	{
	}

	public void PromoCodeBtnClicked()
	{
	}

	public void BackBtnClicked()
	{
	}

	public void SettingsBtnClicked()
	{
	}

	public void HelpBtnClicked()
	{
	}

	public void ContactBtnClicked()
	{
	}

	public void PrivacyBtnClicked()
	{
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		return null;
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
	}

	public void ShareAndroidShareBtnClicked()
	{
	}

	private IEnumerator StartSharingAndroid(string selectedOption, string subject, string text, string sharePhotoPath)
	{
		return null;
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
