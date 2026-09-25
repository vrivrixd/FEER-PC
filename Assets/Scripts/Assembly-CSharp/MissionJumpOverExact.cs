public class MissionJumpOverExact : Mission
{
	public MissionJumpOverExact(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.JumpOverExact, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = L("EXACTLY!") + " " + L("in a single run");
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
		return ExactProgress(CGM.jumpedOver);
	}

	public override int GetLeftNumber()
	{
		return ExactLeftNumber(CGM.jumpedOver);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		ExactUpdateProgress(CGM.jumpedOver);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
