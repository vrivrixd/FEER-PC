using UnityEngine;
using UnityEngine.UI;

public class GameStateIsDead : GameState
{
	public ScoreCoinsPanelUI scoreCoinsPanel;

	public GameObject saveMePanel;

	public GameObject headline;

	public Text saveMeSubtitle;

	protected UAP_BaseElement m_AccessibleSaveMeSubtitle;

	protected int m_SaveMeTimesLeft;

	protected int m_SaveMeCost;

	public RectTransform panelTransform;

	protected bool m_Init;

	protected string m_1For;

	protected string m_2LightsTheFairies;

	protected string m_3Lights;

	protected string m_ttsYouScored;

	protected string m_ttsYouCollected;

	protected string m_ttsLightsThisRun;

	protected string m_ttsPointsThisRun;

	public override void Enter(GameState from)
	{
		if (!m_Init)
		{
			InitIsDead();
		}
		scoreCoinsPanel.RenderPlayGameCoinsUI();
		scoreCoinsPanel.RenderPlayGameScoreUI();
		scoreCoinsPanel.Show(panelTransform, false, false, 0, false);
		MissionManager.Instance.UpdateProgress();
		saveMeSubtitle.text = string.Concat(new string[5]
		{
			m_1For,
			NumberFormatter.FormatToLocale(m_SaveMeCost),
			m_2LightsTheFairies,
			NumberFormatter.FormatToLocale(DataManager.Instance.playerData.coins),
			m_3Lights
		});
		if (m_AccessibleSaveMeSubtitle == null)
		{
			m_AccessibleSaveMeSubtitle = saveMeSubtitle.gameObject.GetComponent<UAP_BaseElement>();
		}
		m_AccessibleSaveMeSubtitle.m_Text = string.Concat(new string[9]
		{
			saveMeSubtitle.text,
			". ",
			m_ttsYouScored,
			CustomGameManager.Instance.score.ToString(),
			m_ttsPointsThisRun,
			". ",
			m_ttsYouCollected,
			CustomGameManager.Instance.sumCollectedGhosts.ToString(),
			m_ttsLightsThisRun
		});
		gameObject.SetActive(true);
	}

	public override GameStateName GetName()
	{
		return GameStateName.IsDead;
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
		if (infoMessage != InfoMessage.IsDeadAnimationFinished)
		{
			return;
		}
		CustomGameManager.Instance.trackManager.fog.PauseFog();
		if (m_SaveMeTimesLeft > 0 && m_SaveMeCost <= DataManager.Instance.playerData.coins)
		{
			saveMePanel.SetActive(true);
			CustomGameManager.Instance.AllowSleepMode(true);
		}
		else
		{
			CustomGameManager.Instance.SwitchState(GameStateName.GameOver);
		}
	}

	public override void Exit(GameState to)
	{
		gameObject.SetActive(false);
		scoreCoinsPanel.Hide();
		saveMePanel.SetActive(false);
		if (to.GetName() == GameStateName.PlayGame)
		{
			m_SaveMeTimesLeft--;
			m_SaveMeCost = DataManager.Instance.saveMeAdditionalMultiplier * m_SaveMeCost;
		}
		else
		{
			m_SaveMeCost = DataManager.Instance.saveMeCosts;
			m_SaveMeTimesLeft = DataManager.Instance.saveMeTimes;
		}
	}

	protected void InitIsDead()
	{
		m_SaveMeCost = DataManager.Instance.saveMeCosts;
		m_SaveMeTimesLeft = DataManager.Instance.saveMeTimes;
		CustomGameManager.Instance.saveMeUsed = 0;
		LocalizationManager localization = LocalizationManager.Instance;
		m_1For = localization.GetLocalizedValue("For") + " ";
		m_2LightsTheFairies = string.Concat(new string[5]
		{
			" ",
			localization.GetLocalizedValue("lights the fairies save your life."),
			" (",
			localization.GetLocalizedValue("You have"),
			" "
		});
		m_3Lights = " " + localization.GetLocalizedValue("lights") + ")";
		m_AccessibleSaveMeSubtitle = saveMeSubtitle.gameObject.GetComponent<UAP_BaseElement>();
		m_ttsYouScored = localization.GetLocalizedValue("tts_you_have_scored") + " ";
		m_ttsYouCollected = localization.GetLocalizedValue("tts_you_have_collected") + " ";
		m_ttsLightsThisRun = " " + localization.GetLocalizedValue("tts_lights_this_run");
		m_ttsPointsThisRun = " " + localization.GetLocalizedValue("tts_points_this_run");
		m_Init = true;
	}

	public void ResetSaveMeCosts()
	{
		CustomGameManager.Instance.saveMeUsed = 0;
		m_SaveMeCost = DataManager.Instance.saveMeCosts;
		m_SaveMeTimesLeft = DataManager.Instance.saveMeTimes;
	}

	public void SaveMeBtnClicked()
	{
		if (!DataManager.Instance.InvestCoins(m_SaveMeCost, (TransactionContext)0, (TransactionalItem)0, (TransactionItemType)0, null))
		{
			return;
		}
		for (int i = 0; i < MissionManager.Instance.currentMissions.Length; i++)
		{
			if (MissionManager.Instance.currentMissions[i].missionType == MissionType.SaveMe)
			{
				MissionManager.Instance.currentMissions[i].IncreaseProgress();
			}
		}
		CustomGameManager.Instance.saveMeUsed = CustomGameManager.Instance.saveMeUsed + 1;
		CustomGameManager.Instance.SwitchState(GameStateName.PlayGame);
	}

	public void SkipBtnClicked()
	{
		CustomGameManager.Instance.SwitchState(GameStateName.GameOver);
	}
}
