public class MissionDieGround : Mission
{
	public override void IncreaseProgress()
	{
	}

	public MissionDieGround(float goal, float progress, bool completed)
	{
	}

	protected void SetMissionThemeText()
	{
	}

	public override void UpdateMissionThemeText()
	{
	}

	public override string GetMissionTitle(bool ttsValue = false)
	{
		return null;
	}

	public override string GetMissionDesc(bool ttsValue = false)
	{
		return null;
	}

	public override string GetMissionProgress(bool ttsValue = false)
	{
		return null;
	}

	public override int GetLeftNumber()
	{
		return 0;
	}

	public override float GetProgressInPercent()
	{
		return 0f;
	}

	public override void UpdateProgress()
	{
	}

	public override float GetResultToSave()
	{
		return 0f;
	}
}
