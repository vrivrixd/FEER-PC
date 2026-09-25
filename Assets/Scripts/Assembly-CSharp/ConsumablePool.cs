using System.Collections.Generic;
using UnityEngine;

public class ConsumablePool
{
	protected Stack<Consumable> m_FreeInstances;

	protected Consumable m_Original;

	protected Transform m_PoolTransform;

	public ConsumablePool(Consumable original, int initialSize, Transform parent)
	{
	}

	public Consumable Get()
	{
		return null;
	}

	public Consumable Get(Vector3 pos, Quaternion quat)
	{
		return null;
	}

	public void Free(Consumable obj)
	{
	}

	public void ResetPool()
	{
	}

	public void ClearPool()
	{
	}
}
