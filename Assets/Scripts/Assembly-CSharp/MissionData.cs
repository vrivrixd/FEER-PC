using System;

[Serializable]
public class MissionData
{
	public MissionScope scope;

	public MissionType type;

	public float goal;

	public float progress;

	public bool completed;
}
