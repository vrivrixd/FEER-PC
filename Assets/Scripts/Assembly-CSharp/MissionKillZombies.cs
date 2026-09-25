public class MissionKillZombies : Mission
{
	public MissionKillZombies(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.KillZombies, goal, progress, completed);
		SetMissionThemeText();
		m_MissionDesc = L("in a single run");
	}

	protected void SetMissionThemeText()
	{
		SetThemeCountTitle("KILL", "ROBOT", "ZOMBIE", "ROBOTS", "ZOMBIES");
	}

	public override void UpdateMissionThemeText()
	{
		SetThemeCountTitle("KILL", "ROBOT", "ZOMBIE", "ROBOTS", "ZOMBIES");
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
		StdUpdateProgress(CGM.zombiesKilled);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
