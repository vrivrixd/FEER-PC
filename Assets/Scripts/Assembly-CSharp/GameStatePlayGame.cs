using System.Collections;
using UnityEngine;

public class GameStatePlayGame : GameState
{
	public GameObject resumePanel;

	public Animator clockAnimator;

	public AudioSource speechAudio;

	public GameObject skipTutorialPanel;

	public GameObject tutorialLoadingPanel;

	public GameObject ConfirmSkipTutorial;

	public RectTransform panelTransform;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject topPanel;

	protected GameStateStatus m_Status;

	protected bool m_Tutorial;

	protected GameStateName m_GameStateFrom;

	protected string m_TutorialType;

	protected AudioClip[] m_TutorialClips;

	protected bool m_AudioClipsLoaded;

	protected ThemeTutorial m_TutorialScript;

	protected bool m_FirstTimePowerUpSpeaking;

	protected ConsumableType m_FirstTimePowerUpSpeakingType;

	public override void Enter(GameState from)
	{
	}

	private IEnumerator ClockResume()
	{
		return null;
	}

	public override void Tick()
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

	public override void Exit(GameState to)
	{
	}

	protected void ShowScoreCoinsPanel()
	{
	}

	protected void UpdateScore()
	{
	}

	protected void UpdateLights()
	{
	}

	protected void StartNewGame()
	{
	}

	public void TutorialStartNewGame()
	{
	}

	private IEnumerator FirstGameInformation()
	{
		return null;
	}

	public void PauseBtnClicked()
	{
	}

	public void PowerUpCollected(Consumable powerUp)
	{
	}

	public void FirstAccessiblePowerUp(ConsumableType consumableType)
	{
	}

	protected IEnumerator FirstAccessiblePowerUpInfo(ConsumableType consumableType)
	{
		return null;
	}

	public void StartTutorial()
	{
	}

	protected void PrepareTutorial()
	{
	}

	private IEnumerator LoadTutorialClips()
	{
		return null;
	}

	public void TutorialAudioClipsLoaded(bool success)
	{
	}

	public void ShowTutorialSkipPanel()
	{
	}

	public void TutorialEnd()
	{
	}

	private IEnumerator InformAnalytics()
	{
		return null;
	}

	public void SkipTutorialBtnClicked()
	{
	}

	protected void PauseTutorial()
	{
	}

	public void ConfirmTutorialQuitClicked()
	{
	}

	public void ConfirmTutorialResumeClicked()
	{
	}

	public void TutorialResume()
	{
	}
}
