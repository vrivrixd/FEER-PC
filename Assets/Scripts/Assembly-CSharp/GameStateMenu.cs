using System.Collections;
using UnityEngine;

public class GameStateMenu : GameState
{
	public MissionPanelUI missionPanel;

	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject navPanel;

	public GameObject quitBtn;

	public RectTransform panelTransform;

	protected string m_strMainMenu;

	public override void Enter(GameState from)
	{
		gameObject.SetActive(true);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			if (m_strMainMenu == null)
			{
				m_strMainMenu = LocalizationManager.Instance.GetLocalizedValue("Main Menu");
			}
			UAP_AccessibilityManager.Say(m_strMainMenu);
		}
		navPanel.SetActive(true);
		quitBtn.SetActive(true);
		scoreCoinsPanel.Show(panelTransform, true, true, 1);
		if (!DataManager.Instance.playerData.isFirstGame)
		{
			missionPanel.Show(GetName());
		}
	}

	public override GameStateName GetName()
	{
		return GameStateName.Menu;
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
		case InfoMessage.MissionSetLevelUpFinished:
			scoreCoinsPanel.UpdateCoins(-1f);
			scoreCoinsPanel.SlideIn();
			navPanel.SetActive(true);
			quitBtn.SetActive(true);
			CustomGameManager.Instance.AllowSleepMode(true);
			break;
		case InfoMessage.MissionSetLevelUp:
			CustomGameManager.Instance.AllowSleepMode(false);
			navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
			quitBtn.GetComponent<Animation>().Play("SetInactiveAnim");
			scoreCoinsPanel.SlideOut();
			Invoke("HideInteractivePanels", 0.5f);
			break;
		case InfoMessage.PurchaseMade:
			scoreCoinsPanel.UpdateCoins();
			break;
		}
	}

	public override void Exit(GameState to)
	{
		StopAllCoroutines();
		gameObject.SetActive(false);
		navPanel.SetActive(false);
		missionPanel.Hide();
		scoreCoinsPanel.Hide();
		quitBtn.SetActive(false);
	}

	protected void HideInteractivePanels()
	{
		navPanel.SetActive(false);
		quitBtn.SetActive(false);
	}

	private IEnumerator SwitchState(GameStateName toState)
	{
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(true);
		}
		missionPanel.missionSetAnim.Play("PanelSlideOut");
		navPanel.GetComponent<Animation>().Play("NavPanelSlideOut");
		scoreCoinsPanel.scoreCoinsAnim.Play("ScoreCoinsSlideOut");
		quitBtn.GetComponent<Animation>().Play("SetInactiveAnim");
		yield return new WaitForSeconds(0.5f);
		if (UAP_AccessibilityManager.IsEnabled())
		{
			UAP_AccessibilityManager.BlockInput(false);
		}
		CustomGameManager.Instance.SwitchState(toState);
	}

	private void GoToState(GameStateName toState)
	{
		missionPanel.StopEverything();
		MissionManager.Instance.ResetProgress();
		if (gameObject.activeSelf)
		{
			StartCoroutine(SwitchState(toState));
		}
	}

	public void PlayBtnClicked()
	{
		GoToState(GameStateName.SelectBoost);
	}

	public void InventoryBtnClicked()
	{
		GoToState(GameStateName.MenuUpgrades);
	}

	public void HighscoreBtnClicked()
	{
		GoToState(GameStateName.MenuLeaderboard);
	}

	public void OptionsBtnClicked()
	{
		GoToState(GameStateName.MenuOptions);
	}

	public void ProfileBtnClicked()
	{
		GoToState(GameStateName.MenuProfile);
	}

	public void QuitGameBtnClicked()
	{
		Application.Quit();
	}
}
