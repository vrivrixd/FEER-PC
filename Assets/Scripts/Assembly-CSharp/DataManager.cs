using System;
using System.Collections;
using UnityEngine;

public class DataManager : MonoBehaviour
{
	public string versionNumber;

	public GameMode gameMode;

	protected string m_Base64_Auth;

	protected const string c_SERVER_USER_KEY = "awCHnzsU34yNBNzyz23s";

	protected const string c_SERVER_PWD_KEY = "Q2j2ArAjX6i7AKKT5pRS";

	protected const string c_SERVER_URL = "https://www.mentalgames.eu/dbapps/feer/FvP5fpJbNxbuWGKCDPpq2UKGm3d9cCyWbyMhtDfV/";

	protected const string c_DEBUG_SERVER_URL = "http://127.0.0.1:8080/mentalgames-local/dbapps/feer/FvP5fpJbNxbuWGKCDPpq2UKGm3d9cCyWbyMhtDfV/";

	protected const string c_SERVER_HASH_KEY = "3BYH2jYHGe5goxmUEUUi";

	protected const string c_INVITE_FRIEND_REQUEST_URL = "https://www.mentalgames.eu/dbapps/feer/AddFriend.php?token=";

	protected const string c_HASH_KEY_MESSAGE = "a3afWoin6sR3TmRinMpN";

	protected string m_tempNickname;

	public PowerUp[] powerUps;

	public int skipCosts;

	public int skipAdditionalCostsPerLevel;

	public int maxSkipCosts;

	public int maxAvailableMissions;

	public int saveMeCosts;

	public int saveMeAdditionalMultiplier;

	public int saveMeTimes;

	public int maxScoreCoinsValue;

	protected PlayerData_v_1_1_3 m_PlayerData;

	protected PlayerThemeData m_PlayerThemeData;

	protected AppRateData m_AppRateData;

	protected PlayerStats_v_1_1_6 m_PlayerStats;

	protected PlayerRemoteSettings_v_1_1_9 m_PlayerRemoteSettings;

	protected bool m_isVersionUpdate;

	protected bool m_UpdateAvailable;

	protected bool m_ForceToUpdate;

	protected const string c_MissionsFile = "/PlayerMissions.dat";

	protected const string c_PlayerDataFile = "/PlayerData.dat";

	protected const string c_PlayerStatsFile = "/PlayerStats.dat";

	protected const string c_PlayerRemoteSettingsFile = "/PlayerRemoteSettings.dat";

	protected const string c_AppRateDataFile = "/AppRateData.dat";

	protected const string c_ShareImageFile = "/Feer-The-Game-App.jpg";

	protected const string c_ShareImageFileStreamingAsset = "Feer-The-Game-App.jpg";

	protected const string c_ThemeFactoryDataFile = "/ThemeFactory.dat";

	protected int m_BOOST;

	protected int m_COIN_MULTIPLIER;

	protected int m_SHIELD;

	protected int m_WEAPON;

	protected AudioClip m_FirstPowerUpInfoClip;

	protected AudioClip m_FirstPowerUpBoostInfoClip;

	protected AudioClip m_FirstPowerUpShieldInfoClip;

	protected AudioClip m_FirstPowerUpCoinDoublerInfoClip;

	protected AudioClip m_FirstPowerUpWeaponInfoClip;

	protected bool m_Init;

	protected int m_InitCount;

	protected int m_CurrentRankLoad;

	protected AudioClip[] m_TutorialClips;

	protected AudioClip m_TutorialHeadphoneClip;

	protected bool m_LanguageChangeFinished;

	protected int m_LanguageChangeCount;

	protected Theme m_SelectedTheme;

	private static DataManager instance;

	public PlayerData_v_1_1_3 playerData => null;

	public PlayerThemeData playerThemeData => null;

	public AppRateData appRateData => null;

	public PlayerStats_v_1_1_6 playerStats => null;

	public PlayerRemoteSettings_v_1_1_9 playerRemoteSettings => null;

	public bool isVersionUpdate => false;

	public bool updateAvailable => false;

	public bool forceToUpdate => false;

	public int BOOST => 0;

	public int COIN_MULTIPLIER => 0;

	public int SHIELD => 0;

	public int WEAPON => 0;

	public AudioClip firstPowerUpInfoClip => null;

	public AudioClip firstPowerUpBoostInfoClip => null;

	public AudioClip firstPowerUpShieldInfoClip => null;

	public AudioClip firstPowerUpCoinDoublerInfoClip => null;

	public AudioClip firstPowerUpWeaponInfoClip => null;

	public bool initFinished => false;

	public bool languageChangeFinished => false;

	public Theme selectedTheme => Theme.Forest;

	public static DataManager Instance => null;

	private void Awake()
	{
	}

	public void Init()
	{
	}

	protected void InitStepFinished()
	{
	}

	public void SetSelectedTheme(Theme theme)
	{
	}

	protected void LoadPlayerRemoteSettings()
	{
	}

	protected void UpdatePlayerRemoteSettingsData(string fromVersionNumber)
	{
	}

	private IEnumerator RemoteSettingsTimeOut()
	{
		return null;
	}

	protected void SavePlayerRemoteSettings()
	{
	}

	private void HandleRemoteUpdate()
	{
	}

	public void RemoteSettingsUpdateCompleted(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
	{
	}

	public void UpdateToNewVersionInfoMessagePresented()
	{
	}

	public string GetInviteFriendRequestUrl()
	{
		return null;
	}

	public string GetHashKeyMessage()
	{
		return null;
	}

	public void LoadGlobalHighscoreList()
	{
	}

	public void GlobalHighscoreListReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void LoadFriendsHighscore()
	{
	}

	public void FriendsHighscoreListReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void RequestFriendInvitationCode()
	{
	}

	public void FriendInvitationCodeReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void RemoveFriend(string friend_id)
	{
	}

	public void RemoveFriendFinished(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void GetMinGlobalHighscore()
	{
	}

	public void MinGlobalHighscoreReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void UpdateAndGetPlayerGlobalRank(int highscore)
	{
	}

	public void UpdateAndGetPlayerGlobalRankReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void UpdateAndGetPlayerServerRanks(int highscore)
	{
	}

	public void UpdateAndGetPlayerServerRanksReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void InitPlayerServerData()
	{
	}

	public void UpdatePlayerDataAndLoadHighscores(bool updatePlayerData, bool informTopState)
	{
	}

	public void UpdatePlayerServerDataAndLoadHighscoresReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void UpdatePlayerServerData(bool informTopState = true)
	{
	}

	public void UpdatePlayerServerDataReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void ChangeNickname(string nicknameNew)
	{
	}

	public void ChangeNicknameReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void SetNickname(string nickname)
	{
	}

	public void SetNicknameReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void RedeemFriendInvitationCode(string invitationCode, bool topState)
	{
	}

	public void FriendInvitationCodeRedeemed(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void FriendInvitationCodeRedeemedTopState(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	public void IncreaseScoreMultiplier()
	{
	}

	public void NewHighscore(int score)
	{
	}

	private IEnumerator InformAnalyticsOfUpgrade(string transactionalItem, int level, int distance)
	{
		return null;
	}

	public bool IncreaseBoostLevel()
	{
		return false;
	}

	public bool IncreaseCoinMultiplierLevel()
	{
		return false;
	}

	public bool IncreaseShieldLevel()
	{
		return false;
	}

	public bool IncreaseWeaponLevel()
	{
		return false;
	}

	public bool InvestCoins(int coins, TransactionContext transactionContext, TransactionalItem itemId, TransactionItemType itemType, string transactionId)
	{
		return false;
	}

	public void SaveCoins(int coins, TransactionContext transactionContext, TransactionalItem itemId, TransactionItemType itemType, string transactionId)
	{
	}

	public void GameOver()
	{
	}

	protected void LoadPlayerThemeData()
	{
	}

	public void SavePlayerThemeData()
	{
	}

	public void ThemeFactoryPurchased(IAPReceiptData receiptData, string originalReceipt)
	{
	}

	public void ThemeFactoryWaitingForApproval(bool waiting)
	{
	}

	protected void UpdatePlayerData(string fromVersionNumber)
	{
	}

	protected void LoadPlayerData()
	{
	}

	public void SetLastRewardedMission(int missionNumber)
	{
	}

	protected void SavePlayerData()
	{
	}

	public void SaveSessionData(DateTime sessionStartTime, long sessionCount, long sessionId)
	{
	}

	public void UpdateVersionGiftReceived()
	{
	}

	public void AppRatingNeverAgain()
	{
	}

	public void SetUserLanguage(SystemLanguage toLanguage)
	{
	}

	public void LanguageChangeStepFinished()
	{
	}

	protected void CreatePlayerID(InfoMessage infoMessage = InfoMessage.None)
	{
	}

	public void PlayerIDReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
	}

	protected IEnumerator ServerPostRequestImplementation(string url, string json, Action<bool, string, InfoMessage> callback, InfoMessage infoMessage = InfoMessage.None)
	{
		return null;
	}

	protected SystemLanguage GetApplicationLanguage()
	{
		return SystemLanguage.Afrikaans;
	}

	protected SystemLanguage GetSystemLanguage()
	{
		return SystemLanguage.Afrikaans;
	}

	protected void CopyImageShareImageFile()
	{
	}

	public void ShareImageRead(bool success, byte[] imageBytes)
	{
	}

	public void LoadImage(string filePath, Action<bool, byte[]> callbackFunction)
	{
	}

	private IEnumerator FetchImageData(string filePath, Action<bool, byte[]> callbackFunction)
	{
		return null;
	}

	protected void UpdatePlayerStatsData(string fromVersionNumber)
	{
	}

	protected void LoadPlayerStats()
	{
	}

	protected void SavePlayerStats()
	{
	}

	public void UpdatePlayerStatsOnGameOver()
	{
	}

	public void UpdatePlayerStatsQuestSkipped()
	{
	}

	protected void LoadAppRateData()
	{
	}

	protected void SaveAppRateData()
	{
	}

	public void LoadFile(string filePath, Action<bool, string> callbackFunction)
	{
	}

	private IEnumerator FetchFileData(string filePath, Action<bool, string> callbackFunction)
	{
		return null;
	}

	public void LoadAudioClipFromStreamingAssets(string filePath, AudioType audioType, Action<bool, AudioClip> callbackFunction)
	{
	}

	private IEnumerator FetchAudioClip(string filePath, AudioType audioType, Action<bool, AudioClip> callbackFunction)
	{
		return null;
	}

	protected void LoadPowerUpAudioClips()
	{
	}

	protected void ReloadPowerUpAudioClips(SystemLanguage forLanguage)
	{
	}

	public void FirstPowerUpInfoClipLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpInfoClipReLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpBoostInfoClipLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpBoostInfoClipReLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpShieldInfoClipLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpShieldInfoClipReLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpWeaponInfoClipLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpWeaponInfoClipReLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpCoinDoublerInfoClipLoaded(bool success, AudioClip clip)
	{
	}

	public void FirstPowerUpCoinDoublerInfoClipReLoaded(bool success, AudioClip clip)
	{
	}

	public MissionSetData LoadMissionSetData()
	{
		return null;
	}

	public void SaveMissionSetData(MissionSetData missionSet)
	{
	}

	public AudioClip[] GetTutorialClips()
	{
		return null;
	}

	public AudioClip GetTutorialHeadphoneClip()
	{
		return null;
	}

	public void LoadTutorialClips(Action<bool> callbackFunction)
	{
	}

	private IEnumerator LoadTutorialClipsIE(Action<bool> callbackFunction)
	{
		return null;
	}

	public void TutorialFinished()
	{
	}

	public void UseVibration(bool enabled)
	{
	}

	public void UseReverseLeftRight(bool enabled)
	{
	}

	public void UseReverseUpDown(bool enabled)
	{
	}

	public void UseCenterLaneOrientation(bool enabled)
	{
	}

	public void FirstGamePlayed()
	{
	}

	public void FirstGameInformationTold()
	{
	}

	public void FirstAccessiblePowerUpBoostTold()
	{
	}

	public void FirstAccessiblePowerUpShieldTold()
	{
	}

	public void FirstAccessiblePowerUpWeaponTold()
	{
	}

	public void FirstAccessiblePowerUpCoinDoublerTold()
	{
	}

	public void SubmitHighscoreAsked()
	{
	}

	public void AppRatingAsked()
	{
	}

	public void ClearAllData()
	{
	}
}
