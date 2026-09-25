using UnityEngine;
using UnityEngine.UI;

public class GameStateIsDead : GameState
{
	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject saveMePanel;

	public GameObject headline;

	public Text saveMeSubtitle;

	protected UAP_BaseElement m_AccessibleSaveMeSubtitle;

	protected int m_SaveMeTimesLeft;

	protected int m_SaveMeCost;

	public RectTransform panelTransform;

	protected bool m_Init;

	protected string m_1For;

	protected string m_2LightsTheFairies;

	protected string m_3Lights;

	protected string m_ttsYouScored;

	protected string m_ttsYouCollected;

	protected string m_ttsLightsThisRun;

	protected string m_ttsPointsThisRun;

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

	protected void InitIsDead()
	{
	}

	public void ResetSaveMeCosts()
	{
	}

	public void SaveMeBtnClicked()
	{
	}

	public void SkipBtnClicked()
	{
	}
}
