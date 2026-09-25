using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameStateInitAnnouncement : GameState
{
	public GameObject updatePopUp;

	public GameObject cancelButton;

	public GameObject updateButton;

	public GameObject versionUpdateMessage;

	public GameObject giftBoxPopUp;

	public Text giftBoxLights;

	public GameObject giftBoxBtn;

	public AudioSource audioProgress;

	public GameObject giftBoxTitle;

	public override void Enter(GameState from)
	{
		if (!DataManager.Instance.updateAvailable)
		{
			if (!DataManager.Instance.isVersionUpdate)
			{
				gameObject.SetActive(true);
				StartCoroutine(GoToNextState());
				return;
			}
			versionUpdateMessage.SetActive(true);
		}
		else
		{
			cancelButton.SetActive(!DataManager.Instance.forceToUpdate);
			updatePopUp.SetActive(true);
			DataManager.Instance.UpdateToNewVersionInfoMessagePresented();
		}
		gameObject.SetActive(true);
	}

	public override void Exit(GameState to)
	{
		gameObject.SetActive(false);
		CustomGameManager.Instance.RemoveState(GameStateName.InitAnnouncement);
	}

	public override GameStateName GetName()
	{
		return GameStateName.InitAnnouncement;
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

	public void UpdateAppBtnClicked()
	{
		// PORT: sem loja/atualizacao online no PC.
	}

	private void GoToMenuOrLeaderboard()
	{
		CustomGameManager.Instance.SwitchState(FeerSceneManager.Instance.friendInvitationReceived ? GameStateName.MenuLeaderboard : GameStateName.Menu);
	}

	public void UpdateAppLaterBtnClicked()
	{
		GoToMenuOrLeaderboard();
	}

	private IEnumerator GoToNextState()
	{
		yield return new WaitForEndOfFrame();
		GoToMenuOrLeaderboard();
	}

	public void OpenGiftButtonClicked(int amount)
	{
		if (!DataManager.Instance.playerData.updateVersionGiftRedeemed)
		{
			DataManager.Instance.SaveCoins(amount, (TransactionContext)2, (TransactionalItem)6, (TransactionItemType)4, null);
			DataManager.Instance.UpdateVersionGiftReceived();
		}
		versionUpdateMessage.SetActive(false);
		giftBoxPopUp.SetActive(true);
		StartCoroutine(StartLightAnimation(amount));
	}

	public void CollectGiftBtnClicked()
	{
		GoToMenuOrLeaderboard();
	}

	public void ClosePopUpClicked()
	{
		GoToMenuOrLeaderboard();
	}

	private IEnumerator StartLightAnimation(int amount)
	{
		string appendLights = " " + LocalizationManager.Instance.GetLocalizedValue("LIGHTS");
		if (UAP_AccessibilityManager.IsEnabled())
		{
			giftBoxLights.text = NumberFormatter.FormatToLocale(amount) + appendLights;
			giftBoxBtn.SetActive(true);
			yield return new WaitForSeconds(0.5f);
			UAP_AccessibilityManager.SelectElement(giftBoxTitle, true);
			yield break;
		}
		giftBoxLights.text = "0" + appendLights;
		yield return new WaitForSeconds(0.5f);
		int startingValue = 0;
		float elapsedTime = 0f;
		float duration = 1f;
		audioProgress.gameObject.SetActive(true);
		float pitchStartValue = 0.8f;
		audioProgress.pitch = 0.8f;
		audioProgress.Play();
		float finalPitch = 1.2f;
		while (elapsedTime < duration)
		{
			float value = Mathf.Lerp(startingValue, amount, elapsedTime / duration);
			giftBoxLights.text = NumberFormatter.FormatToLocale((int)value) + appendLights;
			audioProgress.pitch = Mathf.Lerp(pitchStartValue, finalPitch, elapsedTime / duration);
			elapsedTime += Time.deltaTime;
			yield return null;
		}
		audioProgress.Stop();
		audioProgress.gameObject.SetActive(false);
		giftBoxLights.text = NumberFormatter.FormatToLocale(amount) + appendLights;
		giftBoxBtn.SetActive(true);
	}
}
