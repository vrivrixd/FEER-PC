public class MissionJumpExact : Mission
{
	public MissionJumpExact(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.JumpExact, goal, progress, completed);
		SetCountTitle("JUMP", "time", "times");
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
		return ExactProgress(CGM.jumpCount);
	}

	public override int GetLeftNumber()
	{
		return ExactLeftNumber(CGM.jumpCount);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		ExactUpdateProgress(CGM.jumpCount);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
