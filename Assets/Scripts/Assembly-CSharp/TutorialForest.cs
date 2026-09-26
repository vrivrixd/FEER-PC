using System.Collections;
using UnityEngine;

public class TutorialForest : ThemeTutorial
{

	private static CustomGameManager CGM => CustomGameManager.Instance;

	public override void StartTutorial(GameStatePlayGame parentGameState, AudioSource audioSourceSpeech, AudioClip[] tutorialClips, string tutorialType)
	{
		m_TutorialResumed = false;
		m_TutorialKilled = false;
		m_TutorialPaused = false;
		m_NextLaneToSpawn = 0;
		m_ParentGameState = parentGameState;
		speechAudio = audioSourceSpeech;
		m_TutorialClips = tutorialClips;
		m_TutorialType = tutorialType;
		m_CurrentTutorialStep = -1;
		m_AnalyticsStepIndex = 1;
		m_AnalyticsStepRepeatedCount = 0;
		m_CurrentRepeatTime = 0;
		m_CurrentRepeatStep = -1;
		if (!UAP_AccessibilityManager.IsEnabled())
		{
			NextTutorialStep();
			return;
		}
		string textToSay = LocalizationManager.Instance.GetLocalizedValue("You can skip the tutorial by tapping in the upper left corner or by double-tapping with two fingers on the screen");
		m_TutorialSayCoroutine = StartCoroutine(SayText(0.5f, textToSay, 0f));
	}

	public override void ResumeTutorial()
	{
		m_TutorialPaused = false;
		speechAudio.UnPause();
		UAP_AccessibilityManager.PauseAccessibility(true, false);
		CGM.playerController.TutorialPaused(false);
		CGM.trackManager.TutorialPaused(false);
		CGM.mainCamera.Pause(false, false);
		int step = m_CurrentTutorialStep;
		m_TutorialResumed = true;
		if (step == -1 || step != m_CurrentRepeatStep)
		{
			if (!m_TutorialKilled)
			{
				if (step == -1)
				{
					m_ParentGameState.StartTutorial();
				}
				return;
			}
		}
		else
		{
			// passos com morte possivel: 16,18,19,23,25,26,30,32,33
			bool deadlyStep = (step < 31) ? ((1 << step) & 0x468D0000) != 0 : ((step & -2) == 32);
			if (!deadlyStep)
			{
				return;
			}
			if (m_CurrentRepeatTime != 1)
			{
				if (m_CurrentRepeatTime < 5)
				{
					return;
				}
				m_CurrentRepeatTime = 0;
				RepeatTutorialStep();
				return;
			}
			if (!m_TutorialKilled)
			{
				return;
			}
		}
		m_TutorialKilled = false;
		RepeatTutorialStep();
	}

	private void Say(int clip, float timeToWaitBefore = 0f, float timeToWaitAfter = 0f, bool muteAudio = false)
	{
		StartCoroutine(SayText(timeToWaitBefore, null, timeToWaitAfter, m_TutorialClips[clip], false, muteAudio));
	}

	private void SayRepeat(int clip, float timeToWaitBefore = 0f)
	{
		StartCoroutine(SayText(timeToWaitBefore, null, 0f, m_TutorialClips[clip], true));
	}

	private void ResumeRunning(IEnumerator listener)
	{
		StartCoroutine(listener);
		CGM.mainCamera.Pause(false, false);
		CGM.playerController.TutorialResumeRunning(true, true);
		CGM.trackManager.TutorialResumeRunning();
	}

	// pista (0 esquerda, 1 centro, 2 direita) -> posicao de spawn relativa (-1, 0, 1)
	private static int LaneToSpawn(int lane)
	{
		return lane - 1;
	}

	private void SpawnFairyRandom()
	{
		int lane = CGM.currentLane;
		int spawn;
		if (lane == 2)
		{
			spawn = (Random.Range(0, 2) == 0) ? 0 : (-1);
		}
		else if (lane == 1)
		{
			spawn = (Random.Range(0, 2) != 0) ? (-1) : 1;
		}
		else
		{
			if (lane != 0)
			{
				return;
			}
			spawn = (Random.Range(0, 2) == 0) ? 0 : 1;
		}
		CGM.trackManager.TutorialSpawnFairy(spawn, false);
	}

	public override void NextTutorialStep()
	{
		m_CurrentTutorialStep++;
		switch (m_CurrentTutorialStep)
		{
		case 0:
			CGM.StartTutorial();
			m_ParentGameState.ShowTutorialSkipPanel();
			Say(0, 2.5f);
			break;
		case 1:
			CGM.trackManager.TutorialSpawnFairy(0, true);
			break;
		case 2:
			CGM.playerController.TutorialStopRunning(false, false);
			CGM.mainCamera.Pause(true, false);
			Say(1);
			break;
		case 3:
			StartCoroutine(ListenForGhostCollected());
			CGM.mainCamera.Pause(false, false);
			CGM.playerController.TutorialResumeRunning(false, false);
			CGM.trackManager.TutorialResumeRunning();
			break;
		case 4:
			Say(2);
			break;
		case 5:
			CGM.playerController.TutorialAllowInput(true, false);
			Say(3, 0.5f);
			break;
		case 6:
			CGM.trackManager.TutorialSpawnFairy(-1, true);
			break;
		case 7:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, false);
			Say(4, 0f, 0f, true);
			break;
		case 8:
			ResumeRunning(ListenForGhostCollected());
			break;
		case 9:
			m_AnalyticsStepIndex = 1;
			m_AnalyticsStepRepeatedCount = 0;
			Say(2);
			break;
		case 10:
			Say(5, 0.5f);
			break;
		case 11:
			StartCoroutine(ListenForGhostCollected());
			CGM.trackManager.TutorialSpawnFairy(1, false);
			break;
		case 12:
			m_AnalyticsStepIndex = 2;
			m_AnalyticsStepRepeatedCount = 0;
			StartCoroutine(ListenForGhostCollected());
			SpawnFairyRandom();
			break;
		case 13:
			m_AnalyticsStepIndex = 3;
			m_AnalyticsStepRepeatedCount = 0;
			Say(6);
			break;
		case 14:
		{
			int lane = CGM.currentLane;
			if (lane < 0 || lane > 2)
			{
				break;
			}
			CGM.trackManager.TutorialSpawnZombie(LaneToSpawn(lane), true);
			break;
		}
		case 15:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, true);
			Say(7, 0f, 0f, true);
			break;
		case 16:
			ResumeRunning(ListenForZombiesAvoided());
			break;
		case 17:
			m_AnalyticsStepIndex = 4;
			m_AnalyticsStepRepeatedCount = 0;
			Say(8);
			break;
		case 18:
		case 19:
		{
			if (m_CurrentTutorialStep == 19)
			{
				m_AnalyticsStepIndex = 5;
				m_AnalyticsStepRepeatedCount = 0;
			}
			StartCoroutine(ListenForZombiesAvoided());
			int lane2 = CGM.currentLane;
			if (lane2 < 0 || lane2 > 2)
			{
				break;
			}
			CGM.trackManager.TutorialSpawnZombie(LaneToSpawn(lane2), false);
			break;
		}
		case 20:
			m_AnalyticsStepIndex = 6;
			m_AnalyticsStepRepeatedCount = 0;
			Say(9);
			break;
		case 21:
			CGM.trackManager.TutorialSpawnHands(true);
			break;
		case 22:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, true);
			Say(10, 0.2f, 0f, true);
			break;
		case 23:
			ResumeRunning(ListenForHandsAvoided());
			break;
		case 24:
			m_AnalyticsStepIndex = 7;
			Say((m_AnalyticsStepRepeatedCount < 1) ? 12 : 11);
			m_AnalyticsStepRepeatedCount = 0;
			break;
		case 25:
		case 26:
			if (m_CurrentTutorialStep == 26)
			{
				m_AnalyticsStepIndex = 8;
				m_AnalyticsStepRepeatedCount = 0;
			}
			StartCoroutine(ListenForHandsAvoided());
			CGM.trackManager.TutorialSpawnHands(false);
			break;
		case 27:
			m_AnalyticsStepIndex = 9;
			m_AnalyticsStepRepeatedCount = 0;
			Say(13);
			break;
		case 28:
			CGM.trackManager.TutorialSpawnRavens(true);
			break;
		case 29:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, true);
			Say(14, 0f, 0f, true);
			break;
		case 30:
			ResumeRunning(ListenForRavensAvoided());
			break;
		case 31:
			m_AnalyticsStepIndex = 10;
			m_AnalyticsStepRepeatedCount = 0;
			Say(15);
			break;
		case 32:
		case 33:
			if (m_CurrentTutorialStep == 33)
			{
				m_AnalyticsStepIndex = 11;
				m_AnalyticsStepRepeatedCount = 0;
			}
			StartCoroutine(ListenForRavensAvoided());
			CGM.trackManager.TutorialSpawnRavens(false);
			break;
		case 34:
			m_AnalyticsStepIndex = 12;
			m_AnalyticsStepRepeatedCount = 0;
			Say(16);
			break;
		case 35:
			CGM.playerController.TutorialStopRunning(false, false);
			CGM.mainCamera.Pause(true, true);
			CGM.trackManager.StopRunning(false, false);
			Say(17, 1f, 0.5f);
			break;
		case 36:
			// PORT: CustomAnalyticsTracker.TutorialComplete removido.
			m_ParentGameState.TutorialEnd();
			break;
		}
	}

	private void StopAfterDeath()
	{
		StopAllCoroutines();
		CGM.playerCollider.StopPlaying();
		CGM.trackManager.StopRunning(false, true);
		CGM.playerController.GameStopped();
	}

	private void ReviveAfterDeath()
	{
		CGM.currentLane = 1;
		CGM.mainCamera.Reset(true);
		CGM.bloodOnLens.MoveBloodOnLens();
		CGM.playerCollider.ResumePlaying();
		CGM.trackManager.TutorialRevive();
		CGM.playerController.GameRevived();
		StartCoroutine(WaitSomeTime(1.5f, true));
	}

	private void SpawnFairyAtNextLane()
	{
		StartCoroutine(ListenForGhostCollected());
		int spawn;
		if (m_NextLaneToSpawn == 0)
		{
			spawn = -1;
		}
		else if (m_NextLaneToSpawn == 1)
		{
			spawn = 0;
		}
		else
		{
			spawn = 1;
		}
		CGM.trackManager.TutorialSpawnFairy(spawn, false);
	}

	// Pede para ir a pista indicada (esquerda/centro/direita) e marca a proxima pista do spawn
	private void SayMoveToLane(bool fromRepeatTime2)
	{
		int lane = CGM.currentLane;
		if (lane == 0)
		{
			SayRepeat(21);
			m_NextLaneToSpawn = 1;
		}
		else if (lane == 2)
		{
			SayRepeat(22);
			m_NextLaneToSpawn = 1;
		}
		else
		{
			SayRepeat(23);
			m_NextLaneToSpawn = fromRepeatTime2 ? 0 : 1;
		}
	}

	public override void RepeatTutorialStep()
	{
		int step = m_CurrentTutorialStep;
		int previous;
		if (step == m_CurrentRepeatStep)
		{
			previous = m_CurrentRepeatTime;
		}
		else
		{
			previous = 0;
			m_CurrentRepeatTime = 0;
			m_CurrentRepeatStep = step;
		}
		int times = previous + 1;
		m_CurrentRepeatTime = times;
		if (step > 26)
		{
			if (step != 30 && step != 32 && step != 33)
			{
				return;
			}
			if (times >= 5)
			{
				m_CurrentRepeatTime = 1;
				StopAfterDeath();
				return;
			}
			switch (previous)
			{
			case 0:
				StopAfterDeath();
				break;
			case 1:
				SayRepeat(27);
				break;
			case 2:
				ReviveAfterDeath();
				break;
			case 3:
				StartCoroutine(ListenForRavensAvoided());
				CGM.trackManager.TutorialSpawnRavens(false);
				break;
			}
			return;
		}
		switch (step)
		{
		case 8:
			switch (previous)
			{
			case 1:
				if (CGM.currentLane < 1)
				{
					RepeatTutorialStep();
				}
				else
				{
					SayRepeat(19, 0.5f);
				}
				break;
			case 2:
				CGM.trackManager.TutorialSpawnFairy(-1, false);
				StartCoroutine(ListenForGhostCollected());
				break;
			case 3:
				m_CurrentRepeatTime = 1;
				SayRepeat(18);
				break;
			case 0:
				SayRepeat(18);
				break;
			}
			break;
		case 11:
			if (times == 4 && CGM.currentLane == 0)
			{
				m_CurrentRepeatTime = 1;
				SayRepeat(18);
				break;
			}
			if (times >= 6)
			{
				m_CurrentRepeatTime = 4;
				times = 4;
			}
			switch (times)
			{
			case 1:
				SayRepeat(18);
				break;
			case 2:
				if (CGM.currentLane != 0)
				{
					RepeatTutorialStep();
				}
				else
				{
					SayRepeat(20, 0.5f);
				}
				break;
			case 3:
				StartCoroutine(ListenForGhostCollected());
				CGM.trackManager.TutorialSpawnFairy(0, false);
				break;
			case 4:
				SayMoveToLane(false);
				break;
			case 5:
				SpawnFairyAtNextLane();
				break;
			}
			break;
		case 12:
			if (times >= 5)
			{
				m_CurrentRepeatTime = 1;
				previous = 0;
			}
			switch (previous)
			{
			case 0:
			{
				int r = Random.Range(0, 2);
				if (r == 1)
				{
					SayRepeat(24);
				}
				else if (r == 0)
				{
					SayRepeat(18);
				}
				break;
			}
			case 1:
				StartCoroutine(ListenForGhostCollected());
				SpawnFairyRandom();
				break;
			case 2:
				SayMoveToLane(true);
				break;
			case 3:
				SpawnFairyAtNextLane();
				break;
			}
			break;
		case 16:
		case 18:
		case 19:
			if (times >= 5)
			{
				m_CurrentRepeatTime = 1;
				StopAfterDeath();
				break;
			}
			switch (previous)
			{
			case 0:
				StopAfterDeath();
				break;
			case 1:
				SayRepeat(25);
				break;
			case 2:
				ReviveAfterDeath();
				break;
			case 3:
			{
				StartCoroutine(ListenForZombiesAvoided());
				int lane = CGM.currentLane;
				if (lane >= 0 && lane <= 2)
				{
					CGM.trackManager.TutorialSpawnZombie(LaneToSpawn(lane), false);
				}
				break;
			}
			}
			break;
		case 23:
		case 25:
		case 26:
			if (times > 4)
			{
				m_CurrentRepeatTime = 1;
				StopAfterDeath();
				break;
			}
			switch (previous)
			{
			case 0:
				StopAfterDeath();
				break;
			case 1:
				SayRepeat(26);
				break;
			case 2:
				ReviveAfterDeath();
				break;
			case 3:
				StartCoroutine(ListenForHandsAvoided());
				CGM.trackManager.TutorialSpawnHands(false);
				break;
			}
			break;
		}
	}

	public override void TutorialSkipped()
	{
		// PORT: so enviava o evento de analytics TutorialSkip.
	}

	public override void PlayerDied()
	{
		m_AnalyticsStepRepeatedCount++;
		RepeatTutorialStep();
	}

	public override void TutorialStoppedRunning()
	{
		NextTutorialStep();
	}

	public override void IsDeadAnimationFinished()
	{
		RepeatTutorialStep();
	}

	public override void IsDeadAnimationFinishedWhilePaused()
	{
		m_TutorialKilled = true;
	}
}
