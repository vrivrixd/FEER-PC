public class MissionBeatHighscore : Mission
{
	public MissionBeatHighscore(float goal, float progress, bool completed)
	{
		InitMission(MissionScope.MultiRun, MissionType.BeatScore, goal, progress, completed);
		m_MissionTitle = L("BEAT YOUR HIGHSCORE");
		m_MissionTitleTTS = L("tts_BEAT YOUR HIGHSCORE");
		m_IsMissionTitleTTS = true;
		if (m_Goal <= 1f)
		{
			m_MissionDesc = L("score more than") + " " + NumberFormatter.FormatToLocale(DataManager.Instance.playerData.highscore) + " " + L("points");
		}
		else
		{
			m_MissionDesc = NumberFormatter.FormatToLocale((int)goal) + " " + L("times");
		}
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
		if (!IsOpen)
		{
			return;
		}
		float value;
		if (DataManager.Instance.playerData.highscore < CGM.score)
		{
			value = m_StartProgress + 1f;
			m_Progress = value;
		}
		else
		{
			value = m_Progress;
		}
		if (!(0f < (float)(int)m_Goal - value))
		{
			m_Finished = true;
		}
	}

	public override float GetResultToSave()
	{
		return TotalResultToSave();
	}
}
