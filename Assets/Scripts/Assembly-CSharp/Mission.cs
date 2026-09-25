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
			return 0f;
		}
		set
		{
		}
	}

	public bool completed
	{
		get
		{
			return false;
		}
		set
		{
		}
	}

	public float startProgress => 0f;

	public float goal => 0f;

	public bool finished => false;

	public MissionType missionType => MissionType.JumpOver;

	public MissionScope missionScope => MissionScope.SingleRun;

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
