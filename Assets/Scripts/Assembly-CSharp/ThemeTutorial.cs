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
		return null;
	}

	public IEnumerator ListenForGhostCollected()
	{
		return null;
	}

	public IEnumerator ListenForZombiesAvoided()
	{
		return null;
	}

	public IEnumerator ListenForHandsAvoided()
	{
		return null;
	}

	public IEnumerator ListenForRavensAvoided()
	{
		return null;
	}

	public IEnumerator WaitSomeTime(float time, bool repeatStep = false)
	{
		return null;
	}

	public void TutorialPaused()
	{
	}

	public void TutorialEnded()
	{
	}
}
