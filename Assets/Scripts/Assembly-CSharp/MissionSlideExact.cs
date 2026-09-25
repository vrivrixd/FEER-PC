public class MissionSlideExact : Mission
{
	public MissionSlideExact(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.SlideExact, goal, progress, completed);
		SetCountTitle("SLIDE", "time", "times");
		m_MissionDesc = L("EXACTLY!") + " " + L("in a single run");
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
		return ExactProgress(CGM.slideCount);
	}

	public override int GetLeftNumber()
	{
		return ExactLeftNumber(CGM.slideCount);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		ExactUpdateProgress(CGM.slideCount);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
