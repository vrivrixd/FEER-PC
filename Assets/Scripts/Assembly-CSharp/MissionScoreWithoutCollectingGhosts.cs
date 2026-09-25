public class MissionScoreWithoutCollectingGhosts : Mission
{
	public MissionScoreWithoutCollectingGhosts(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.ScoreWithoutCollectingGhosts, goal, progress, completed);
		m_MissionTitle = L("mission_score") + " " + NumberFormatter.FormatToLocale((int)goal) + " " + L("POINTS");
		m_MissionDesc = L("no lights") + ", " + L("in a single run");
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
		return WithoutProgress(CGM.ghostsCollected);
	}

	public override int GetLeftNumber()
	{
		return WithoutLeftNumber(CGM.ghostsCollected);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		WithoutUpdateProgress(CGM.ghostsCollected, CGM.score);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
