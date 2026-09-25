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

	protected UAP_BaseElement[] accessibleMissionText;

	protected UAP_BaseElement[] accessibleButtonText;

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

	protected bool m_GameOver;

	protected bool m_LevelUp;

	protected bool m_EverythingStopped;

	protected bool m_Init;

	protected void Init()
	{
	}

	public void Show(GameStateName gameStateName)
	{
	}

	public void Hide()
	{
	}

	public void DisabelAccessibility()
	{
	}

	public void StopEverything()
	{
	}

	public bool SkipAnimation()
	{
		return false;
	}

	public bool AccessibleFocusChanged()
	{
		return false;
	}

	protected void UpdateHeadlines(bool showUpshot)
	{
	}

	protected bool ShowMissionResults()
	{
		return false;
	}

	protected void OnMissionSetFinished()
	{
	}

	public void ShowNextMissionSet(bool nextMissionAvailable)
	{
	}

	private IEnumerator PresentNextMissionSetVoiceOver()
	{
		return null;
	}

	private IEnumerator PresentNextMissionSet()
	{
		return null;
	}

	private IEnumerator TellTopStateWhenAnimationFinished()
	{
		return null;
	}

	protected void UpdateMissionsProgress()
	{
	}

	public void HideSkipButtons()
	{
	}

	public void ShowSkipButtons()
	{
	}

	protected void UpdateMissionProgress(int missionNumber)
	{
	}

	protected void ShowButtonCompleted(int missionNumber)
	{
	}

	protected void HideButton(int missionNumber)
	{
	}

	protected void ShowSkipButton(int missionNumber)
	{
	}

	protected void UpdateMissionsForAnimation()
	{
	}

	protected void ShowMissionStartProgress(int missionNumber)
	{
	}

	public void StartGameOverAnimation()
	{
	}

	protected void GameOverAnimationFinished(int missionNumber)
	{
	}

	private IEnumerator StartGameOverAnimationVoiceOver(bool firstGame)
	{
		return null;
	}

	private IEnumerator GameOverBlendIn(int currentMission)
	{
		return null;
	}

	private IEnumerator AnimateMissionProgressGameOver(int missionNumber)
	{
		return null;
	}

	private IEnumerator AnimateMissionProgress(int missionNumber, bool skipping = false)
	{
		return null;
	}

	protected void SkipAnimationFinished(int missionNumber)
	{
	}

	protected void SkipMission(int missionNumber)
	{
	}

	public void SkipMission1()
	{
	}

	public void SkipMission2()
	{
	}

	public void SkipMission3()
	{
	}

	protected void UpdateMissionSkipButtons()
	{
	}

	public void SaveProgress()
	{
	}
}
