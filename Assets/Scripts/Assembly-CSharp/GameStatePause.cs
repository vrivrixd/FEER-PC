using System.Collections;
using UnityEngine;

public class GameStatePause : GameState
{
	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject confirmPanel;

	public GameObject confirmQuitPanel;

	public GameObject confirmRestartPanel;

	public MissionPanelUI missionSetPanel;

	public GameObject navPanel;

	public RectTransform panelTransform;

	protected string m_ttsGamePaused;

	protected bool m_Init;

	public override void Enter(GameState from)
	{
		if (!m_Init)
		{
			Init();
		}
		MissionManager.Instance.UpdateProgress();
		confirmPanel.SetActive(false);
		confirmQuitPanel.SetActive(false);
		confirmRestartPanel.SetActive(false);
		navPanel.SetActive(true);
		if (!DataManager.Instance.playerData.isFirstGame)
		{
			missionSetPanel.Show(GetName());
		}
		scoreCoinsPanel.RenderPlayGameScoreUI(true);
		scoreCoinsPanel.RenderPlayGameCoinsUI(true);
		gameObject.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1, false);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.Say(m_ttsGamePaused);
		}
		CustomGameManager.Instance.PauseGame();
	}

	public override void Exit(GameState to)
	{
		missionSetPanel.Hide();
		scoreCoinsPanel.Hide();
		navPanel.SetActive(false);
		StopAllCoroutines();
		gameObject.SetActive(false);
	}

	public override GameStateName GetName()
	{
		return GameStateName.Pause;
	}

	public override void Tick()
	{
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	protected void Init()
	{
		m_ttsGamePaused = LocalizationManager.Instance.GetLocalizedValue("Game paused");
		m_Init = true;
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true);
		}
		confirmPanel.SetActive(false);
		confirmQuitPanel.SetActive(false);
		confirmRestartPanel.SetActive(false);
		missionSetPanel.missionSetAnim.Play("PanelSlideOut");
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		yield return new WaitForSeconds(0.5f);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false);
		}
		CustomGameManager.Instance.SwitchState(toState);
	}

	public void ResumeBtnClicked()
	{
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.PlayGame));
		}
	}

	// PORT: on PC, Esc/Circle in the pause menu resumes the game (in the original only the Resume button did).
	// With a confirmation open, Esc cancels the confirmation.
	public override bool PortBack()
	{
		if (!gameObject.activeSelf)
		{
			return true;
		}
		if (confirmQuitPanel.activeSelf)
		{
			ConfirmQuitNoClicked();
		}
		else if (confirmRestartPanel.activeSelf)
		{
			ConfirmRestartNoClicked();
		}
		else
		{
			ResumeBtnClicked();
		}
		return true;
	}

	public void QuitBtnClicked()
	{
		confirmQuitPanel.SetActive(true);
		confirmPanel.SetActive(true);
	}

	public void RestartBtnClicked()
	{
		confirmRestartPanel.SetActive(true);
		confirmPanel.SetActive(true);
	}

	public void ConfirmRestartYesClicked()
	{
		MissionManager.Instance.ResetProgress();
		CustomGameManager.Instance.QuitGame();
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.SelectBoost));
		}
	}

	public void ConfirmRestartNoClicked()
	{
		confirmRestartPanel.SetActive(false);
		confirmPanel.SetActive(false);
	}

	public void ConfirmQuitYesClicked()
	{
		MissionManager.Instance.ResetProgress();
		CustomGameManager.Instance.QuitGame();
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(GameStateName.Menu));
		}
	}

	public void ConfirmQuitNoClicked()
	{
		confirmQuitPanel.SetActive(false);
		confirmPanel.SetActive(false);
	}
}
