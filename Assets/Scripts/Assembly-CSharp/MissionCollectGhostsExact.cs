public class MissionCollectGhostsExact : Mission
{
	public MissionCollectGhostsExact(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.CollectGhostsExact, goal, progress, completed);
		SetCountTitle("COLLECT", "LIGHT", "LIGHTS");
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
		return ExactProgress(CGM.ghostsCollected);
	}

	public override int GetLeftNumber()
	{
		return ExactLeftNumber(CGM.ghostsCollected);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		StdUpdateProgress(CGM.ghostsCollected);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
