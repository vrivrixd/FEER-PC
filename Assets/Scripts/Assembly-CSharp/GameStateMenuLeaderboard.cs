using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateMenuLeaderboard : GameState
{
	protected bool m_Init;

	protected int m_SelectedDisplay;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject highscoreBoardPanel;

	public GameObject connectionErrorPanel;

	public GameObject navPanel;

	public RectTransform panelTransform;

	public GameObject RemoveAFriendPanel;

	public GameObject blockInputPanel;

	public GameObject connecttionErrorPopUp;

	public Text connErrorPopUpTitle;

	public Text connErrorPopUpMessage;

	public Button navFriendsBtn;

	public Button navGlobalBtn;

	protected Text m_NavFriendBtnText;

	protected Text m_NavGlobalBtnText;

	public GameObject highscoreResults;

	public Text highscoreTitle;

	public UAP_BaseElement accessibleHighscoreTitle;

	public Text highscoreSubtitle;

	public HighscoreDataUI highscoreDataPrefab;

	public GameObject highscoreContentContainer;

	public GameObject highscoreStarPanel;

	public Text txtRank;

	public ScrollRect highscoreScrollView;

	public GameObject loadingWheel;

	public Color dataColor;

	public Color levelColor;

	public Color highlightColor;

	public GameObject submitScoreBtn;

	public GameObject addFriendBtn;

	public GameObject noFriendsText;

	protected HighscoreDataUI[] m_HighscoreGameObjects;

	protected bool m_LoadingFinished;

	protected HighscoreData m_GlobalHighscoreData;

	protected HighscoreFriendsData m_FriendsHighscoreData;

	public InputField usernameInputField;

	public ButtonUI usernameSubmitBtn;

	public InputFieldUI inputField;

	public GameObject usernameCancelBtn;

	public GameObject usernameLoadingWheel;

	public GameObject usernameText;

	public GameObject usernameOkBtn;

	public GameObject usernameErrorText;

	public UAP_BaseElement accesibleInputField;

	public GameObject enterUserNamePanel;

	protected bool m_SubmitScoreBtnVisible;

	protected bool m_TickInputField;

	public GameObject sharePopUpAndroid;

	public Dropdown shareAndroidDropDown;

	protected bool m_SharePopUpAndroidPopulated;

	public GameObject connErrorText;

	public Text friendRemoveText;

	public AudioSource uiSingleClick;

	public GameObject accessibleHighscorePanel;

	protected bool m_TimeOutRequest;

	protected const int c_HIGHSCORE_TABLE = 2;

	protected const int c_FRIENDS_TABLE = 1;

	protected int m_CurrentDisplayNumber;

	protected bool m_PlayerRegistered;

	protected bool m_GlobalHighscoreLoaded;

	protected bool m_FriendsLoaded;

	protected bool m_TimeOutRequestUsername;

	protected bool m_TimeOutRequestInvitation;

	protected bool m_TimeOutRequestRemoveFriend;

	protected bool m_TimeOutRequestInvitationCode;

	protected bool m_UsernameFinished;

	protected int m_UserRankGlobal;

	protected int m_UserRankFriends;

	protected bool m_SubmittedUsername;

	protected float m_TimeOutTime;

	protected string m_ttsLeaderboardRank1;

	protected string m_ttsLeaderboardRank2;

	protected string m_ttsLeaderboardRankPoints;

	protected string m_ttsUserRank1;

	protected string m_ttsUserRank2;

	protected string m_ttsUserRank3;

	protected string m_friendsTitle;

	protected string m_friendsSubtitle;

	protected string m_highscoreTitle;

	protected string m_highscoreSubtitle;

	protected string m_ttsHighscoreTitle;

	protected string m_FriendInvitationCode;

	protected UAP_BaseElement m_UAPRankText;

	protected int m_CurrentStatus;

	protected const int c_STATUS_LOADING = 1;

	protected const int c_STATUS_ERROR = 2;

	protected const int c_STATUS_DISPLAY = 3;

	protected bool m_AddFriendClicked;

	protected bool m_InvitationReceived;

	protected string m_FriendsName;

	protected bool m_ProcessInvitation;

	protected string m_strPanelAnnouncement;

	protected string m_FriendToRemove;

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

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override void Tick()
	{
	}

	public override void Exit(GameState to)
	{
	}

	private IEnumerator TimeOutRequestUsername(float timeOut)
	{
		return null;
	}

	private IEnumerator TimeOutRedeemInvitation(float timeOut)
	{
		return null;
	}

	private IEnumerator TimeOutRequestRemoveFriend(float timeOut)
	{
		return null;
	}

	private IEnumerator TimeOutRequestInvitation(float timeOut)
	{
		return null;
	}

	public void TryConnectionAgainBtnClicked()
	{
	}

	public void SubmitYourScoreBtnClicked()
	{
	}

	public void OnInputFieldValueChanged()
	{
	}

	public void UsernameSubmitBtnClicked()
	{
	}

	protected void UsernameSuccess()
	{
	}

	public void UsernameCancelBtnClicked()
	{
	}

	protected void UsernameError()
	{
	}

	public void UsernameErrorOKClicked()
	{
	}

	protected void ReshowUsernameInputPanel()
	{
	}

	protected void DisplayUserRank(int rank, int panel)
	{
	}

	public void OnNavGlobalClicked()
	{
	}

	public void OnNavFriendsClicked()
	{
	}

	public void OnFriendElementBtnClicked(string friendname, string friendId)
	{
	}

	public void AddFriendBtnClicked()
	{
	}

	public void RemoveASelectedFriendBtnClicked()
	{
	}

	public void CancelRemoveFriendClicked()
	{
	}

	public void MenuBtnClicked()
	{
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	protected void Init()
	{
	}

	public void PlaySingleClick()
	{
	}

	protected void StartLoading()
	{
	}

	private IEnumerator TimeOutRequest(float timeOut)
	{
		return null;
	}

	protected void ConnectionError()
	{
	}

	protected void ExtractHighscoreData(string receivedData)
	{
	}

	protected void ExtractGlobalHighscoreData(string receivedData)
	{
	}

	protected void ExtractFriendsHighscoreData(string receivedData)
	{
	}

	protected void DisplayScores()
	{
	}

	private IEnumerator SelectVOElement(GameObject elementToSelect, bool forceRepeat)
	{
		return null;
	}

	protected void RequestAddFriendCode()
	{
	}

	protected void ProcessInvitation()
	{
	}

	protected void OnInvitationSuccess(string friendname)
	{
	}

	protected void ShowFriendsPanel(string friendname)
	{
	}

	protected void OnInvitationFailed()
	{
	}

	protected void RequestInvitationError()
	{
	}

	public void OnConnectionErrorPopUpClosed()
	{
	}

	protected void RemoveFriendError()
	{
	}

	protected void RemoveFriendSuccess()
	{
	}

	protected void RequestInvitationSuccess(string code)
	{
	}

	public void OpenShareDialog(string code)
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

	private IEnumerator ChangeDisplay(int toDisplay)
	{
		return null;
	}

	protected void CloseDisplay()
	{
	}
}
