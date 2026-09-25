public class MissionPowerUpBoost : Mission
{
	public MissionPowerUpBoost(MissionScope scope, float goal, float progress, bool completed)
	{
		InitMission(scope, MissionType.PowerUpBoost, goal, progress, completed);
		SetCountTitleTTS("COLLECT", "COLLECT", "BOOST", "tts_BOOST", "BOOSTS", "tts_BOOSTS");
		m_MissionDesc = ScopeDesc();
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
		StdUpdateProgress(CGM.powerUpBoostCollected);
	}

	public override float GetResultToSave()
	{
		return StdResultToSave();
	}
}
