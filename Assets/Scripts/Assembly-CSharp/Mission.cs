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
