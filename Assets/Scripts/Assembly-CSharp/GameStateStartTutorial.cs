using System.Collections;
using UnityEngine;

public class GameStateStartTutorial : GameState
{
	public GameObject startTutorialBtn;

	public AudioSource speechAudio;

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

	public void StartTutorialBtnClicked()
	{
	}

	private IEnumerator SpeakText()
	{
		return null;
	}
}
