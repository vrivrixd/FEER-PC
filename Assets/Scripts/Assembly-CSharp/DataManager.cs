using System;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class DataManager : MonoBehaviour
{
	public string versionNumber = "1.1.1";

	public GameMode gameMode;

	protected string m_Base64_Auth = "";

	protected const string c_SERVER_USER_KEY = "awCHnzsU34yNBNzyz23s";

	protected const string c_SERVER_PWD_KEY = "Q2j2ArAjX6i7AKKT5pRS";

	protected const string c_SERVER_URL = "https://www.mentalgames.eu/dbapps/feer/FvP5fpJbNxbuWGKCDPpq2UKGm3d9cCyWbyMhtDfV/";

	protected const string c_DEBUG_SERVER_URL = "http://127.0.0.1:8080/mentalgames-local/dbapps/feer/FvP5fpJbNxbuWGKCDPpq2UKGm3d9cCyWbyMhtDfV/";

	protected const string c_SERVER_HASH_KEY = "3BYH2jYHGe5goxmUEUUi";

	protected const string c_INVITE_FRIEND_REQUEST_URL = "https://www.mentalgames.eu/dbapps/feer/AddFriend.php?token=";

	protected const string c_HASH_KEY_MESSAGE = "a3afWoin6sR3TmRinMpN";

	protected string m_tempNickname;

	public PowerUp[] powerUps;

	public int skipCosts = 300;

	public int skipAdditionalCostsPerLevel = 25;

	public int maxSkipCosts = 5000;

	public int maxAvailableMissions = 99;

	public int saveMeCosts = 400;

	public int saveMeAdditionalMultiplier = 2;

	public int saveMeTimes = 5;

	public int maxScoreCoinsValue = 999999999;

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

	protected int m_COIN_MULTIPLIER = 1;

	protected int m_SHIELD = 2;

	protected int m_WEAPON = 3;

	protected AudioClip m_FirstPowerUpInfoClip;

	protected AudioClip m_FirstPowerUpBoostInfoClip;

	protected AudioClip m_FirstPowerUpShieldInfoClip;

	protected AudioClip m_FirstPowerUpCoinDoublerInfoClip;

	protected AudioClip m_FirstPowerUpWeaponInfoClip;

	protected bool m_Init;

	protected int m_InitCount;

	protected int m_CurrentRankLoad = -1;

	protected AudioClip[] m_TutorialClips;

	protected AudioClip m_TutorialHeadphoneClip;

	protected bool m_LanguageChangeFinished;

	protected int m_LanguageChangeCount;

	protected Theme m_SelectedTheme = Theme.None;

	private static DataManager instance;

	public PlayerData_v_1_1_3 playerData => m_PlayerData;

	public PlayerThemeData playerThemeData => m_PlayerThemeData;

	public AppRateData appRateData => m_AppRateData;

	public PlayerStats_v_1_1_6 playerStats => m_PlayerStats;

	public PlayerRemoteSettings_v_1_1_9 playerRemoteSettings => m_PlayerRemoteSettings;

	public bool isVersionUpdate => m_isVersionUpdate;

	public bool updateAvailable => m_UpdateAvailable;

	public bool forceToUpdate => m_ForceToUpdate;

	public int BOOST => m_BOOST;

	public int COIN_MULTIPLIER => m_COIN_MULTIPLIER;

	public int SHIELD => m_SHIELD;

	public int WEAPON => m_WEAPON;

	public AudioClip firstPowerUpInfoClip => m_FirstPowerUpInfoClip;

	public AudioClip firstPowerUpBoostInfoClip => m_FirstPowerUpBoostInfoClip;

	public AudioClip firstPowerUpShieldInfoClip => m_FirstPowerUpShieldInfoClip;

	public AudioClip firstPowerUpCoinDoublerInfoClip => m_FirstPowerUpCoinDoublerInfoClip;

	public AudioClip firstPowerUpWeaponInfoClip => m_FirstPowerUpWeaponInfoClip;

	public bool initFinished => m_Init;

	public bool languageChangeFinished => m_LanguageChangeFinished;

	public Theme selectedTheme => m_SelectedTheme;

	public static DataManager Instance => instance;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			UnityEngine.Object.Destroy(gameObject);
			return;
		}
		instance = this;
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
	}

	public void Init()
	{
		m_Init = false;
		string storedVersion = PlayerPrefs.GetString("feer_version_number", "not found");
		if (FeerSceneManager.Instance.debugMode && !string.IsNullOrEmpty(FeerSceneManager.Instance.overrideVersionNumber) && !FeerSceneManager.Instance.overrideVersionNumber.Equals(""))
		{
			storedVersion = FeerSceneManager.Instance.overrideVersionNumber;
		}
		if (storedVersion.Equals("not found"))
		{
			PlayerPrefs.SetString("feer_version_number", versionNumber);
		}
		else if (!storedVersion.Equals(versionNumber))
		{
			if (storedVersion.Equals("1.1.1") || storedVersion.Equals("1.1.2"))
			{
				UpdatePlayerData(storedVersion);
				UpdatePlayerRemoteSettingsData(versionNumber);
				UpdatePlayerStatsData(storedVersion);
			}
			else if (storedVersion.Equals("1.1.3") || storedVersion.Equals("1.1.4") || storedVersion.Equals("1.1.5"))
			{
				UpdatePlayerRemoteSettingsData(versionNumber);
				UpdatePlayerStatsData(storedVersion);
			}
			else if (storedVersion.Equals("1.1.6") || storedVersion.Equals("1.1.7") || storedVersion.Equals("1.1.8"))
			{
				UpdatePlayerRemoteSettingsData(versionNumber);
			}
			PlayerPrefs.SetString("feer_version_number", versionNumber);
		}
		for (int i = 0; i < powerUps.Length; i++)
		{
			switch (powerUps[i].powerUpType)
			{
			case ConsumableType.Boost:
				m_BOOST = i;
				break;
			case ConsumableType.CoinMultiplier:
				m_COIN_MULTIPLIER = i;
				break;
			case ConsumableType.Shield:
				m_SHIELD = i;
				break;
			case ConsumableType.Weapon:
				m_WEAPON = i;
				break;
			}
		}
		m_InitCount = 4;
		LoadPlayerData();
		LoadPlayerStats();
		LoadPlayerRemoteSettings();
		LoadPlayerThemeData();
		if (m_PlayerData.showAppRateDialog)
		{
			LoadAppRateData();
		}
	}

	protected void InitStepFinished()
	{
		m_InitCount--;
		if (m_InitCount == 0)
		{
			m_Init = true;
		}
	}

	private void InitStepDone()
	{
		if (!m_Init)
		{
			m_InitCount--;
			if (m_InitCount == 0)
			{
				m_Init = true;
			}
		}
	}

	public void SetSelectedTheme(Theme theme)
	{
		if (m_SelectedTheme != theme)
		{
			if (m_SelectedTheme != Theme.None)
			{
				m_TutorialClips = null;
			}
			m_SelectedTheme = theme;
		}
	}

	private static T LoadBinary<T>(string fileName, FileAccess access) where T : class
	{
		string path = Application.persistentDataPath + fileName;
		if (!File.Exists(path))
		{
			return null;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = (access == FileAccess.ReadWrite) ? File.Open(path, FileMode.Open, FileAccess.ReadWrite) : File.Open(path, FileMode.Open);
		T result = (T)binaryFormatter.Deserialize(fileStream);
		fileStream.Close();
		return result;
	}

	private static void SaveBinary(string fileName, object data)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Create(Application.persistentDataPath + fileName);
		binaryFormatter.Serialize(fileStream, data);
		fileStream.Close();
	}

	protected void LoadPlayerRemoteSettings()
	{
		m_PlayerRemoteSettings = LoadBinary<PlayerRemoteSettings_v_1_1_9>("/PlayerRemoteSettings.dat", FileAccess.Read);
		if (m_PlayerRemoteSettings == null)
		{
			m_PlayerRemoteSettings = new PlayerRemoteSettings_v_1_1_9();
			m_PlayerRemoteSettings.agbURL_de = "https://www.mentalhome.eu/de/nutzungsbedingungen/";
			m_PlayerRemoteSettings.agbURL_en = "https://www.mentalhome.eu/terms-of-use/";
			m_PlayerRemoteSettings.agbURL_es = "https://www.mentalhome.eu/terms-of-use/";
			m_PlayerRemoteSettings.agbURL_fr = "https://www.mentalhome.eu/terms-of-use/";
			m_PlayerRemoteSettings.agbURL_it = "https://www.mentalhome.eu/terms-of-use/";
			m_PlayerRemoteSettings.androidRatingURL = "market://details?id=eu.mentalhome.feer";
			m_PlayerRemoteSettings.appStoreLink = "https://itunes.apple.com/app/id1422421760";
			m_PlayerRemoteSettings.faqURL_de = "https://www.mentalhome.eu/de/hilfe/feer/";
			m_PlayerRemoteSettings.faqURL_en = "https://www.mentalhome.eu/support/feer/";
			m_PlayerRemoteSettings.faqURL_es = "https://www.mentalhome.eu/support/feer/";
			m_PlayerRemoteSettings.faqURL_fr = "https://www.mentalhome.eu/support/feer/";
			m_PlayerRemoteSettings.faqURL_it = "https://www.mentalhome.eu/support/feer/";
			m_PlayerRemoteSettings.forcedUpdateForVersionNumberLower = "0.0.0";
			m_PlayerRemoteSettings.googlePlayStoreLink = "http://play.google.com/store/apps/details?id=eu.mentalhome.feer";
			m_PlayerRemoteSettings.iOSRatingURL = "itms-apps://itunes.apple.com/app/id1422421760?action=write-review";
			m_PlayerRemoteSettings.newestAvailableVersionNumber = versionNumber;
			m_PlayerRemoteSettings.privacyURL_de = "https://www.mentalhome.eu/de/datenschutzerklaerung/";
			m_PlayerRemoteSettings.privacyURL_en = "https://www.mentalhome.eu/privacy-policy/";
			m_PlayerRemoteSettings.privacyURL_es = "https://www.mentalhome.eu/privacy-policy/";
			m_PlayerRemoteSettings.privacyURL_fr = "https://www.mentalhome.eu/privacy-policy/";
			m_PlayerRemoteSettings.privacyURL_it = "https://www.mentalhome.eu/privacy-policy/";
			m_PlayerRemoteSettings.supportMail = "support@mentalhome.eu";
			m_PlayerRemoteSettings.websiteURL_de = "https://www.mentalhome.eu/de/feer-spiel/";
			m_PlayerRemoteSettings.websiteURL_en = "https://www.mentalhome.eu/feer/";
			SavePlayerRemoteSettings();
		}
		// PORT: no Unity RemoteSettings (online). The game continues as if the server had not answered.
		m_UpdateAvailable = false;
		m_ForceToUpdate = false;
		InitStepDone();
	}

	protected void UpdatePlayerRemoteSettingsData(string fromVersionNumber)
	{
		// PORT: save migration from old Android versions; never runs on a fresh PC install.
	}

	private IEnumerator RemoteSettingsTimeOut()
	{
		yield return new WaitForSeconds(10f);
		if (!m_Init)
		{
			InitStepFinished();
		}
	}

	protected void SavePlayerRemoteSettings()
	{
		SaveBinary("/PlayerRemoteSettings.dat", m_PlayerRemoteSettings);
	}

	private void HandleRemoteUpdate()
	{
		// PORT: Unity RemoteSettings removed (online).
	}

	public void RemoteSettingsUpdateCompleted(bool wasUpdatedFromServer, bool settingsChanged, int serverResponse)
	{
		// PORT: Unity RemoteSettings removed (online).
		m_UpdateAvailable = false;
		m_ForceToUpdate = false;
	}

	public void UpdateToNewVersionInfoMessagePresented()
	{
		m_PlayerData.updateNewVersionInfoMessageCount++;
		SavePlayerData();
	}

	public string GetInviteFriendRequestUrl()
	{
		return "https://www.mentalgames.eu/dbapps/feer/AddFriend.php?token=";
	}

	public string GetHashKeyMessage()
	{
		return "a3afWoin6sR3TmRinMpN";
	}

	private void SendTopStateMessage(InfoMessage infoMessage, string additionalData)
	{
		CustomGameManager.Instance.topState.ReceiveInfoMessage(infoMessage, additionalData);
	}

	public void LoadGlobalHighscoreList()
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, GlobalHighscoreListReceived, InfoMessage.InformTopState));
	}

	public void GlobalHighscoreListReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.GlobalHighscoreLoadedSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.GlobalHighscoreLoadedFailed, null);
		}
	}

	public void LoadFriendsHighscore()
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, FriendsHighscoreListReceived, InfoMessage.InformTopState));
	}

	public void FriendsHighscoreListReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.FriendsHighscoreLoadedSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.FriendsHighscoreLoadedFailed, null);
		}
	}

	public void RequestFriendInvitationCode()
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, FriendInvitationCodeReceived, InfoMessage.InformTopState));
	}

	public void FriendInvitationCodeReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (!success)
		{
			SendTopStateMessage(InfoMessage.RequestFriendInvitationCodeFailed, null);
			return;
		}
		JsonServerResponseGetInvitationCode jsonServerResponseGetInvitationCode = JsonUtility.FromJson<JsonServerResponseGetInvitationCode>(wwwText);
		SendTopStateMessage(InfoMessage.RequestFriendInvitationCodeSuccess, jsonServerResponseGetInvitationCode.invitationCode);
	}

	public void RemoveFriend(string friend_id)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, RemoveFriendFinished, InfoMessage.InformTopState));
	}

	public void RemoveFriendFinished(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.RemoveFriendSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.RemoveFriendFailed, null);
		}
	}

	public void GetMinGlobalHighscore()
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, MinGlobalHighscoreReceived, InfoMessage.InformTopState));
	}

	public void MinGlobalHighscoreReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.MinLeaderboardSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.MinLeaderboardFailed, null);
		}
	}

	public void UpdateAndGetPlayerGlobalRank(int highscore)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, UpdateAndGetPlayerGlobalRankReceived, InfoMessage.InformTopState));
	}

	public void UpdateAndGetPlayerGlobalRankReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.PlayerRanksSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.PlayerRanksFailed, null);
		}
	}

	public void UpdateAndGetPlayerServerRanks(int highscore)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, UpdateAndGetPlayerServerRanksReceived, InfoMessage.InformTopState));
	}

	public void UpdateAndGetPlayerServerRanksReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.PlayerRanksSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.PlayerRanksFailed, null);
		}
	}

	public void InitPlayerServerData()
	{
		if (string.IsNullOrEmpty(m_PlayerData.userId) || m_PlayerData.userId.Equals(""))
		{
			CreatePlayerID();
		}
		else if (!string.IsNullOrEmpty(m_PlayerData.highscoreNickname) && !m_PlayerData.highscoreNickname.Equals(""))
		{
			UpdatePlayerServerData(false);
		}
	}

	public void UpdatePlayerDataAndLoadHighscores(bool updatePlayerData, bool informTopState)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, UpdatePlayerServerDataAndLoadHighscoresReceived, informTopState ? InfoMessage.InformTopState : InfoMessage.None));
	}

	public void UpdatePlayerServerDataAndLoadHighscoresReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (infoMessage != InfoMessage.InformTopState)
		{
			return;
		}
		if (success)
		{
			SendTopStateMessage(InfoMessage.UpdatePlayerDataAndLoadHighscoresSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.UpdatePlayerDataAndLoadHighscoresFailed, null);
		}
	}

	public void UpdatePlayerServerData(bool informTopState = true)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, UpdatePlayerServerDataReceived, informTopState ? InfoMessage.InformTopState : InfoMessage.None));
	}

	public void UpdatePlayerServerDataReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			JsonServerResponsePlayerID jsonServerResponsePlayerID = JsonUtility.FromJson<JsonServerResponsePlayerID>(wwwText);
			if (m_PlayerData.userId != jsonServerResponsePlayerID.playerId)
			{
				m_PlayerData.userId = jsonServerResponsePlayerID.playerId;
				SavePlayerData();
			}
			if (infoMessage == InfoMessage.InformTopState)
			{
				SendTopStateMessage(InfoMessage.UpdatePlayerServerDataSuccess, null);
			}
		}
		else if (infoMessage == InfoMessage.InformTopState)
		{
			SendTopStateMessage(InfoMessage.UpdatePlayerServerDataFailed, null);
		}
	}

	public void ChangeNickname(string nicknameNew)
	{
		m_tempNickname = nicknameNew;
		StartCoroutine(ServerPostRequestImplementation(null, null, ChangeNicknameReceived, InfoMessage.InformTopState));
	}

	public void ChangeNicknameReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			m_PlayerData.highscoreNickname = m_tempNickname;
			SavePlayerData();
			if (infoMessage == InfoMessage.InformTopState)
			{
				SendTopStateMessage(InfoMessage.ChangeNicknameSuccess, null);
			}
		}
		else if (infoMessage == InfoMessage.InformTopState)
		{
			SendTopStateMessage(InfoMessage.ChangeNicknameFailed, null);
		}
		m_tempNickname = "";
	}

	public void SetNickname(string nickname)
	{
		m_tempNickname = nickname;
		if (string.IsNullOrEmpty(m_PlayerData.userId) || m_PlayerData.userId.Equals(""))
		{
			CreatePlayerID(InfoMessage.NextSetNickname);
			return;
		}
		StartCoroutine(ServerPostRequestImplementation(null, null, SetNicknameReceived, InfoMessage.InformTopState));
	}

	public void SetNicknameReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			JsonServerResponseAddPlayer jsonServerResponseAddPlayer = JsonUtility.FromJson<JsonServerResponseAddPlayer>(wwwText);
			m_PlayerData.userId = jsonServerResponseAddPlayer.playerId;
			m_PlayerData.highscoreNickname = jsonServerResponseAddPlayer.playerName;
			SavePlayerData();
			if (infoMessage == InfoMessage.InformTopState)
			{
				SendTopStateMessage(InfoMessage.SetNicknameSuccess, null);
			}
		}
		else if (infoMessage == InfoMessage.InformTopState)
		{
			SendTopStateMessage(InfoMessage.SetNicknameFailed, null);
		}
		m_tempNickname = "";
	}

	public void RedeemFriendInvitationCode(string invitationCode, bool topState)
	{
		if (topState)
		{
			StartCoroutine(ServerPostRequestImplementation(null, null, FriendInvitationCodeRedeemedTopState, InfoMessage.InformTopState));
		}
		else
		{
			StartCoroutine(ServerPostRequestImplementation(null, null, FriendInvitationCodeRedeemed, InfoMessage.InformTopState));
		}
	}

	public void FriendInvitationCodeRedeemed(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			FeerSceneManager.Instance.FriendInvitationReceived(true, wwwText);
		}
		else
		{
			FeerSceneManager.Instance.customURLReceived = false;
		}
	}

	public void FriendInvitationCodeRedeemedTopState(bool success, string wwwText, InfoMessage infoMessage)
	{
		if (success)
		{
			SendTopStateMessage(InfoMessage.RedeemInvitationSuccess, wwwText);
		}
		else
		{
			SendTopStateMessage(InfoMessage.RedeemInvitationFailed, null);
		}
	}

	public void IncreaseScoreMultiplier()
	{
		m_PlayerData.scoreMultiplier++;
		SavePlayerData();
	}

	public void NewHighscore(int score)
	{
		m_PlayerData.highscore = (score <= maxScoreCoinsValue) ? score : maxScoreCoinsValue;
		SavePlayerData();
	}

	private IEnumerator InformAnalyticsOfUpgrade(string transactionalItem, int level, int distance)
	{
		// PORT: analytics removed.
		yield break;
	}

	private bool IncreasePowerUpLevel(int powerUpIndex, TransactionalItem item, ref int level, ref int distance)
	{
		PowerUpLevel[] powerUpLevels = powerUps[powerUpIndex].powerUpLevels;
		if (!InvestCoins(powerUpLevels[level].coinsToPay, TransactionContext.Store, item, TransactionItemType.Upgrade, null))
		{
			return false;
		}
		level++;
		distance = powerUpLevels[level - 1].upgradeValue + distance;
		SavePlayerData();
		return true;
	}

	public bool IncreaseBoostLevel()
	{
		return IncreasePowerUpLevel(m_BOOST, (TransactionalItem)2, ref m_PlayerData.boostLevel, ref m_PlayerData.boostDistance);
	}

	public bool IncreaseCoinMultiplierLevel()
	{
		return IncreasePowerUpLevel(m_COIN_MULTIPLIER, (TransactionalItem)3, ref m_PlayerData.coinMultiplierLevel, ref m_PlayerData.coinMultiplierDistance);
	}

	public bool IncreaseShieldLevel()
	{
		return IncreasePowerUpLevel(m_SHIELD, (TransactionalItem)4, ref m_PlayerData.shieldLevel, ref m_PlayerData.shieldDistance);
	}

	public bool IncreaseWeaponLevel()
	{
		return IncreasePowerUpLevel(m_WEAPON, (TransactionalItem)5, ref m_PlayerData.weaponLevel, ref m_PlayerData.weaponDistance);
	}

	public bool InvestCoins(int coins, TransactionContext transactionContext, TransactionalItem itemId, TransactionItemType itemType, string transactionId)
	{
		int remaining = m_PlayerData.coins - coins;
		if (remaining < 0)
		{
			return false;
		}
		m_PlayerData.coins = remaining;
		SavePlayerData();
		m_PlayerStats.lightsSpent += coins;
		if ((long)maxScoreCoinsValue < m_PlayerStats.lightsSpent)
		{
			m_PlayerStats.lightsSpent = maxScoreCoinsValue;
		}
		SavePlayerStats();
		// PORT: analytics event (ItemSpent) removed.
		return true;
	}

	public void SaveCoins(int coins, TransactionContext transactionContext, TransactionalItem itemId, TransactionItemType itemType, string transactionId)
	{
		if (maxScoreCoinsValue <= m_PlayerData.coins)
		{
			return;
		}
		m_PlayerData.coins += coins;
		if (maxScoreCoinsValue < m_PlayerData.coins)
		{
			m_PlayerData.coins = maxScoreCoinsValue;
		}
		SavePlayerData();
		// PORT: analytics event (ItemAcquired) removed.
	}

	public void GameOver()
	{
		if (m_PlayerData.showAppRateDialog)
		{
			m_AppRateData.gamesPlayedSinceAsked++;
			SaveAppRateData();
		}
	}

	protected void LoadPlayerThemeData()
	{
		m_PlayerThemeData = LoadBinary<PlayerThemeData>("/ThemeFactory.dat", FileAccess.ReadWrite);
		if (m_PlayerThemeData == null)
		{
			m_PlayerThemeData = new PlayerThemeData();
			m_PlayerThemeData.themeFactoryPurchased = false;
			m_PlayerThemeData.themeFactoryWaitingForApproval = false;
			m_PlayerThemeData.themeFactoryApprovalStartDate = DateTime.Now;
			m_PlayerThemeData.themeFactoryTutorialPlayed = false;
			m_PlayerThemeData.themeFactoryReceiptData = null;
			m_PlayerThemeData.themeFactoryOriginalReceipt = null;
			SavePlayerThemeData();
		}
		if (m_PlayerThemeData.themeFactoryWaitingForApproval && 24.0 < (DateTime.Now - m_PlayerThemeData.themeFactoryApprovalStartDate).TotalHours)
		{
			m_PlayerThemeData.themeFactoryWaitingForApproval = false;
			SavePlayerThemeData();
		}
		// PORT: purchases removed; the Factory theme (bought on Android) is always unlocked.
		if ((gameMode == GameMode.Presentation || true) && !m_PlayerThemeData.themeFactoryPurchased)
		{
			m_PlayerThemeData.themeFactoryPurchased = true;
			SavePlayerThemeData();
		}
		InitStepDone();
	}

	public void SavePlayerThemeData()
	{
		SaveBinary("/ThemeFactory.dat", m_PlayerThemeData);
	}

	public void ThemeFactoryPurchased(IAPReceiptData receiptData, string originalReceipt)
	{
		m_PlayerThemeData.themeFactoryPurchased = true;
		m_PlayerThemeData.themeFactoryWaitingForApproval = false;
		m_PlayerThemeData.themeFactoryReceiptData = receiptData;
		m_PlayerThemeData.themeFactoryOriginalReceipt = originalReceipt;
		SavePlayerThemeData();
	}

	public void ThemeFactoryWaitingForApproval(bool waiting)
	{
		m_PlayerThemeData.themeFactoryWaitingForApproval = waiting;
		m_PlayerThemeData.themeFactoryApprovalStartDate = DateTime.Now;
		SavePlayerThemeData();
	}

	protected void UpdatePlayerData(string fromVersionNumber)
	{
		// PORT: save migration from old Android versions; never runs on a fresh PC install.
	}

	protected void LoadPlayerData()
	{
		m_PlayerData = LoadBinary<PlayerData_v_1_1_3>("/PlayerData.dat", FileAccess.ReadWrite);
		if (m_PlayerData == null)
		{
			m_PlayerData = new PlayerData_v_1_1_3();
			m_PlayerData.installationTime = DateTime.Now;
			m_PlayerData.updateNewVersionInfoMessageCount = 0;
			m_PlayerData.updateVersionGiftRedeemed = false;
			m_PlayerData.sessionCount = 0L;
			m_PlayerData.sessionId = -1L;
			m_PlayerData.highscore = 0;
			m_PlayerData.scoreMultiplier = 1;
			m_PlayerData.coins = 0;
			m_PlayerData.boostLevel = 1;
			m_PlayerData.boostDistance = powerUps[m_BOOST].powerUpLevels[0].upgradeValue;
			m_PlayerData.coinMultiplierLevel = 1;
			m_PlayerData.coinMultiplierDistance = powerUps[m_COIN_MULTIPLIER].powerUpLevels[0].upgradeValue;
			m_PlayerData.shieldLevel = 1;
			m_PlayerData.shieldDistance = powerUps[m_SHIELD].powerUpLevels[0].upgradeValue;
			m_PlayerData.weaponLevel = 1;
			m_PlayerData.weaponDistance = powerUps[m_WEAPON].powerUpLevels[0].upgradeValue;
			m_PlayerData.timesSubmitHighscoreAsked = 0;
			m_PlayerData.highscoreNickname = "";
			m_PlayerData.userLanguage = GetApplicationLanguage();
			m_PlayerData.playTutorial = true;
			m_PlayerData.isFirstGame = true;
			m_PlayerData.isFirstAccesibleGameEver = true;
			m_PlayerData.isFirstAccessiblePowerUp = true;
			m_PlayerData.isFirstAccessiblePowerUpBoost = true;
			m_PlayerData.isFirstAccessiblePowerUpShield = true;
			m_PlayerData.isFirstAccessiblePowerUpCoinDoubler = true;
			m_PlayerData.isFirstAccessiblePowerUpWeapon = true;
			m_PlayerData.useReverseLeftRight = false;
			m_PlayerData.useReverseUpDown = false;
			m_PlayerData.useCenterLaneOrientation = false;
			m_PlayerData.useVibration = true;
			m_PlayerData.showAppRateDialog = true;
			m_PlayerData.lastRewardedMissionNumber = -1;
			SavePlayerData();
			m_InitCount++;
			CopyImageShareImageFile();
		}
		LocalizationManager.Instance.SetUserLanguage(m_PlayerData.userLanguage);
		LoadPowerUpAudioClips();
		InitStepDone();
	}

	public void SetLastRewardedMission(int missionNumber)
	{
		m_PlayerData.lastRewardedMissionNumber = missionNumber;
		SavePlayerData();
	}

	protected void SavePlayerData()
	{
		SaveBinary("/PlayerData.dat", m_PlayerData);
	}

	public void SaveSessionData(DateTime sessionStartTime, long sessionCount, long sessionId)
	{
		m_PlayerData.sessionStartTime = sessionStartTime;
		m_PlayerData.sessionCount = sessionCount;
		m_PlayerData.sessionId = sessionId;
		SavePlayerData();
		if (m_PlayerData.showAppRateDialog)
		{
			m_AppRateData.sessionsSinceAsked++;
			SaveAppRateData();
		}
	}

	public void UpdateVersionGiftReceived()
	{
		m_PlayerData.updateVersionGiftRedeemed = true;
		SavePlayerData();
	}

	public void AppRatingNeverAgain()
	{
		if (m_PlayerData.showAppRateDialog)
		{
			m_PlayerData.showAppRateDialog = false;
			SavePlayerData();
		}
	}

	public void SetUserLanguage(SystemLanguage toLanguage)
	{
		m_LanguageChangeFinished = false;
		m_LanguageChangeCount = 1;
		m_PlayerData.userLanguage = toLanguage;
		SavePlayerData();
		ReloadPowerUpAudioClips(toLanguage);
	}

	public void LanguageChangeStepFinished()
	{
		m_LanguageChangeCount--;
		if (m_LanguageChangeCount == 0)
		{
			m_LanguageChangeFinished = true;
		}
	}

	protected void CreatePlayerID(InfoMessage infoMessage = InfoMessage.None)
	{
		StartCoroutine(ServerPostRequestImplementation(null, null, PlayerIDReceived, infoMessage));
	}

	public void PlayerIDReceived(bool success, string wwwText, InfoMessage infoMessage)
	{
		Debug.Log("Player ID received, text: " + wwwText);
		if (success)
		{
			JsonServerResponsePlayerID jsonServerResponsePlayerID = JsonUtility.FromJson<JsonServerResponsePlayerID>(wwwText);
			m_PlayerData.userId = jsonServerResponsePlayerID.playerId;
			SavePlayerData();
			if (infoMessage == InfoMessage.NextSetNickname)
			{
				SetNickname(m_tempNickname);
			}
		}
		else if (infoMessage == InfoMessage.NextSetNickname)
		{
			SendTopStateMessage(InfoMessage.SetNicknameFailed, null);
		}
	}

	protected IEnumerator ServerPostRequestImplementation(string url, string json, Action<bool, string, InfoMessage> callback, InfoMessage infoMessage = InfoMessage.None)
	{
		// PORT: online features removed. Every server request fails as if there were no internet.
		yield return null;
		if (callback != null)
		{
			callback(false, "offline", infoMessage);
		}
	}

	protected SystemLanguage GetApplicationLanguage()
	{
		SystemLanguage language = GetSystemLanguage();
		if (!LocalizationManager.Instance.IsSystemLanguageSupported(language))
		{
			language = SystemLanguage.English;
		}
		return language;
	}

	protected SystemLanguage GetSystemLanguage()
	{
		if (Application.systemLanguage != SystemLanguage.Unknown)
		{
			return Application.systemLanguage;
		}
		string shortLanguageCode = PlayerPrefs.GetString("language", null);
		if (shortLanguageCode == null)
		{
			return SystemLanguage.Unknown;
		}
		if (!LocalizationManager.Instance.IsShortCodeLanguageSupported(shortLanguageCode))
		{
			return SystemLanguage.Unknown;
		}
		return LocalizationManager.Instance.GetSystemLanguageForShortLangCode(shortLanguageCode);
	}

	protected void CopyImageShareImageFile()
	{
		StartCoroutine(FetchImageData(Path.Combine(Application.streamingAssetsPath, "Feer-The-Game-App.jpg"), ShareImageRead));
	}

	public void ShareImageRead(bool success, byte[] imageBytes)
	{
		if (success)
		{
			File.WriteAllBytes(Application.persistentDataPath + "/Feer-The-Game-App.jpg", imageBytes);
		}
		else
		{
			Debug.LogError("Data Manager: FAILED TO COPY SHARE IMAGE");
		}
		InitStepDone();
	}

	public void LoadImage(string filePath, Action<bool, byte[]> callbackFunction)
	{
		StartCoroutine(FetchImageData(filePath, callbackFunction));
	}

	// PORT: on Windows streamingAssetsPath is a plain local path; UnityWebRequest needs "file://".
	private static string ToRequestUrl(string filePath)
	{
		if (filePath.Contains("://"))
		{
			return filePath;
		}
		return "file:///" + filePath.Replace('\\', '/');
	}

	private IEnumerator FetchImageData(string filePath, Action<bool, byte[]> callbackFunction)
	{
		using (UnityWebRequest www = UnityWebRequest.Get(ToRequestUrl(filePath)))
		{
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.Success)
			{
				callbackFunction?.Invoke(true, www.downloadHandler.data);
			}
			else
			{
				callbackFunction?.Invoke(false, null);
			}
		}
	}

	protected void UpdatePlayerStatsData(string fromVersionNumber)
	{
		// PORT: save migration from old Android versions; never runs on a fresh PC install.
	}

	protected void LoadPlayerStats()
	{
		m_PlayerStats = LoadBinary<PlayerStats_v_1_1_6>("/PlayerStats.dat", FileAccess.Read);
		if (m_PlayerStats == null)
		{
			m_PlayerStats = new PlayerStats_v_1_1_6();
			m_PlayerStats.mostLights = 0;
			m_PlayerStats.longestRun = 0;
			m_PlayerStats.mostZombiesSurvived = 0;
			m_PlayerStats.mostHandsSurvived = 0;
			m_PlayerStats.mostRavensSurvived = 0;
			m_PlayerStats.mostPowerUps = 0;
			m_PlayerStats.mostZombiesKilled = 0;
			m_PlayerStats.runDistance = 0L;
			m_PlayerStats.lightsCollected = 0L;
			m_PlayerStats.powerUpsCollected = 0L;
			m_PlayerStats.zombiesKilled = 0L;
			m_PlayerStats.gamesPlayed = 0L;
			m_PlayerStats.lightsSpent = 0L;
			m_PlayerStats.zombiesSurvived = 0L;
			m_PlayerStats.ravensSurvived = 0L;
			m_PlayerStats.handsSurvived = 0L;
			m_PlayerStats.deathByZombies = 0L;
			m_PlayerStats.deathByRavens = 0L;
			m_PlayerStats.deathByHands = 0L;
			m_PlayerStats.boostsCollected = 0L;
			m_PlayerStats.shieldsCollected = 0L;
			m_PlayerStats.weaponsCollected = 0L;
			m_PlayerStats.lightDoublerCollected = 0L;
			m_PlayerStats.questsSkipped = 0L;
			m_PlayerStats.mostRobotsSurvived = 0;
			m_PlayerStats.mostSawBladesSurvived = 0;
			m_PlayerStats.mostCranesSurvived = 0;
			m_PlayerStats.mostRobotsKilled = 0;
			m_PlayerStats.robotsKilled = 0L;
			m_PlayerStats.robotsSurvived = 0L;
			m_PlayerStats.sawBladesSurvived = 0L;
			m_PlayerStats.cranesSurvived = 0L;
			m_PlayerStats.deathByRobot = 0L;
			m_PlayerStats.deathBySawBlade = 0L;
			m_PlayerStats.deathByCrane = 0L;
			SavePlayerStats();
		}
		InitStepDone();
	}

	protected void SavePlayerStats()
	{
		SaveBinary("/PlayerStats.dat", m_PlayerStats);
	}

	private int ClampInt(int value)
	{
		return (maxScoreCoinsValue < value) ? maxScoreCoinsValue : value;
	}

	private long ClampLong(long value)
	{
		return ((long)maxScoreCoinsValue < value) ? maxScoreCoinsValue : value;
	}

	public void UpdatePlayerStatsOnGameOver()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		PlayerStats_v_1_1_6 stats = m_PlayerStats;
		if (stats.mostLights < cgm.sumCollectedGhosts)
		{
			stats.mostLights = cgm.sumCollectedGhosts;
		}
		stats.mostLights = ClampInt(stats.mostLights);
		int run = (int)cgm.worldDistance;
		if (stats.longestRun < run)
		{
			stats.longestRun = run;
		}
		stats.longestRun = ClampInt(stats.longestRun);
		if (stats.mostPowerUps < cgm.powerUpCollected)
		{
			stats.mostPowerUps = cgm.powerUpCollected;
		}
		stats.mostPowerUps = ClampInt(stats.mostPowerUps);
		stats.runDistance = ClampLong(stats.runDistance + run);
		stats.lightsCollected = ClampLong(stats.lightsCollected + cgm.sumCollectedGhosts);
		stats.powerUpsCollected = ClampLong(stats.powerUpsCollected + cgm.powerUpCollected);
		stats.gamesPlayed = ClampLong(stats.gamesPlayed + 1);
		stats.boostsCollected = ClampLong(stats.boostsCollected + cgm.powerUpBoostCollected);
		stats.shieldsCollected = ClampLong(stats.shieldsCollected + cgm.powerUpShieldCollected);
		stats.weaponsCollected = ClampLong(stats.weaponsCollected + cgm.powerUpWeaponCollected);
		stats.lightDoublerCollected = ClampLong(stats.lightDoublerCollected + cgm.powerUpCoinDoublerCollected);
		if (m_SelectedTheme == Theme.Factory)
		{
			if (stats.mostRobotsSurvived < cgm.zombiesDodged)
			{
				stats.mostRobotsSurvived = cgm.zombiesDodged;
			}
			stats.mostRobotsSurvived = ClampInt(stats.mostRobotsSurvived);
			if (stats.mostSawBladesSurvived < cgm.jumpedOver)
			{
				stats.mostSawBladesSurvived = cgm.jumpedOver;
			}
			stats.mostSawBladesSurvived = ClampInt(stats.mostSawBladesSurvived);
			if (stats.mostCranesSurvived < cgm.slidedUnder)
			{
				stats.mostCranesSurvived = cgm.slidedUnder;
			}
			stats.mostCranesSurvived = ClampInt(stats.mostCranesSurvived);
			if (stats.mostRobotsKilled < cgm.zombiesKilled)
			{
				stats.mostRobotsKilled = cgm.zombiesKilled;
			}
			stats.mostRobotsKilled = ClampInt(stats.mostRobotsKilled);
			stats.robotsKilled = ClampLong(stats.robotsKilled + cgm.zombiesKilled);
			stats.robotsSurvived = ClampLong(stats.robotsSurvived + cgm.zombiesDodged);
			stats.cranesSurvived = ClampLong(stats.cranesSurvived + cgm.slidedUnder);
			stats.sawBladesSurvived = ClampLong(stats.sawBladesSurvived + cgm.jumpedOver);
			stats.deathByRobot = ClampLong(stats.deathByRobot + cgm.killedByZombie);
			stats.deathByCrane = ClampLong(stats.deathByCrane + cgm.killedByAir);
			stats.deathBySawBlade = ClampLong(stats.deathBySawBlade + cgm.killedByGround);
		}
		else
		{
			if (stats.mostZombiesSurvived < cgm.zombiesDodged)
			{
				stats.mostZombiesSurvived = cgm.zombiesDodged;
			}
			stats.mostZombiesSurvived = ClampInt(stats.mostZombiesSurvived);
			if (stats.mostHandsSurvived < cgm.jumpedOver)
			{
				stats.mostHandsSurvived = cgm.jumpedOver;
			}
			stats.mostHandsSurvived = ClampInt(stats.mostHandsSurvived);
			if (stats.mostRavensSurvived < cgm.slidedUnder)
			{
				stats.mostRavensSurvived = cgm.slidedUnder;
			}
			stats.mostRavensSurvived = ClampInt(stats.mostRavensSurvived);
			if (stats.mostZombiesKilled < cgm.zombiesKilled)
			{
				stats.mostZombiesKilled = cgm.zombiesKilled;
			}
			stats.mostZombiesKilled = ClampInt(stats.mostZombiesKilled);
			stats.zombiesKilled = ClampLong(stats.zombiesKilled + cgm.zombiesKilled);
			stats.zombiesSurvived = ClampLong(stats.zombiesSurvived + cgm.zombiesDodged);
			stats.ravensSurvived = ClampLong(stats.ravensSurvived + cgm.slidedUnder);
			stats.handsSurvived = ClampLong(stats.handsSurvived + cgm.jumpedOver);
			stats.deathByZombies = ClampLong(stats.deathByZombies + cgm.killedByZombie);
			stats.deathByRavens = ClampLong(stats.deathByRavens + cgm.killedByAir);
			stats.deathByHands = ClampLong(stats.deathByHands + cgm.killedByGround);
		}
		SavePlayerStats();
	}

	public void UpdatePlayerStatsQuestSkipped()
	{
		m_PlayerStats.questsSkipped++;
		m_PlayerStats.questsSkipped = ClampLong(m_PlayerStats.questsSkipped);
		SavePlayerStats();
	}

	protected void LoadAppRateData()
	{
		m_AppRateData = LoadBinary<AppRateData>("/AppRateData.dat", FileAccess.Read);
		if (m_AppRateData == null)
		{
			m_AppRateData = new AppRateData();
			m_AppRateData.gamesPlayedSinceAsked = 0;
			m_AppRateData.lastTimeAsked = DateTime.Now;
			m_AppRateData.sessionsSinceAsked = 0;
			m_AppRateData.timesAsked = 0;
			SaveAppRateData();
		}
	}

	protected void SaveAppRateData()
	{
		SaveBinary("/AppRateData.dat", m_AppRateData);
	}

	public void LoadFile(string filePath, Action<bool, string> callbackFunction)
	{
		StartCoroutine(FetchFileData(filePath, callbackFunction));
	}

	private IEnumerator FetchFileData(string filePath, Action<bool, string> callbackFunction)
	{
		using (UnityWebRequest www = UnityWebRequest.Get(ToRequestUrl(filePath)))
		{
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.Success)
			{
				callbackFunction(true, www.downloadHandler.text);
			}
			else
			{
				callbackFunction(false, null);
			}
		}
	}

	public void LoadAudioClipFromStreamingAssets(string filePath, AudioType audioType, Action<bool, AudioClip> callbackFunction)
	{
		StartCoroutine(FetchAudioClip(Path.Combine(Application.streamingAssetsPath, filePath), audioType, callbackFunction));
	}

	private IEnumerator FetchAudioClip(string filePath, AudioType audioType, Action<bool, AudioClip> callbackFunction)
	{
		using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(ToRequestUrl(filePath), audioType))
		{
			yield return www.SendWebRequest();
			if (www.result == UnityWebRequest.Result.Success)
			{
				callbackFunction(true, DownloadHandlerAudioClip.GetContent(www));
			}
			else
			{
				callbackFunction(false, null);
			}
		}
	}

	private string GetPowerUpClipSuffix(SystemLanguage language)
	{
		return "_" + LocalizationManager.Instance.GetShortLanguageCodeForSystemLanguage(language) + ".ogg";
	}

	protected void LoadPowerUpAudioClips()
	{
		string suffix = GetPowerUpClipSuffix(m_PlayerData.userLanguage);
		if (m_PlayerData.isFirstAccessiblePowerUp)
		{
			m_InitCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpInfoClipLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpBoost)
		{
			m_InitCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_boost_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpBoostInfoClipLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpShield)
		{
			m_InitCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_shield_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpShieldInfoClipLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpCoinDoubler)
		{
			m_InitCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_coin_doubler_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpCoinDoublerInfoClipLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpWeapon)
		{
			m_InitCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_weapon_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpWeaponInfoClipLoaded);
		}
	}

	protected void ReloadPowerUpAudioClips(SystemLanguage forLanguage)
	{
		string suffix = GetPowerUpClipSuffix(forLanguage);
		if (m_PlayerData.isFirstAccessiblePowerUp)
		{
			m_LanguageChangeCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpInfoClipReLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpBoost)
		{
			m_LanguageChangeCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_boost_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpBoostInfoClipReLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpShield)
		{
			m_LanguageChangeCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_shield_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpShieldInfoClipReLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpCoinDoubler)
		{
			m_LanguageChangeCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_coin_doubler_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpCoinDoublerInfoClipReLoaded);
		}
		if (m_PlayerData.isFirstAccessiblePowerUpWeapon)
		{
			m_LanguageChangeCount++;
			LoadAudioClipFromStreamingAssets("first_power_up_weapon_info" + suffix, AudioType.OGGVORBIS, FirstPowerUpWeaponInfoClipReLoaded);
		}
		m_LanguageChangeCount--;
		if (m_LanguageChangeCount == 0)
		{
			m_LanguageChangeFinished = true;
		}
	}

	private void ClipLoaded(bool success)
	{
		if (success && !m_Init)
		{
			m_InitCount--;
			if (m_InitCount == 0)
			{
				m_Init = true;
			}
		}
	}

	private void ClipReLoaded(bool success)
	{
		if (success && !m_LanguageChangeFinished)
		{
			m_LanguageChangeCount--;
			if (m_LanguageChangeCount == 0)
			{
				m_LanguageChangeFinished = true;
			}
		}
	}

	public void FirstPowerUpInfoClipLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpInfoClip = clip;
		}
		ClipLoaded(success);
	}

	public void FirstPowerUpInfoClipReLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpInfoClip = clip;
		}
		ClipReLoaded(success);
	}

	public void FirstPowerUpBoostInfoClipLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpBoostInfoClip = clip;
		}
		ClipLoaded(success);
	}

	public void FirstPowerUpBoostInfoClipReLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpBoostInfoClip = clip;
		}
		ClipReLoaded(success);
	}

	public void FirstPowerUpShieldInfoClipLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpShieldInfoClip = clip;
		}
		ClipLoaded(success);
	}

	public void FirstPowerUpShieldInfoClipReLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpShieldInfoClip = clip;
		}
		ClipReLoaded(success);
	}

	public void FirstPowerUpWeaponInfoClipLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpWeaponInfoClip = clip;
		}
		ClipLoaded(success);
	}

	public void FirstPowerUpWeaponInfoClipReLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpWeaponInfoClip = clip;
		}
		ClipReLoaded(success);
	}

	public void FirstPowerUpCoinDoublerInfoClipLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpCoinDoublerInfoClip = clip;
		}
		ClipLoaded(success);
	}

	public void FirstPowerUpCoinDoublerInfoClipReLoaded(bool success, AudioClip clip)
	{
		if (success)
		{
			m_FirstPowerUpCoinDoublerInfoClip = clip;
		}
		ClipReLoaded(success);
	}

	public MissionSetData LoadMissionSetData()
	{
		string path = Application.persistentDataPath + "/PlayerMissions.dat";
		if (!File.Exists(path))
		{
			return null;
		}
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		FileStream fileStream = File.Open(path, FileMode.Open);
		MissionSetData result = (MissionSetData)binaryFormatter.Deserialize(fileStream);
		fileStream.Close();
		return result;
	}

	public void SaveMissionSetData(MissionSetData missionSet)
	{
		SaveBinary("/PlayerMissions.dat", missionSet);
	}

	public AudioClip[] GetTutorialClips()
	{
		return m_TutorialClips;
	}

	public AudioClip GetTutorialHeadphoneClip()
	{
		return m_TutorialHeadphoneClip;
	}

	public void LoadTutorialClips(Action<bool> callbackFunction)
	{
		if (m_TutorialClips != null)
		{
			callbackFunction(true);
		}
		else
		{
			StartCoroutine(LoadTutorialClipsIE(callbackFunction));
		}
	}

	private IEnumerator LoadTutorialClipsIE(Action<bool> callbackFunction)
	{
		string langCode = LocalizationManager.Instance.GetShortLanguageCodeForSystemLanguage(m_PlayerData.userLanguage);
		bool forest = m_SelectedTheme != Theme.Factory;
		int numberOfClips = forest ? 28 : 14;
		string fileName = forest ? "tutorial_" : "factory_";
		string fileExtension = forest ? ".ogg" : ".mp3";
		AudioType audioType = forest ? AudioType.OGGVORBIS : AudioType.MPEG;
		m_TutorialClips = new AudioClip[numberOfClips];
		fileName = fileName + langCode + "_";
		bool error = false;
		for (int i = 0; i < numberOfClips; i++)
		{
			string path = Path.Combine(Application.streamingAssetsPath, fileName + i.ToString() + fileExtension);
			using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(ToRequestUrl(path), audioType))
			{
				yield return www.SendWebRequest();
				if (www.result != UnityWebRequest.Result.Success)
				{
					error = true;
					break;
				}
				m_TutorialClips[i] = DownloadHandlerAudioClip.GetContent(www);
			}
		}
		if (!error)
		{
			if (m_PlayerData.playTutorial)
			{
				string path2 = Path.Combine(Application.streamingAssetsPath, "tutorial_headphone_" + langCode + ".ogg");
				using (UnityWebRequest www2 = UnityWebRequestMultimedia.GetAudioClip(ToRequestUrl(path2), AudioType.OGGVORBIS))
				{
					yield return www2.SendWebRequest();
					if (www2.result == UnityWebRequest.Result.Success)
					{
						m_TutorialHeadphoneClip = DownloadHandlerAudioClip.GetContent(www2);
					}
					else
					{
						error = true;
					}
				}
			}
			else
			{
				m_TutorialHeadphoneClip = null;
			}
		}
		if (!error)
		{
			callbackFunction(true);
			yield break;
		}
		m_TutorialClips = null;
		m_TutorialHeadphoneClip = null;
		callbackFunction(false);
	}

	public void TutorialFinished()
	{
		if (m_SelectedTheme == Theme.Factory)
		{
			if (!m_PlayerThemeData.themeFactoryTutorialPlayed)
			{
				m_PlayerThemeData.themeFactoryTutorialPlayed = true;
				SavePlayerThemeData();
			}
		}
		else if (m_SelectedTheme == Theme.Forest && m_PlayerData.playTutorial)
		{
			m_PlayerData.playTutorial = false;
			SavePlayerData();
		}
		m_TutorialClips = null;
		m_TutorialHeadphoneClip = null;
	}

	public void UseVibration(bool enabled)
	{
		if (m_PlayerData.useVibration != enabled)
		{
			m_PlayerData.useVibration = enabled;
			SavePlayerData();
		}
	}

	public void UseReverseLeftRight(bool enabled)
	{
		if (m_PlayerData.useReverseLeftRight != enabled)
		{
			m_PlayerData.useReverseLeftRight = enabled;
			SavePlayerData();
		}
	}

	public void UseReverseUpDown(bool enabled)
	{
		if (m_PlayerData.useReverseUpDown != enabled)
		{
			m_PlayerData.useReverseUpDown = enabled;
			SavePlayerData();
		}
	}

	public void UseCenterLaneOrientation(bool enabled)
	{
		if (m_PlayerData.useCenterLaneOrientation != enabled)
		{
			m_PlayerData.useCenterLaneOrientation = enabled;
			SavePlayerData();
		}
	}

	public void FirstGamePlayed()
	{
		m_PlayerData.isFirstGame = false;
		SavePlayerData();
	}

	public void FirstGameInformationTold()
	{
		m_PlayerData.isFirstAccesibleGameEver = false;
		SavePlayerData();
	}

	public void FirstAccessiblePowerUpBoostTold()
	{
		m_PlayerData.isFirstAccessiblePowerUpBoost = false;
		m_PlayerData.isFirstAccessiblePowerUp = false;
		SavePlayerData();
	}

	public void FirstAccessiblePowerUpShieldTold()
	{
		m_PlayerData.isFirstAccessiblePowerUpShield = false;
		m_PlayerData.isFirstAccessiblePowerUp = false;
		SavePlayerData();
	}

	public void FirstAccessiblePowerUpWeaponTold()
	{
		m_PlayerData.isFirstAccessiblePowerUpWeapon = false;
		m_PlayerData.isFirstAccessiblePowerUp = false;
		SavePlayerData();
	}

	public void FirstAccessiblePowerUpCoinDoublerTold()
	{
		m_PlayerData.isFirstAccessiblePowerUpCoinDoubler = false;
		m_PlayerData.isFirstAccessiblePowerUp = false;
		SavePlayerData();
	}

	public void SubmitHighscoreAsked()
	{
		m_PlayerData.timesSubmitHighscoreAsked++;
		SavePlayerData();
	}

	public void AppRatingAsked()
	{
		if (m_AppRateData.timesAsked == 3)
		{
			m_AppRateData.timesAsked = 0;
		}
		m_AppRateData.timesAsked++;
		m_AppRateData.sessionsSinceAsked = 0;
		m_AppRateData.gamesPlayedSinceAsked = 0;
		m_AppRateData.lastTimeAsked = DateTime.Now;
		SaveAppRateData();
	}

	public void ClearAllData()
	{
		string[] files = new string[5] { "/Feer-The-Game-App.jpg", "/PlayerMissions.dat", "/AppRateData.dat", "/PlayerData.dat", "/PlayerStats.dat" };
		for (int i = 0; i < files.Length; i++)
		{
			string path = Application.persistentDataPath + files[i];
			if (File.Exists(path))
			{
				File.Delete(path);
			}
		}
		m_PlayerData = null;
		m_AppRateData = null;
		m_PlayerStats = null;
	}
}
