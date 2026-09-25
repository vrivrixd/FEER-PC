using System.Collections.Generic;
using UnityEngine;

public class TrackPartPooler : MonoBehaviour
{
	private static Dictionary<string, TrackPartPool> s_TrackPartPool = new Dictionary<string, TrackPartPool>();

	protected const int k_StartingPoolSize = 2;

	public TrackPartPool GetPool(TrackPart trackPart)
	{
		if (!s_TrackPartPool.ContainsKey(trackPart.objectID))
		{
			CreatePool(trackPart);
		}
		return s_TrackPartPool[trackPart.objectID];
	}

	private void CreatePool(TrackPart trackPart)
	{
		TrackPartPool pool = new TrackPartPool(trackPart, k_StartingPoolSize, transform);
		s_TrackPartPool.Add(trackPart.objectID, pool);
	}

	public void ResetPools()
	{
		foreach (KeyValuePair<string, TrackPartPool> item in s_TrackPartPool)
		{
			item.Value.ResetPool();
		}
		s_TrackPartPool = new Dictionary<string, TrackPartPool>();
	}

	public void ClearPools()
	{
		foreach (KeyValuePair<string, TrackPartPool> item in s_TrackPartPool)
		{
			item.Value.ClearPool();
		}
		s_TrackPartPool = new Dictionary<string, TrackPartPool>();
	}
}
