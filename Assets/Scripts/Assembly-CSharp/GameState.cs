using UnityEngine;

public abstract class GameState : MonoBehaviour
{
	public abstract void Enter(GameState from);

	public abstract void Exit(GameState to);

	public abstract void Tick();

	public abstract GameStateName GetName();

	public abstract GameStateStatus GetStatus();

	public abstract void ReceiveInfoMessage(InfoMessage infoMessage, string additionalData);
}
