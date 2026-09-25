using System.Collections;
using UnityEngine;

public class GameStateSelectBoost : GameState
{

	public override void Enter(GameState from)
	{
		gameObject.SetActive(true);
		if (gameObject.activeSelf)
		{
			StartCoroutine(StartGame(from.GetName()));
		}
	}

	public override GameStateName GetName()
	{
		return GameStateName.SelectBoost;
	}

	public override GameStateStatus GetStatus()
	{
		return GameStateStatus.None;
	}

	public override void Tick()
	{
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
	}

	private IEnumerator StartGame(GameStateName fromState)
	{
		yield return new WaitForEndOfFrame();
		CustomGameManager.Instance.currentGameNumber = CustomGameManager.Instance.currentGameNumber + 1;
		CustomAnalyticsTracker.Instance.NewGameStarted(fromState);
		CustomGameManager.Instance.SwitchState(GameStateName.PlayGame);
	}
}
