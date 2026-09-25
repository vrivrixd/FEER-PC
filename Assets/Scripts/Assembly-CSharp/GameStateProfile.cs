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

	protected int m_currentPanel;

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
	}

	public override GameStateName GetName()
	{
		return GameStateName.None;
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override void Tick()
	{
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override void Exit(GameState to)
	{
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	public void MainMenuBtnClicked()
	{
	}

	protected void InitUI()
	{
	}

	protected void UpdateUI()
	{
	}

	public void NavProfileBtnClicked()
	{
	}

	public void NavStatsBtnClicked()
	{
	}

	private IEnumerator SwitchPanel(int toPanel)
	{
		return null;
	}

	public void ChangeUsernameBtnClicked()
	{
	}

	public void ShareBtnClicked()
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

	public void ChangeUsernameSubmitBtnClicked()
	{
	}

	private IEnumerator TimeOutRequestUsername(float timeOut)
	{
		return null;
	}

	protected void UsernameError()
	{
	}

	public void UsernameErrorOKClicked()
	{
	}

	protected void ChangeUsernameSuccess()
	{
	}

	protected void UsernameSuccess()
	{
	}

	public void UsernameCancelBtnClicked()
	{
	}

	protected void ReshowUsernameInputPanel()
	{
	}

	public void OnInputFieldValueChanged()
	{
	}
}
