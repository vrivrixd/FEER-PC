public class MissionScore : Mission
{
	public MissionScore(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.Score, goal, progress, completed);
		m_MissionTitle = L("mission_score") + " " + NumberFormatter.FormatToLocale((int)goal) + " " + L("POINTS");
		m_MissionDesc = ScopeDesc();
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
		return m_MissionDesc;
	}

	public override string GetMissionProgress(bool ttsValue = false)
	{
		return StdProgress();
	}

	public override int GetLeftNumber()
	{
		return StdLeftNumber();
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		StdUpdateProgress(CGM.score);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
