using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooler : MonoBehaviour
{
	private static Dictionary<string, ObstaclePool> s_ObstaclePool = new Dictionary<string, ObstaclePool>();

	protected const int k_StartingPoolSize = 2;

	public ObstaclePool GetPool(Obstacle obstacle)
	{
		if (!s_ObstaclePool.ContainsKey(obstacle.objectID))
		{
			CreatePool(obstacle);
		}
		return s_ObstaclePool[obstacle.objectID];
	}

	private void CreatePool(Obstacle obstacle)
	{
		ObstaclePool pool = new ObstaclePool(obstacle, k_StartingPoolSize, transform);
		s_ObstaclePool.Add(obstacle.objectID, pool);
	}

	public void ResetPools()
	{
		foreach (KeyValuePair<string, ObstaclePool> item in s_ObstaclePool)
		{
			item.Value.ResetPool();
		}
		s_ObstaclePool = new Dictionary<string, ObstaclePool>();
	}

	public void ClearPools()
	{
		foreach (KeyValuePair<string, ObstaclePool> item in s_ObstaclePool)
		{
			item.Value.ClearPool();
		}
		s_ObstaclePool = new Dictionary<string, ObstaclePool>();
	}
}
