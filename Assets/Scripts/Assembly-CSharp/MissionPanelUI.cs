using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MissionPanelUI : MonoBehaviour
{
	public Text missionSetHeadline;

	public Text missionSetReward;

	public Text[] missionTitle;

	public Text[] missionDesc;

	public Image[] missionProgressImage;

	public Button[] missionSkipBtns;

	public Text[] missionSkipBtnText;

	public Text[] missionSkipTextPrice;

	protected GameStateName m_CurrentGameStateName;

	protected bool m_MissionSetFinished;

	public Animation missionSetAnim;

	public AudioSource audioMissionProgress;

	public AudioSource audioMissionComplete;

	public AudioSource audioLevelUp;

	public AudioSource audioMissionShowUp;

	protected UAP_BaseElement accessibleHeadlineText;

	protected UAP_BaseElement accessibleRewardText;

	protected UAP_BaseElement[] accessibleMissionText = new UAP_BaseElement[3];

	protected UAP_BaseElement[] accessibleButtonText = new UAP_BaseElement[3];

	protected string m_strMissionLevel;

	protected string m_strRewardScoreX;

	protected string m_strRewardLights;

	protected string m_strCompleted;

	protected string m_strSkip;

	protected string m_strLeft;

	protected string m_ttsRewardScore1;

	protected string m_ttsRewardScore2;

	protected string m_ttsRewardLights;

	protected string m_ttsLevelUp;

	protected string m_ttsLevelUpScoreMultiplier;

	protected string m_ttsQuest;

	protected string m_ttsSkipQuest1;

	protected string m_ttsSkipQuest2;

	protected string m_ttsSkipQuest3;

	protected string m_ttsUnlocked;

	protected string m_ttsFirstUnlocked;

	protected string m_ttsProgressed;

	protected string m_ttsOnly;

	protected string m_ttsLevelUpLightReward;

	protected bool m_GameOver = true;

	protected bool m_LevelUp;

	protected bool m_EverythingStopped;

	protected bool m_Init;

	private const string c_White = "<color=#ffffffff>";

	private const string c_Grey = "<color=#b3b3b3ff>";

	private const string c_Red = "</color><color=#cc0000ff> ";

	private const string c_End = "</color>";

	private static Mission[] Missions => MissionManager.Instance.currentMissions;

	private static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	protected void Init()
	{
		accessibleHeadlineText = missionSetHeadline.GetComponent<UAP_BaseElement>();
		accessibleRewardText = missionSetReward.GetComponent<UAP_BaseElement>();
		for (int i = 0; i < 3; i++)
		{
			accessibleMissionText[i] = missionTitle[i].GetComponent<UAP_BaseElement>();
			accessibleButtonText[i] = missionSkipBtns[i].GetComponent<UAP_BaseElement>();
			if (m_CurrentGameStateName == GameStateName.Pause && accessibleButtonText[i] != null)
			{
				Object.Destroy(accessibleButtonText[i]);
			}
		}
		m_strMissionLevel = L("MISSION LEVEL") + " ";
		m_strRewardScoreX = L("REWARD SCORE x");
		m_strRewardLights = L("REWARD") + " " + NumberFormatter.FormatToLocale(5000) + " " + L("LIGHTS");
		m_strCompleted = L("COMPLETED");
		m_strSkip = L("SKIP");
		m_strLeft = " " + L("left");
		m_ttsRewardScore1 = L("tts_reward_score_1") + " ";
		m_ttsRewardScore2 = " " + L("tts_reward_score_2");
		m_ttsRewardLights = L("tts_reward_lights");
		m_ttsLevelUp = L("Level up!");
		m_ttsLevelUpScoreMultiplier = L("Your Score is now multiplied by") + " ";
		m_ttsLevelUpLightReward = L("tts_reward_lights_received") + " " + NumberFormatter.FormatToLocale(5000) + " " + L("LIGHTS");
		m_ttsQuest = L("Quest") + " ";
		m_ttsSkipQuest1 = L("tts_skip_quest_1") + " ";
		m_ttsSkipQuest2 = " " + L("tts_skip_quest_2") + " ";
		m_ttsSkipQuest3 = " " + L("tts_skip_quest_3");
		m_ttsUnlocked = L("Unlocked!");
		m_ttsFirstUnlocked = L("tts_first_unlocked");
		m_ttsOnly = L("Only") + " ";
		m_ttsProgressed = L("Progressed");
		m_Init = true;
	}

	public void Show(GameStateName gameStateName)
	{
		m_EverythingStopped = false;
		GetComponent<AccessibleUIGroupRoot>().enabled = true;
		m_CurrentGameStateName = gameStateName;
		m_GameOver = true;
		m_LevelUp = false;
		if (!m_Init)
		{
			Init();
		}
		UpdateHeadlines(false);
		if (m_CurrentGameStateName == GameStateName.GameOver)
		{
			UpdateMissionsForAnimation();
		}
		else
		{
			for (int i = 0; i < 3; i++)
			{
				UpdateMissionProgress(i);
			}
		}
		audioLevelUp.gameObject.SetActive(true);
		audioMissionShowUp.gameObject.SetActive(true);
		audioMissionComplete.gameObject.SetActive(true);
		audioMissionProgress.gameObject.SetActive(true);
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		m_MissionSetFinished = false;
		StopAllCoroutines();
		audioMissionShowUp.Stop();
		audioMissionComplete.Stop();
		audioMissionProgress.Stop();
		audioLevelUp.Stop();
		audioLevelUp.gameObject.SetActive(false);
		audioMissionShowUp.gameObject.SetActive(false);
		audioMissionComplete.gameObject.SetActive(false);
		audioMissionProgress.gameObject.SetActive(false);
		gameObject.SetActive(false);
		ShowSkipButtons();
	}

	public void DisabelAccessibility()
	{
		GetComponent<AccessibleUIGroupRoot>().enabled = false;
	}

	public void StopEverything()
	{
		StopAllCoroutines();
		m_EverythingStopped = true;
		audioMissionShowUp.Stop();
		audioMissionComplete.Stop();
		audioMissionProgress.Stop();
		audioLevelUp.Stop();
	}

	public bool SkipAnimation()
	{
		if (m_LevelUp)
		{
			return false;
		}
		StopAllCoroutines();
		return ShowMissionResults();
	}

	public bool AccessibleFocusChanged()
	{
		if (m_LevelUp)
		{
			return false;
		}
		StopAllCoroutines();
		if (UAP_AccessibilityManager.IsSpeaking())
		{
			UAP_AccessibilityManager.StopSpeaking();
		}
		audioMissionProgress.Stop();
		return ShowMissionResults();
	}

	protected void UpdateHeadlines(bool showUpshot)
	{
		string level = (MissionManager.Instance.currentMissionSet + 1).ToString();
		if (level.Length == 1)
		{
			level = "00" + level;
		}
		else if (level.Length == 2)
		{
			level = "0" + level;
		}
		missionSetHeadline.text = m_strMissionLevel + level;
		if (accessibleHeadlineText != null)
		{
			accessibleHeadlineText.m_Text = m_strMissionLevel + (MissionManager.Instance.currentMissionSet + 1).ToString();
		}
		DataManager dataManager = DataManager.Instance;
		if (MissionManager.Instance.currentMissionSet < dataManager.maxAvailableMissions)
		{
			missionSetReward.text = m_strRewardScoreX + (dataManager.playerData.scoreMultiplier + 1).ToString();
			if (accessibleRewardText != null)
			{
				accessibleRewardText.m_Text = m_ttsRewardScore1 + (dataManager.playerData.scoreMultiplier + 1).ToString() + m_ttsRewardScore2;
			}
		}
		else
		{
			missionSetReward.text = m_strRewardLights;
			if (accessibleRewardText != null)
			{
				accessibleRewardText.m_Text = m_ttsRewardLights;
			}
		}
		for (int i = 0; i < 3; i++)
		{
			missionTitle[i].text = Missions[i].GetMissionTitle();
		}
	}

	protected bool ShowMissionResults()
	{
		bool allCompleted = true;
		for (int i = 0; i < 3; i++)
		{
			Mission mission = Missions[i];
			if (!mission.finished)
			{
				allCompleted &= mission.completed;
			}
			if (mission.finished)
			{
				mission.completed = true;
			}
		}
		if (allCompleted)
		{
			OnMissionSetFinished();
			return false;
		}
		for (int j = 0; j < 3; j++)
		{
			UpdateMissionProgress(j);
		}
		for (int k = 0; k < 3; k++)
		{
			missionTitle[k].gameObject.SetActive(true);
			missionDesc[k].gameObject.SetActive(true);
			missionSkipBtns[k].gameObject.SetActive(true);
		}
		return true;
	}

	protected void OnMissionSetFinished()
	{
		DataManager dataManager = DataManager.Instance;
		if (MissionManager.Instance.currentMissionSet < dataManager.maxAvailableMissions)
		{
			dataManager.IncreaseScoreMultiplier();
		}
		else
		{
			dataManager.SaveCoins(5000, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
		}
		dataManager.SetLastRewardedMission(MissionManager.Instance.currentMissionSet);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		MissionManager.Instance.LoadNextMissionSet(ShowNextMissionSet);
	}

	private void SelectHeadline()
	{
		UAP_AccessibilityManager.BlockInput(false, true);
		UAP_AccessibilityManager.SelectElement(missionSetHeadline.gameObject, true);
	}

	public void ShowNextMissionSet(bool nextMissionAvailable)
	{
		if (nextMissionAvailable)
		{
			CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionSetLevelUp, null);
			audioLevelUp.Play();
			if (UAP_AccessibilityManager.IsEnabled())
			{
				missionSetAnim.Play("MissionSetCompleteVoiceOverIn");
				if (gameObject.activeSelf && !m_EverythingStopped)
				{
					StartCoroutine(PresentNextMissionSetVoiceOver());
				}
			}
			else
			{
				missionSetAnim.Play("MissionSetComplete3");
				if (gameObject.activeSelf && !m_EverythingStopped)
				{
					StartCoroutine(PresentNextMissionSet());
				}
			}
			return;
		}
		if (m_GameOver)
		{
			CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionSetAnimationFinished, null);
			if (m_GameOver && (!UAP_AccessibilityManager.IsEnabled() || m_CurrentGameStateName != GameStateName.Menu))
			{
				return;
			}
		}
		SelectHeadline();
	}

	// Speaks the text and waits for it to start (or 1.5 s) and finish
	private IEnumerator SayAndWait(string text)
	{
		UAP_AccessibilityManager.Say(text, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
		bool startedSpeaking = false;
		float startedSpeakingTimeOut = 0f;
		while (!startedSpeaking)
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				startedSpeaking = true;
			}
			startedSpeakingTimeOut += Time.deltaTime;
			if (1.5f < startedSpeakingTimeOut)
			{
				startedSpeaking = true;
			}
			yield return null;
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
	}

	private IEnumerator PresentNextMissionSetVoiceOver()
	{
		yield return SayAndWait(m_strMissionLevel + MissionManager.Instance.currentMissionSet.ToString() + " " + m_strCompleted);
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		yield return SayAndWait(m_ttsLevelUp);
		string reward = (MissionManager.Instance.currentMissionSet < DataManager.Instance.maxAvailableMissions) ? (m_ttsLevelUpScoreMultiplier + DataManager.Instance.playerData.scoreMultiplier.ToString()) : m_ttsLevelUpLightReward;
		yield return SayAndWait(reward);
		missionSetAnim.Play("MissionSetCompleteVoiceOverOut");
		yield return new WaitForSeconds(0.5f);
		UpdateHeadlines(false);
		UpdateMissionsProgress();
		missionSetAnim.Play("MissionSetNew");
		if (gameObject.activeSelf && !m_EverythingStopped)
		{
			StartCoroutine(TellTopStateWhenAnimationFinished());
		}
	}

	private IEnumerator PresentNextMissionSet()
	{
		yield return new WaitForSeconds(3.5f);
		UpdateHeadlines(false);
		UpdateMissionsProgress();
		missionSetAnim.Play("MissionSetNew");
		if (gameObject.activeSelf && !m_EverythingStopped)
		{
			StartCoroutine(TellTopStateWhenAnimationFinished());
		}
	}

	private IEnumerator TellTopStateWhenAnimationFinished()
	{
		yield return new WaitForSeconds(1f);
		if (!m_GameOver || (UAP_AccessibilityManager.IsEnabled() && m_CurrentGameStateName == GameStateName.Menu))
		{
			SelectHeadline();
		}
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionSetLevelUpFinished, null);
		if (m_GameOver && UAP_AccessibilityManager.IsEnabled())
		{
			SelectHeadline();
		}
		m_GameOver = false;
	}

	protected void UpdateMissionsProgress()
	{
		for (int i = 0; i < 3; i++)
		{
			UpdateMissionProgress(i);
		}
	}

	public void HideSkipButtons()
	{
		for (int i = 0; i < missionSkipBtns.Length; i++)
		{
			missionSkipBtns[i].gameObject.SetActive(false);
		}
	}

	public void ShowSkipButtons()
	{
		for (int i = 0; i < missionSkipBtns.Length; i++)
		{
			missionSkipBtns[i].gameObject.SetActive(true);
		}
	}

	private string AccessibleMissionText(int missionNumber, string last, string separator = "\n")
	{
		Mission mission = Missions[missionNumber];
		return m_ttsQuest + (missionNumber + 1).ToString() + separator + mission.GetMissionTitle(true) + " " + mission.GetMissionDesc(true) + ".\n " + last;
	}

	protected void UpdateMissionProgress(int missionNumber)
	{
		Mission mission = Missions[missionNumber];
		missionDesc[missionNumber].text = c_White + mission.GetMissionDesc() + c_Red + mission.GetMissionProgress() + c_End;
		missionProgressImage[missionNumber].fillAmount = mission.GetProgressInPercent();
		if (accessibleMissionText[missionNumber] != null)
		{
			accessibleMissionText[missionNumber].m_Text = AccessibleMissionText(missionNumber, mission.completed ? m_strCompleted : mission.GetMissionProgress(true));
		}
		if (mission.completed)
		{
			ShowButtonCompleted(missionNumber);
		}
		else if (m_CurrentGameStateName == GameStateName.Pause)
		{
			HideButton(missionNumber);
		}
		else
		{
			ShowSkipButton(missionNumber);
		}
	}

	protected void ShowButtonCompleted(int missionNumber)
	{
		missionSkipBtns[missionNumber].interactable = false;
		missionSkipBtnText[missionNumber].text = c_White + m_strCompleted + c_End;
		missionDesc[missionNumber].text = c_White + Missions[missionNumber].GetMissionDesc() + c_End;
		missionSkipTextPrice[missionNumber].text = "";
		UAP_BaseElement element = accessibleButtonText[missionNumber];
		if (element == null)
		{
			return;
		}
		if (!element.enabled)
		{
			element.enabled = true;
		}
		element.m_Text = m_ttsQuest + (missionNumber + 1).ToString() + m_strCompleted;
		element.m_ReadType = false;
		element.m_CustomHint = true;
		element.m_Hint = "";
	}

	protected void HideButton(int missionNumber)
	{
		missionSkipBtns[missionNumber].interactable = false;
		missionSkipBtnText[missionNumber].text = "";
		missionSkipTextPrice[missionNumber].text = "";
	}

	private int GetSkipCosts(int missionNumber)
	{
		DataManager dataManager = DataManager.Instance;
		int costs = dataManager.skipCosts + dataManager.skipAdditionalCostsPerLevel * MissionManager.Instance.currentMissionSet;
		if (dataManager.maxSkipCosts < costs)
		{
			costs = dataManager.maxSkipCosts;
		}
		Mission mission = Missions[missionNumber];
		if (0f < mission.GetProgressInPercent())
		{
			costs = (costs - (int)(mission.GetProgressInPercent() * (float)costs)) / 5 * 5 + 5;
		}
		return costs;
	}

	protected void ShowSkipButton(int missionNumber)
	{
		int costs = GetSkipCosts(missionNumber);
		bool affordable = DataManager.Instance.playerData.coins >= costs;
		string color = affordable ? c_White : c_Grey;
		if (!affordable)
		{
			missionSkipBtns[missionNumber].interactable = false;
		}
		missionSkipBtnText[missionNumber].text = color + m_strSkip + c_End;
		missionSkipTextPrice[missionNumber].text = color + NumberFormatter.FormatToLocale(costs) + c_End;
		if (affordable)
		{
			missionSkipBtns[missionNumber].interactable = true;
		}
		UAP_BaseElement element = accessibleButtonText[missionNumber];
		if (element == null)
		{
			return;
		}
		if (!element.enabled)
		{
			element.enabled = true;
		}
		element.m_Text = m_ttsSkipQuest1 + (missionNumber + 1).ToString() + m_ttsSkipQuest2 + NumberFormatter.FormatToLocale(costs) + m_ttsSkipQuest3;
		element.m_ReadType = true;
		if (affordable)
		{
			element.m_CustomHint = false;
		}
		else
		{
			element.m_CustomHint = true;
			element.m_Hint = "";
		}
	}

	protected void UpdateMissionsForAnimation()
	{
		for (int i = 0; i < Missions.Length; i++)
		{
			Mission mission = Missions[i];
			if (!mission.completed && mission.startProgress < mission.progress)
			{
				ShowMissionStartProgress(i);
			}
			else
			{
				UpdateMissionProgress(i);
			}
			missionTitle[i].gameObject.SetActive(false);
			missionDesc[i].gameObject.SetActive(false);
			missionSkipBtns[i].gameObject.SetActive(false);
		}
	}

	protected void ShowMissionStartProgress(int missionNumber)
	{
		Mission mission = Missions[missionNumber];
		missionDesc[missionNumber].text = c_White + mission.GetMissionDesc() + c_Red + NumberFormatter.FormatToLocale((int)(mission.goal - mission.startProgress)) + m_strLeft + c_End;
		missionProgressImage[missionNumber].fillAmount = mission.startProgress / mission.goal;
		HideButton(missionNumber);
	}

	public void StartGameOverAnimation()
	{
		bool firstGame = false;
		if (DataManager.Instance.playerData.isFirstGame)
		{
			DataManager.Instance.FirstGamePlayed();
			firstGame = true;
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (gameObject.activeSelf && !m_EverythingStopped)
			{
				StartCoroutine(StartGameOverAnimationVoiceOver(firstGame));
			}
			return;
		}
		GameOverAnimationFinished(-1);
	}

	protected void GameOverAnimationFinished(int missionNumber)
	{
		if (missionNumber == -1 && m_GameOver)
		{
			for (int i = 0; i < 3; i++)
			{
				missionSkipBtns[i].interactable = false;
			}
		}
		int next = missionNumber + 1;
		if (Missions.Length <= next)
		{
			if (m_MissionSetFinished)
			{
				m_LevelUp = true;
				Invoke("OnMissionSetFinished", 0.5f);
				return;
			}
			if (m_GameOver)
			{
				for (int j = 0; j < 3; j++)
				{
					if (!Missions[j].completed)
					{
						missionSkipBtns[j].interactable = true;
					}
				}
				CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.MissionSetAnimationFinished, null);
				if (m_GameOver && (!UAP_AccessibilityManager.IsEnabled() || m_CurrentGameStateName != GameStateName.Menu))
				{
					m_GameOver = false;
					return;
				}
			}
			SelectHeadline();
			m_GameOver = false;
			return;
		}
		Mission mission = Missions[next];
		IEnumerator routine;
		if (mission.finished)
		{
			mission.completed = true;
			routine = AnimateMissionProgressGameOver(next);
		}
		else if (!mission.completed && mission.startProgress < mission.progress)
		{
			routine = AnimateMissionProgressGameOver(next);
		}
		else
		{
			routine = GameOverBlendIn(next);
		}
		if (gameObject.activeSelf && !m_EverythingStopped)
		{
			StartCoroutine(routine);
		}
	}

	private IEnumerator StartGameOverAnimationVoiceOver(bool firstGame)
	{
		bool startedSpeaking = false;
		float timeToWait = 0f;
		while (true)
		{
			if (UAP_AccessibilityManager.IsSpeaking())
			{
				startedSpeaking = true;
			}
			else if (1f < timeToWait)
			{
				UAP_AccessibilityManager.Say(m_strMissionLevel + (MissionManager.Instance.currentMissionSet + 1).ToString(), true, true, (UAP_AudioQueue.EInterrupt)0x4f);
				timeToWait = 0f;
			}
			timeToWait += Time.deltaTime;
			yield return null;
			if (startedSpeaking)
			{
				break;
			}
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		if (firstGame)
		{
			foreach (string text in new string[2] { m_ttsUnlocked, m_ttsFirstUnlocked })
			{
				startedSpeaking = false;
				UAP_AccessibilityManager.Say(text, true, true, (UAP_AudioQueue.EInterrupt)0x4f);
				while (!startedSpeaking)
				{
					if (UAP_AccessibilityManager.IsSpeaking())
					{
						startedSpeaking = true;
					}
					yield return null;
				}
				while (UAP_AccessibilityManager.IsSpeaking())
				{
					yield return null;
				}
			}
		}
		GameOverAnimationFinished(-1);
	}

	private IEnumerator GameOverBlendIn(int currentMission)
	{
		if (currentMission == 0)
		{
			yield return new WaitForSeconds(0.5f);
		}
		audioMissionShowUp.Play();
		missionTitle[currentMission].gameObject.SetActive(true);
		missionDesc[currentMission].gameObject.SetActive(true);
		missionSkipBtns[currentMission].gameObject.SetActive(true);
		if (!UAP_AccessibilityManager.IsEnabled())
		{
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
			Mission mission = Missions[currentMission];
			string text;
			if (mission.completed)
			{
				text = AccessibleMissionText(currentMission, m_strCompleted, "\n ");
			}
			else if (!DataManager.Instance.playerData.isFirstGame)
			{
				text = AccessibleMissionText(currentMission, mission.GetMissionProgress(true), "\n ");
			}
			else
			{
				text = m_ttsQuest + (currentMission + 1).ToString() + "\n " + mission.GetMissionTitle(true) + " " + mission.GetMissionDesc(true);
			}
			yield return SayAndWait(text);
			yield return new WaitForSeconds(0.2f);
		}
		GameOverAnimationFinished(currentMission);
	}

	// Animates the mission progress bar (and the "left" number); shared by both animations
	private IEnumerator AnimateProgressBar(int missionNumber, bool zeroFinalLeft, string leftSuffix)
	{
		Mission mission = Missions[missionNumber];
		float elapsedTime = 0f;
		float finalProgress = mission.GetProgressInPercent();
		int finalLeft = mission.GetLeftNumber();
		bool updateLeft = finalLeft != -1;
		if (zeroFinalLeft)
		{
			finalLeft = 0;
		}
		float missionStartFillAmount = missionProgressImage[missionNumber].fillAmount;
		int missionStartLeft = (int)(mission.goal - mission.startProgress);
		float pitchStartValue = 0.8f;
		audioMissionProgress.pitch = 0.8f;
		audioMissionProgress.Play();
		float finalPitch = pitchStartValue + (finalProgress - missionStartFillAmount) * (1.2f - pitchStartValue);
		float duration = (finalProgress - missionStartFillAmount) * 0.8f;
		while (elapsedTime < duration)
		{
			missionProgressImage[missionNumber].fillAmount = Mathf.Lerp(missionStartFillAmount, finalProgress, elapsedTime / duration);
			audioMissionProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
			if (updateLeft)
			{
				int left = (int)Mathf.Lerp(missionStartLeft, finalLeft, elapsedTime / duration);
				missionDesc[missionNumber].text = c_White + mission.GetMissionDesc() + c_Red + NumberFormatter.FormatToLocale(left) + leftSuffix + c_End;
			}
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		missionProgressImage[missionNumber].fillAmount = finalProgress;
		audioMissionProgress.Stop();
	}

	private void ShowMissionCompleted(int missionNumber)
	{
		audioMissionComplete.Play();
		missionDesc[missionNumber].text = c_White + Missions[missionNumber].GetMissionDesc() + c_End;
		accessibleMissionText[missionNumber].m_Text = AccessibleMissionText(missionNumber, m_strCompleted);
		ShowButtonCompleted(missionNumber);
	}

	private IEnumerator AnimateMissionProgressGameOver(int missionNumber)
	{
		if (missionNumber == 0)
		{
			yield return new WaitForSeconds(UAP_AccessibilityManager.IsEnabled() ? 0.2f : 0.5f);
		}
		audioMissionShowUp.Play();
		missionTitle[missionNumber].gameObject.SetActive(true);
		missionDesc[missionNumber].gameObject.SetActive(true);
		missionSkipBtns[missionNumber].gameObject.SetActive(true);
		Mission mission = Missions[missionNumber];
		if (UAP_AccessibilityManager.IsEnabled())
		{
			yield return SayAndWait(m_ttsQuest + (missionNumber + 1).ToString() + "\n" + mission.GetMissionTitle(true) + " " + mission.GetMissionDesc(true));
			yield return new WaitForSeconds(0.1f);
		}
		yield return AnimateProgressBar(missionNumber, false, m_strLeft);
		if (!mission.completed)
		{
			missionDesc[missionNumber].text = c_White + mission.GetMissionDesc() + c_Red + mission.GetMissionProgress() + c_End;
			ShowSkipButton(missionNumber);
			if (UAP_AccessibilityManager.IsEnabled())
			{
				yield return SayAndWait(m_ttsProgressed + ".\n " + m_ttsOnly + mission.GetMissionProgress(true));
			}
		}
		else
		{
			ShowMissionCompleted(missionNumber);
			if (UAP_AccessibilityManager.IsEnabled())
			{
				yield return SayAndWait(m_strCompleted);
			}
		}
		if (!UAP_AccessibilityManager.IsEnabled())
		{
			yield return new WaitForSeconds(0.5f);
		}
		GameOverAnimationFinished(missionNumber);
	}

	private IEnumerator AnimateMissionProgress(int missionNumber, bool skipping = false)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true, true);
		}
		yield return AnimateProgressBar(missionNumber, true, " " + L("left"));
		if (Missions[missionNumber].completed)
		{
			ShowMissionCompleted(missionNumber);
			if (UAP_AccessibilityManager.IsEnabled())
			{
				yield return SayAndWait(m_ttsQuest + (missionNumber + 1).ToString() + " " + m_strCompleted);
			}
		}
		if (skipping)
		{
			UpdateMissionSkipButtons();
		}
		SkipAnimationFinished(missionNumber);
	}

	protected void SkipAnimationFinished(int missionNumber)
	{
		if (m_MissionSetFinished)
		{
			Invoke("OnMissionSetFinished", 0.5f);
			return;
		}
		for (int i = 0; i < Missions.Length; i++)
		{
			Mission mission = Missions[i];
			if (!mission.completed && mission.missionType == MissionType.SkipMission)
			{
				mission.IncreaseProgress();
				if (mission.finished)
				{
					m_MissionSetFinished = MissionManager.Instance.CompleteMission(i);
					if (gameObject.activeSelf && !m_EverythingStopped)
					{
						StartCoroutine(AnimateMissionProgress(i));
					}
					return;
				}
			}
			else if (UAP_AccessibilityManager.IsEnabled())
			{
				UAP_AccessibilityManager.BlockInput(false, true);
				UAP_AccessibilityManager.SelectElement(missionSkipBtns[missionNumber].gameObject, false);
			}
		}
	}

	protected void SkipMission(int missionNumber)
	{
		HideButton(missionNumber);
		int costs = GetSkipCosts(missionNumber);
		if (!DataManager.Instance.InvestCoins(costs, (TransactionContext)0, (TransactionalItem)1, (TransactionItemType)1, null))
		{
			ShowSkipButton(missionNumber);
			return;
		}
		UpdateMissionSkipButtons();
		switch (missionNumber)
		{
		case 0:
			missionSkipBtns[1].interactable = false;
			missionSkipBtns[2].interactable = false;
			break;
		case 1:
			missionSkipBtns[0].interactable = false;
			missionSkipBtns[2].interactable = false;
			break;
		case 2:
			missionSkipBtns[0].interactable = false;
			missionSkipBtns[1].interactable = false;
			break;
		}
		m_MissionSetFinished = MissionManager.Instance.SkipMission(missionNumber);
		CustomGameManager.Instance.topState.ReceiveInfoMessage(InfoMessage.PurchaseMade, null);
		if (gameObject.activeSelf && !m_EverythingStopped)
		{
			StartCoroutine(AnimateMissionProgress(missionNumber, true));
		}
	}

	public void SkipMission1()
	{
		SkipMission(0);
	}

	public void SkipMission2()
	{
		SkipMission(1);
	}

	public void SkipMission3()
	{
		SkipMission(2);
	}

	protected void UpdateMissionSkipButtons()
	{
		for (int i = 0; i < 3; i++)
		{
			if (Missions[i].completed)
			{
				continue;
			}
			int costs = GetSkipCosts(i);
			bool affordable = DataManager.Instance.playerData.coins >= costs;
			string color = affordable ? c_White : c_Grey;
			missionSkipBtnText[i].text = color + m_strSkip + c_End;
			missionSkipTextPrice[i].text = color + NumberFormatter.FormatToLocale(costs) + c_End;
			missionSkipBtns[i].interactable = affordable;
		}
	}

	public void SaveProgress()
	{
		m_MissionSetFinished = MissionManager.Instance.SaveProgress();
	}
}
