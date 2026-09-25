using System.Collections.Generic;
using UnityEngine;

public class ObstaclePooler : MonoBehaviour
{
	private static Dictionary<string, ObstaclePool> s_ObstaclePool;

	protected const int k_StartingPoolSize = 2;

	public ObstaclePool GetPool(Obstacle obstacle)
	{
		return null;
	}

	private void CreatePool(Obstacle obstacle)
	{
	}

	public void ResetPools()
	{
	}

	public void ClearPools()
	{
	}
}
