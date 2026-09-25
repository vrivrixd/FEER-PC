using System.Collections;

public class GameStateSelectBoost : GameState
{
	public override void Enter(GameState from)
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

	public override void Tick()
	{
	}

	public override void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData)
	{
	}

	public override void Exit(GameState to)
	{
	}

	private IEnumerator StartGame(GameStateName fromState)
	{
		return null;
	}
}
