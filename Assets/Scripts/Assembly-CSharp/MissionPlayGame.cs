public class MissionPlayGame : Mission
{
	public MissionPlayGame(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.MultiRun, MissionType.PlayGame, goal, progress, completed);
		SetCountTitle("PLAY", "GAME", "GAMES");
		m_MissionDesc = L("in total");
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
		return TotalResultToSave();
	}
}
