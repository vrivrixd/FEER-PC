using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateMenuLeaderboard : GameState
{
	protected bool m_Init;

	protected int m_SelectedDisplay = -1;

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

	protected HighscoreDataUI[] m_HighscoreGameObjects = new HighscoreDataUI[100];

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

	protected int m_CurrentDisplayNumber = 1;

	protected bool m_PlayerRegistered;

	protected bool m_GlobalHighscoreLoaded;

	protected bool m_FriendsLoaded;

	protected bool m_TimeOutRequestUsername;

	protected bool m_TimeOutRequestInvitation;

	protected bool m_TimeOutRequestRemoveFriend;

	protected bool m_TimeOutRequestInvitationCode;

	protected bool m_UsernameFinished;

	protected int m_UserRankGlobal = -1;

	protected int m_UserRankFriends = -1;

	protected bool m_SubmittedUsername;

	protected float m_TimeOutTime = 10f;

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

	protected int m_CurrentStatus = 1;

	protected const int c_STATUS_LOADING = 1;

	protected const int c_STATUS_ERROR = 2;

	protected const int c_STATUS_DISPLAY = 3;

	protected bool m_AddFriendClicked;

	protected bool m_InvitationReceived;

	protected string m_FriendsName;

	protected bool m_ProcessInvitation;

	protected string m_strPanelAnnouncement;

	protected string m_FriendToRemove;

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	private void SetTitles(int display)
	{
		if (display == 2)
		{
			highscoreTitle.text = m_highscoreTitle;
			highscoreSubtitle.text = m_highscoreSubtitle;
			accessibleHighscoreTitle.m_Text = m_ttsHighscoreTitle + ".\n" + m_highscoreSubtitle;
		}
		else if (display == 1)
		{
			highscoreTitle.text = m_friendsTitle;
			highscoreSubtitle.text = m_friendsSubtitle;
			accessibleHighscoreTitle.m_Text = m_friendsTitle + ".\n" + m_friendsSubtitle;
		}
	}

	private void SetNavColors(int display)
	{
		if (display == 2)
		{
			m_NavFriendBtnText.color = Color.white;
			m_NavGlobalBtnText.color = highlightColor;
		}
		else if (display == 1)
		{
			m_NavFriendBtnText.color = highlightColor;
			m_NavGlobalBtnText.color = Color.white;
		}
	}

	public override void Enter(GameState from)
	{
		blockInputPanel.SetActive(false);
		m_CurrentStatus = 1;
		m_AddFriendClicked = false;
		m_InvitationReceived = false;
		m_ProcessInvitation = false;
		m_SubmittedUsername = false;
		sharePopUpAndroid.SetActive(false);
		string nickname = DataManager.Instance.playerData.highscoreNickname;
		m_PlayerRegistered = !nickname.Equals("") && !string.IsNullOrEmpty(nickname);
		if (!m_Init)
		{
			Init();
		}
		StartLoading();
		SetTitles(m_CurrentDisplayNumber);
		SetNavColors(m_CurrentDisplayNumber);
		highscoreBoardPanel.SetActive(true);
		gameObject.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (m_strPanelAnnouncement == null)
			{
				m_strPanelAnnouncement = L("tts_highscore");
			}
			UAP_AccessibilityManager.Say(m_strPanelAnnouncement, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		}
		navPanel.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, true, GameStateName.MenuLeaderboard);
		if (from == null || from.GetName() != GameStateName.Init)
		{
			return;
		}
		// Convite de amigo recebido por link (mentalhomefeer://); no PC nao acontece.
		if (!m_PlayerRegistered)
		{
			m_ProcessInvitation = true;
			m_InvitationReceived = true;
			usernameInputField.text = "";
			usernameSubmitBtn.gameObject.SetActive(true);
			inputField.gameObject.SetActive(true);
			accesibleInputField.m_Text = L("Your Nickname");
			enterUserNamePanel.SetActive(true);
			m_TickInputField = !UAP_AccessibilityManager.IsEnabled();
		}
		else
		{
			m_FriendsName = FeerSceneManager.Instance.friendInvitationName;
			ShowFriendsPanel(m_FriendsName);
			FeerSceneManager.Instance.friendInvitationName = "";
		}
		FeerSceneManager.Instance.friendInvitationReceived = false;
	}

	public override GameStateName GetName()
	{
		return GameStateName.MenuLeaderboard;
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
		switch (infoMessage)
		{
		case InfoMessage.GlobalHighscoreLoadedSuccess:
			if (m_CurrentStatus == 1)
			{
				StopAllCoroutines();
				ExtractGlobalHighscoreData(additionalData);
			}
			break;
		case InfoMessage.GlobalHighscoreLoadedFailed:
		case InfoMessage.UpdatePlayerServerDataFailed:
		case InfoMessage.FriendsHighscoreLoadedFailed:
		case InfoMessage.UpdatePlayerDataAndLoadHighscoresFailed:
			if (m_CurrentStatus == 1)
			{
				StopAllCoroutines();
				ConnectionError();
			}
			break;
		case InfoMessage.SetNicknameFailed:
			if (!m_TimeOutRequestUsername)
			{
				StopAllCoroutines();
				UsernameError();
			}
			break;
		case InfoMessage.SetNicknameSuccess:
			if (!m_TimeOutRequestUsername)
			{
				StopAllCoroutines();
				UsernameSuccess();
			}
			break;
		case InfoMessage.UpdatePlayerServerDataSuccess:
			if (m_CurrentStatus == 1)
			{
				StopAllCoroutines();
				DataManager.Instance.LoadFriendsHighscore();
				DataManager.Instance.LoadGlobalHighscoreList();
			}
			break;
		case InfoMessage.FriendsHighscoreLoadedSuccess:
			if (m_CurrentStatus == 1)
			{
				StopAllCoroutines();
				ExtractFriendsHighscoreData(additionalData);
			}
			break;
		case InfoMessage.RequestFriendInvitationCodeFailed:
			if (!m_TimeOutRequestInvitation)
			{
				StopAllCoroutines();
				RequestInvitationError();
			}
			break;
		case InfoMessage.RequestFriendInvitationCodeSuccess:
			if (!m_TimeOutRequestInvitation)
			{
				StopAllCoroutines();
				OpenShareDialog(additionalData);
			}
			break;
		case InfoMessage.RemoveFriendSuccess:
			if (!m_TimeOutRequestRemoveFriend)
			{
				StopAllCoroutines();
				RemoveFriendSuccess();
			}
			break;
		case InfoMessage.RemoveFriendFailed:
			if (!m_TimeOutRequestRemoveFriend)
			{
				StopAllCoroutines();
				RemoveFriendError();
			}
			break;
		case InfoMessage.RedeemInvitationSuccess:
			if (!m_TimeOutRequestInvitationCode)
			{
				StopAllCoroutines();
				OnInvitationSuccess(additionalData);
			}
			break;
		case InfoMessage.RedeemInvitationFailed:
			if (!m_TimeOutRequestInvitationCode)
			{
				StopAllCoroutines();
				OnInvitationFailed();
			}
			break;
		case InfoMessage.UpdatePlayerDataAndLoadHighscoresSuccess:
			if (m_CurrentStatus == 1)
			{
				StopAllCoroutines();
				ExtractHighscoreData(additionalData);
			}
			break;
		}
	}

	public override void Tick()
	{
		if (m_TickInputField)
		{
			inputField.Tick();
		}
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
		highscoreResults.SetActive(false);
		highscoreScrollView.gameObject.SetActive(false);
		highscoreStarPanel.SetActive(false);
		if (m_CurrentStatus == 3)
		{
			CloseDisplay();
		}
		connectionErrorPanel.SetActive(false);
		loadingWheel.SetActive(false);
		navPanel.SetActive(false);
		scoreCoinsPanel.Hide();
		m_TickInputField = false;
		m_AddFriendClicked = false;
		m_InvitationReceived = false;
		m_ProcessInvitation = false;
	}

	private IEnumerator TimeOut(float timeOut, Action onTimeOut)
	{
		float timePassed = 0f;
		while (timePassed < timeOut)
		{
			timePassed += Time.deltaTime;
			yield return null;
		}
		onTimeOut();
	}

	private IEnumerator TimeOutRequestUsername(float timeOut)
	{
		return TimeOut(timeOut, delegate
		{
			if (!m_UsernameFinished)
			{
				m_TimeOutRequestUsername = true;
				UsernameError();
			}
		});
	}

	private IEnumerator TimeOutRedeemInvitation(float timeOut)
	{
		return TimeOut(timeOut, delegate
		{
			m_TimeOutRequestInvitationCode = true;
			OnInvitationFailed();
		});
	}

	private IEnumerator TimeOutRequestRemoveFriend(float timeOut)
	{
		return TimeOut(timeOut, delegate
		{
			m_TimeOutRequestRemoveFriend = true;
			RemoveFriendError();
		});
	}

	private IEnumerator TimeOutRequestInvitation(float timeOut)
	{
		return TimeOut(timeOut, delegate
		{
			m_TimeOutRequestInvitation = true;
			RequestInvitationError();
		});
	}

	public void TryConnectionAgainBtnClicked()
	{
		StopAllCoroutines();
		connectionErrorPanel.SetActive(false);
		StartLoading();
	}

	public void SubmitYourScoreBtnClicked()
	{
		usernameInputField.text = "";
		usernameSubmitBtn.gameObject.SetActive(true);
		inputField.gameObject.SetActive(true);
		accesibleInputField.m_Text = L("Your Nickname");
		enterUserNamePanel.SetActive(true);
		m_TickInputField = !UAP_AccessibilityManager.IsEnabled();
	}

	public void OnInputFieldValueChanged()
	{
		string text = usernameInputField.text;
		if (text.Equals(""))
		{
			accesibleInputField.m_Text = L("Your Nickname");
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
		StopAllCoroutines();
		usernameSubmitBtn.EnableButton(false);
		inputField.gameObject.SetActive(false);
		usernameSubmitBtn.gameObject.SetActive(false);
		usernameCancelBtn.SetActive(false);
		usernameText.SetActive(false);
		usernameLoadingWheel.SetActive(true);
		m_TickInputField = false;
		inputField.Reset();
		m_SubmittedUsername = true;
		m_TimeOutRequestUsername = false;
		m_UsernameFinished = false;
		StartCoroutine(TimeOutRequestUsername(m_TimeOutTime));
		DataManager.Instance.SetNickname(value);
	}

	// PORT: sucesso so acontece com o servidor online (removido).
	protected void UsernameSuccess()
	{
		m_UsernameFinished = true;
		m_PlayerRegistered = true;
		enterUserNamePanel.SetActive(false);
		usernameLoadingWheel.SetActive(false);
		StartLoading();
	}

	public void UsernameCancelBtnClicked()
	{
		m_TickInputField = false;
		inputField.Reset();
		enterUserNamePanel.SetActive(false);
		usernameSubmitBtn.EnableButton(false);
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

	protected void ReshowUsernameInputPanel()
	{
		usernameLoadingWheel.SetActive(false);
		accesibleInputField.m_Text = L("Your Nickname");
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

	protected void DisplayUserRank(int rank, int panel)
	{
	}

	public void OnNavGlobalClicked()
	{
		if (m_CurrentDisplayNumber == 2)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessibleHighscorePanel, true);
			}
			return;
		}
		StartCoroutine(ChangeDisplay(2));
	}

	public void OnNavFriendsClicked()
	{
		if (m_CurrentDisplayNumber == 1)
		{
			if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.SelectElement(accessibleHighscorePanel, true);
			}
			return;
		}
		StartCoroutine(ChangeDisplay(1));
	}

	public void OnFriendElementBtnClicked(string friendname, string friendId)
	{
	}

	public void AddFriendBtnClicked()
	{
	}

	public void RemoveASelectedFriendBtnClicked()
	{
		RemoveAFriendPanel.SetActive(false);
	}

	public void CancelRemoveFriendClicked()
	{
		RemoveAFriendPanel.SetActive(false);
	}

	public override bool PortBack()
	{
		if (!gameObject.activeSelf)
		{
			return false;
		}
		if (sharePopUpAndroid.activeSelf)
		{
			ShareAndroidCancelBtnClicked();
		}
		else if (connecttionErrorPopUp.activeSelf)
		{
			OnConnectionErrorPopUpClosed();
		}
		else if (RemoveAFriendPanel.activeSelf)
		{
			CancelRemoveFriendClicked();
		}
		else if (enterUserNamePanel.activeSelf)
		{
			if (usernameOkBtn.activeSelf)
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
			MenuBtnClicked();
		}
		return true;
	}

	public void MenuBtnClicked()
	{
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.Menu));
		}
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		highscoreBoardPanel.GetComponent<Animation>().Play("PanelSlideOut");
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		CustomGameManager.Instance.SwitchState(toState);
	}

	protected void Init()
	{
		for (int i = 0; i < m_HighscoreGameObjects.Length; i++)
		{
			HighscoreDataUI element = UnityEngine.Object.Instantiate(highscoreDataPrefab);
			element.transform.SetParent(highscoreContentContainer.transform);
			element.transform.localScale = new Vector3(1f, 1f, 1f);
			element.btn.onClick.AddListener(PlaySingleClick);
			element.gameObject.SetActive(false);
			m_HighscoreGameObjects[i] = element;
		}
		m_ttsLeaderboardRank1 = L("tts_leaderboard_rank_1");
		if (m_ttsLeaderboardRank1.Length > 0)
		{
			m_ttsLeaderboardRank1 += " ";
		}
		m_ttsLeaderboardRank2 = L("tts_leaderboard_rank_2");
		if (m_ttsLeaderboardRank2.Length > 0)
		{
			m_ttsLeaderboardRank2 = " " + m_ttsLeaderboardRank2;
		}
		m_ttsLeaderboardRankPoints = " " + L("points");
		m_ttsUserRank1 = L("Congrats") + "! " + L("You rank on the") + " ";
		m_ttsUserRank2 = " " + L("place of the global leaderboard");
		m_ttsUserRank3 = " " + L("place of the friends board");
		m_UAPRankText = txtRank.gameObject.GetComponent<UAP_BaseElement>();
		m_friendsTitle = L("FRIENDS BOARD");
		m_friendsSubtitle = L("COMPETE WITH YOUR FRIENDS");
		m_highscoreTitle = L("HIGHSCORE BOARD");
		m_highscoreSubtitle = L("COMPETE WITH THE BEST");
		m_ttsHighscoreTitle = L("tts_highscore_board");
		m_NavFriendBtnText = navFriendsBtn.GetComponentInChildren<Text>();
		m_NavGlobalBtnText = navGlobalBtn.GetComponentInChildren<Text>();
		m_Init = true;
	}

	public void PlaySingleClick()
	{
		uiSingleClick.Play();
	}

	protected void StartLoading()
	{
		m_CurrentStatus = 1;
		loadingWheel.SetActive(true);
		m_GlobalHighscoreLoaded = false;
		m_FriendsLoaded = false;
		m_LoadingFinished = false;
		m_TimeOutRequest = false;
		m_UserRankGlobal = -1;
		m_UserRankFriends = -1;
		DataManager.Instance.UpdatePlayerDataAndLoadHighscores(m_PlayerRegistered, true);
		if (gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(TimeOutRequest(m_TimeOutTime));
		}
	}

	private IEnumerator TimeOutRequest(float timeOut)
	{
		float timePassed = 0f;
		while (timePassed < timeOut)
		{
			timePassed += Time.deltaTime;
			yield return null;
		}
		if (!m_LoadingFinished && m_CurrentStatus == 1)
		{
			m_TimeOutRequest = true;
			ConnectionError();
		}
	}

	protected void ConnectionError()
	{
		m_CurrentStatus = 2;
		loadingWheel.SetActive(false);
		if (m_AddFriendClicked)
		{
			m_AddFriendClicked = false;
			blockInputPanel.SetActive(false);
		}
		if (m_InvitationReceived)
		{
			m_InvitationReceived = false;
			blockInputPanel.SetActive(false);
		}
		connectionErrorPanel.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.SelectElement(connErrorText, true);
		}
	}

	// PORT: placar online removido; os dados do servidor nunca chegam.
	protected void ExtractHighscoreData(string receivedData)
	{
		ConnectionError();
	}

	protected void ExtractGlobalHighscoreData(string receivedData)
	{
		ConnectionError();
	}

	protected void ExtractFriendsHighscoreData(string receivedData)
	{
		ConnectionError();
	}

	protected void DisplayScores()
	{
	}

	private IEnumerator SelectVOElement(GameObject elementToSelect, bool forceRepeat)
	{
		yield return null;
		UAP_AccessibilityManager.SelectElement(elementToSelect, forceRepeat);
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
		blockInputPanel.SetActive(false);
	}

	protected void RequestInvitationError()
	{
		blockInputPanel.SetActive(false);
	}

	public void OnConnectionErrorPopUpClosed()
	{
		connecttionErrorPopUp.SetActive(false);
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

	private IEnumerator ChangeDisplay(int toDisplay)
	{
		SetNavColors(toDisplay);
		highscoreBoardPanel.GetComponent<Animation>().Play("PanelSlideOut");
		yield return new WaitForSeconds(0.5f);
		highscoreResults.SetActive(false);
		highscoreScrollView.gameObject.SetActive(false);
		highscoreStarPanel.SetActive(false);
		CloseDisplay();
		m_CurrentDisplayNumber = toDisplay;
		SetTitles(toDisplay);
		if (m_CurrentStatus == 3)
		{
			DisplayScores();
		}
		highscoreBoardPanel.GetComponent<Animation>().Play("PanelSlideIn");
		yield return new WaitForSeconds(0.5f);
	}

	protected void CloseDisplay()
	{
		if (m_CurrentDisplayNumber == 2)
		{
			submitScoreBtn.SetActive(false);
		}
		else if (m_CurrentDisplayNumber == 1)
		{
			noFriendsText.SetActive(false);
			addFriendBtn.SetActive(false);
		}
	}
}
