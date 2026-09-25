using System.Collections;
using UnityEngine;
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

	public SystemLanguage overrideLanguageAtStart;

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

	protected string m_CustomURLSchemeAndroidReceived;

	protected bool m_TutorialAudioClipsLoaded;

	private static FeerSceneManager instance;

	public bool changeLanguage => false;

	public bool changeTheme => false;

	public bool initAudioFinished => false;

	public bool customURLReceived
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public bool friendInvitationReceived
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public string friendInvitationName
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public string invitationCode
	{
		get
		{
			return null;
		}
		set
		{
		}
	}

	public bool sceneFeerLoaded => false;

	public static FeerSceneManager Instance => null;

	private void Awake()
	{
	}

	private void Start()
	{
	}

	private IEnumerator ListenForCustomURLSchemes(float listeningTime)
	{
		return null;
	}

	public void StopListeningForURLSchemes()
	{
	}

	public void StartListeningForURLSchemes()
	{
	}

	protected void Init()
	{
	}

	private IEnumerator InitAllData()
	{
		return null;
	}

	public void ChangeTheme(Theme toTheme)
	{
	}

	public void ThemeForestSelected()
	{
	}

	public void ThemeFactorySelected()
	{
	}

	public void InitAllDataFinished()
	{
	}

	private IEnumerator LoadLevelFeer()
	{
		return null;
	}

	public void ChangeLanguage(SystemLanguage toLanguage)
	{
	}

	public void ChangeLanguageFinished()
	{
	}

	protected IEnumerator ShowGameState(GameStateName stateName)
	{
		return null;
	}

	public void ChangeThemeFinished(bool playTutorial)
	{
	}

	protected IEnumerator ReloadSceneLanguage(SystemLanguage toLanguage)
	{
		return null;
	}

	protected IEnumerator ReloadSceneTheme(Theme toTheme)
	{
		return null;
	}

	public void ResetAllData()
	{
	}

	private IEnumerator ResetDataIE()
	{
		return null;
	}

	public void CustomURLSchemeReceived(string urlString)
	{
	}

	protected void CheckInviteURLScheme(string[] urlParams)
	{
	}

	private IEnumerator CustomUrlWaitForInit(bool friendInvite)
	{
		return null;
	}

	public void FriendInvitationReceived(bool success, string friendsname)
	{
	}
}
