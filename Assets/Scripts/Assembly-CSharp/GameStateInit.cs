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

	protected int m_CurrentInitRoutine = -1;

	protected const int c_INIT_THEME = 1;

	protected const int c_INIT_MISSIONS = 2;

	protected const int c_INIT_TRACK = 3;

	protected const int c_INIT_TUTORIAL = 4;

	protected const int c_INIT_ANALYTICS = 5;

	public override GameStateName GetName()
	{
		return GameStateName.Init;
	}

	public override void Enter(GameState from)
	{
		m_CurrentInitRoutine = -1;
		gameObject.SetActive(true);
		if (FeerSceneManager.Instance.customURLReceived)
		{
			StartCoroutine(WaitForCustomURL());
		}
		else
		{
			StartInitRoutines();
		}
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
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
		switch (infoMessage)
		{
		case InfoMessage.GameManagerInitFinished:
			StartRoutine(c_INIT_MISSIONS);
			break;
		case InfoMessage.MissionManagerInitFinished:
			StartRoutine(c_INIT_TRACK);
			break;
		case InfoMessage.TrackManagerInitFinished:
			if (m_CurrentInitRoutine == c_ROUTINE_LANGUAGE_CHANGE || m_CurrentInitRoutine == c_ROUTINE_CHANGE_THEME || m_CurrentInitRoutine == c_ROUTINE_CHANGE_THEME_TUTORIAL)
			{
				EndInitRoutine();
			}
			else
			{
				StartRoutine(c_INIT_ANALYTICS);
			}
			break;
		case InfoMessage.AnalyticsNewSessionInitFinished:
			EndInitRoutine();
			break;
		}
	}

	protected void StartRoutine(int routineNumber)
	{
		while (routineNumber == c_INIT_TUTORIAL)
		{
			DataManager.Instance.LoadTutorialClips(TutorialAudioClipsLoaded);
			routineNumber = c_INIT_THEME;
		}
		switch (routineNumber)
		{
		case c_INIT_THEME:
			CustomGameManager.Instance.InitThemeData();
			break;
		case c_INIT_MISSIONS:
			MissionManager.Instance.Init();
			break;
		case c_INIT_TRACK:
			CustomGameManager.Instance.trackManager.InitTheTrack();
			break;
		case c_INIT_ANALYTICS:
			CustomAnalyticsTracker.Instance.NewSession();
			break;
		}
	}

	protected void EndInitRoutine()
	{
		switch (m_CurrentInitRoutine)
		{
		case c_ROUTINE_LANGUAGE_CHANGE:
			FeerSceneManager.Instance.ChangeLanguageFinished();
			break;
		case c_ROUTINE_TUTORIAL:
			StartCoroutine(SwitchToTutorial());
			break;
		case c_ROUTINE_ANNOUNCEMENT:
			StartCoroutine(SwitchToAnnouncment());
			break;
		case c_ROUTINE_START_GAME:
			if (FeerSceneManager.Instance.friendInvitationReceived)
			{
				StartCoroutine(SwitchToLeaderboard());
			}
			else
			{
				StartCoroutine(SwitchToMenu());
			}
			break;
		case c_ROUTINE_CHANGE_THEME:
			FeerSceneManager.Instance.ChangeThemeFinished(false);
			break;
		case c_ROUTINE_CHANGE_THEME_TUTORIAL:
			StartCoroutine(SwitchToThemeTutorial());
			break;
		}
	}

	private bool FactoryTutorialPending()
	{
		return DataManager.Instance.selectedTheme == Theme.Factory && !DataManager.Instance.playerThemeData.themeFactoryTutorialPlayed;
	}

	protected void StartInitRoutines()
	{
		if (FeerSceneManager.Instance.changeLanguage)
		{
			m_CurrentInitRoutine = c_ROUTINE_LANGUAGE_CHANGE;
			StartRoutine(c_INIT_THEME);
		}
		else if (!FeerSceneManager.Instance.changeTheme)
		{
			if (!DataManager.Instance.playerData.playTutorial && !FactoryTutorialPending())
			{
				m_CurrentInitRoutine = c_ROUTINE_START_GAME;
				StartRoutine(c_INIT_THEME);
			}
			else
			{
				m_CurrentInitRoutine = c_ROUTINE_TUTORIAL;
				StartRoutine(c_INIT_TUTORIAL);
			}
		}
		else if (FactoryTutorialPending() || (DataManager.Instance.selectedTheme == Theme.Forest && DataManager.Instance.playerData.playTutorial))
		{
			m_CurrentInitRoutine = c_ROUTINE_CHANGE_THEME_TUTORIAL;
			StartRoutine(c_INIT_TUTORIAL);
		}
		else
		{
			m_CurrentInitRoutine = c_ROUTINE_CHANGE_THEME;
			StartRoutine(c_INIT_THEME);
		}
	}

	private IEnumerator SwitchToTutorial()
	{
		while (!m_TutorialAudioClipsLoaded)
		{
			yield return null;
		}
		DataManager.Instance.InitPlayerServerData();
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		FeerSceneManager.Instance.InitAllDataFinished();
		CustomGameManager.Instance.SwitchState(GameStateName.StartTutorial);
	}

	private IEnumerator SwitchToThemeTutorial()
	{
		while (!m_TutorialAudioClipsLoaded)
		{
			yield return null;
		}
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		FeerSceneManager.Instance.ChangeThemeFinished(true);
	}

	private IEnumerator SwitchToMenu()
	{
		DataManager.Instance.InitPlayerServerData();
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		FeerSceneManager.Instance.InitAllDataFinished();
		CustomGameManager.Instance.SwitchState(GameStateName.Menu);
	}

	private IEnumerator SwitchToLeaderboard()
	{
		DataManager.Instance.InitPlayerServerData();
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		FeerSceneManager.Instance.InitAllDataFinished();
		CustomGameManager.Instance.SwitchState(GameStateName.MenuLeaderboard);
	}

	private IEnumerator SwitchToAnnouncment()
	{
		DataManager.Instance.InitPlayerServerData();
		while (UAP_AccessibilityManager.IsSpeaking())
		{
			yield return null;
		}
		FeerSceneManager.Instance.InitAllDataFinished();
		CustomGameManager.Instance.SwitchState(GameStateName.InitAnnouncement);
		CustomGameManager.Instance.RemoveState(GameStateName.StartTutorial);
	}

	private IEnumerator WaitForCustomURL()
	{
		while (FeerSceneManager.Instance.customURLReceived)
		{
			if (FeerSceneManager.Instance.friendInvitationReceived)
			{
				FeerSceneManager.Instance.customURLReceived = false;
				break;
			}
			yield return null;
		}
		StartInitRoutines();
	}

	public void TutorialAudioClipsLoaded(bool success)
	{
		if (success)
		{
			m_TutorialAudioClipsLoaded = true;
		}
	}
}
