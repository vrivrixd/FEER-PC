using System;

[Serializable]
public class AppRateData
{
	public int timesAsked;

	public DateTime lastTimeAsked;

	public int sessionsSinceAsked;

	public int gamesPlayedSinceAsked;
}
