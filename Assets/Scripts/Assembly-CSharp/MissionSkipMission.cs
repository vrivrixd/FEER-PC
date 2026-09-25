public class MissionSkipMission : Mission
{
	public MissionSkipMission(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.SingleRun, MissionType.SkipMission, goal, progress, completed);
		m_IsMissionTitleTTS = true;
		string quest = (goal <= 1f) ? "QUEST" : "QUESTS";
		string number = NumberFormatter.FormatToLocale((int)goal);
		m_MissionTitle = L("SKIP") + " " + number + " " + L(quest);
		m_MissionTitleTTS = L("tts_SKIP") + " " + ((goal <= 1f) ? L("tts_one") : number) + " " + L(quest);
		m_MissionDesc = L("choose any quest you like");
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
		return (IsOpen ? ((int)m_Goal - (int)m_Progress) : 0).ToString() + m_strLocalizedLeft;
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
