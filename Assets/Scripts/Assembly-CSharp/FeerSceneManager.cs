using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FeerSceneManager : MonoBehaviour
{
	public GameObject blackCanvas;

	public GameObject loadingCanvas;

	public GameObject loadingWheelUAP;

	public GameObject loadingTutorial;

	public GameObject initCanvas;

	public AudioSource initAudio;

	public Image initBackgroundImage;

	public Sprite initBackground3To4;

	public SelectThemeInputController themeSelectController;

	public GameObject selectThemeCanvas;

	public GameObject selectThemeOptions;

	public GameObject selectThemeLoading;

	public bool debugMode;

	public string overrideVersionNumber;

	public int overrideCoinsAtStart;

	public bool showTutorial;

	public SystemLanguage overrideLanguageAtStart = SystemLanguage.Unknown;

	public bool showAppRateDialog;

	public bool fakeNewHighscore;

	public bool showHighscoreUsernameInput;

	public bool showHighscoreSubmitNameError;

	public bool showHighscoreGlobalRank;

	public bool showHighscoreFriendsRank;

	public bool useLocalServer;

	public string debugCustomURLNick;

	public bool useCustomStatsFile;

	public PlayerStats_v_1_1_6 customPlayerStats;

	public bool useCustomMissionSetData;

	public MissionSetData customMissionSetData;

	public bool showNewVersionInfo;

	public bool showUpdateAvailableInfo;

	public bool stopTrack;

	public bool ignorePause;

	public bool overrideThemeData;

	public bool themeFactoryPurchased;

	public bool themeFactoryTutorialPlayed;

	public bool themeFactoryWaitingForApproval;

	public bool useSimulatedIAPPurchaser;

	protected UAP_BaseElement m_ChangingLanguageUAP;

	protected bool m_ChangeLanguage;

	protected bool m_ChangeTheme;

	protected bool m_InitAudioFinished;

	protected bool m_CustomURLReceived;

	protected bool m_FriendInvitationReceived;

	protected string m_FriendInvitationName;

	protected string m_InvitationCode;

	protected bool m_sceneLoaded;

	protected bool m_sceneCurrentlyLoading;

	protected Coroutine androidURLSchemeListener;

	protected string m_CustomURLSchemeAndroidReceived = "";

	protected bool m_TutorialAudioClipsLoaded;

	private static FeerSceneManager instance;

	public bool changeLanguage => m_ChangeLanguage;

	public bool changeTheme => m_ChangeTheme;

	public bool initAudioFinished => m_InitAudioFinished;

	public bool customURLReceived
	{
		get
		{
			return m_CustomURLReceived;
		}
		set
		{
			m_CustomURLReceived = value;
		}
	}

	public bool friendInvitationReceived
	{
		get
		{
			return m_FriendInvitationReceived;
		}
		set
		{
			m_FriendInvitationReceived = value;
		}
	}

	public string friendInvitationName
	{
		get
		{
			return m_FriendInvitationName;
		}
		set
		{
			m_FriendInvitationName = value;
		}
	}

	public string invitationCode
	{
		get
		{
			return m_InvitationCode;
		}
		set
		{
			m_InvitationCode = value;
		}
	}

	public bool sceneFeerLoaded => m_sceneLoaded;

	public static FeerSceneManager Instance => instance;

	private void Awake()
	{
		m_CustomURLReceived = false;
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	private void Start()
	{
		androidURLSchemeListener = StartCoroutine(ListenForCustomURLSchemes(60f));
		m_ChangingLanguageUAP = loadingWheelUAP.GetComponent<UAP_BaseElement>();
		Init();
	}

	private IEnumerator ListenForCustomURLSchemes(float listeningTime)
	{
		// PORT: on Android this reads the Intent (mentalhomefeer:// links) via AndroidJavaClass. Not available on Windows.
		yield break;
	}

	public void StopListeningForURLSchemes()
	{
		if (androidURLSchemeListener != null)
		{
			StopCoroutine(androidURLSchemeListener);
		}
	}

	public void StartListeningForURLSchemes()
	{
		if (androidURLSchemeListener != null)
		{
			StopCoroutine(androidURLSchemeListener);
		}
		androidURLSchemeListener = StartCoroutine(ListenForCustomURLSchemes(60f));
	}

	protected void Init()
	{
		Application.targetFrameRate = 30;
		Screen.sleepTimeout = -1;
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToLandscapeLeft = true;
		Screen.autorotateToLandscapeRight = true;
		Screen.orientation = ScreenOrientation.LandscapeLeft;
		if ((float)Screen.width / (float)Screen.height < 1.7f)
		{
			initBackgroundImage.sprite = initBackground3To4;
		}
		blackCanvas.SetActive(false);
		initCanvas.SetActive(true);
		// PORT: look for a newer release while the logo is shown (see PortUpdater)
		PortUpdater.StartCheck(this);
		StartCoroutine(InitAllData());
	}

	private IEnumerator InitAllData()
	{
		CustomAnalyticsTracker.Instance.Init();
		DataManager.Instance.Init();
		while (!DataManager.Instance.initFinished)
		{
			yield return null;
		}
		LocalizationManager.Instance.Init();
		while (!LocalizationManager.Instance.initFinished)
		{
			yield return null;
		}
		IAPManager.Instance.Init();
		yield return null;
		UAP_AccessibilityManager.StartPlugin();
		while (!UAP_AccessibilityManager.IsPluginInit())
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		initAudio.Play();
		while (initAudio.isPlaying)
		{
			yield return null;
		}
		Screen.orientation = ScreenOrientation.AutoRotation;
		yield return new WaitForSeconds(1f);
		// PORT: after the logo, offer the update (if any) before the theme selection
		yield return PortUpdater.OfferUpdate();
		if (!DataManager.Instance.playerData.playTutorial)
		{
			selectThemeOptions.SetActive(true);
			selectThemeCanvas.SetActive(true);
			initCanvas.SetActive(false);
		}
		else
		{
			loadingTutorial.SetActive(true);
			DataManager.Instance.SetSelectedTheme(Theme.Forest);
			m_sceneCurrentlyLoading = true;
			StartCoroutine(LoadLevelFeer());
		}
	}

	public void ChangeTheme(Theme toTheme)
	{
		Screen.sleepTimeout = -1;
		m_ChangeTheme = true;
		m_ChangingLanguageUAP.m_Text = LocalizationManager.Instance.GetLocalizedValue("loading");
		loadingCanvas.SetActive(true);
		m_sceneLoaded = false;
		m_sceneCurrentlyLoading = true;
		StartCoroutine(ReloadSceneTheme(toTheme));
	}

	public void ThemeForestSelected()
	{
		DataManager.Instance.SetSelectedTheme(Theme.Forest);
		m_sceneCurrentlyLoading = true;
		StartCoroutine(LoadLevelFeer());
	}

	public void ThemeFactorySelected()
	{
		DataManager.Instance.SetSelectedTheme(Theme.Factory);
		m_sceneCurrentlyLoading = true;
		StartCoroutine(LoadLevelFeer());
	}

	public void InitAllDataFinished()
	{
		m_sceneLoaded = true;
		m_sceneCurrentlyLoading = false;
		if (initCanvas != null)
		{
			initCanvas.SetActive(false);
			UnityEngine.Object.Destroy(initCanvas);
		}
		if (selectThemeCanvas != null)
		{
			selectThemeCanvas.SetActive(false);
			UnityEngine.Object.Destroy(selectThemeCanvas);
		}
		loadingCanvas.SetActive(false);
	}

	private IEnumerator LoadLevelFeer()
	{
		AsyncOperation async = SceneManager.LoadSceneAsync("Feer", LoadSceneMode.Single);
		async.allowSceneActivation = false;
		while (async.progress < 0.9f)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		async.allowSceneActivation = true;
	}

	public void ChangeLanguage(SystemLanguage toLanguage)
	{
		Screen.sleepTimeout = -1;
		// PORT: CustomAnalyticsTracker.LanguageChanged removed (analytics).
		m_ChangeLanguage = true;
		m_ChangingLanguageUAP.m_Text = LocalizationManager.Instance.GetLocalizedValue("changing_language");
		loadingCanvas.SetActive(true);
		m_sceneLoaded = false;
		m_sceneCurrentlyLoading = true;
		StartCoroutine(ReloadSceneLanguage(toLanguage));
	}

	public void ChangeLanguageFinished()
	{
		m_sceneLoaded = true;
		m_sceneCurrentlyLoading = false;
		StartCoroutine(ShowGameState(GameStateName.MenuOptions));
		m_ChangeLanguage = false;
	}

	protected IEnumerator ShowGameState(GameStateName stateName)
	{
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		CustomGameManager.Instance.SwitchState(stateName);
		loadingCanvas.SetActive(false);
	}

	public void ChangeThemeFinished(bool playTutorial)
	{
		m_sceneLoaded = true;
		m_sceneCurrentlyLoading = false;
		StartCoroutine(ShowGameState(playTutorial ? GameStateName.StartTutorial : GameStateName.Menu));
		m_ChangeTheme = false;
	}

	// Waits (without yielding the frame, as in the original) up to 3 s of accumulated deltaTime for speech to start
	private static void WaitForSpeechStart()
	{
		float time = 0f;
		do
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				break;
			}
			time += Time.deltaTime;
		}
		while (time <= 3f);
	}

	protected IEnumerator ReloadSceneLanguage(SystemLanguage toLanguage)
	{
		CustomGameManager.Instance.EnableGameMusic(false);
		CustomGameManager.Instance.EnableMenuMusic(false);
		yield return new WaitForEndOfFrame();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			WaitForSpeechStart();
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
			UAP_AccessibilityManager.BlockInput(true, true);
			yield return new WaitForEndOfFrame();
		}
		DataManager.Instance.SetUserLanguage(toLanguage);
		LocalizationManager.Instance.ChangeLanguage(toLanguage);
		while (!LocalizationManager.Instance.initFinished || !DataManager.Instance.languageChangeFinished)
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
			UAP_AccessibilityManager.BlockInput(false, true);
		}
		StartCoroutine(LoadLevelFeer());
	}

	protected IEnumerator ReloadSceneTheme(Theme toTheme)
	{
		CustomGameManager.Instance.EnableGameMusic(false);
		CustomGameManager.Instance.EnableMenuMusic(false);
		yield return new WaitForEndOfFrame();
		if (UAP_AccessibilityManager.IsEnabled())
		{
			WaitForSpeechStart();
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
			UAP_AccessibilityManager.BlockInput(true, true);
			yield return new WaitForEndOfFrame();
		}
		DataManager.Instance.SetSelectedTheme(toTheme);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
			UAP_AccessibilityManager.BlockInput(false, true);
		}
		StartCoroutine(LoadLevelFeer());
	}

	public void ResetAllData()
	{
		m_sceneLoaded = false;
		m_sceneCurrentlyLoading = true;
		initCanvas.SetActive(true);
		DataManager.Instance.ClearAllData();
		StartCoroutine(ResetDataIE());
	}

	private IEnumerator ResetDataIE()
	{
		UAP_AccessibilityManager.ResetEnabledState();
		DataManager.Instance.Init();
		while (!DataManager.Instance.initFinished)
		{
			yield return null;
		}
		LocalizationManager.Instance.Init();
		while (!LocalizationManager.Instance.initFinished)
		{
			yield return null;
		}
		yield return new WaitForEndOfFrame();
		StartCoroutine(LoadLevelFeer());
	}

	public void CustomURLSchemeReceived(string urlString)
	{
		m_CustomURLReceived = true;
		if (string.IsNullOrEmpty(urlString) || !urlString.StartsWith("mentalhomefeer://", StringComparison.OrdinalIgnoreCase))
		{
			m_CustomURLReceived = false;
			return;
		}
		string[] urlParams = urlString.Split(new char[1] { char.Parse("?") });
		if (string.IsNullOrEmpty(urlParams[0]))
		{
			m_CustomURLReceived = false;
		}
		else if (urlParams[0] == "mentalhomefeer://id_invite")
		{
			CheckInviteURLScheme(urlParams);
		}
	}

	protected void CheckInviteURLScheme(string[] urlParams)
	{
		if (urlParams.Length != 3 || string.IsNullOrEmpty(urlParams[1]))
		{
			m_CustomURLReceived = false;
			return;
		}
		string[] tokenParam = urlParams[1].Split(new char[1] { char.Parse("=") });
		if (tokenParam.Length != 2 || string.IsNullOrEmpty(tokenParam[0]) || string.IsNullOrEmpty(tokenParam[1]) || !tokenParam[0].Equals("token"))
		{
			return;
		}
		m_InvitationCode = tokenParam[1];
		if (string.IsNullOrEmpty(urlParams[2]))
		{
			m_CustomURLReceived = false;
			return;
		}
		string[] hashParam = urlParams[2].Split(new char[1] { char.Parse("=") });
		if (hashParam.Length != 2 || string.IsNullOrEmpty(hashParam[0]) || string.IsNullOrEmpty(hashParam[1]) || !hashParam[0].Equals("hash"))
		{
			return;
		}
		string hash = hashParam[1];
		string expected = HashGenerator.Md5Sum(m_InvitationCode + "a3afWoin6sR3TmRinMpN");
		if (!hash.Equals(expected))
		{
			m_CustomURLReceived = false;
			return;
		}
		if (m_sceneLoaded)
		{
			m_sceneLoaded = false;
			m_sceneCurrentlyLoading = true;
			Screen.sleepTimeout = -1;
			m_ChangingLanguageUAP.m_Text = LocalizationManager.Instance.GetLocalizedValue("loading");
			loadingCanvas.SetActive(true);
			StartCoroutine(LoadLevelFeer());
		}
		StartCoroutine(CustomUrlWaitForInit(true));
	}

	private IEnumerator CustomUrlWaitForInit(bool friendInvite)
	{
		while (!DataManager.Instance.initFinished)
		{
			yield return null;
		}
		if (!friendInvite)
		{
			m_CustomURLReceived = false;
			yield break;
		}
		string nickname = DataManager.Instance.playerData.highscoreNickname;
		if (!nickname.Equals("") && !string.IsNullOrEmpty(DataManager.Instance.playerData.highscoreNickname))
		{
			DataManager.Instance.RedeemFriendInvitationCode(m_InvitationCode, false);
			m_InvitationCode = "";
		}
		else
		{
			m_FriendInvitationReceived = true;
		}
	}

	public void FriendInvitationReceived(bool success, string friendsname)
	{
		if (success)
		{
			JsonServerResponseRedeemInvitationCode jsonServerResponseRedeemInvitationCode = JsonUtility.FromJson<JsonServerResponseRedeemInvitationCode>(friendsname);
			m_FriendInvitationReceived = true;
			m_FriendInvitationName = jsonServerResponseRedeemInvitationCode.friendName;
		}
		else
		{
			m_CustomURLReceived = false;
		}
	}
}
