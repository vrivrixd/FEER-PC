using System.Collections;
using UnityEngine;

public abstract class ThemeTutorial : MonoBehaviour
{
	protected GameStatePlayGame m_ParentGameState;

	protected bool m_TutorialResumed;

	protected bool m_TutorialKilled;

	protected int m_CurrentTutorialStep;

	protected bool m_TutorialPaused;

	protected int m_CurrentRepeatTime;

	protected int m_CurrentRepeatStep;

	protected int m_AnalyticsStepIndex;

	protected int m_AnalyticsStepRepeatedCount;

	protected Coroutine m_TutorialSayCoroutine;

	protected int m_NextLaneToSpawn;

	protected AudioSource speechAudio;

	protected AudioClip[] m_TutorialClips;

	protected string m_TutorialType;

	public abstract void StartTutorial(GameStatePlayGame parentGameState, AudioSource audioSourceSpeech, AudioClip[] tutorialClips, string tutorialType);

	public abstract void ResumeTutorial();

	public abstract void TutorialSkipped();

	public abstract void PlayerDied();

	public abstract void TutorialStoppedRunning();

	public abstract void IsDeadAnimationFinished();

	public abstract void IsDeadAnimationFinishedWhilePaused();

	public abstract void NextTutorialStep();

	public abstract void RepeatTutorialStep();

	public IEnumerator SayText(float timeToWaitBefore, string textToSay, float timeToWaitAfter, AudioClip audioClip = null, bool switchToRepeat = false, bool muteAudio = false)
	{
		if (muteAudio)
		{
			CustomGameManager.Instance.PlaySpeechMuted(true, 0.25f);
		}
		yield return new WaitForSeconds(timeToWaitBefore);
		if (audioClip == null)
		{
			bool startedSpeaking = false;
			float timeOut = 0f;
			UAP_AccessibilityManager.Say(LocalizationManager.Instance.GetLocalizedValue(textToSay), true, true, (UAP_AudioQueue.EInterrupt)0x4f);
			while (!startedSpeaking)
			{
				if (UAP_AccessibilityManager.IsSpeaking())
				{
					startedSpeaking = true;
				}
				timeOut += Time.deltaTime;
				if (1f < timeOut)
				{
					startedSpeaking = true;
				}
				yield return null;
			}
			while (UAP_AccessibilityManager.IsSpeaking())
			{
				yield return null;
			}
		}
		else
		{
			speechAudio.clip = audioClip;
			while (m_TutorialPaused)
			{
				yield return null;
			}
			speechAudio.Play();
			while (speechAudio.isPlaying || m_TutorialPaused)
			{
				yield return null;
			}
		}
		if (muteAudio)
		{
			CustomGameManager.Instance.PlaySpeechMuted(false, 0.25f);
		}
		yield return new WaitForSeconds(timeToWaitAfter);
		if (switchToRepeat)
		{
			RepeatTutorialStep();
		}
		else
		{
			NextTutorialStep();
		}
	}

	// Espera 1 s e, se o tutorial foi pausado nesse meio tempo, espera a retomada e mais 1 s
	private IEnumerator WaitAfterEvent()
	{
		yield return new WaitForSeconds(1f);
		bool wasPaused = m_TutorialPaused;
		while (m_TutorialPaused)
		{
			yield return null;
		}
		if (wasPaused)
		{
			yield return new WaitForSeconds(1f);
		}
	}

	public IEnumerator ListenForGhostCollected()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		int ghostsSpawned = cgm.ghostsSpawned;
		int ghostsCollected = cgm.ghostsReallyCollected;
		float startDistance = cgm.worldDistance;
		while (cgm.ghostsSpawned == ghostsSpawned && cgm.ghostsReallyCollected == ghostsCollected && cgm.worldDistance - startDistance <= 20f)
		{
			yield return null;
		}
		yield return WaitAfterEvent();
		if (ghostsCollected < cgm.ghostsReallyCollected)
		{
			NextTutorialStep();
			yield break;
		}
		m_AnalyticsStepRepeatedCount++;
		RepeatTutorialStep();
	}

	public IEnumerator ListenForZombiesAvoided()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		int zombiesDodged = cgm.zombiesDodged;
		float startDistance = cgm.worldDistance;
		bool timeout = false;
		while (cgm.zombiesDodged == zombiesDodged)
		{
			if (cgm.worldDistance - startDistance > 20f)
			{
				timeout = true;
				break;
			}
			yield return null;
		}
		yield return WaitAfterEvent();
		if (zombiesDodged < cgm.zombiesDodged || timeout)
		{
			NextTutorialStep();
		}
	}

	public IEnumerator ListenForHandsAvoided()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		int jumpedOver = cgm.jumpedOver;
		float startDistance = cgm.worldDistance;
		bool timeout = false;
		while (cgm.jumpedOver == jumpedOver)
		{
			if (cgm.worldDistance - startDistance > 23f)
			{
				timeout = true;
				break;
			}
			yield return null;
		}
		yield return WaitAfterEvent();
		if (jumpedOver < cgm.jumpedOver || timeout)
		{
			NextTutorialStep();
		}
	}

	public IEnumerator ListenForRavensAvoided()
	{
		CustomGameManager cgm = CustomGameManager.Instance;
		int slidedUnder = cgm.slidedUnder;
		float startDistance = cgm.worldDistance;
		bool timeout = false;
		while (cgm.slidedUnder == slidedUnder)
		{
			if (cgm.worldDistance - startDistance > 23f)
			{
				timeout = true;
				break;
			}
			yield return null;
		}
		yield return WaitAfterEvent();
		if (slidedUnder < cgm.slidedUnder || timeout)
		{
			NextTutorialStep();
		}
	}

	public IEnumerator WaitSomeTime(float time, bool repeatStep = false)
	{
		yield return new WaitForSeconds(time);
		bool wasPaused = m_TutorialPaused;
		while (m_TutorialPaused)
		{
			yield return null;
		}
		if (wasPaused)
		{
			yield return new WaitForSeconds(time);
		}
		if (repeatStep)
		{
			RepeatTutorialStep();
		}
		else
		{
			NextTutorialStep();
		}
	}

	public void TutorialPaused()
	{
		m_TutorialPaused = true;
		speechAudio.Pause();
		UAP_AccessibilityManager.PauseAccessibility(false, false);
		if (UAP_AccessibilityManager.IsSpeaking())
		{
			UAP_AccessibilityManager.StopSpeaking();
		}
		CustomGameManager cgm = CustomGameManager.Instance;
		cgm.playerController.m_IsPlaying = false;
		cgm.playerController.m_IsSwiping = false;
		cgm.playerController.PauseAllAudio(true);
		cgm.trackManager.TutorialPaused(true);
		cgm.mainCamera.Pause(true, false);
		cgm.EnableMenuMusic(true);
		cgm.EnableGameMusic(false);
		if (m_TutorialSayCoroutine != null)
		{
			StopCoroutine(m_TutorialSayCoroutine);
		}
		cgm.PlaySpeechMuted(false, 0f);
	}

	public void TutorialEnded()
	{
		StopAllCoroutines();
		m_TutorialResumed = false;
		m_TutorialKilled = false;
		m_TutorialPaused = false;
		m_NextLaneToSpawn = 0;
		m_ParentGameState = null;
		m_TutorialClips = null;
		m_TutorialType = null;
		m_CurrentTutorialStep = -1;
		m_AnalyticsStepIndex = 1;
		m_AnalyticsStepRepeatedCount = 0;
		m_CurrentRepeatTime = 0;
		m_CurrentRepeatStep = -1;
		speechAudio = null;
	}
}
