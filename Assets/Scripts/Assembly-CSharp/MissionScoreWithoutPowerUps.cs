public class MissionScoreWithoutPowerUps : Mission
{
	public MissionScoreWithoutPowerUps(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.ScoreWithoutPowerUps, goal, progress, completed);
		m_MissionTitle = L("mission_score") + " " + NumberFormatter.FormatToLocale((int)goal) + " " + L("POINTS");
		m_IsMissionDescTTS = true;
		m_MissionDesc = L("no powerups") + ", " + L("in a single run");
		m_MissionDescTTS = L("tts_no powerups") + ", " + L("in a single run");
	}

	public override void UpdateMissionThemeText()
	{
	}

	public override void IncreaseProgress()
	{
	}

	public override string GetMissionTitle(bool ttsValue = false)
	{
		return m_MissionTitle;
	}

	public override string GetMissionDesc(bool ttsValue = false)
	{
		return StdDescTTS(ttsValue);
	}

	public override string GetMissionProgress(bool ttsValue = false)
	{
		return WithoutProgress(CGM.powerUpCollected);
	}

	public override int GetLeftNumber()
	{
		return WithoutLeftNumber(CGM.powerUpCollected);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		WithoutUpdateProgress(CGM.powerUpCollected, CGM.score);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
