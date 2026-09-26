using System;
using UnityEngine;

public class CustomAnalyticsTracker : MonoBehaviour
{
	protected long m_sessionId = -1;

	protected DateTime m_CurrentGameStartTime;

	protected DateTime m_CurrentGameReviveTime;

	protected DateTime m_InitStartTime;

	private static CustomAnalyticsTracker instance;

	// PORT: analytics (Unity Analytics) removido. Todos os eventos viraram no-op;
	// so foi mantido o que tem efeito local: contagem de sessoes (DataManager.SaveSessionData)
	// e a mensagem AnalyticsNewSessionInitFinished, que continua a inicializacao.
	public static CustomAnalyticsTracker Instance => instance;

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
		m_InitStartTime = DateTime.Now;
		// PORT: substitui AnalyticsSessionInfo.sessionId (um id novo a cada execucao do jogo)
		m_sessionId = DateTime.Now.Ticks;
	}

	public void NewSession()
	{
		PlayerData_v_1_1_3 playerData = DataManager.Instance.playerData;
		if (playerData.sessionId != m_sessionId)
		{
			DateTime now = DateTime.Now;
			long sessionCount = playerData.sessionCount + 1;
			DataManager.Instance.SaveSessionData(now, sessionCount, m_sessionId);
		}
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.AnalyticsNewSessionInitFinished, null);
	}

	public void NewGameStarted(GameStateName fromStateName)
	{
	}

	public void ItemSpent(string transactionContext, int coins, string itemId, string itemType, string transactionId)
	{
	}

	public void ItemAcquired(string transactionContext, int coins, string itemId, string itemType, string transactionId)
	{
	}

	public void IAPTransactionSuccess(string transactionContext, string productId, string productType, string transactionId)
	{
	}

	public void IAPTransactionFailed(string transactionContext, string productId, string productType, string failedType, string failedReason)
	{
	}

	public void IAPTransactionDeferred(string transactionContext, string productId, string productType)
	{
	}

	public void StoreItemClicked(string productId)
	{
	}

	public void GameOver(int saveMeUsed)
	{
	}

	public void SocialShare(int shareNr, bool newHeighscore, GameStateName gameState, bool addFriend = false)
	{
	}

	public void RateAppDialog(string rateType, string rated)
	{
	}

	public void ItemUpgraded(string itemId, int itemLevelTo, int itemValueTo)
	{
	}

	public void MissionUnlocked()
	{
	}

	public void TutorialStart(string tutorialType, bool voiceOver)
	{
	}

	public void TutorialComplete(string tutorialType)
	{
	}

	public void TutorialSkip(string tutorialType, int stepIndex, int detailedStepIndex, int stepRepeatedCount)
	{
	}

	public void TutorialStep(string tutorialType, int stepIndex, int repeatedCount)
	{
	}

	public void LanguageChanged(SystemLanguage fromLanguage, SystemLanguage toLanguage)
	{
	}

	public void SettingsChanged(string changedValue, bool selected)
	{
	}

	public void HelpOptionSelected(string name)
	{
	}

	public void VisitUsClicked(string name)
	{
	}

	public void LegalClicked(string name)
	{
	}

	public void ResetGameClicked()
	{
	}

	public void UserSignedUp()
	{
	}

	public void UserAcceptedFriend(string friendName)
	{
	}
}
