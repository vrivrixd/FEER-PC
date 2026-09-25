public class MissionDieAir : Mission
{
	public MissionDieAir(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.MultiRun, MissionType.DieAir, goal, progress, completed);
		SetCountTitle("DIE", "time", "times");
		SetMissionThemeText();
	}

	protected void SetMissionThemeText()
	{
		m_MissionDesc = L((DataManager.Instance.selectedTheme == Theme.Factory) ? "through GANTRY CRANES" : "through RAVENS") + ", " + L("in total");
	}

	public override void UpdateMissionThemeText()
	{
		m_MissionDesc = L((DataManager.Instance.selectedTheme == Theme.Factory) ? "through GANTRY CRANES" : "through RAVENS") + ", " + L("in total");
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
		TotalUpdateProgress(CGM.killedByAir);
	}

	public override float GetResultToSave()
	{
		return TotalResultToSave();
	}
}
