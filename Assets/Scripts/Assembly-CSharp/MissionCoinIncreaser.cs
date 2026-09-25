public class MissionCoinIncreaser : Mission
{
	public MissionCoinIncreaser(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.MultiRun, MissionType.CoinIncreaser, goal, progress, completed);
		SetCountTitleTTS("USE A LIGHT BOOST", "tts_USE LIGHT BOOST", "time", "time", "times", "times");
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
