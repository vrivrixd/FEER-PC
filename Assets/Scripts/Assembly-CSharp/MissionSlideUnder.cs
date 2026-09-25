public class MissionSlideUnder : Mission
{
	public MissionSlideUnder(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.SlideUnder, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = ScopeDesc();
	}

	protected void SetMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "GANTRY CRANE", "RAVEN", "GANTRY CRANES", "RAVENS");
	}

	public override void UpdateMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "GANTRY CRANE", "RAVEN", "GANTRY CRANES", "RAVENS");
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
		StdUpdateProgress(CGM.slidedUnder);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
