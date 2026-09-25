using System.Collections;
using UnityEngine;

public class GameStateMenu : GameState
{
	public MissionPanelUI missionPanel;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject navPanel;

	public GameObject quitBtn;

	public RectTransform panelTransform;

	protected string m_strMainMenu;

	public override void Enter(GameState from)
	{
	}

	public override GameStateName GetName()
	{
		return GameStateName.None;
	}

	public override void Tick()
	{
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

	protected void HideInteractivePanels()
	{
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		return null;
	}

	public void PlayBtnClicked()
	{
	}

	public void InventoryBtnClicked()
	{
	}

	public void HighscoreBtnClicked()
	{
	}

	public void OptionsBtnClicked()
	{
	}

	public void ProfileBtnClicked()
	{
	}

	public void QuitGameBtnClicked()
	{
	}
}
