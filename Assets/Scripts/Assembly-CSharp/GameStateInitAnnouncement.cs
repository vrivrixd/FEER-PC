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

	public void UpdateAppBtnClicked()
	{
	}

	public void UpdateAppLaterBtnClicked()
	{
	}

	private IEnumerator GoToNextState()
	{
		return null;
	}

	public void OpenGiftButtonClicked(int amount)
	{
	}

	public void CollectGiftBtnClicked()
	{
	}

	public void ClosePopUpClicked()
	{
	}

	private IEnumerator StartLightAnimation(int amount)
	{
		return null;
	}
}
