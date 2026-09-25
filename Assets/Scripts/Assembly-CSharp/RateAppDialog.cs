using System;
using UnityEngine;

public class RateAppDialog : MonoBehaviour
{
	public int minGamesTillPrompting = 10;

	public int minLaunchesTillPrompting = 5;

	public int minDaysBetweenPrompting = 10;

	public int minGamesBetweenPrompting = 20;

	public int minLaunchesBetweenPrompting = 10;

	public GameStateGameOver gameOverState;

	public void Show()
	{
		// PORT: o pedido de avaliacao na Play Store nao existe no PC; segue direto como se o dialogo nao fosse exibido.
		if (false && DataManager.Instance.playerData.showAppRateDialog && ShouldPrompt())
		{
			ShowRatePanel();
			return;
		}
		gameOverState.RateAppDialogClosed();
	}

	// Regras originais de exibicao (mantidas para referencia)
	private bool ShouldPrompt()
	{
		AppRateData appRateData = DataManager.Instance.appRateData;
		if (appRateData.timesAsked == 0)
		{
			return minGamesTillPrompting <= appRateData.gamesPlayedSinceAsked && minLaunchesTillPrompting <= appRateData.sessionsSinceAsked;
		}
		if (minGamesBetweenPrompting > appRateData.gamesPlayedSinceAsked || minLaunchesBetweenPrompting > appRateData.sessionsSinceAsked)
		{
			return false;
		}
		if ((double)minDaysBetweenPrompting > (DateTime.Now - appRateData.lastTimeAsked).TotalDays)
		{
			return false;
		}
		if (appRateData.timesAsked > 2 && (DateTime.Now - appRateData.lastTimeAsked).TotalDays >= 365.0)
		{
			return true;
		}
		return appRateData.timesAsked < 3;
	}

	protected void ShowRatePanel()
	{
		DataManager.Instance.AppRatingAsked();
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
		gameOverState.RateAppDialogClosed();
	}

	public void RateAppBtnClicked()
	{
		DataManager.Instance.AppRatingNeverAgain();
		// PORT: abrir a Play Store e analytics removidos.
		Hide();
	}

	public void NeverAskMeAgainBtnClicked()
	{
		DataManager.Instance.AppRatingNeverAgain();
		Hide();
	}

	public void MaybeLaterBtnClicked()
	{
		Hide();
	}
}
