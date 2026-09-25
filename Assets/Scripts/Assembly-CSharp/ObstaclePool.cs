using System.Collections.Generic;
using UnityEngine;

public class ObstaclePool
{
	protected Stack<Obstacle> m_FreeInstances;

	protected Obstacle m_Original;

	protected Transform m_PoolTransform;

	public ObstaclePool(Obstacle original, int initialSize, Transform parent)
	{
	}

	public Obstacle Get()
	{
		return null;
	}

	public Obstacle Get(Vector3 pos, Quaternion quat)
	{
		return null;
	}

	public void Free(Obstacle obj)
	{
	}

	public void ResetPool()
	{
	}

	public void ClearPool()
	{
	}
}
