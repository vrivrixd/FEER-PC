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
	}

	public override void Exit(GameState to)
	{
	}

	public override GameStateName GetName()
	{
		return GameStateName.None;
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
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	public void ResumeBtnClicked()
	{
	}

	public void QuitBtnClicked()
	{
	}

	public void RestartBtnClicked()
	{
	}

	public void ConfirmRestartYesClicked()
	{
	}

	public void ConfirmRestartNoClicked()
	{
	}

	public void ConfirmQuitYesClicked()
	{
	}

	public void ConfirmQuitNoClicked()
	{
	}
}
