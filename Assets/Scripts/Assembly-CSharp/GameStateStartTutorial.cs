using System.Collections;
using UnityEngine;

public class GameStateStartTutorial : GameState
{
	public GameObject startTutorialBtn;

	public AudioSource speechAudio;

	public override void Enter(GameState from)
	{
		gameObject.SetActive(true);
		if (DataManager.Instance.selectedTheme == Theme.Forest)
		{
			if (!UAP_AccessibilityManager.IsEnabled())
			{
				startTutorialBtn.SetActive(true);
			}
			StartCoroutine(SpeakText());
		}
		else
		{
			startTutorialBtn.SetActive(true);
		}
	}

	public override void Exit(GameState to)
	{
		gameObject.SetActive(false);
		CustomGameManager.Instance.RemoveState(GameStateName.StartTutorial);
	}

	public override GameStateName GetName()
	{
		return GameStateName.StartTutorial;
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
		if (speechAudio.isPlaying)
		{
			speechAudio.Stop();
		}
		speechAudio.clip = null;
		speechAudio.gameObject.SetActive(false);
		if (DataManager.Instance.selectedTheme == Theme.Forest)
		{
			bool voiceOver = UAP_AccessibilityManager.IsEnabled();
			CustomAnalyticsTracker.Instance.TutorialStart("FirstStartTutorial", voiceOver);
		}
		CustomGameManager.Instance.SwitchState(GameStateName.PlayGame);
	}

	private IEnumerator SpeakText()
	{
		yield return new WaitForSeconds(0.5f);
		speechAudio.gameObject.SetActive(true);
		AudioClip clip = DataManager.Instance.GetTutorialHeadphoneClip();
		speechAudio.clip = clip;
		speechAudio.Play();
		yield return new WaitForSeconds(clip.length);
		while (speechAudio.isPlaying)
		{
			yield return null;
		}
		if (UAP_AccessibilityManager.IsEnabled())
		{
			yield return new WaitForSeconds(0.2f);
			startTutorialBtn.SetActive(true);
		}
	}
}
