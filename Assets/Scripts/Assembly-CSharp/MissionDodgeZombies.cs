public class MissionDodgeZombies : Mission
{
	public MissionDodgeZombies(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.DodgeZombies, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = ScopeDesc();
	}

	protected void SetMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "ROBOT", "ZOMBIE", "ROBOTS", "ZOMBIES");
	}

	public override void UpdateMissionThemeText()
	{
		SetThemeCountTitle("SURVIVE", "ROBOT", "ZOMBIE", "ROBOTS", "ZOMBIES");
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
		StdUpdateProgress(CGM.zombiesDodged);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
