using System.Collections.Generic;
using UnityEngine;

public class ConsumablePool
{
	protected Stack<Consumable> m_FreeInstances = new Stack<Consumable>();

	protected Consumable m_Original;

	protected Transform m_PoolTransform;

	public ConsumablePool(Consumable original, int initialSize, Transform parent)
	{
		m_Original = original;
		m_PoolTransform = parent;
		m_FreeInstances = new Stack<Consumable>(initialSize);
		for (int i = 0; i < initialSize; i++)
		{
			Consumable obj = Object.Instantiate(original);
			obj.transform.SetParent(m_PoolTransform);
			obj.gameObject.SetActive(false);
			m_FreeInstances.Push(obj);
		}
	}

	public Consumable Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

	public Consumable Get(Vector3 pos, Quaternion quat)
	{
		Consumable obj = (m_FreeInstances.Count > 0) ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);
		obj.gameObject.SetActive(true);
		obj.transform.position = pos;
		obj.transform.rotation = quat;
		return obj;
	}

	public void Free(Consumable obj)
	{
		obj.transform.SetParent(m_PoolTransform);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Push(obj);
	}

	public void ResetPool()
	{
		m_FreeInstances = new Stack<Consumable>();
	}

	public void ClearPool()
	{
		while (m_FreeInstances.Count > 0)
		{
			Object.DestroyImmediate(m_FreeInstances.Pop().gameObject);
		}
		m_FreeInstances = new Stack<Consumable>();
	}
}
