public class MissionJumpLeft : Mission
{
	public MissionJumpLeft(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.JumpLeft, goal, progress, completed);
		SetCountTitle("JUMP", "time", "times");
		m_MissionDesc = L("in left lane") + ", " + ScopeDesc();
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
		StdUpdateProgress(CGM.jumpCountLeft);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
