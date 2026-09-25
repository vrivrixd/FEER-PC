public class MissionJumpRight : Mission
{
	public MissionJumpRight(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.JumpRight, goal, progress, completed);
		SetCountTitle("JUMP", "time", "times");
		m_MissionDesc = L("in right lane") + ", " + ScopeDesc();
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
		StdUpdateProgress(CGM.jumpCountRight);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
