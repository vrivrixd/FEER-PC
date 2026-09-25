public class MissionSlideUnderExact : Mission
{
	public MissionSlideUnderExact(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.SlideUnderExact, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = L("EXACTLY!") + " " + L("in a single run");
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
		return ExactProgress(CGM.slidedUnder);
	}

	public override int GetLeftNumber()
	{
		return ExactLeftNumber(CGM.slidedUnder);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		ExactUpdateProgress(CGM.slidedUnder);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
