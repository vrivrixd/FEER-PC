using System;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
	protected int m_CurrentMissionSet;

	protected Mission[] m_CurrentMissions;

	protected MissionSetData missionSetData;

	protected int m_NextMissionNumber;

	protected Action<bool> m_NextMissionCallbackFunction;

	protected const string c_MissionSetFileName = "missionset_";

	protected const string c_MissionSetFileType = ".json";

	private static MissionManager instance;

	public Mission[] currentMissions => null;

	public int currentMissionSet => 0;

	public static MissionManager Instance => null;

	private void Awake()
	{
	}

	public void Init()
	{
	}

	public void UpdateMissionThemeText()
	{
	}

	protected void InitCheckMissionComplete()
	{
	}

	public void InitLatestMissionSetReceived(bool success)
	{
	}

	public void FirstMissionSetReceived(bool success, string fileContent)
	{
	}

	protected void ExtractFileData(string fileContent)
	{
	}

	protected void CreateMissions()
	{
	}

	public void UpdateProgress()
	{
	}

	public bool SkipMission(int missionNumber)
	{
		return false;
	}

	public bool CompleteMission(int missionNumber)
	{
		return false;
	}

	public bool SaveProgress()
	{
		return false;
	}

	public void ResetProgress()
	{
	}

	public void LoadNextMissionSet(Action<bool> callbackFunction)
	{
	}

	public void NextMissionSetReceived(bool success, string fileContent)
	{
	}

	protected void GenerateRandomMissionSet()
	{
	}
}
