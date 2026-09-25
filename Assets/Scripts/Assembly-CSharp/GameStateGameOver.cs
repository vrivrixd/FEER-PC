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

	protected bool m_WaitForServer;

	protected bool m_IsNewHighscore;

	protected int m_SelectedLeaderBoardPanel;

	protected int m_UserRank;

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

	protected int m_PlayerGlobalRank;

	protected int m_PlayerFriendsRank;

	protected int m_MinLeaderboardScore;

	public override void Enter(GameState from)
	{
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
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

	public override void Exit(GameState to)
	{
	}

	protected void Init()
	{
	}

	private IEnumerator TimeOutRequestWebUsername(float timeOut)
	{
		return null;
	}

	protected void HideInteractivePanels()
	{
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	private IEnumerator StartGameOverMissionAnimation()
	{
		return null;
	}

	protected void UpdateMissionSet()
	{
	}

	public void NextMissionSetReceived(bool nextMissionAvailable)
	{
	}

	private IEnumerator StartCounterPanelScoreAnimation(bool accessible)
	{
		return null;
	}

	private IEnumerator CounterPanelScoreAnimation(bool accessible)
	{
		return null;
	}

	private IEnumerator CounterPanelLightsAnimation()
	{
		return null;
	}

	private IEnumerator StartLightAnimation()
	{
		return null;
	}

	private IEnumerator ShowHighscorePanel(bool fromUsernameSubmit)
	{
		return null;
	}

	protected bool IsNewHighscore()
	{
		return false;
	}

	protected void ShowUsernameInputPanel()
	{
	}

	public void OnInputFieldValueChanged()
	{
	}

	public void UsernameSubmitBtnClicked()
	{
	}

	protected void UsernameError()
	{
	}

	protected void UsernameSuccess()
	{
	}

	public void CancelBtnClicked()
	{
	}

	public void UsernameOKBtnClicked()
	{
	}

	protected void ShowCongratsOnlyPanel()
	{
	}

	private IEnumerator SayCongratsText()
	{
		return null;
	}

	protected void ShowRankPanel(bool fromUsernameSubmit)
	{
	}

	public void OKButtonClicked()
	{
	}

	protected void ReturnToCounterPanel()
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

	public void PlayAgainBtnClicked()
	{
	}

	public void BackToMainMenuBtnClicked()
	{
	}

	public void RateAppDialogClosed()
	{
	}

	public void NavNextBtnClicked()
	{
	}
}
