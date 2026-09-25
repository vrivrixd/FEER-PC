using System.Collections.Generic;
using UnityEngine;

public class TrackPartPooler : MonoBehaviour
{
	private static Dictionary<string, TrackPartPool> s_TrackPartPool;

	protected const int k_StartingPoolSize = 2;

	public TrackPartPool GetPool(TrackPart trackPart)
	{
		return null;
	}

	private void CreatePool(TrackPart trackPart)
	{
	}

	public void ResetPools()
	{
	}

	public void ClearPools()
	{
	}
}
