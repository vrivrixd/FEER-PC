using System.Collections;

public class GameStateInit : GameState
{
	protected int m_InitCount;

	protected bool m_TutorialAudioClipsLoaded;

	protected const int c_ROUTINE_LANGUAGE_CHANGE = 1;

	protected const int c_ROUTINE_TUTORIAL = 2;

	protected const int c_ROUTINE_ANNOUNCEMENT = 3;

	protected const int c_ROUTINE_START_GAME = 4;

	protected const int c_ROUTINE_CHANGE_THEME = 5;

	protected const int c_ROUTINE_CHANGE_THEME_TUTORIAL = 8;

	protected int m_CurrentInitRoutine;

	protected const int c_INIT_THEME = 1;

	protected const int c_INIT_MISSIONS = 2;

	protected const int c_INIT_TRACK = 3;

	protected const int c_INIT_TUTORIAL = 4;

	protected const int c_INIT_ANALYTICS = 5;

	public override GameStateName GetName()
	{
		return GameStateName.None;
	}

	public override void Enter(GameState from)
	{
	}

	public override void Exit(GameState to)
	{
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

	protected void StartRoutine(int routineNumber)
	{
	}

	protected void EndInitRoutine()
	{
	}

	protected void StartInitRoutines()
	{
	}

	private IEnumerator SwitchToTutorial()
	{
		return null;
	}

	private IEnumerator SwitchToThemeTutorial()
	{
		return null;
	}

	private IEnumerator SwitchToMenu()
	{
		return null;
	}

	private IEnumerator SwitchToLeaderboard()
	{
		return null;
	}

	private IEnumerator SwitchToAnnouncment()
	{
		return null;
	}

	private IEnumerator WaitForCustomURL()
	{
		return null;
	}

	public void TutorialAudioClipsLoaded(bool success)
	{
	}
}
