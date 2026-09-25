using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool
{
	protected Stack<Collectible> m_FreeInstances;

	protected Collectible m_Original;

	protected Transform m_PoolTransform;

	public CollectiblePool(Collectible original, int initialSize, Transform parent)
	{
	}

	public Collectible Get()
	{
		return null;
	}

	public Collectible Get(Vector3 pos, Quaternion quat)
	{
		return null;
	}

	public void Free(Collectible obj)
	{
	}

	public void ResetPool()
	{
	}

	public void ClearPool()
	{
	}
}
