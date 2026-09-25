public class MissionCollectGhostsWithoutPowerUps : Mission
{
	public MissionCollectGhostsWithoutPowerUps(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.CollectGhostsWithoutPowerUps, goal, progress, completed);
		SetCountTitle("COLLECT", "LIGHT", "LIGHTS");
		m_IsMissionDescTTS = true;
		m_MissionDesc = L("no powerups") + ", " + L("in a single run");
		m_MissionDescTTS = L("tts_no powerups") + ", " + L("in a single run");
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
		return StdDescTTS(ttsValue);
	}

	public override string GetMissionProgress(bool ttsValue = false)
	{
		return WithoutProgress(CGM.powerUpCollected);
	}

	public override int GetLeftNumber()
	{
		return WithoutLeftNumber(CGM.powerUpCollected);
	}

	public override float GetProgressInPercent()
	{
		return StdPercent();
	}

	public override void UpdateProgress()
	{
		WithoutUpdateProgress(CGM.powerUpCollected, CGM.ghostsCollected);
	}

	public override float GetResultToSave()
	{
		return ExactResultToSave();
	}
}
