using System.Collections.Generic;
using UnityEngine;

public class CollectiblePool
{
	protected Stack<Collectible> m_FreeInstances = new Stack<Collectible>();

	protected Collectible m_Original;

	protected Transform m_PoolTransform;

	public CollectiblePool(Collectible original, int initialSize, Transform parent)
	{
		m_Original = original;
		m_PoolTransform = parent;
		m_FreeInstances = new Stack<Collectible>(initialSize);
		for (int i = 0; i < initialSize; i++)
		{
			Collectible obj = Object.Instantiate(original);
			obj.transform.SetParent(m_PoolTransform);
			obj.gameObject.SetActive(false);
			m_FreeInstances.Push(obj);
		}
	}

	public Collectible Get()
	{
		return Get(Vector3.zero, Quaternion.identity);
	}

	public Collectible Get(Vector3 pos, Quaternion quat)
	{
		Collectible obj = (m_FreeInstances.Count > 0) ? m_FreeInstances.Pop() : Object.Instantiate(m_Original);
		obj.gameObject.SetActive(true);
		obj.transform.position = pos;
		obj.transform.rotation = quat;
		return obj;
	}

	public void Free(Collectible obj)
	{
		obj.transform.SetParent(m_PoolTransform);
		obj.gameObject.SetActive(false);
		m_FreeInstances.Push(obj);
	}

	public void ResetPool()
	{
		m_FreeInstances = new Stack<Collectible>();
	}

	public void ClearPool()
	{
		while (m_FreeInstances.Count > 0)
		{
			Object.DestroyImmediate(m_FreeInstances.Pop().gameObject);
		}
		m_FreeInstances = new Stack<Collectible>();
	}
}
