public class MissionJumpOver : Mission
{
	public MissionJumpOver(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.JumpOver, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = ScopeDesc();
	}

	protected void SetMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "SAW BLADE", "HAND", "SAW BLADES", "HANDS");
	}

	public override void UpdateMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "SAW BLADE", "HAND", "SAW BLADES", "HANDS");
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
		StdUpdateProgress(CGM.jumpedOver);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
