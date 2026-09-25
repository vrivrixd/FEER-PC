public class MissionSlideCenter : Mission
{
	public MissionSlideCenter(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.SlideCenter, goal, progress, completed);
		SetCountTitle("SLIDE", "time", "times");
		m_MissionDesc = L("in center lane") + ", " + ScopeDesc();
	}

	public override void UpdateMissionThemeText()
	{
	}

	public override void IncreaseProgress()
	{
	}

	public override string GetMissionTitle(bool ttsValue = false)
	{
		return StdTitle(ttsValue);
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
		StdUpdateProgress(CGM.slideCountCenter);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
