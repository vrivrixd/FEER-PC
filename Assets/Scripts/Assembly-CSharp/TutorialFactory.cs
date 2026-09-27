using System.Collections;
using UnityEngine;

public class TutorialFactory : ThemeTutorial
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
			// steps where the player can die: 8,10,11,16,17,21,22
			if (step < 0 || step > 22 || ((1 << step) & ((step < 12) ? 0xD00 : 0x630000)) == 0)
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

	private void ResumeRunning(IEnumerator listener)
	{
		StartCoroutine(listener);
		CGM.mainCamera.Pause(false, false);
		CGM.playerController.TutorialResumeRunning(true, true);
		CGM.trackManager.TutorialResumeRunning();
	}

	// lane (0 left, 1 center, 2 right) -> relative spawn position (-1, 0, 1)
	private static int LaneToSpawn(int lane)
	{
		return lane - 1;
	}

	public override void NextTutorialStep()
	{
		m_CurrentTutorialStep++;
		switch (m_CurrentTutorialStep)
		{
		case 0:
			CGM.StartTutorial();
			m_ParentGameState.ShowTutorialSkipPanel();
			CGM.playerController.m_TutotalState = 1;
			Say(0, 2.5f);
			break;
		case 1:
			Say(1, 1.5f);
			break;
		case 2:
		{
			// fairy in the opposite/adjacent lane: right->center, center->left, left->right
			int lane = CGM.currentLane;
			int spawn;
			if (lane == 2)
			{
				spawn = 0;
			}
			else if (lane == 1)
			{
				spawn = -1;
			}
			else
			{
				if (lane != 0)
				{
					break;
				}
				spawn = 1;
			}
			CGM.trackManager.TutorialSpawnFairy(spawn, true);
			break;
		}
		case 3:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, false);
			Say(2, 0f, 0f, true);
			break;
		case 4:
			ResumeRunning(ListenForGhostCollected());
			break;
		case 5:
			Say(3, 0.5f);
			break;
		case 6:
		{
			int lane2 = CGM.currentLane;
			if (lane2 < 0 || lane2 > 2)
			{
				break;
			}
			CGM.trackManager.TutorialSpawnZombie(LaneToSpawn(lane2), true);
			break;
		}
		case 7:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, false);
			Say(4, 0f, 0f, true);
			break;
		case 8:
			ResumeRunning(ListenForZombiesAvoided());
			break;
		case 9:
			Say(5, 0.5f);
			break;
		case 10:
		case 11:
		{
			StartCoroutine(ListenForZombiesAvoided());
			int lane3 = CGM.currentLane;
			if (lane3 < 0 || lane3 > 2)
			{
				break;
			}
			CGM.trackManager.PlaceElement(17f, LaneToSpawn(lane3), 2);
			break;
		}
		case 12:
			Say(12, 0.5f);
			break;
		case 13:
			Say(6);
			break;
		case 14:
			CGM.trackManager.TutorialSpawnHands(true);
			break;
		case 15:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, true);
			Say(7, 0f, 0f, true);
			break;
		case 16:
			ResumeRunning(ListenForHandsAvoided());
			break;
		case 17:
			StartCoroutine(ListenForHandsAvoided());
			CGM.trackManager.PlaceElement(23f, 0, 3);
			break;
		case 18:
			Say(8, 0.5f);
			break;
		case 19:
			CGM.trackManager.TutorialSpawnRavens(true);
			break;
		case 20:
			CGM.playerController.TutorialStopRunning(true, false);
			CGM.mainCamera.Pause(true, false);
			Say(9, 0f, 0f, true);
			break;
		case 21:
			ResumeRunning(ListenForRavensAvoided());
			break;
		case 22:
			StartCoroutine(ListenForRavensAvoided());
			CGM.trackManager.PlaceElement(23f, 0, 4);
			break;
		case 23:
			Say(10, 0.5f);
			break;
		case 24:
			CGM.playerController.TutorialStopRunning(false, false);
			CGM.mainCamera.Pause(true, true);
			CGM.trackManager.StopRunning(false, false);
			Say(11, 0.5f, 0.5f);
			break;
		case 25:
			// PORT: CustomAnalyticsTracker.TutorialComplete removed.
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
		if (step == 4)
		{
			// fairy missed: new fairy in another lane
			StopAllCoroutines();
			StartCoroutine(ListenForGhostCollected());
			int r = Random.Range(0, 2);
			int lane = CGM.currentLane;
			int spawn;
			if (lane == 2)
			{
				spawn = (r == 0) ? 0 : (-1);
			}
			else if (lane == 1)
			{
				spawn = (r == 0) ? (-1) : 1;
			}
			else
			{
				if (lane != 0)
				{
					return;
				}
				spawn = (r == 0) ? 0 : 1;
			}
			CGM.trackManager.PlaceElement(17f, spawn, 1);
			return;
		}
		int obstacle;
		if (step == 8 || step == 10 || step == 11)
		{
			obstacle = 2;
		}
		else if (step == 16 || step == 17)
		{
			obstacle = 3;
		}
		else if (step == 21 || step == 22)
		{
			obstacle = 4;
		}
		else
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
			StartCoroutine(SayText(0f, null, 0f, m_TutorialClips[13], true));
			break;
		case 2:
			ReviveAfterDeath();
			break;
		case 3:
			if (obstacle == 2)
			{
				StartCoroutine(ListenForZombiesAvoided());
				int lane2 = CGM.currentLane;
				if (lane2 < 0 || lane2 > 2)
				{
					return;
				}
				CGM.trackManager.PlaceElement(17f, LaneToSpawn(lane2), 2);
			}
			else if (obstacle == 3)
			{
				StartCoroutine(ListenForHandsAvoided());
				CGM.trackManager.PlaceElement(23f, 0, 3);
			}
			else
			{
				StartCoroutine(ListenForRavensAvoided());
				CGM.trackManager.PlaceElement(23f, 0, 4);
			}
			break;
		}
	}

	public override void TutorialSkipped()
	{
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
