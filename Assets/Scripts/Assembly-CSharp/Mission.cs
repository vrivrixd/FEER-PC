public abstract class Mission
{
	protected MissionScope m_Scope;

	protected MissionType m_Type;

	protected bool m_Completed;

	protected bool m_Finished;

	protected float m_Goal;

	protected float m_Progress;

	protected float m_StartProgress;

	protected string m_MissionTitle;

	protected string m_MissionDesc;

	protected string m_MissionTitleTTS;

	protected string m_MissionDescTTS;

	protected bool m_IsMissionTitleTTS;

	protected bool m_IsMissionDescTTS;

	protected string m_strLocalizedLeft;

	public float progress
	{
		get
		{
			return m_Progress;
		}
		set
		{
			m_Progress = value;
		}
	}

	public bool completed
	{
		get
		{
			return m_Completed;
		}
		set
		{
			m_Completed = value;
		}
	}

	public float startProgress => m_StartProgress;

	public float goal => m_Goal;

	public bool finished => m_Finished;

	public MissionType missionType => m_Type;

	public MissionScope missionScope => m_Scope;

	// ---- Rotinas comuns (no original estao duplicadas, identicas, em cada subclasse) ----

	protected static string L(string key)
	{
		return LocalizationManager.Instance.GetLocalizedValue(key);
	}

	protected static CustomGameManager CGM => CustomGameManager.Instance;

	protected bool IsOpen => !m_Completed && !m_Finished;

	protected void InitMission(MissionScope scope, MissionType type, float goal, float progress, bool completed)
	{
		m_strLocalizedLeft = " " + L("left");
		m_Goal = goal;
		m_Progress = progress;
		m_StartProgress = progress;
		m_Finished = false;
		m_Completed = completed;
		m_Scope = scope;
		m_Type = type;
	}

	// "VERBO 3 vezes" / tts: "VERBO um vez"
	protected void SetCountTitle(string verbKey, string oneKey, string manyKey)
	{
		if (m_Goal <= 1f)
		{
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(oneKey);
			m_IsMissionTitleTTS = true;
			m_MissionTitleTTS = L(verbKey) + " " + L("tts_one") + " " + L(oneKey);
		}
		else
		{
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(manyKey);
		}
	}

	// Titulo com versao TTS propria tambem no plural
	protected void SetCountTitleTTS(string verbKey, string verbTTSKey, string oneKey, string oneTTSKey, string manyKey, string manyTTSKey)
	{
		m_IsMissionTitleTTS = true;
		if (m_Goal <= 1f)
		{
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(oneKey);
			m_MissionTitleTTS = L(verbTTSKey) + " " + L("tts_one") + " " + L(oneTTSKey);
		}
		else
		{
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(manyKey);
			m_MissionTitleTTS = L(verbTTSKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(manyTTSKey);
		}
	}

	// Titulos que dependem do tema (Forest/Factory): "SOBREVIVA a 3 ZUMBIS"
	protected void SetThemeCountTitle(string verbKey, string oneFactory, string oneForest, string manyFactory, string manyForest)
	{
		bool factory = DataManager.Instance.selectedTheme == Theme.Factory;
		if (m_Goal <= 1f)
		{
			string noun = factory ? oneFactory : oneForest;
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(noun);
			m_IsMissionTitleTTS = true;
			m_MissionTitleTTS = L(verbKey) + " " + L("tts_one") + " " + L(noun);
		}
		else
		{
			string noun2 = factory ? manyFactory : manyForest;
			m_MissionTitle = L(verbKey) + " " + NumberFormatter.FormatToLocale((int)m_Goal) + " " + L(noun2);
		}
	}

	protected string ScopeDesc()
	{
		return L((m_Scope == MissionScope.SingleRun) ? "in a single run" : "in total");
	}

	protected string StdTitle(bool ttsValue)
	{
		return (ttsValue && m_IsMissionTitleTTS) ? m_MissionTitleTTS : m_MissionTitle;
	}

	protected string StdDescTTS(bool ttsValue)
	{
		return (ttsValue && m_IsMissionDescTTS) ? m_MissionDescTTS : m_MissionDesc;
	}

	protected int StdLeftNumber()
	{
		if (IsOpen)
		{
			return (int)m_Goal - (int)m_Progress;
		}
		return 0;
	}

	protected string StdProgress()
	{
		return NumberFormatter.FormatToLocale(StdLeftNumber()) + m_strLocalizedLeft;
	}

	protected float StdPercent()
	{
		if (IsOpen)
		{
			return m_Progress / m_Goal;
		}
		return 1f;
	}

	protected void StdIncrease()
	{
		if (!m_Finished && !m_Completed)
		{
			m_Progress += 1f;
			if (m_Goal <= m_Progress)
			{
				m_Finished = true;
			}
		}
	}

	protected void StdUpdateProgress(int count)
	{
		if (IsOpen)
		{
			float value = (m_Scope == MissionScope.MultiRun) ? (m_StartProgress + (float)count) : ((float)count);
			m_Progress = value;
			if ((float)(int)m_Goal - value <= 0f)
			{
				m_Finished = true;
			}
		}
	}

	protected void TotalUpdateProgress(int count)
	{
		if (IsOpen)
		{
			float value = m_StartProgress + (float)count;
			m_Progress = value;
			if ((float)(int)m_Goal - value <= 0f)
			{
				m_Finished = true;
			}
		}
	}

	protected float StdResultToSave()
	{
		float value;
		if (IsOpen)
		{
			if (m_Scope == MissionScope.SingleRun)
			{
				m_Progress = 0f;
				return 0f;
			}
			value = m_Progress;
		}
		else
		{
			value = m_Goal;
		}
		m_Progress = (int)value;
		return (int)value;
	}

	protected float TotalResultToSave()
	{
		float value = IsOpen ? m_Progress : m_Goal;
		m_Progress = (int)value;
		return (int)value;
	}

	protected float ExactResultToSave()
	{
		float value = IsOpen ? 0f : ((float)(int)m_Goal);
		m_Progress = value;
		return value;
	}

	// Missoes "EXATAMENTE n": passar do objetivo zera o progresso
	protected string ExactProgress(int count)
	{
		int number = 0;
		if (IsOpen)
		{
			number = ((float)count <= m_Goal) ? ((int)m_Goal - (int)m_Progress) : ((int)m_Goal);
		}
		return NumberFormatter.FormatToLocale(number) + m_strLocalizedLeft;
	}

	protected int ExactLeftNumber(int count)
	{
		if (m_Goal < (float)count)
		{
			return -1;
		}
		return StdLeftNumber();
	}

	protected void ExactUpdateProgress(int count)
	{
		bool finished = m_Finished;
		if (!m_Completed)
		{
			if (!finished)
			{
				float value = count;
				m_Progress = value;
				if ((int)value == (int)m_Goal)
				{
					m_Finished = true;
					return;
				}
				if (value <= m_Goal)
				{
					return;
				}
				m_Progress = 0f;
				return;
			}
		}
		else if (!finished)
		{
			return;
		}
		if ((float)count <= m_Goal)
		{
			return;
		}
		m_Finished = false;
		m_Progress = 0f;
	}

	// Missoes "sem X": se X aconteceu (blocker >= 1) o progresso fica 0
	protected string WithoutProgress(int blocker)
	{
		int number = 0;
		if (IsOpen)
		{
			number = (blocker < 1) ? ((int)m_Goal - (int)m_Progress) : ((int)m_Goal);
		}
		return NumberFormatter.FormatToLocale(number) + m_strLocalizedLeft;
	}

	protected int WithoutLeftNumber(int blocker)
	{
		if (blocker < 1)
		{
			return StdLeftNumber();
		}
		return -1;
	}

	protected void WithoutUpdateProgress(int blocker, int count)
	{
		if (IsOpen)
		{
			float value = 0f;
			if (blocker < 1)
			{
				value = count;
			}
			m_Progress = value;
			if ((float)(int)m_Goal - value <= 0f)
			{
				m_Finished = true;
			}
		}
	}

	public abstract string GetMissionTitle(bool ttsValue = false);

	public abstract string GetMissionDesc(bool ttsValue = false);

	public abstract string GetMissionProgress(bool ttsValue = false);

	public abstract int GetLeftNumber();

	public abstract float GetProgressInPercent();

	public abstract float GetResultToSave();

	public abstract void UpdateProgress();

	public abstract void IncreaseProgress();

	public abstract void UpdateMissionThemeText();
}
