public class MissionSaveMe : Mission
{
	public MissionSaveMe(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.SaveMe, goal, progress, completed);
		SetCountTitle("USE SAVE ME", "time", "times");
		m_MissionDesc = ScopeDesc();
	}

	public override void UpdateMissionThemeText()
	{
	}

	public override void IncreaseProgress()
	{
		StdIncrease();
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
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
